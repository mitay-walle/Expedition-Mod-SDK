using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Expedition.ModApi
{
	public static class ModPackageValidation
	{
		public static IReadOnlyList<ModPackage> ValidateAndOrder(IEnumerable<ModPackage> packages, string gameVersion, string unityVersion)
		{
			var byId = new SortedDictionary<string, ModPackage>(StringComparer.Ordinal);
			var contentIds = new HashSet<string>(StringComparer.Ordinal);
			var addresses = new HashSet<string>(StringComparer.Ordinal);
			var replacements = new HashSet<string>(StringComparer.Ordinal);
			foreach (ModPackage package in packages)
			{
				ModManifest manifest = package.Manifest;
				Require(ValidId(manifest.modId), "A mod has an invalid modId.");
				Require(!byId.ContainsKey(manifest.modId), $"Duplicate mod '{manifest.modId}'.");
				Require(manifest.schemaVersion == ModContract.SchemaVersion && manifest.apiVersion == ModContract.ApiVersion,
					$"Mod '{manifest.modId}' requires a different SDK/API release.");
				Require(manifest.unityVersion == unityVersion && manifest.addressablesVersion == ModContract.AddressablesVersion &&
					manifest.renderPipelineVersion == ModContract.RenderPipelineVersion && manifest.buildTarget == ModContract.BuildTarget,
					$"Mod '{manifest.modId}' was built for an incompatible Unity, Addressables, render pipeline or platform.");
				ParseVersion(manifest.version);
				Require(ParseVersion(gameVersion).CompareTo(ParseVersion(manifest.minimumGameVersion)) >= 0,
					$"Mod '{manifest.modId}' requires game {manifest.minimumGameVersion}.");
				Require(manifest.dependencies != null && manifest.content != null && manifest.content.Count > 0,
					$"Mod '{manifest.modId}' requires dependencies and nonempty content arrays.");
				Require(File.Exists(package.ResolvePath(manifest.catalog)), $"Mod '{manifest.modId}' catalog is missing.");
				var dependencies = new HashSet<string>(StringComparer.Ordinal);
				foreach (ModDependency dependency in manifest.dependencies)
				{
					Require(dependency != null && ValidId(dependency.modId) && dependency.modId != manifest.modId && dependencies.Add(dependency.modId),
						$"Mod '{manifest.modId}' has an invalid or duplicate dependency.");
					ParseVersion(dependency.minimumVersion);
				}
				foreach (ModContentEntry entry in manifest.content)
				{
					Require(entry != null && Namespaced(entry.id, manifest.modId) && Namespaced(entry.address, manifest.modId),
						$"Mod '{manifest.modId}' content requires namespaced ids and addresses.");
					Require(contentIds.Add(entry.id) && addresses.Add(entry.address), $"Duplicate content '{entry.id}' or address '{entry.address}'.");
					Require(entry.kind is ModContract.KindPrefab or ModContract.KindData or ModContract.KindTexture or ModContract.KindMaterial or ModContract.KindAudio,
						$"Content '{entry.id}' has unsupported kind '{entry.kind}'.");
					Require(entry.mode is ModContract.ModeAdditive or ModContract.ModeReplacement, $"Content '{entry.id}' has unsupported mode '{entry.mode}'.");
					if (entry.mode == ModContract.ModeReplacement)
					{
						Require(!string.IsNullOrWhiteSpace(entry.targetId) && replacements.Add(entry.targetId), $"Conflicting or empty replacement slot '{entry.targetId}'.");
					}
					else
					{
						Require(string.IsNullOrWhiteSpace(entry.targetId), $"Additive content '{entry.id}' cannot replace a slot.");
					}
				}
				byId.Add(manifest.modId, package);
			}
			var result = new List<ModPackage>();
			var visited = new HashSet<string>(StringComparer.Ordinal);
			var active = new HashSet<string>(StringComparer.Ordinal);
			foreach (ModPackage package in byId.Values)
			{
				Visit(package, byId, visited, active, result);
			}
			return result;
		}

		private static void Visit(ModPackage package, IDictionary<string, ModPackage> packages, HashSet<string> visited, HashSet<string> active, List<ModPackage> ordered)
		{
			string id = package.Manifest.modId;
			if (visited.Contains(id))
			{
				return;
			}
			Require(active.Add(id), $"Cyclic mod dependency at '{id}'.");
			foreach (ModDependency dependency in package.Manifest.dependencies.OrderBy(value => value.modId, StringComparer.Ordinal))
			{
				Require(packages.TryGetValue(dependency.modId, out ModPackage required), $"Mod '{id}' requires missing mod '{dependency.modId}'.");
				Require(ParseVersion(required.Manifest.version).CompareTo(ParseVersion(dependency.minimumVersion)) >= 0,
					$"Mod '{id}' requires '{dependency.modId}' version {dependency.minimumVersion}.");
				Visit(required, packages, visited, active, ordered);
			}
			active.Remove(id);
			visited.Add(id);
			ordered.Add(package);
		}

		private static Version ParseVersion(string value)
		{
			Require(value != null && Regex.IsMatch(value, @"^\d+\.\d+\.\d+$", RegexOptions.CultureInvariant), $"Invalid version '{value}'; expected major.minor.patch.");
			Require(Version.TryParse(value, out Version version), $"Version '{value}' is out of range.");
			return version;
		}

		private static bool ValidId(string value) => value != null && Regex.IsMatch(value, @"^[a-z0-9]+(?:[.-][a-z0-9-]+)+$", RegexOptions.CultureInvariant);
		private static bool Namespaced(string value, string modId) => value != null && value.StartsWith(modId + "/", StringComparison.Ordinal) && value.Length > modId.Length + 1;

		private static void Require(bool condition, string message)
		{
			if (!condition)
			{
				throw new InvalidOperationException(message);
			}
		}
	}
}

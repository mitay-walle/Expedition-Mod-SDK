using System;
using System.IO;

namespace Expedition.ModApi
{
	public sealed class ModPackage
	{
		public string DirectoryPath { get; }
		public ModManifest Manifest { get; }

		public ModPackage(string directoryPath, ModManifest manifest)
		{
			DirectoryPath = Path.GetFullPath(directoryPath);
			Manifest = manifest ?? throw new ArgumentNullException(nameof(manifest));
		}

		public string ResolvePath(string relativePath)
		{
			if (string.IsNullOrWhiteSpace(relativePath) || Path.IsPathRooted(relativePath) || relativePath.Contains(":") || relativePath.Contains("\\"))
			{
				throw new InvalidOperationException($"Mod '{Manifest.modId}' has invalid relative path '{relativePath}'.");
			}
			string path = Path.GetFullPath(Path.Combine(DirectoryPath, relativePath));
			if (!path.StartsWith(DirectoryPath + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
			{
				throw new InvalidOperationException($"Mod '{Manifest.modId}' path escapes its package: '{relativePath}'.");
			}
			return path;
		}
	}
}

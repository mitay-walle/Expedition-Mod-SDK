using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Expedition.ModApi;
using UnityEngine;

namespace Expedition.ModSdk.Editor
{
    public static class ModPackageInstaller
    {
        public static string Install(string packageDirectory, string modsDirectory)
        {
            if (string.IsNullOrWhiteSpace(modsDirectory) || !Path.IsPathRooted(modsDirectory) ||
                !string.Equals(new DirectoryInfo(modsDirectory).Name, "Mods", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Choose the game's absolute persistentDataPath/Mods directory.");
            string source = Path.GetFullPath(packageDirectory);
            string root = Path.GetFullPath(modsDirectory);
            if (root.StartsWith(ModSdkPaths.ProjectRoot + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Choose the game's Mods directory outside the SDK project.");
            ModPackage candidate = ReadPackage(source);
            if (!Directory.GetFiles(source, "*.bundle", SearchOption.AllDirectories).Any() ||
                !File.Exists(Path.ChangeExtension(candidate.ResolvePath(candidate.Manifest.catalog), ".hash")))
                throw new InvalidOperationException("The built package is missing bundles or its catalog hash. Build again.");
            ModPackage[] installed = Directory.Exists(root)
                ? Directory.GetDirectories(root).Select(ReadPackage).ToArray() : Array.Empty<ModPackage>();
            ModPackage existing = installed.SingleOrDefault(p => p.Manifest.modId == candidate.Manifest.modId);
            if (existing != null)
            {
                if (existing.Manifest.version == candidate.Manifest.version && SameFiles(source, existing.DirectoryPath))
                    return existing.DirectoryPath;
                throw new InvalidOperationException("This mod is already installed with different content. Close the game and remove its old package folder before installing the update.");
            }
            ModPackageValidation.ValidateAndOrder(installed.Append(candidate), "0.1.0", Application.unityVersion);
            string destination = Path.Combine(root, candidate.Manifest.modId + "-" + candidate.Manifest.version);
            if (Directory.Exists(destination)) throw new InvalidOperationException("The destination already exists.");
            Directory.CreateDirectory(destination);
            try
            {
                foreach (string file in Directory.GetFiles(source, "*", SearchOption.AllDirectories))
                {
                    if ((File.GetAttributes(file) & FileAttributes.ReparsePoint) != 0)
                        throw new InvalidOperationException("Package links are not allowed.");
                    string target = Path.Combine(destination, Path.GetRelativePath(source, file));
                    Directory.CreateDirectory(Path.GetDirectoryName(target));
                    File.Copy(file, target, false);
                }
                if (!SameFiles(source, destination)) throw new IOException("Installed package verification failed.");
            }
            catch { Directory.Delete(destination, true); throw; }
            return destination;
        }

        private static ModPackage ReadPackage(string path)
        {
            string manifest = Path.Combine(path, "mod.json");
            if (!File.Exists(manifest)) throw new InvalidOperationException("Missing mod.json: " + path);
            return new ModPackage(path, JsonUtility.FromJson<ModManifest>(File.ReadAllText(manifest)));
        }

        private static bool SameFiles(string source, string destination)
        {
            string[] files = Directory.GetFiles(source, "*", SearchOption.AllDirectories);
            if (files.Length != Directory.GetFiles(destination, "*", SearchOption.AllDirectories).Length) return false;
            using var hash = SHA256.Create();
            foreach (string file in files)
            {
                string target = Path.Combine(destination, Path.GetRelativePath(source, file));
                if (!File.Exists(target)) return false;
                using var left = File.OpenRead(file);
                using var right = File.OpenRead(target);
                if (!hash.ComputeHash(left).SequenceEqual(hash.ComputeHash(right))) return false;
            }
            return true;
        }
    }
}

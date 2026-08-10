using System;
using System.Collections.Generic;

namespace Expedition.ModApi
{
    [Serializable]
    public sealed class ModManifest
    {
        public string schemaVersion;
        public string modId;
        public string version;
        public string displayName;
        public string description;
        public string minimumGameVersion;
        public string catalog;
        public List<ModDependency> dependencies = new();
        public List<ModContentEntry> content = new();
    }
}
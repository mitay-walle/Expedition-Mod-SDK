using System;

namespace Expedition.ModApi
{
    [Serializable]
    public sealed class ModDependency
    {
        public string modId;
        public string minimumVersion;
    }
}
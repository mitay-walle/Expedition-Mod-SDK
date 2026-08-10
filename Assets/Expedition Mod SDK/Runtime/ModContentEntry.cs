using System;

namespace Expedition.ModApi
{
    [Serializable]
    public sealed class ModContentEntry
    {
        public string id;
        public string address;
        public string kind;
        public string mode;
        public string targetId;
    }
}
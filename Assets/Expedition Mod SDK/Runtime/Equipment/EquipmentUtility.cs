namespace Items
{
    [UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
    public static class EquipmentUtility
    {
        public const string RightHandSocket = "Player/Right Hand";
		public const string FeetSocket = "Player/Feet";

        public static bool RemoveFromInventoryOnEquip(this string socket)
        {
            return socket switch
            {
                RightHandSocket => false,
                _ => true
            };
        }
    }
}

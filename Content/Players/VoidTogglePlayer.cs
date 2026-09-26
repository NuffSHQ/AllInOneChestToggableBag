using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AllInOneChestToggableBag.Content.Players
{
    /// <summary>
    /// Class that keeps the saves, loads and keeps the "IsVoidVaultEnabled" disabled if needed.
    /// </summary>
    public class VoidTogglePlayer : ModPlayer
    {
        public bool IsVoidBagDisabled = false;

        // Saves the variable of the "IsVoidBagDisabled"
        public override void SaveData(TagCompound tag)
        {
            tag["VoidBagDisabledKey"] = IsVoidBagDisabled;
        }

        // Updates the "IsVoidVaultEnabled" to false, if needed after vanilla updates to the item
        public override void PostUpdateEquips() 
        {
            if (IsVoidBagDisabled)
            {
                Player.IsVoidVaultEnabled = false;
            }
        }

        // Loads the data on world join
        public override void LoadData(TagCompound tag)
        {
            if (tag.ContainsKey("VoidBagDisabledKey"))
            {
                IsVoidBagDisabled = tag.GetBool("VoidBagDisabledKey");
            }
        }
    }
}

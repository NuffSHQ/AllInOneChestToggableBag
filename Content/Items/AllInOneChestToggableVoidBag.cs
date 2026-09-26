using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;
using AllInOneChestToggableBag.Content.Players;
using System.Collections.Generic;
using Terraria.Localization;

namespace AllInOneChestToggableBag.Content.Items
{
    /// <summary>
    /// Allows the "AllInOne" item to be right-clicked inside the inventory to disable/enable the void bag functionality while updating
    /// the inventory sprite.
    /// </summary>
    public class AllInOneChestToggableVoidBag : GlobalItem 
    {
        public static LocalizedText ToolTipAction { get; private set; }
        public static LocalizedText ToolTipStatus { get; private set; }
        public static LocalizedText ActionOpen { get; private set; }
        public static LocalizedText ActionClose { get; private set; }
        public static LocalizedText StatusEnabled { get; private set; }
        public static LocalizedText StatusDisabled { get; private set; }
        private const string TargetModName = "miningcracks_take_on_luiafk";
        private const string TargetItemName = "AllInOneChest";

        public override void SetStaticDefaults()
        {
            ToolTipAction =  Mod.GetLocalization(nameof(ToolTipAction));
            ToolTipStatus =  Mod.GetLocalization(nameof(ToolTipStatus));
            ActionOpen =     Mod.GetLocalization(nameof(ActionOpen));
            ActionClose =    Mod.GetLocalization(nameof(ActionClose));
            StatusEnabled =  Mod.GetLocalization(nameof(StatusEnabled));
            StatusDisabled = Mod.GetLocalization(nameof(StatusDisabled));
        }

        // Helper method that returns if targeted mod and modded item exists
        private static bool IsTargetItem(Item item)
        {
            return item.ModItem != null && 
                   item.ModItem.Mod.Name == TargetModName && 
                   item.ModItem.Name == TargetItemName;
        }

        // Enables the "AllInOne" item that it can be right clicked
        public override bool CanRightClick(Item item) 
        {
            if (IsTargetItem(item)) 
            {
                return true;
            }
            return base.CanRightClick(item);
        }

        // Prevents the item from being consumed on right click
        public override bool ConsumeItem(Item item, Player player) 
        {
            if (IsTargetItem(item))
            {
                return false;
            }
            return base.ConsumeItem(item, player);
        }

        // Disables/Enables the void bag on right click
        public override void RightClick(Item item, Player player) 
        {
            if (IsTargetItem(item))
            {
                var voidTogglePlayer = player.GetModPlayer<VoidTogglePlayer>();
                voidTogglePlayer.IsVoidBagDisabled = !voidTogglePlayer.IsVoidBagDisabled;
            }
        }

        // Appends and modifies the tooltip description of the "AllInOne" item depending on if the item
        // void functionality is disabled/enabled
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            // Returns the default tooltip if item doesn't exist
            if (!IsTargetItem(item))
            {
                base.ModifyTooltips(item, tooltips);
                return;
            }

            var modPlayer = Main.LocalPlayer.GetModPlayer<VoidTogglePlayer>();
            TooltipLine descriptionLine = tooltips.Find(line => line.Mod == "Terraria" && line.Name == "Tooltip0");

            if (descriptionLine != null)
            {
                // Fetch the localized action fragment (open/close)
                string actionFragment = Language.GetTextValue(modPlayer.IsVoidBagDisabled
                    ? ActionOpen.ToString() 
                    : ActionClose.ToString());

                // Fetch the status fragment (Enabled/Disabled)
                string statusFragment = Language.GetTextValue(modPlayer.IsVoidBagDisabled 
                    ? StatusDisabled.ToString()
                    : StatusEnabled.ToString());

                // 2. Format the complete strings using the template definitions from your .hjson
                string completeActionText = ToolTipAction.ToString() + " " + actionFragment;
                string completeStatusText = ToolTipStatus.ToString() + statusFragment;
                
                TooltipLine rightClickDescription = new(Mod, "AllInOne_Action", completeActionText);
                TooltipLine voidBagDescription = new(Mod, "AllInOne_Status", completeStatusText);

                tooltips.Add(rightClickDescription);
                tooltips.Add(voidBagDescription);
            }
            base.ModifyTooltips(item, tooltips);
        } 

        // Dynamically changes the texture of the "AllInOne" item inside of the inventory depending
        // on the "IsVoidBagDisabled" state
        public override bool PreDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, 
                                                Color itemColor, Vector2 origin, float scale) 
        {
            if (IsTargetItem(item))  
            {
                var modPlayer = Main.LocalPlayer.GetModPlayer<VoidTogglePlayer>();

                string suffix = modPlayer.IsVoidBagDisabled ? "Disabled" : "Enabled";
                string texturePath = $"AllInOneChestToggableBag/Images/Items/AllInOneChest{suffix}";

                Texture2D customTexture = ModContent.Request<Texture2D>(texturePath, AssetRequestMode.AsyncLoad).Value;

                spriteBatch.Draw
                (
                    customTexture, 
                    position, 
                    new Rectangle(0, 0, customTexture.Width, customTexture.Height), 
                    drawColor, 
                    0f, 
                    origin, 
                    scale, 
                    SpriteEffects.None, 
                    0f
                );

                return false; // Prevent drawing the original texture over the new one
            }

            return base.PreDrawInInventory(item, spriteBatch, position, frame, drawColor, itemColor, origin, scale);
        }
    }
}

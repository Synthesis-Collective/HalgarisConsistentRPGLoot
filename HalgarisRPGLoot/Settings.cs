using System.Collections.Generic;
using Mutagen.Bethesda.Synthesis.Settings;
using HalgarisRPGLoot.DataModels;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.FormKeys.SkyrimSE;
using Mutagen.Bethesda.WPF.Reflection.Attributes;

namespace HalgarisRPGLoot
{
    public class Settings
    {
        [MaintainOrder]
        public int RandomSeed = 42;

        [MaintainOrder]
        [SynthesisSettingName("Item Uniqueness Keywords")]
        [SynthesisDescription("Keywords that indicate an item is Unique.")]
        [SynthesisTooltip("Keywords that indicate an item is Unique.")]
        public HashSet<IFormLinkGetter<IKeywordGetter>> ItemUniquenessKeywords = new HashSet<IFormLinkGetter<IKeywordGetter>>()
        {
            Skyrim.Keyword.MagicDisallowEnchanting,
            Skyrim.Keyword.DaedricArtifact
        };
        
        [MaintainOrder]
        public EnchantmentManager EnchantmentManager = new EnchantmentManager();

        [MaintainOrder]
        public ArmorSettings ArmorSettings = new ArmorSettings();
        [MaintainOrder]
        public WeaponSettings WeaponSettings = new WeaponSettings();

    }

    public class EnchantmentManager
    {
        [MaintainOrder]
        [SynthesisSettingName("List Mode")]
        [SynthesisDescription(
            "Blacklist: Selected Enchantments wont be distributed.\nWhitelist: Only the selected Enchantments get Distributed.")]
        [SynthesisTooltip(
            "Blacklist: Selected Enchantments wont be distributed.\nWhitelist: Only the selected Enchantments get Distributed.")]
        public ListMode ListMode = ListMode.Blacklist;

        [MaintainOrder]
        [SynthesisSettingName("Enchantment List")]
        [SynthesisDescription("List of Enchantments")]
        public HashSet<IFormLinkGetter<IObjectEffect>> EnchantmentList;
        
        [MaintainOrder]
        [SynthesisSettingName("Exclude Weapon Exclusive Locked Enchantments")]
        [SynthesisDescription("This makes it so unique weapon specific enchantments of lore relevant items can't appear on some random bandit equipment.")]
        [SynthesisTooltip("This makes it so unique weapon specific enchantments of lore relevant items can't appear on some random bandit equipment.")]
        public bool ExcludeLockedEnchantments = true;

    }

    public class ArmorSettings
    {
        [SynthesisSettingName("Number of variations per Item")]
        [SynthesisTooltip("This determines how many different versions\n" +
            "of the same Armor you can find.")]
        public int VarietyCountPerItem = 8;
        [SynthesisSettingName("Rarity Classes")]
        [SynthesisTooltip("Custom definable rarity classes")]
        public List<RarityClass> RarityClasses = new List<RarityClass>() {
                new RarityClass() { Label= "Magical", NumEnchantments=1, Rarity=80 },
                new RarityClass() { Label= "Rare", NumEnchantments=2, Rarity=13 },
                new RarityClass() { Label= "Epic", NumEnchantments=3, Rarity=5 },
                new RarityClass() { Label= "Legendary", NumEnchantments=4, Rarity=2 },
                };
    }

    public class WeaponSettings
    {
        [SynthesisSettingName("Number of variations per Item")]
        [SynthesisTooltip("This determines how many different versions\n" +
            "of the same Weapon you can find.")]
        public int VarietyCountPerItem = 8;
        [SynthesisSettingName("Rarity Classes")]
        [SynthesisTooltip("Custom definable rarity classes")]
        public List<RarityClass> RarityClasses = new List<RarityClass>() {
                new RarityClass() { Label= "Magical", NumEnchantments=1, Rarity=80 },
                new RarityClass() { Label= "Rare", NumEnchantments=2, Rarity=13 },
                new RarityClass() { Label= "Epic", NumEnchantments=3, Rarity=5 },
                new RarityClass() { Label= "Legendary", NumEnchantments=4, Rarity=2 },
                };
    }

    public class RarityClass
    {
        [SynthesisSettingName("RarityClass Label")]
        public string Label;
        [SynthesisSettingName("Number of Enchantments")]
        public int NumEnchantments;
        [SynthesisSettingName("Percentage chance for this rarity. (0-100)")]
        [SynthesisTooltip("The higher the number the more common it is. All your rarities are not to add up above 100.")]
        public int Rarity;
    }

}

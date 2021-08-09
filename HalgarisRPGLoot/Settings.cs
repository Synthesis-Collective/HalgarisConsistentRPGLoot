using System.Collections.Generic;
using Mutagen.Bethesda.Synthesis.Settings;

namespace HalgarisRPGLoot
{
    public class Settings
    {
        public int RandomSeed = 42;

        [SynthesisSettingName("Leveled Enchantment Settings")]
        [SynthesisTooltip("If the Patcher encounters an weapon in a Leveled List Level\n" +
            "that has no enchantments attached, then the patcher needs to\n" +
            "find the enchantment with the closest level,\n" +
            "these settings allow to control the rules applied it the search.")]
        public EnchantmentLevelSettings EnchantmentLevelSettings = new EnchantmentLevelSettings();
        public ArmorSettings ArmorSettings = new ArmorSettings();
        public WeaponSettings WeaponSettings = new WeaponSettings();

    }

    public class EnchantmentLevelSettings
    {
        //[SynthesisSettingName("SearchMode")]
        //[SynthesisTooltip("Closest Level: Searches for the closest level enchantment in higher and lower levels." +
        //    "\nOnlyHigherLevel: Only searches in the higher levels for enchantments." +
        //    "\nOnlyLowerLevel: Only searches in the lower levels for enchantments.")]
        //public SearchMode SearchMode{ get; set; } = SearchMode.ClosestLevel;
        [SynthesisSettingName("Preferred Level")]
        [SynthesisTooltip("If you prefer the higher or lower leveled enchantment, if both are equally close.")]
        public PreferredLevel PreferredLevel { get; set; } = PreferredLevel.Higher;
    }

    public class ArmorSettings
    {
        [SynthesisSettingName("Number of variations per Item")]
        [SynthesisTooltip("This determines how many different versions\n" +
            "of the same Armor you can find.")]
        public int VarietyCountPerItem = 8;
        [SynthesisSettingName("Rarity Levels")]
        [SynthesisTooltip("Custom defineable rarity levels")]
        public List<Rarity> Rarities = new List<Rarity>() {
                new Rarity() { Label= "Magical", NumEnchantments=1, LLEntries=80 },
                new Rarity() { Label= "Rare", NumEnchantments=2, LLEntries=13 },
                new Rarity() { Label= "Epic", NumEnchantments=3, LLEntries=5 },
                new Rarity() { Label= "Legendary", NumEnchantments=4, LLEntries=2 },
                };
        [SynthesisSettingName("Use RNGRarity")]
        [SynthesisTooltip("With this set to true the number of variations\n" +
                          "per item will be randomised.")]
        public bool UseRNGRarities = true;
    }

    public class WeaponSettings
    {
        [SynthesisSettingName("Number of variations per Item")]
        [SynthesisTooltip("This determines how many different versions\n" +
            "of the same Weapon you can find.")]
        public int VarietyCountPerItem = 8;
        [SynthesisSettingName("Rarity Levels")]
        [SynthesisTooltip("Custom defineable rarity levels")]
        public List<Rarity> Rarities = new List<Rarity>() {
                new Rarity() { Label= "Magical", NumEnchantments=1, LLEntries=80 },
                new Rarity() { Label= "Rare", NumEnchantments=2, LLEntries=13 },
                new Rarity() { Label= "Epic", NumEnchantments=3, LLEntries=5 },
                new Rarity() { Label= "Legendary", NumEnchantments=4, LLEntries=2 },
                };
        [SynthesisSettingName("Use RNGRarity")]
        [SynthesisTooltip("With this set to true the number of variations\n" +
            "per item will be randomised.")]
        public bool UseRNGRarities = true;
    }

    public class Rarity
    {
        [SynthesisSettingName("Rarity Label")]
        public string Label;
        [SynthesisSettingName("Number of Enchantments")]
        public int NumEnchantments;
        [SynthesisSettingName("Number of LevedList Entries")]
        [SynthesisTooltip("The higher the number the more common it is.")]
        public int LLEntries;
    }

    public enum SearchMode
    {
        ClosestLevel,
        OnlyHigherLevel,
        OnlyLowerLevel
        
    }

    public enum PreferredLevel
    {
        Higher,
        Lower
    }

}

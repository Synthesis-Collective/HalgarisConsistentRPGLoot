using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mutagen.Bethesda.Synthesis.Settings;

namespace HalgarisRPGLoot
{
    public class Settings
    {
        public int RandomSeed = 42;

        public ArmorSettings ArmorSettings = new ArmorSettings();
        public WeaponSettings WeaponSettings = new WeaponSettings();

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
}

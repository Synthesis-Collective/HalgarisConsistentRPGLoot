using System;
using System.Collections.Generic;
using System.Text;

namespace HalgarisRPGLoot
{
    [System.Serializable]
    public class Settings
    {
        public int VarietyCountPerItem;
        public List<Rarity> Rarities = new List<Rarity>();
        public bool UseRNGRarities;

        public void InitializeDefault()
        {
            UseRNGRarities = true;
            VarietyCountPerItem = 8;
            Rarities = new List<Rarity>() {
                new Rarity() { Label= "Magical", NumEnchantments=1, LLEntries=80 },
                new Rarity() { Label= "Rare", NumEnchantments=2, LLEntries=13 },
                new Rarity() { Label= "Epic", NumEnchantments=3, LLEntries=5 },
                new Rarity() { Label= "Legendary", NumEnchantments=4, LLEntries=2 },
                };
        }
    }

    [System.Serializable]
    public class Rarity
    {
        public string Label;
        public int NumEnchantments;
        public int LLEntries;
    }
}

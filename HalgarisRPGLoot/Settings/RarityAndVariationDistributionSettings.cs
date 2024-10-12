using System;
using System.Collections.Generic;
using Mutagen.Bethesda.Synthesis.Settings;
using HalgarisRPGLoot.Settings.Enums;
using Mutagen.Bethesda.WPF.Reflection.Attributes;
// ReSharper disable ConvertToConstant.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable CollectionNeverUpdated.Global

namespace HalgarisRPGLoot.Settings;

public class RarityAndVariationDistributionSettings
    {
        /* Currently Impossible to implement in the game unless someone creates a SKSE plugin that allows COBJs to accept
           a LL as a crafting target.
        [MaintainOrder]
        [SynthesisDescription("Controls if weapons and armor from crafting are replaced by RPG Loot Leveled Lists.")]
        [SynthesisTooltip("Controls if weapons and armor from crafting are replaced by RPG Loot Leveled Lists.")]
        public bool DistributeForCrafting = false;
        */
        
        [MaintainOrder] 
        [SynthesisDescription("The items that will be replaced by the RPG Loot Leveled Lists")]
        [SynthesisTooltip("The items that will be replaced by the RPG Loot Leveled Lists")]
        public LeveledListBase LeveledListBase = LeveledListBase.AllValidUnenchantedItems;

        [MaintainOrder] public GearSettings ArmorSettings = new(16,20, new()
        {
            new() {Label = "", NumEnchantments = 1, RarityWeight = 17, AllowDisenchanting = true },
            new() {Label = "Rare", NumEnchantments = 2, RarityWeight = 8,AllowDisenchanting = false },
            new() {Label = "Epic", NumEnchantments = 3, RarityWeight = 3, AllowDisenchanting = false },
            new() {Label = "Legendary", NumEnchantments = 4, RarityWeight = 1,AllowDisenchanting = false }
        });

        [MaintainOrder] public GearSettings WeaponSettings = new(16,20, new()
        {
            new() {Label = "", NumEnchantments = 1, RarityWeight = 17, AllowDisenchanting = true },
            new() {Label = "Rare", NumEnchantments = 2, RarityWeight = 8,AllowDisenchanting = false },
            new() {Label = "Epic", NumEnchantments = 3, RarityWeight = 3, AllowDisenchanting = false },
            new() {Label = "Legendary", NumEnchantments = 4, RarityWeight = 1,AllowDisenchanting = false }
        });
    }

    public class GearSettings
    {
        public GearSettings(int varietyCountPerItem,short baseItemChanceWeight, List<RarityClass> rarityClasses)
        {
            VarietyCountPerItem = varietyCountPerItem;
            BaseItemChanceWeight = baseItemChanceWeight;
            RarityClasses = rarityClasses;
        }

        [MaintainOrder]
        [SynthesisSettingName("Number of variations per Rarity")]
        [SynthesisTooltip("This determines how many different versions\n" +
                          "of the same Item you can find.")]
        [SynthesisDescription("This determines how many different versions\n" +
                              "of the same Item you can find.")]
        public int VarietyCountPerItem;

        [MaintainOrder]
        [SynthesisDescription("Weight for the unenchanted items to appear.")]
        [SynthesisTooltip("Weight for the unenchanted items to appear.")]
        public short BaseItemChanceWeight; 

        [MaintainOrder] [SynthesisSettingName("Rarity Classes")] [SynthesisTooltip("Custom definable rarity classes")]
        public List<RarityClass> RarityClasses;
    }
    
    public class LeveledListFlagSettings
    {
        public bool CalculateFromAllLevelsLessThanOrEqualPlayer = true;
        public bool SpecialLoot = false;
    }

    public class RarityClass : IComparable<RarityClass>
    {
        [SynthesisSettingName("Rarity Label")] public string Label;

        [SynthesisSettingName("Number of Enchantments")]
        public int NumEnchantments;

        [SynthesisSettingName("Rarity Weight")]
        [SynthesisTooltip("The higher the number the more likely it is" +
                          "\nthat an item gets generated with that rarity.")]
        public short RarityWeight;

        [SynthesisSettingName("Allow Disenchanting")]
        [SynthesisTooltip("Determines if loot of this rarity can be disenchanted.")]
        public bool AllowDisenchanting = true;

        public int CompareTo(RarityClass other)
        {
            return RarityWeight.CompareTo(other.RarityWeight) * -1;
        }
    }
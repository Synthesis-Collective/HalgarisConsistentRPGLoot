using System;
using System.Collections.Generic;
using Mutagen.Bethesda.Synthesis.Settings;
using HalgarisRPGLoot.DataModels;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.FormKeys.SkyrimSE;
using Mutagen.Bethesda.WPF.Reflection.Attributes;
// ReSharper disable ConvertToConstant.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable CollectionNeverUpdated.Global

namespace HalgarisRPGLoot
{
    public class Settings
    {
        [MaintainOrder] public GeneralSettings GeneralSettings = new();

        [MaintainOrder] public EnchantmentSettings EnchantmentSettings = new();

        [MaintainOrder] public RarityAndVariationDistributionSettings RarityAndVariationDistributionSettings = new();
    }

    public class GeneralSettings
    {        
        [MaintainOrder] 
        [SynthesisDescription("When set to \"JustDistributeEnchantments\" you can ignore the \"Rarity And Variation Distribution Settings\"")]
        public GenerationMode GenerationMode = GenerationMode.GenerateRarities;

        [MaintainOrder]
        [SynthesisSettingName("Only process constructable equipment")]
        [SynthesisDescription(
            "This Setting makes it so only Armor that is a CNAM (Created Object) in an COBJ (Constructable Object) Record will be considered." +
            "\nThis is to keep unique artefacts and rewards unique in their look and enchantment.")]
        [SynthesisTooltip(
            "This Setting makes it so only Armor that is a CNAM (Created Object) in an COBJ (Constructable Object) Record will be considered." +
            "\nThis is to keep unique artefacts and rewards unique in their look and enchantment.")]
        public bool OnlyProcessConstructableEquipment = true;
        
        
        [MaintainOrder]
        [SynthesisSettingName("LeveledList Flags")]
        [SynthesisDescription("Flags that will be set on generated LeveledLists")]
        [SynthesisTooltip("Flags that will be set on generated LeveledLists")]
        public LeveledListFlagSettings LeveledListFlagSettings = new();

        [MaintainOrder]
        [SynthesisSettingName("Untouchable Equipment Keywords")]
        [SynthesisDescription("Keywords that define Items you don't want processed.")]
        [SynthesisTooltip("Keywords that define Items you don't want processed.")]
        public HashSet<IFormLinkGetter<IKeywordGetter>> UntouchableEquipmentKeywords =
            new()
            {
                Skyrim.Keyword.MagicDisallowEnchanting,
                Skyrim.Keyword.DaedricArtifact,
                Skyrim.Keyword.WeapTypeStaff
            };
    }

    public class EnchantmentSettings
    {
        [MaintainOrder]
        [SynthesisSettingName("Enchantment List Mode")]
        [SynthesisDescription(
            "Blacklist: Selected Enchantments wont be distributed.\nWhitelist: Only the selected Enchantments get Distributed.")]
        [SynthesisTooltip(
            "Blacklist: Selected Enchantments wont be distributed.\nWhitelist: Only the selected Enchantments get Distributed.")]
        public ListMode EnchantmentListMode = ListMode.Blacklist;

        [MaintainOrder] [SynthesisSettingName("Enchantment List")] [SynthesisDescription("List of Enchantments")]
        public HashSet<IFormLinkGetter<IObjectEffectGetter>> EnchantmentList =
            new()
            {
                Skyrim.ObjectEffect.BoundBattleaxeEnchantment,
                Skyrim.ObjectEffect.BoundBowEnchantment,
                Skyrim.ObjectEffect.BoundSwordEnchantment
            };

        [MaintainOrder]
        [SynthesisSettingName("Plugin List Mode")]
        [SynthesisDescription(
            "Blacklist: Enchantments of selected Plugins wont be distributed." +
            "\nWhitelist: Only the Enchantments of selected Plugins  get Distributed.")]
        [SynthesisTooltip(
            "Blacklist: Enchantments of selected Plugins wont be distributed." +
            "\nWhitelist: Only the Enchantments of selected Plugins  get Distributed.")]
        public ListMode PluginListMode = ListMode.Blacklist;

        [MaintainOrder] [SynthesisSettingName("Plugin List")] [SynthesisDescription("List of Plugins")]
        public HashSet<ModKey> PluginList = new();
    }

    public class RarityAndVariationDistributionSettings
    {
        [MaintainOrder] public int RandomSeed = 42;

        [MaintainOrder] public GearSettings ArmorSettings = new(16,40, new()
        {
            new() {Label = "Magical", NumEnchantments = 1, RarityWeight = 40, AllowDisenchanting = true },
            new() {Label = "Rare", NumEnchantments = 2, RarityWeight = 13,AllowDisenchanting = false },
            new() {Label = "Epic", NumEnchantments = 3, RarityWeight = 5, AllowDisenchanting = false },
            new() {Label = "Legendary", NumEnchantments = 4, RarityWeight = 2,AllowDisenchanting = false }
        });

        [MaintainOrder] public GearSettings WeaponSettings = new(16,40, new()
        {
            new() {Label = "Magical", NumEnchantments = 1, RarityWeight = 40, AllowDisenchanting = true },
            new() {Label = "Rare", NumEnchantments = 2, RarityWeight = 13,AllowDisenchanting = false },
            new() {Label = "Epic", NumEnchantments = 3, RarityWeight = 5, AllowDisenchanting = false },
            new() {Label = "Legendary", NumEnchantments = 4, RarityWeight = 2,AllowDisenchanting = false }
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
        [SynthesisSettingName("Number of variations per Item")]
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
        public bool CalculateForEachItemInCount = true;
        public bool UseAll = false;
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
    
}
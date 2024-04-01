using System;
using System.Collections.Generic;
using System.Linq;
using HalgarisRPGLoot.DataModels;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Noggog;

// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace HalgarisRPGLoot.Analyzers
{
    public abstract class GearAnalyzer<TType>
        where TType : class, IMajorRecordGetter

    {
        protected GearSettings GearSettings;

        protected RarityAndVariationDistributionSettings RarityAndVariationDistributionSettings;

        protected List<RarityClass> RarityClasses;

        protected int VarietyCountPerRarity;
        protected IPatcherState<ISkyrimMod, ISkyrimModGetter> State { get; init; }

        protected Dictionary<int, ResolvedEnchantment[]> ByLevelIndexed;

        protected SortedList<string, ResolvedEnchantment[]>[] AllRpgEnchants { get; init; }

        protected Dictionary<string, FormKey>[] ChosenRpgEnchants { get; init; }
        protected Dictionary<FormKey, ResolvedEnchantment[]>[] ChosenRpgEnchantEffects { get; init; }

        protected HashSet<ILeveledItemGetter> AllLeveledLists { get; set; }
        protected HashSet<ResolvedListItem<TType>> AllListItems { get; set; }
        protected HashSet<ResolvedListItem<TType>> AllEnchantedItems { get; set; }
        protected HashSet<ResolvedListItem<TType>> AllUnenchantedItems { get; set; }

        private HashSet<ResolvedListItem<TType>> BaseItems { get; set; }

        protected Dictionary<FormKey, IObjectEffectGetter> AllObjectEffects { get; set; }

        protected ResolvedEnchantment[] AllEnchantments { get; set; }

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        protected HashSet<short> AllLevels { get; set; }

        protected (short Key, HashSet<ResolvedEnchantment>)[] ByLevel { get; set; }


        protected readonly Random Random = new(Program.Settings.RarityAndVariationDistributionSettings.RandomSeed);

        private readonly LeveledListFlagSettings _leveledListFlagSettings =
            Program.Settings.GeneralSettings.LeveledListFlagSettings;

        private readonly string _enchantmentSeparatorString = Program.Settings.GeneralSettings.EnchantmentSeparator;

        private readonly string _lastEnchantmentSeparatorString =
            Program.Settings.GeneralSettings.LastEnchantmentSeparator;

        protected string EditorIdPrefix;

        protected string ItemTypeDescriptor;


        public void Analyze()
        {
            AnalyzeGear();
        }

        protected abstract void AnalyzeGear();

        public void Generate()
        {
            BaseItems = RarityAndVariationDistributionSettings.LeveledListBase switch
            {
                LeveledListBase.AllValidEnchantedItems => AllEnchantedItems,
                LeveledListBase.AllValidUnenchantedItems => AllUnenchantedItems,
                _ => BaseItems
            };

            foreach (var ench in BaseItems)
            {
                var oldLeveledItem = State.PatchMod.LeveledItems.GetOrAddAsOverride(ench.List);

                var entries = oldLeveledItem.Entries?.Where(entry =>
                    entry.Data?.Reference.FormKey == ench.Resolved.FormKey);

                if (entries == null) continue;
                if (ench.Entry.Data == null) continue;
                if (ench.List?.Entries == null) continue;
                var topLevelListEditorId = "HAL_TOP_LList_" + ench.Resolved.EditorID;
                if (State.LinkCache.TryResolve<ILeveledItemGetter>(topLevelListEditorId, out _)) continue;
                var topLevelList = State.PatchMod.LeveledItems.AddNewLocking(State.PatchMod.GetNextFormKey());
                
                topLevelList.DeepCopyIn(ench.List);
                topLevelList.Entries?.Clear();
                topLevelList.EditorID = topLevelListEditorId;
                topLevelList.Flags = GetLeveledItemFlags();

                var rarityClassNumber = 0;


                foreach (var rarityClass in RarityClasses)
                {
                    var leveledItemEditorId = "HAL_SUB_LList_" + rarityClass.Label + "_" + ench.Resolved.EditorID;
                    var leveledItem = State.PatchMod.LeveledItems.AddNewLocking(State.PatchMod.GetNextFormKey());
                    leveledItem.DeepCopyIn(ench.List);
                    leveledItem.Entries?.Clear();
                    leveledItem.EditorID = leveledItemEditorId;
                    leveledItem.Flags = GetLeveledItemFlags();

                    for (var i = 0; i < VarietyCountPerRarity; i++)
                    {
                        var level = ench.Entry.Data.Level;
                        var forLevel = ByLevelIndexed[level];
                        if (forLevel.Length.Equals(0)) continue;

                        var itm = EnchantItem(ench, rarityClassNumber);
                        var entry = ench.Entry.DeepCopy();
                        entry.Data?.Reference.SetTo(itm);
                        leveledItem.Entries?.Add(entry);
                    }

                    for (var i = 0; i < rarityClass.RarityWeight; i++)
                    {
                        var newRarityEntry = ench.Entry.DeepCopy();
                        newRarityEntry.Data?.Reference.SetTo(leveledItem);

                        topLevelList.Entries?.Add(newRarityEntry);
                    }

                    rarityClassNumber++;
                }

                var topLeveledEntry = ench.Entry.DeepCopy();
                topLeveledEntry.Data?.Reference.SetTo(topLevelList);


                foreach (var entry in entries)
                {
                    entry.Data?.Reference.SetTo(topLevelList);
                }


                for (var i = 0; i < GearSettings.BaseItemChanceWeight; i++)
                {
                    var oldEntryChanceAdjustmentCopy = ench.Entry.DeepCopy();
                    topLevelList.Entries?.Add(oldEntryChanceAdjustmentCopy);
                }
            }
        }

        protected abstract FormKey EnchantItem(ResolvedListItem<TType> item, int rarity);

        protected FormKey GenerateEnchantment(int rarity)
        {
            var array = AllRpgEnchants[rarity].ToArray();
            var allRpgEnchantmentsCount = AllRpgEnchants[rarity].Count;
            var effects = array.ElementAt(Random.Next(0,
                (0 < allRpgEnchantmentsCount - 1) ? allRpgEnchantmentsCount - 1 : array.Length - 1)).Value;

            var oldObjectEffectGetter = effects.First().Enchantment;

            Console.WriteLine("Generating " + RarityClasses[rarity].Label + ItemTypeDescriptor + " enchantment of " +
                              oldObjectEffectGetter.Name);
            if (ChosenRpgEnchants[rarity].ContainsKey(RarityClasses[rarity].Label + " " + oldObjectEffectGetter.Name))
            {
                return ChosenRpgEnchants[rarity]
                    .GetValueOrDefault(RarityClasses[rarity].Label + " " + oldObjectEffectGetter.Name);
            }

            var key = State.PatchMod.GetNextFormKey();
            var newObjectEffectGetter = State.PatchMod.ObjectEffects.AddNewLocking(key);
            newObjectEffectGetter.DeepCopyIn(effects.First().Enchantment);
            newObjectEffectGetter.EditorID = EditorIdPrefix + "ENCH_" + RarityClasses[rarity].Label.ToUpper() + "_" +
                                             oldObjectEffectGetter.EditorID;
            newObjectEffectGetter.Name = RarityClasses[rarity].Label + " " + oldObjectEffectGetter.Name;
            newObjectEffectGetter.Effects.Clear();
            newObjectEffectGetter.Effects.AddRange(effects.SelectMany(e => e.Enchantment.Effects)
                .Select(e => e.DeepCopy()));
            newObjectEffectGetter.WornRestrictions.SetTo(effects.First().Enchantment.WornRestrictions);

            ChosenRpgEnchants[rarity].Add(RarityClasses[rarity].Label + " " + oldObjectEffectGetter.Name,
                newObjectEffectGetter.FormKey);
            ChosenRpgEnchantEffects[rarity].Add(newObjectEffectGetter.FormKey, effects);
            Console.WriteLine("Enchantment Generated");
            return newObjectEffectGetter.FormKey;
        }

        protected string GetEnchantmentsStringForName(ResolvedEnchantment[] resolvedEnchantments,
            bool isEditorId = false)
        {
            if (isEditorId)
            {
                return string.Join("_", resolvedEnchantments
                    .Select(resolvedEnchantment => resolvedEnchantment.Enchantment.Name!.String).ToArray());
            }

            return BeatifyLabel(string.Join(_enchantmentSeparatorString, resolvedEnchantments
                .Select(resolvedEnchantment => resolvedEnchantment.Enchantment.Name!.String).ToArray()));
        }

        private string BeatifyLabel(string labelString)
        {
            var lastSeparatorIndex = labelString.LastIndexOf(_enchantmentSeparatorString, StringComparison.Ordinal);
            if (lastSeparatorIndex == -1) return labelString;
            return labelString.Remove(lastSeparatorIndex, _enchantmentSeparatorString.Length)
                .Insert(lastSeparatorIndex, _lastEnchantmentSeparatorString);
        }

        private LeveledItem.Flag GetLeveledItemFlags()
        {
            var flag = LeveledItem.Flag.CalculateForEachItemInCount;
            if (_leveledListFlagSettings.CalculateFromAllLevelsLessThanOrEqualPlayer)
                flag |= LeveledItem.Flag.CalculateFromAllLevelsLessThanOrEqualPlayer;
            if (_leveledListFlagSettings.SpecialLoot)
                flag |= LeveledItem.Flag.SpecialLoot;
            return flag;
        }

        // Forgot what I wanted to use this for but will keep it just in case I ever remember
        // It might have been planned for a random distribution of rarities mode
        private int RandomRarity()
        {
            var rar = 0;
            var total = RarityClasses.Sum(t => t.RarityWeight);

            var roll = Random.Next(0, total);
            while (roll >= RarityClasses[rar].RarityWeight && rar < RarityClasses.Count)
            {
                roll -= RarityClasses[rar].RarityWeight;
                rar++;
            }

            return rar;
        }
    }
}
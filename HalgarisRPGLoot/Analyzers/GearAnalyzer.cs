using System;
using System.Collections.Generic;
using System.Linq;
using HalgarisRPGLoot.DataModels;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace HalgarisRPGLoot.Analyzers
{
    public abstract class GearAnalyzer<TType>
        where TType : class, IMajorRecordGetter

    {
        protected GearSettings GearSettings;

        protected RarityVariationDistributionSettings RarityVariationDistributionSettings;

        protected List<RarityClass> RarityClasses;

        protected int VarietyCountPerItem;
        protected IPatcherState<ISkyrimMod, ISkyrimModGetter> State { get; init; }

        protected ResolvedListItem<TType>[] AllUnenchantedItems { get; set; }

        protected Dictionary<int, ResolvedEnchantment[]> ByLevelIndexed;

        protected SortedList<String, ResolvedEnchantment[]>[] AllRpgEnchants { get; init; }

        protected Dictionary<String, FormKey>[] ChosenRpgEnchants { get; init; }
        protected Dictionary<FormKey, ResolvedEnchantment[]>[] ChosenRpgEnchantEffects { get; init; }
        
        protected ILeveledItemGetter[] AllLeveledLists { get; set; }
        protected ResolvedListItem<TType>[] AllListItems { get; set; }
        protected ResolvedListItem<TType>[] AllEnchantedItems { get; set; }
        protected Dictionary<FormKey, IObjectEffectGetter> AllObjectEffects { get; set; }

        protected ResolvedEnchantment[] AllEnchantments { get; set; }

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        protected HashSet<short> AllLevels { get; set; }

        protected (short Key, ResolvedEnchantment[])[] ByLevel { get; set; }


        protected readonly Random Random = new(Program.Settings.GeneralSettings.RandomSeed);

        protected string EditorIdPrefix;

        protected string ItemTypeDescriptor;


        public void Analyze()
        {
            AnalyzeGear();
        }

        protected abstract void AnalyzeGear();

        public void Generate()
        {
            foreach (var ench in AllUnenchantedItems)
            {
                var leveledItem = State.PatchMod.LeveledItems.AddNewLocking(State.PatchMod.GetNextFormKey());
                leveledItem.DeepCopyIn(ench.List);
                leveledItem.EditorID = "HAL_TOP_LList" + ench.Resolved.EditorID;
                leveledItem.Entries!.Clear();
                VarietyCountPerItem = RarityVariationDistributionSettings.GenerationMode switch
                {
                    GenerationMode.JustDistributeEnchantments => AllRpgEnchants[RarityClasses.Count].Count,
                    _ => VarietyCountPerItem
                };

                foreach (var leveledListFlag in RarityVariationDistributionSettings.LeveledListFlagSet)
                {
                    leveledItem.Flags &= ~leveledListFlag;
                }
                for (var i = 0; i < VarietyCountPerItem; i++)
                {
                    var level = ench.Entry.Data!.Level;
                    var forLevel = ByLevelIndexed[level];
                    if (forLevel.Length.Equals(0)) continue;

                    var itm = EnchantItem(ench, RandomRarity(),i);
                    var entry = ench.Entry.DeepCopy();
                    entry.Data!.Reference.SetTo(itm);
                    leveledItem.Entries.Add(entry);
                }

                var oldLeveledItem = State.PatchMod.LeveledItems.GetOrAddAsOverride(ench.List);
                foreach (var entry in oldLeveledItem.Entries!.Where(entry =>
                             entry.Data!.Reference.FormKey == ench.Resolved.FormKey))
                {
                    entry.Data!.Reference.SetTo(leveledItem);
                }
            }
        }

        protected abstract FormKey EnchantItem(ResolvedListItem<TType> item, int rarity, int currentVariation);

        protected FormKey GenerateEnchantment(int rarity, int currentVariation)
        {
            var array = AllRpgEnchants[rarity].ToArray();
            var allRpgEnchantmentsCount = AllRpgEnchants[rarity].Count;
            var effects = RarityVariationDistributionSettings.GenerationMode switch
            {
                GenerationMode.GenerateRarities => array.ElementAt(Random.Next(0,
                        (0 < allRpgEnchantmentsCount - 1) ? allRpgEnchantmentsCount - 1 : array.Length - 1))
                    .Value,
                _ => array.ElementAt(currentVariation).Value
            };

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
            newObjectEffectGetter.Effects.AddRange(effects.SelectMany(e => e.Enchantment.Effects).Select(e => e.DeepCopy()));
            newObjectEffectGetter.WornRestrictions.SetTo(effects.First().Enchantment.WornRestrictions);

            ChosenRpgEnchants[rarity].Add(RarityClasses[rarity].Label + " " + oldObjectEffectGetter.Name, newObjectEffectGetter.FormKey);
            ChosenRpgEnchantEffects[rarity].Add(newObjectEffectGetter.FormKey, effects);
            Console.WriteLine("Enchantment Generated");
            return newObjectEffectGetter.FormKey;
        }

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
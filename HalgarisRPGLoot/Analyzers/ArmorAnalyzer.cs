using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HalgarisRPGLoot.DataModels;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Strings;
using Mutagen.Bethesda.Synthesis;

namespace HalgarisRPGLoot.Analyzers
{
    public class ArmorAnalyzer
    {
        private readonly ArmorSettings _settings = Program.Settings.RarityAndVariationSettings.ArmorSettings;

        private IPatcherState<ISkyrimMod, ISkyrimModGetter> State { get; set; }
        private ILeveledItemGetter[] AllLeveledLists { get; set; }
        private ResolvedListItem<IArmorGetter>[] AllListItems { get; set; }
        private ResolvedListItem<IArmorGetter>[] AllEnchantedItems { get; set; }
        private ResolvedListItem<IArmorGetter>[] AllUnenchantedItems { get; set; }

        private Dictionary<int, ResolvedEnchantment[]> ByLevelIndexed { get; set; }

        private Dictionary<FormKey, IObjectEffectGetter> AllObjectEffects { get; set; }

        private ResolvedEnchantment[] AllEnchantments { get; set; }

        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        private HashSet<short> AllLevels { get; set; }

        private (short Key, ResolvedEnchantment[])[] ByLevel { get; set; }


        private SortedList<String, ResolvedEnchantment[]>[] AllRpgEnchants { get; set; }

        private Dictionary<String, FormKey>[] ChosenRpgEnchants { get; set; }
        private Dictionary<FormKey, ResolvedEnchantment[]>[] ChosenRpgEnchantEffects { get; set; }

        private static readonly Random Random = new Random(Program.Settings.GeneralSettings.RandomSeed);

        private Dictionary<IArmorGetter, IConstructibleObjectGetter> _armorDictionary;

        private ObjectEffectsAnalyzer _objectEffectsAnalyzer;

        public ArmorAnalyzer(IPatcherState<ISkyrimMod, ISkyrimModGetter> state,
            Dictionary<IArmorGetter, IConstructibleObjectGetter> armorDictionary,
            ObjectEffectsAnalyzer objectEffectsAnalyzer)
        {
            State = state;
            _armorDictionary = armorDictionary;
            _objectEffectsAnalyzer = objectEffectsAnalyzer;

            AllRpgEnchants = new SortedList<String, ResolvedEnchantment[]>[_settings.RarityClasses.Count];
            for (int i = 0; i < AllRpgEnchants.Length; i++)
            {
                AllRpgEnchants[i] = new SortedList<String, ResolvedEnchantment[]>();
            }

            ChosenRpgEnchants = new Dictionary<String, FormKey>[_settings.RarityClasses.Count()];
            for (int i = 0; i < ChosenRpgEnchants.Length; i++)
            {
                ChosenRpgEnchants[i] = new Dictionary<String, FormKey>();
            }

            ChosenRpgEnchantEffects = new Dictionary<FormKey, ResolvedEnchantment[]>[_settings.RarityClasses.Count];
            for (int i = 0; i < ChosenRpgEnchantEffects.Length; i++)
            {
                ChosenRpgEnchantEffects[i] = new Dictionary<FormKey, ResolvedEnchantment[]>();
            }
        }

        public void Analyze()
        {
            AllLeveledLists = State.LoadOrder.PriorityOrder.WinningOverrides<ILeveledItemGetter>().ToArray();

            AllListItems = AllLeveledLists.SelectMany(lst => lst.Entries?.Select(entry =>
                                                             {
                                                                 if (entry.Data?.Reference.FormKey == default)
                                                                     return default;
                                                                 if (entry.Data == null) return default;
                                                                 if (!State.LinkCache.TryResolve<IArmorGetter>(
                                                                         entry.Data.Reference.FormKey,
                                                                         out var resolved))
                                                                     return default;
                                                                 return new ResolvedListItem<IArmorGetter>
                                                                 {
                                                                     List = lst,
                                                                     Entry = entry,
                                                                     Resolved = resolved
                                                                 };
                                                             }).Where(resolvedListItem => resolvedListItem != default)
                                                             ?? Array.Empty<ResolvedListItem<IArmorGetter>>())
                .Where(e =>
                {
                    if (Program.Settings.GeneralSettings.OnlyProcessConstructableEquipment)
                    {
                        var kws = (e.Resolved.Keywords ?? Array.Empty<IFormLink<IKeywordGetter>>());
                        return !Extensions.CheckKeywords(kws) && _armorDictionary.ContainsKey(e.Resolved);
                    }
                    else
                    {
                        var kws = (e.Resolved.Keywords ?? Array.Empty<IFormLink<IKeywordGetter>>());
                        return !Extensions.CheckKeywords(kws);
                    }
                })
                .ToArray();

            AllUnenchantedItems = AllListItems.Where(e => e.Resolved.ObjectEffect.IsNull).ToArray();

            AllEnchantedItems = AllListItems.Where(e => !e.Resolved.ObjectEffect.IsNull).ToArray();

            AllObjectEffects = _objectEffectsAnalyzer.AllObjectEffects;

            AllEnchantments = AllEnchantedItems
                .Select(e => (e.Entry.Data?.Level, e.Resolved.EnchantmentAmount, e.Resolved.ObjectEffect.FormKey))
                .Distinct()
                .Select(e =>
                {
                    var (level, enchantmentAmount, formKey) = e;
                    if (!AllObjectEffects.TryGetValue(formKey, out var ench))
                        return default;
                    return new ResolvedEnchantment
                    {
                        Level = level ?? 1,
                        Amount = enchantmentAmount,
                        Enchantment = ench
                    };
                })
                .Where(e => e != default)
                .ToArray();

            AllLevels = AllEnchantments.Select(e => e.Level).Distinct().ToHashSet();

            var maxLvl = AllListItems.Select(i => i.Entry.Data?.Level).Distinct().ToHashSet().Max() ?? 1;

            ByLevel = AllEnchantments.GroupBy(e => e.Level)
                .OrderBy(e => e.Key)
                .Select(e => (e.Key, e.ToArray()))
                .ToArray();

            ByLevelIndexed = Enumerable.Range(0, maxLvl + 1)
                .Select(lvl => (lvl, ByLevel.Where(bl => bl.Key <= lvl).SelectMany(e => e.Item2).ToArray()))
                .ToDictionary(kv => kv.lvl, kv => kv.Item2);


            for (int coreEnchant = 0; coreEnchant < AllEnchantments.Length; coreEnchant++)
            {
                for (int i = 0; i < AllRpgEnchants.Length; i++)
                {
                    var forLevel = AllEnchantments;
                    var takeMin = Math.Min(_settings.RarityClasses[i].NumEnchantments, forLevel.Length);
                    if (takeMin <= 0) continue;
                    var enchs = new ResolvedEnchantment[takeMin];
                    enchs[0] = AllEnchantments[coreEnchant];

                    int[] result = new int[takeMin];
                    for (int j = 0; j < takeMin; ++j)
                        result[j] = j;

                    for (int t = takeMin; t < AllEnchantments.Length; ++t)
                    {
                        var m = Random.Next(0, t + 1);
                        if (m >= takeMin) continue;
                        result[m] = t;
                        if (t != coreEnchant) continue;
                        result[m] = result[0];
                        result[0] = t;
                    }

                    result[0] = coreEnchant;

                    for (int len = 0; len < takeMin; len++)
                    {
                        enchs[len] = AllEnchantments[result[len]];
                    }

                    var oldench = enchs.First().Enchantment;
                    SortedList<String, ResolvedEnchantment[]> enchants = AllRpgEnchants[i];
                    Console.WriteLine("Generated raw " + _settings.RarityClasses[i].Label + " armor enchantment of " +
                                      oldench.Name);
                    if (!enchants.ContainsKey(_settings.RarityClasses[i].Label + " " + oldench.Name))
                    {
                        enchants.Add(_settings.RarityClasses[i].Label + " " + oldench.Name, enchs);
                    }
                }
            }
        }

        public void Generate()
        {
            foreach (var ench in AllUnenchantedItems)
            {
                var lst = State.PatchMod.LeveledItems.AddNewLocking(State.PatchMod.GetNextFormKey());
                lst.DeepCopyIn(ench.List);
                lst.EditorID = "HAL_TOP_LList" + ench.Resolved.EditorID;
                lst.Entries!.Clear();
                //TODO: Make Flag Setting
                lst.Flags &= ~LeveledItem.Flag.UseAll;
                for (int i = 0; i < _settings.VarietyCountPerItem; i++)
                {
                    var level = ench.Entry.Data?.Level;
                    var forLevel = ByLevelIndexed[level ?? 1];
                    if (forLevel.Length.Equals(0)) continue;

                    var itm = EnchantItem(ench, RandomRarity());
                    var entry = ench.Entry.DeepCopy();
                    entry.Data!.Reference.SetTo(itm);
                    lst.Entries.Add(entry);
                }

                var olst = State.PatchMod.LeveledItems.GetOrAddAsOverride(ench.List);
                foreach (var entry in olst.Entries!.Where(entry =>
                             entry.Data!.Reference.FormKey == ench.Resolved.FormKey))
                {
                    entry.Data!.Reference.SetTo(lst);
                }
            }
        }

        private FormKey EnchantItem(ResolvedListItem<IArmorGetter> item, int rarity)
        {
            if (!(item.Resolved?.Name?.TryLookup(Language.English, out var itemName) ?? false))
            {
                itemName = MakeName(item.Resolved?.EditorID);
            }

            Console.WriteLine("Generating Enchanted version of " + itemName);
            if (_settings.RarityClasses[rarity].NumEnchantments != 0)
            {
                var nitm = State.PatchMod.Armors.AddNewLocking(State.PatchMod.GetNextFormKey());
                var nrec = GenerateEnchantment(rarity);
                var effects = ChosenRpgEnchantEffects[rarity].GetValueOrDefault(nrec);
                if (item.Resolved != null) nitm.DeepCopyIn(item.Resolved);
                nitm.EditorID = "HAL_ARMOR_" + _settings.RarityClasses[rarity].Label.ToUpper() + "_" + nitm.EditorID +
                                "_of_" + effects?.First().Enchantment.Name;
                nitm.ObjectEffect.SetTo(nrec);
                nitm.EnchantmentAmount = (ushort) (effects ?? Array.Empty<ResolvedEnchantment>())
                    .Where(e => e.Amount.HasValue).Sum(e => e.Amount.Value);
                nitm.Name = _settings.RarityClasses[rarity].Label + " " + itemName + " of " +
                            effects?.First().Enchantment.Name;
                if (item.Resolved != null)
                    nitm.TemplateArmor = (IFormLinkNullable<IArmorGetter>) item.Resolved.ToNullableLinkGetter();

                Console.WriteLine("Generated " + _settings.RarityClasses[rarity].Label + " " + itemName + " of " +
                                  effects?.First().Enchantment.Name);
                return nitm.FormKey;
            }
            else
            {
                var nitm = State.PatchMod.Armors.AddNewLocking(State.PatchMod.GetNextFormKey());
                if (item.Resolved != null) nitm.DeepCopyIn(item.Resolved);
                nitm.EditorID = "HAL_ARMOR_" + nitm.EditorID;

                nitm.Name = _settings.RarityClasses[rarity].Label.Equals("")
                    ? itemName
                    : _settings.RarityClasses[rarity].Label + " " + itemName;

                Console.WriteLine("Generated " + nitm.Name);


                return nitm.FormKey;
            }
        }

        private FormKey GenerateEnchantment(int rarity)
        {
            var array = AllRpgEnchants[rarity].ToArray();
            var allRpgEnchantmentsCount = AllRpgEnchants[rarity].Count;
            var effects = array.ElementAt(Random.Next(0,
                (0 < allRpgEnchantmentsCount - 1) ? allRpgEnchantmentsCount - 1 : array.Length - 1)).Value;

            var oldench = effects.First().Enchantment;

            Console.WriteLine("Generating " + _settings.RarityClasses[rarity].Label + " armor enchantment of " +
                              oldench.Name);
            if (ChosenRpgEnchants[rarity].ContainsKey(_settings.RarityClasses[rarity].Label + " " + oldench.Name))
            {
                return ChosenRpgEnchants[rarity]
                    .GetValueOrDefault(_settings.RarityClasses[rarity].Label + " " + oldench.Name);
            }

            var key = State.PatchMod.GetNextFormKey();
            var nrec = State.PatchMod.ObjectEffects.AddNewLocking(key);
            nrec.DeepCopyIn(effects.First().Enchantment);
            nrec.EditorID = "HAL_ARMOR_ENCH_" + _settings.RarityClasses[rarity].Label.ToUpper() + "_" +
                            oldench.EditorID;
            nrec.Name = _settings.RarityClasses[rarity].Label + " " + oldench.Name;
            nrec.Effects.Clear();
            nrec.Effects.AddRange(effects.SelectMany(e => e.Enchantment.Effects).Select(e => e.DeepCopy()));
            nrec.WornRestrictions.SetTo(effects.First().Enchantment.WornRestrictions);

            ChosenRpgEnchants[rarity].Add(_settings.RarityClasses[rarity].Label + " " + oldench.Name, nrec.FormKey);
            ChosenRpgEnchantEffects[rarity].Add(nrec.FormKey, effects);
            Console.WriteLine("Enchantment Generated");
            return nrec.FormKey;
        }

        public int RandomRarity()
        {
            int rar = 0;
            int total = 0;
            foreach (var t in _settings.RarityClasses)
            {
                total += t.RarityWeight;
            }

            int roll = Random.Next(0, total);
            while (roll >= _settings.RarityClasses[rar].RarityWeight && rar < _settings.RarityClasses.Count)
            {
                roll -= _settings.RarityClasses[rar].RarityWeight;
                rar++;
            }

            return rar;
        }

        // ReSharper disable once UnusedMember.Local
        private static char[] _unusedNumbers = "123456890".ToCharArray();

        private static readonly Regex Splitter =
            new Regex("(?<=[A-Z])(?=[A-Z][a-z])|(?<=[^A-Z])(?=[A-Z])|(?<=[A-Za-z])(?=[^A-Za-z])");

        private readonly Dictionary<string, string> _knownMapping = new Dictionary<string, string>();

        private string MakeName(string resolvedEditorId)
        {
            string returning;
            if (resolvedEditorId == null)
            {
                returning = "Armor";
            }
            else
            {
                if (_knownMapping.TryGetValue(resolvedEditorId, out var cached))
                    return cached;

                var parts = Splitter.Split(resolvedEditorId)
                    .Where(e => e.Length > 1)
                    .Where(e => e != "DLC" && e != "Armor" && e != "Variant")
                    .Where(e => !int.TryParse(e, out var _))
                    .ToArray();
                if (parts.First() == "Clothes" && parts.Last() == "Clothes")
                    parts = parts.Skip(1).ToArray();
                if (parts.Length >= 2 && parts.First() == "Clothes")
                    parts = parts.Skip(1).ToArray();
                returning = string.Join(" ", parts);
                _knownMapping[resolvedEditorId] = returning;
            }

            Console.WriteLine($"Missing armor name for {resolvedEditorId ?? "<null>"} using {returning}");

            return returning;
        }
    }
}
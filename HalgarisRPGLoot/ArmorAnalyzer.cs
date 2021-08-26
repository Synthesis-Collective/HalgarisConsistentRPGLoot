using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Mutagen.Bethesda;
using Mutagen.Bethesda.FormKeys.SkyrimSE;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Noggog;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Strings;


namespace HalgarisRPGLoot
{
    public class ArmorAnalyzer
    {
        private ArmorSettings Settings = Program.Settings.ArmorSettings;

        public IPatcherState<ISkyrimMod, ISkyrimModGetter> State { get; set; }
        public ILeveledItemGetter[] AllLeveledLists { get; set; }
        public ResolvedListItem<IArmorGetter>[] AllListItems { get; set; }
        public ResolvedListItem<IArmorGetter>[] AllEnchantedItems { get; set; }
        public ResolvedListItem<IArmorGetter>[] AllUnenchantedItems { get; set; }


        public Dictionary<int, ResolvedEnchantment[]> ByLevelIndexed { get; set; }

        public Dictionary<FormKey, IObjectEffectGetter> AllObjectEffects { get; set; }


        public ResolvedEnchantment[] AllEnchantments { get; set; }
        public HashSet<short> AllLevels { get; set; }

        public (short Key, ResolvedEnchantment[])[] ByLevel { get; set; }

        public SortedList<String, ResolvedEnchantment[]>[] AllRPGEnchants { get; set; }
        public Dictionary<String, FormKey>[] ChosenRPGEnchants { get; set; }
        public Dictionary<FormKey, ResolvedEnchantment[]>[] ChosenRPGEnchantEffects { get; set; }

        private static Random r = new Random(Program.Settings.RandomSeed);

        public ArmorAnalyzer(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)

        {
            State = state;
            AllRPGEnchants = new SortedList<String, ResolvedEnchantment[]>[Settings.Rarities.Count()];
            for (int i = 0; i < AllRPGEnchants.Length; i++)
            {
                AllRPGEnchants[i] = new SortedList<String, ResolvedEnchantment[]>();
            }
            ChosenRPGEnchants = new Dictionary<String, FormKey>[Settings.Rarities.Count()];
            for (int i = 0; i < ChosenRPGEnchants.Length; i++)
            {
                ChosenRPGEnchants[i] = new Dictionary<String, FormKey>();
            }
            ChosenRPGEnchantEffects = new Dictionary<FormKey, ResolvedEnchantment[]>[Settings.Rarities.Count()];
            for (int i = 0; i < ChosenRPGEnchantEffects.Length; i++)
            {
                ChosenRPGEnchantEffects[i] = new Dictionary<FormKey, ResolvedEnchantment[]>();
            }
        }

        public void Analyze()
        {
            AllLeveledLists = State.LoadOrder.PriorityOrder.WinningOverrides<ILeveledItemGetter>().ToArray();

            AllListItems = AllLeveledLists.SelectMany(lst => lst.Entries?.Select(entry =>
                                                             {
                                                                 if (entry?.Data?.Reference.FormKey == default) return default;
                    
                                                                 if (!State.LinkCache.TryResolve<IArmorGetter>(entry.Data.Reference.FormKey,

                                                                     out var resolved))
                                                                     return default;
                                                                 return new ResolvedListItem<IArmorGetter>
                                                                 {
                                                                     List = lst,
                                                                     Entry = entry,
                                                                     Resolved = resolved
                                                                 };
                                                             }).Where(r => r != default)
                                                             ?? new ResolvedListItem<IArmorGetter>[0])
                .Where(e =>
                {
                    var kws = (e.Resolved.Keywords ?? new IFormLink<IKeywordGetter>[0]);
                    return !kws.Contains(Skyrim.Keyword.MagicDisallowEnchanting);
                })
                .ToArray();

            AllUnenchantedItems = AllListItems.Where(e => e.Resolved.ObjectEffect.IsNull).ToArray();

            AllEnchantedItems = AllListItems.Where(e => !e.Resolved.ObjectEffect.IsNull).ToArray();

            AllObjectEffects = State.LoadOrder.PriorityOrder.ObjectEffect().WinningOverrides()
                .ToDictionary(k => k.FormKey);

            AllEnchantments = AllEnchantedItems
                .Select(e => (e.Entry.Data.Level, e.Resolved.EnchantmentAmount, e.Resolved.ObjectEffect.FormKey))
                .Distinct()
                .Select(e =>
                {
                    if (!AllObjectEffects.TryGetValue(e.FormKey, out var ench))
                        return default;
                    return new ResolvedEnchantment
                    {
                        Level = e.Level,
                        Amount = e.Item2,
                        Enchantment = ench
                    };
                })
                .Where(e => e != default)
                .ToArray();

            AllLevels = AllEnchantments.Select(e => e.Level).Distinct().ToHashSet();

            short maxLvl = AllListItems.Select(i => i.Entry.Data.Level).Distinct().ToHashSet().Max();

            ByLevel = AllEnchantments.GroupBy(e => e.Level)
                .OrderBy(e => e.Key)
                .Select(e => (e.Key, e.ToArray()))
                .ToArray();

            ByLevelIndexed = Enumerable.Range(0, maxLvl+1)
                .Select(lvl => (lvl, ByLevel.Where(bl => bl.Key <= lvl).SelectMany(e => e.Item2).ToArray()))
                .ToDictionary(kv => kv.lvl, kv => kv.Item2);


            for (int coreEnchant = 0; coreEnchant < AllEnchantments.Length; coreEnchant++)
            {
                for (int i = 0; i < AllRPGEnchants.Length; i++)
                {

                    var forLevel = AllEnchantments;
                    var takeMin = Math.Min(Settings.Rarities[i].NumEnchantments, forLevel.Length);
                    if (takeMin == 0) continue;
                    var enchs = new ResolvedEnchantment[takeMin];
                    enchs[0] = AllEnchantments[coreEnchant];

                    int[] result = new int[takeMin];
                    for (int j = 0; j < takeMin; ++j)
                        result[j] = j;

                    for (int t = takeMin; t < AllEnchantments.Length; ++t)
                    {
                        int m = r.Next(0, t + 1);
                        if (m < takeMin)
                        {
                            result[m] = t;
                            if (t == coreEnchant)
                            {
                                result[m] = result[0];
                                result[0] = t;
                            }
                        }
                    }
                    if (result[0] != coreEnchant)
                    {
                        result[0] = coreEnchant;
                    }
                    for (int len = 0; len < takeMin; len++)
                    {
                        enchs[len] = AllEnchantments[result[len]];
                    }

                    var oldench = enchs.First().Enchantment;
                    SortedList<String, ResolvedEnchantment[]> enchants = AllRPGEnchants[i];
                    Console.WriteLine("Generated raw " + Settings.Rarities[i].Label + " armor enchantment of " + oldench.Name);
                    if (!enchants.ContainsKey(Settings.Rarities[i].Label + " " + oldench.Name))
                    {
                        enchants.Add(Settings.Rarities[i].Label + " " + oldench.Name, enchs);
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
                lst.Flags &= ~LeveledItem.Flag.UseAll;
                for (int i = 0; i < Settings.VarietyCountPerItem; i++)
                {
                    var level = ench.Entry.Data.Level;
                    var forLevel = ByLevelIndexed[level];
                    if (forLevel.Length.Equals(0)) continue;

                    var itm = enchantItem(ench, RandomRarity());
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

        private FormKey enchantItem(ResolvedListItem<IArmorGetter> item, int rarity)
        {


            string itemName = "";
            if (!(item.Resolved?.Name?.TryLookup(Language.English, out itemName) ?? false))
            {
                itemName = MakeName(item.Resolved.EditorID);
            }
            Console.WriteLine("Generating Enchanted version of " + itemName);
            var nitm = State.PatchMod.Armors.AddNewLocking(State.PatchMod.GetNextFormKey());
            var nrec = GenerateEnchantment(rarity);
            var effects = ChosenRPGEnchantEffects[rarity].GetValueOrDefault(nrec);
            nitm.DeepCopyIn(item.Resolved);
            nitm.EditorID = "HAL_ARMOR_" + nitm.EditorID;
            nitm.ObjectEffect.SetTo(nrec);
            nitm.EnchantmentAmount = (ushort)effects.Where(e => e.Amount.HasValue).Sum(e => e.Amount.Value);
            nitm.Name = Settings.Rarities[rarity].Label + " " + itemName + " of " + effects.First().Enchantment.Name;


            Console.WriteLine("Generated " + Settings.Rarities[rarity].Label + " " + itemName + " of " + effects.First().Enchantment.Name);
            return nitm.FormKey;
        }
        private FormKey GenerateEnchantment(int rarity)
        {
            int rarityEnchCount = Settings.Rarities[rarity].NumEnchantments;
            var takeMin = Math.Min(rarityEnchCount, AllRPGEnchants[rarity].Count);
            var array = AllRPGEnchants[rarity].ToArray();
            var effects = array.ElementAt(r.Next(0, AllRPGEnchants[rarity].Count)).Value;

            var oldench = effects.First().Enchantment;

            Console.WriteLine("Generating " + Settings.Rarities[rarity].Label + " armor enchantment of " + oldench.Name);
            if (ChosenRPGEnchants[rarity].ContainsKey(Settings.Rarities[rarity].Label + " " + oldench.Name))
            {
                return ChosenRPGEnchants[rarity].GetValueOrDefault(Settings.Rarities[rarity].Label + " " + oldench.Name);
            }

            var key = State.PatchMod.GetNextFormKey();
            var nrec = State.PatchMod.ObjectEffects.AddNewLocking(key);
            nrec.DeepCopyIn(effects.First().Enchantment);
            nrec.EditorID = "HAL_ARMOR_ENCH_" + Settings.Rarities[rarity].Label.ToUpper() + "_" + oldench.EditorID;
            nrec.Name = Settings.Rarities[rarity].Label + " " + oldench.Name;
            nrec.Effects.Clear();
            nrec.Effects.AddRange(effects.SelectMany(e => e.Enchantment.Effects).Select(e => e.DeepCopy()));
            nrec.WornRestrictions.SetTo(effects.First().Enchantment.WornRestrictions);

            ChosenRPGEnchants[rarity].Add(Settings.Rarities[rarity].Label + " " + oldench.Name, nrec.FormKey);
            ChosenRPGEnchantEffects[rarity].Add(nrec.FormKey, effects);
            Console.WriteLine("Enchantment Generated");
            return nrec.FormKey;
        }
        public int RandomRarity()
        {
            int rar = 0;
            int total = 0;
            for (int i = 0; i < Settings.Rarities.Count; i++)
            {
                total += Settings.Rarities[i].LLEntries;
            }
            int roll = r.Next(0, total);
            while (roll >= Settings.Rarities[rar].LLEntries && rar < Settings.Rarities.Count)
            {
                roll -= Settings.Rarities[rar].LLEntries;
                rar++;
            }
            return rar;
        }
        private static char[] Numbers = "123456890".ToCharArray();
        private static Regex Splitter = new Regex("(?<=[A-Z])(?=[A-Z][a-z])|(?<=[^A-Z])(?=[A-Z])|(?<=[A-Za-z])(?=[^A-Za-z])");
        private Dictionary<string, string> KnownMapping = new Dictionary<string, string>();
        private string MakeName(string? resolvedEditorId)
        {
            string returning;
            if (resolvedEditorId == null)
            {
                returning = "Armor";
            }
            else
            {
                if (KnownMapping.TryGetValue(resolvedEditorId, out var cached))
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
                KnownMapping[resolvedEditorId] = returning;
            }
            Console.WriteLine($"Missing armor name for {resolvedEditorId ?? "<null>"} using {returning}");

            return returning;
        }
    }
}
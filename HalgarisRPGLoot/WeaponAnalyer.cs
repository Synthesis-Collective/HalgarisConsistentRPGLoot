using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Mutagen.Bethesda;
using Mutagen.Bethesda.FormKeys.SkyrimSE;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;

namespace HalgarisRPGLoot
{
    public class WeaponAnalyzer
    {

        public const int ENCHANTED_VARIETY_COUNT_PER_ITEM = 8;

        public static IEnumerable<(string Name, int EnchCount, int LLEntries)> Rarities = new (string Name, int EnchCount, int LLEntries)[]
        {
            ("Magical", 1, 80),
            ("Rare", 2, 13),
            ("Epic", 3, 5),
            ("Legendary", 4, 2)
        };

        public SynthesisState<ISkyrimMod, ISkyrimModGetter> State { get; set; }
        public ILeveledItemGetter[] AllLeveledLists { get; set; }
        public ResolvedListItem<IWeapon, IWeaponGetter>[] AllListItems { get; set; }
        public ResolvedListItem<IWeapon, IWeaponGetter>[] AllEnchantedItems { get; set; }
        public ResolvedListItem<IWeapon, IWeaponGetter>[] AllUnenchantedItems { get; set; }

        public Dictionary<int, ResolvedEnchantment[]> ByLevelIndexed { get; set; }

        public ResolvedEnchantment[] AllEnchantments { get; set; }
        public HashSet<short> AllLevels { get; set; }

        public (short Key, ResolvedEnchantment[])[] ByLevel { get; set; }

        public Dictionary<FormKey, IObjectEffectGetter> AllObjectEffects { get; set; }




        public WeaponAnalyzer(SynthesisState<ISkyrimMod, ISkyrimModGetter> state)
        {
            State = state;
        }

        public void Analyze()
        {
            AllLeveledLists = State.LoadOrder.PriorityOrder.WinningOverrides<ILeveledItemGetter>().ToArray();

            AllListItems = AllLeveledLists.SelectMany(lst => lst.Entries?.Select(entry =>
                                                             {
                                                                 if (entry?.Data?.Reference.FormKey == default) return default;

                                                                 if (!State.LinkCache.TryLookup<IWeaponGetter>(entry.Data.Reference.FormKey,
                                                                     out var resolved))
                                                                     return default;
                                                                 return new ResolvedListItem<IWeapon, IWeaponGetter>
                                                                 {
                                                                     List = lst,
                                                                     Entry = entry,
                                                                     Resolved = resolved
                                                                 };
                                                             }).Where(r => r != default)
                                                             ?? new ResolvedListItem<IWeapon, IWeaponGetter>[0])
                .Where(e =>
                {
                    var kws = (e.Resolved.Keywords ?? new IFormLink<IKeywordGetter>[0]);
                    return (!kws.Contains(Skyrim.Keyword.WeapTypeStaff))
                           && (!kws.Contains(Skyrim.Keyword.MagicDisallowEnchanting));
                })
                .ToArray();

            AllUnenchantedItems = AllListItems.Where(e => e.Resolved.ObjectEffect.IsNull).ToArray();

            AllEnchantedItems = AllListItems.Where(e => !e.Resolved.ObjectEffect.IsNull).ToArray();

            AllObjectEffects = State.LoadOrder.PriorityOrder.ObjectEffect().WinningOverrides()
                .ToDictionary(k => k.FormKey);

            AllEnchantments = AllEnchantedItems
                .Select(e => (e.Entry.Data.Level, e.Resolved.EnchantmentAmount, e.Resolved.ObjectEffect.FormKey!.Value))
                .Distinct()
                .Select(e =>
                {
                    if (!AllObjectEffects.TryGetValue(e.Value, out var ench))
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

            ByLevel = AllEnchantments.GroupBy(e => e.Level)
                .OrderBy(e => e.Key)
                .Select(e => (e.Key, e.ToArray()))
                .ToArray();

            ByLevelIndexed = Enumerable.Range(0, 100)
                .Select(lvl => (lvl, ByLevel.Where(bl => bl.Key <= lvl).SelectMany(e => e.Item2).ToArray()))
                .ToDictionary(kv => kv.lvl, kv => kv.Item2);
        }

        public void Report()
        {
            Console.WriteLine($"Found: {AllLeveledLists.Length} leveled lists");
            Console.WriteLine($"Found: {AllListItems.Length} items");
            Console.WriteLine($"Found: {AllUnenchantedItems.Length} un-enchanted items");
            Console.WriteLine($"Found: {AllEnchantedItems.Length} enchanted items");
        }

        public void Generate()
        {
            var rand = new Random(Guid.NewGuid().GetHashCode());

            foreach (var ench in AllUnenchantedItems)
            {
                var lst = State.PatchMod.LeveledItems.AddNewLocking(State.PatchMod.GetNextFormKey());
                lst.DeepCopyIn(ench.List);
                lst.EditorID = "HAL_TOP_LList" + ench.Resolved.EditorID;
                lst.Entries!.Clear();
                lst.Flags &= ~LeveledItem.Flag.UseAll;

                int[] numEntriesPerRarity = GenerateRarityEntryCounts(rand);

                for (int i = 0; i < Rarities.Count(); i++)
                {
                    var e = Rarities.ElementAt(i);
                    var numEntries = numEntriesPerRarity[i];
                    var nlst = State.PatchMod.LeveledItems.AddNewLocking(State.PatchMod.GetNextFormKey());
                    nlst.DeepCopyIn(ench.List);
                    nlst.EditorID = "HAL_LList_" + e.Name + "_" + ench.Resolved.EditorID;
                    nlst.Entries!.Clear();
                    nlst.Flags &= ~LeveledItem.Flag.UseAll;

                    for (var j = 0; j < numEntries; j++)
                    {
                        var itm = GenerateEnchantment(ench, e.Name, e.EnchCount);
                        var entry = ench.Entry.DeepCopy();
                        entry.Data!.Reference = itm;
                        nlst.Entries.Add(entry);
                    }

                    for (var j = 0; j < e.LLEntries; j++)
                    {
                        var lentry = ench.Entry.DeepCopy();
                        lentry.Data!.Reference = nlst;
                        lst.Entries.Add(lentry);
                    }
                }

                var remain = 240 - Rarities.Sum(e => e.LLEntries);
                for (var i = 0; i < remain; i++)
                {
                    var lentry = ench.Entry.DeepCopy();
                    lentry.Data!.Reference = ench.Resolved.FormKey;
                    lst.Entries.Add(lentry);
                }

                lock (State)
                {
                    var olst = State.PatchMod.LeveledItems.GetOrAddAsOverride(ench.List);
                    foreach (var entry in olst.Entries!.Where(entry =>
                        entry.Data!.Reference.FormKey == ench.Resolved.FormKey))
                    {
                        entry.Data!.Reference = lst.FormKey;
                    }
                }
            }
        }

        private static int[] GenerateRarityEntryCounts(Random rand)
        {
            var rarityWeight = Rarities.Sum(r => r.LLEntries);
            var oddsMagical = Rarities.ElementAt(0).LLEntries;
            var oddsRare = Rarities.ElementAt(1).LLEntries;
            var oddsEpic = Rarities.ElementAt(2).LLEntries;

            // Roll EnchantmentsPer times
            // use value to assign which Rarities we create
            var counts = new int[4] { 0, 0, 0, 0 };
            for (int i = 0; i < ENCHANTED_VARIETY_COUNT_PER_ITEM; i++)
            {
                var v = rand.Next() % rarityWeight;
                if (v <= oddsMagical)
                {
                    counts[0]++;
                }
                else if (v <= oddsMagical + oddsRare)
                {
                    counts[1]++;
                }
                else if (v <= oddsMagical + oddsRare + oddsEpic)
                {
                    counts[2]++;
                }
                else
                {
                    counts[3]++;
                }
            }

            return counts;
        }

        private FormKey GenerateEnchantment(
            ResolvedListItem<IWeapon, IWeaponGetter> item,
            string rarityName, int rarityEnchCount)
        {
            var level = item.Entry.Data.Level;
            var forLevel = ByLevelIndexed[level];
            var effects = Extensions.Repeatedly(() => forLevel.RandomItem())
                .Distinct()
                .Take(rarityEnchCount)
                .Shuffle();

            var oldench = effects.First().Enchantment;
            var key = State.PatchMod.GetNextFormKey();
            var nrec = State.PatchMod.ObjectEffects.AddNewLocking(key);
            nrec.DeepCopyIn(effects.First().Enchantment);
            nrec.EditorID = "HAL_WEAPON_ENCH_" + oldench.EditorID;
            nrec.Name = rarityName + " " + oldench.Name;
            nrec.Effects.Clear();
            nrec.Effects.AddRange(effects.SelectMany(e => e.Enchantment.Effects).Select(e => e.DeepCopy()));
            nrec.WornRestrictions = effects.First().Enchantment.WornRestrictions;

            string itemName = "";
            if (!(item.Resolved?.Name?.TryLookup(Language.English, out itemName) ?? false))
            {
                itemName = MakeName(item.Resolved.EditorID);
            }

            var nitm = State.PatchMod.Weapons.AddNewLocking(State.PatchMod.GetNextFormKey());
            nitm.DeepCopyIn(item.Resolved);
            nitm.EditorID = "HAL_WEAPON_" + nitm.EditorID;
            nitm.ObjectEffect = nrec.FormKey;
            nitm.EnchantmentAmount = (ushort)effects.Where(e => e.Amount.HasValue).Sum(e => e.Amount.Value);
            nitm.Name = rarityName + " " + itemName + " of " + effects.First().Enchantment.Name;



            return nitm.FormKey;
        }

        private static char[] Numbers = "123456890".ToCharArray();
        private static Regex Splitter = new Regex("(?<=[A-Z])(?=[A-Z][a-z])|(?<=[^A-Z])(?=[A-Z])|(?<=[A-Za-z])(?=[^A-Za-z])");
        private Dictionary<string, string> KnownMapping = new Dictionary<string, string>();
        private string MakeName(string? resolvedEditorId)
        {
            string returning;
            if (resolvedEditorId == null)
            {
                returning = "Weapon";
            }
            else
            {
                if (KnownMapping.TryGetValue(resolvedEditorId, out var cached))
                    return cached;

                var parts = Splitter.Split(resolvedEditorId)
                    .Where(e => e.Length > 1)
                    .Where(e => e != "DLC" && e != "Weapon" && e != "Variant")
                    .Where(e => !int.TryParse(e, out var _))
                    .ToArray();

                returning = string.Join(" ", parts);
                KnownMapping[resolvedEditorId] = returning;
            }
            Console.WriteLine($"Missing weapon name for {resolvedEditorId ?? "<null>"} using {returning}");

            return returning;
        }
    }
}
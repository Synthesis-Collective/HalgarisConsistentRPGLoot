using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;

namespace HalgarisRPGLoot
{
    public class ArmorAnalyzer
    {

        public const int MAX_GENERATED_ENCHANTMENTS = 100_000; 
        
        public static IEnumerable<(string Name, int EnchCount, int LLEntries)> Rarities = new (string Name, int EnchCount, int LLEntries)[]
        {
            ("Magical", 1, 150),
            ("Rare", 2, 40),
            ("Epic", 3, 15),
            ("Legenedary", 4, 2)
        };
        
        public SynthesisState<ISkyrimMod, ISkyrimModGetter> State { get; set; }
        public ILeveledItemGetter[] AllLeveledLists { get; set; }
        public ResolvedListItem<IArmor, IArmorGetter>[] AllListItems { get; set; }
        public ResolvedListItem<IArmor, IArmorGetter>[] AllEnchantedItems { get; set; }
        public ResolvedListItem<IArmor, IArmorGetter>[] AllUnenchantedItems { get; set; }
        
        public Dictionary<int, ResolvedEnchantment[]> ByLevelIndexed { get; set; }

        public ResolvedEnchantment[] AllEnchantments { get; set; }
        public HashSet<short> AllLevels { get; set; }
        
        public (short Key, ResolvedEnchantment[])[] ByLevel { get; set; }
        
        public Dictionary<FormKey, FormKey> Replacements { get; set; } = new Dictionary<FormKey, FormKey>(); 


        
        public ArmorAnalyzer(SynthesisState<ISkyrimMod, ISkyrimModGetter> state)
        {
            State = state;
        }

        public void Analyze()
        {
            AllLeveledLists = State.LoadOrder.PriorityOrder.WinningOverrides<ILeveledItemGetter>().ToArray();
            AllListItems = AllLeveledLists.SelectMany(lst => lst.Entries?.Select(entry =>
                {
                    if (entry?.Data?.Reference.FormKey == default) return default;
                    
                    if (!State.LinkCache.TryLookup<IArmorGetter>(entry.Data.Reference.FormKey,
                        out var resolved))
                        return default;
                    return new ResolvedListItem<IArmor, IArmorGetter>
                    {
                        List = lst,
                        Entry = entry,
                        Resolved = resolved
                    };
                }).Where(r => r != default)
                ?? new ResolvedListItem<IArmor, IArmorGetter>[0])
                .ToArray();
            AllUnenchantedItems = AllListItems.Where(e => e.Resolved.ObjectEffect.IsNull).ToArray();
            AllEnchantedItems = AllListItems.Where(e => !e.Resolved.ObjectEffect.IsNull).ToArray();

            AllEnchantments = AllEnchantedItems
                .Select(e => (e.Entry.Data.Level, e.Resolved.EnchantmentAmount, e.Resolved.ObjectEffect.FormKey!.Value))
                .Distinct()
                .Select(e =>
                {
                    if (!State.LinkCache.TryLookup<IObjectEffectGetter>(e.Value, out var ench))
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
            var enchantmentsPer = MAX_GENERATED_ENCHANTMENTS / AllUnenchantedItems.Length;
            var rarityWeight = Rarities.Sum(r => r.LLEntries);
            
            foreach (var ench in AllUnenchantedItems)
            {
                var lst = State.PatchMod.LeveledItems.AddNew(State.PatchMod.GetNextFormKey());
                lst.DeepCopyIn(ench.List);
                lst.EditorID = "HAL_TOP_LList" + ench.Resolved.EditorID;
                lst.Entries!.Clear();

                foreach (var e in Rarities)
                {

                    var nlst = State.PatchMod.LeveledItems.AddNew(State.PatchMod.GetNextFormKey());
                    nlst.DeepCopyIn(ench.List);
                    nlst.EditorID = "HAL_LList_" + e.Name + "_" + ench.Resolved.EditorID;
                    nlst.Entries!.Clear();

                    var numEntries = e.LLEntries * enchantmentsPer / rarityWeight;


                    for (var i = 0; i < numEntries; i++)
                    {
                        var itm = GenerateEnchantment(ench, e.Name, e.EnchCount);
                        var entry = ench.Entry.DeepCopy();
                        entry.Data!.Reference = itm;
                        nlst.Entries.Add(entry);
                    }

                    for (var i = 0; i < e.LLEntries; i++)
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

                var olst = State.PatchMod.LeveledItems.GetOrAddAsOverride(ench.List);
                foreach (var entry in olst.Entries!.Where(entry =>
                    entry.Data!.Reference.FormKey == ench.Resolved.FormKey))
                {
                    entry.Data!.Reference = lst.FormKey;
                }
            }
        }
        private FormKey GenerateEnchantment(
            ResolvedListItem<IArmor, IArmorGetter> item,
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
            var nrec = State.PatchMod.ObjectEffects.AddNew(key);
            nrec.DeepCopyIn(effects.First().Enchantment);
            nrec.EditorID = "HAL_ARMOR_ENCH_" + oldench.EditorID;
            nrec.Name = rarityName + " " + oldench.Name;
            nrec.Effects.Clear();
            nrec.Effects.AddRange(effects.SelectMany(e => e.Enchantment.Effects).Select(e => e.DeepCopy()));
            nrec.WornRestrictions = effects.First().Enchantment.WornRestrictions;

            if (!item.Resolved.Name.TryLookup(Language.English, out var itemName))
            {
                
            }
            var nitm = State.PatchMod.Armors.AddNewLocking(State.PatchMod.GetNextFormKey());
            nitm.DeepCopyIn(item.Resolved);
            nitm.EditorID = "HAL_ARMOR_" + nitm.EditorID;
            nitm.ObjectEffect = nrec.FormKey;
            nitm.Name = rarityName + " " + itemName + " of " + effects.First().Enchantment.Name;



            return nitm.FormKey;
        }
    }
}
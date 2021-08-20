using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Mutagen.Bethesda;
using Mutagen.Bethesda.FormKeys.SkyrimSE;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Strings;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Noggog;


namespace HalgarisRPGLoot
{
    public class WeaponAnalyzer
    {
        private WeaponSettings Settings = Program.Settings.WeaponSettings;

        

        public IPatcherState<ISkyrimMod, ISkyrimModGetter> State { get; set; }
        public ILeveledItemGetter[] AllLeveledLists { get; set; }
        public ResolvedListItem<IWeaponGetter>[] AllListItems { get; set; }
        public ResolvedListItem<IWeaponGetter>[] AllEnchantedItems { get; set; }
        public ResolvedListItem<IWeaponGetter>[] AllUnenchantedItems { get; set; }

        public Dictionary<int, ResolvedEnchantment[]> ByLevelIndexed { get; set; }

        public ResolvedEnchantment[] AllEnchantments { get; set; }
        public HashSet<short> AllLevels { get; set; }

        public (short Key, ResolvedEnchantment[])[] ByLevel { get; set; }

        public Dictionary<FormKey, IObjectEffectGetter> AllObjectEffects { get; set; }


        public SortedList<String, ResolvedEnchantment[]>[] AllRPGEnchants { get; set; }
        public Dictionary<String, FormKey>[] ChosenRPGEnchants { get; set; }
        public Dictionary<FormKey, ResolvedEnchantment[]>[] ChosenRPGEnchantEffects { get; set; }

        private static Random r = new Random(Program.Settings.RandomSeed);


        public WeaponAnalyzer(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)

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

                                                                 if (!State.LinkCache.TryResolve<IWeaponGetter>(entry.Data.Reference.FormKey,
                                                                     out var resolved))
                                                                     return default;
                                                                 return new ResolvedListItem<IWeaponGetter>
                                                                 {
                                                                     List = lst,
                                                                     Entry = entry,
                                                                     Resolved = resolved
                                                                 };
                                                             }).Where(r => r != default)
                                                             ?? new ResolvedListItem<IWeaponGetter>[0])
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
                .Where(x => x.Name != null).Where(k => !k.Name.ToString().EndsWith("FX")) // Excluding Bound Weapon FX Object Effects as they don't do a thing on non bound weapons. 
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

            //Random r = new Random(Program.Settings.RandomSeed);

            for (int coreEnchant = 0; coreEnchant < AllEnchantments.Length; coreEnchant++)
            {
                for (int i = 0; i < AllRPGEnchants.Length; i++)
                {

                    var forLevel = AllEnchantments;
                    var takeMin = Math.Min(Settings.Rarities[i].NumEnchantments, forLevel.Length);
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
                            if(t == coreEnchant)
                            {
                                result[m] = result[0];
                                result[0] = t;
                            }
                        }
                    }
                    if(result[0] != coreEnchant)
                    {
                        result[0] = coreEnchant;
                    }
                    for(int len = 0; len < takeMin; len++)
                    {
                        enchs[len] = AllEnchantments[result[len]];
                    }

                    var oldench = enchs.First().Enchantment;
                    SortedList<String, ResolvedEnchantment[]> enchants = AllRPGEnchants[i];
                    Console.WriteLine("Generated raw " + Settings.Rarities[i].Label + " weapon enchantment of " + oldench.Name);
                    if (!enchants.ContainsKey(Settings.Rarities[i].Label + " " + oldench.Name))
                    {
                        enchants.Add(Settings.Rarities[i].Label + " " + oldench.Name, enchs);
                    }
                }
            }

        }

        private FormKey newGenerateEnchantment( int rarity)
        {
            int rarityEnchCount = Settings.Rarities[rarity].NumEnchantments;
            var takeMin = Math.Min(rarityEnchCount, AllRPGEnchants[rarity].Count);
            var array = AllRPGEnchants[rarity].ToArray();
            var effects = array.ElementAt(r.Next(0, AllRPGEnchants[rarity].Count)).Value;

            var oldench = effects.First().Enchantment;

            Console.WriteLine("Generated " + Settings.Rarities[rarity].Label + " weapon enchantment of " + oldench.Name);
            if (ChosenRPGEnchants[rarity].ContainsKey(Settings.Rarities[rarity].Label + " " + oldench.Name))
            {
                return ChosenRPGEnchants[rarity].GetValueOrDefault(Settings.Rarities[rarity].Label + " " + oldench.Name);
            }

            var key = State.PatchMod.GetNextFormKey();
            var nrec = State.PatchMod.ObjectEffects.AddNewLocking(key);
            nrec.DeepCopyIn(effects.First().Enchantment);
            nrec.EditorID = "HAL_WEAPON_ENCH_" +Settings.Rarities[rarity].Label.ToUpper() + "_" + oldench.EditorID;
            nrec.Name = Settings.Rarities[rarity].Label + " " + oldench.Name;
            nrec.Effects.Clear();
            nrec.Effects.AddRange(effects.SelectMany(e => e.Enchantment.Effects).Select(e => e.DeepCopy()));
            nrec.WornRestrictions.SetTo(effects.First().Enchantment.WornRestrictions);

            ChosenRPGEnchants[rarity].Add(Settings.Rarities[rarity].Label + " " + oldench.Name, nrec.FormKey);
            ChosenRPGEnchantEffects[rarity].Add(nrec.FormKey, effects);
            return nrec.FormKey;
        }

        private FormKey enchantItem(ResolvedListItem<IWeaponGetter> item,int rarity)
        {


            string itemName = "";
            if (!(item.Resolved?.Name?.TryLookup(Language.English, out itemName) ?? false))
            {
                itemName = MakeName(item.Resolved.EditorID);
            }

            var nitm = State.PatchMod.Weapons.AddNewLocking(State.PatchMod.GetNextFormKey());
            var nrec = newGenerateEnchantment(rarity);
            var effects = ChosenRPGEnchantEffects[rarity].GetValueOrDefault(nrec);
            nitm.DeepCopyIn(item.Resolved);
            nitm.EditorID = "HAL_WEAPON_" + Settings.Rarities[rarity].Label.ToUpper() + "_" + nitm.EditorID;
            nitm.ObjectEffect.SetTo(nrec);
            nitm.EnchantmentAmount = (ushort)effects.Where(e => e.Amount.HasValue).Sum(e => e.Amount.Value);
            nitm.Name = Settings.Rarities[rarity].Label + " " + itemName + " of " + effects.First().Enchantment.Name;



            return nitm.FormKey;
        }

            public void newGenerate()
        {
            foreach (var ench in AllUnenchantedItems)
            {
                var lst = State.PatchMod.LeveledItems.AddNewLocking(State.PatchMod.GetNextFormKey());
                lst.DeepCopyIn(ench.List);
                lst.EditorID = "HAL_TOP_LList" + ench.Resolved.EditorID;
                lst.Entries!.Clear();
                lst.Flags &= ~LeveledItem.Flag.UseAll;
                for(int i = 0; i < Settings.VarietyCountPerItem; i++)
                {
                    var level = ench.Entry.Data.Level;
                    var forLevel = ByLevelIndexed[level];
                    if (forLevel.Length.Equals(0)) continue;

                    var itm = enchantItem(ench, randomRarity());
                    var entry = ench.Entry.DeepCopy();
                    entry.Data!.Reference.SetTo(itm);
                    lst.Entries.Add(entry);
                }

            }
        }

        public int randomRarity()
        {
            int rar = 0;
            int total = 0;
            for(int i = 0; i < Settings.Rarities.Count; i++)
            {
                total += Settings.Rarities[i].LLEntries;
            }
            int roll = r.Next(0, total);
            while(roll > Settings.Rarities[rar].LLEntries && rar < Settings.Rarities.Count)
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
using System;
using System.Collections.Generic;
using System.Linq;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins;

namespace HalgarisRPGLoot
{
    public static class Extensions
    {
        private static EnchantmentLevelSettings LevelSettings = Program.Settings.EnchantmentLevelSettings;

        public static Random Random { get; set; } = new Random(Program.Settings.RandomSeed);

        public static T RandomItem<T>(this T[] itms)
        {
            return itms[Random.Next(0, (itms.Length-1 <= 0 )? 1 : itms.Length-1)];
        }

        public static IEnumerable<T> Repeatedly<T>(Func<T> f)
        {
            while (true)
                yield return f();
        }

        public static T[] Shuffle<T>(this IEnumerable<T> coll)
        {
            var itms = coll.ToArray();
            for (var i = 0; i < itms.Length / 2; i++)
            {
                var idx1 = Random.Next(0, itms.Length);
                var idx2 = Random.Next(0, itms.Length);
                var old = itms[idx1];
                itms[idx1] = itms[idx2];
                itms[idx2] = old;
            }

            return itms;
        }
        
        public static TC AddNewLocking<TC>(this Group<TC> itms, FormKey val)
        where TC : class, ISkyrimMajorRecordInternal
        {
            lock (itms)
            {
                return itms.AddNew(val);
            }
        }

        public static Exception IncompatibleLoadOrderException
        {
            get
            {
                return IncompatibleLoadOrderException;
            }
        }
    }
}

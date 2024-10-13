using System.Collections.Generic;
using System.Linq;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins;


namespace SynthesisRPGLoot
{
    public static class Extensions
    {
        public static TC AddNewLocking<TC>(this SkyrimGroup<TC> items, FormKey val)
        where TC : class, ISkyrimMajorRecordInternal
        {
            lock (items)
            {
                return items.AddNew(val);
            }
        }
        public static bool CheckKeywords( IEnumerable<IFormLinkGetter<IKeywordGetter>> kws)
        {
            return kws.Any(itemKeyword => Program.Settings.GeneralSettings.UntouchableEquipmentKeywords.Contains(itemKeyword));
        }
    }
}

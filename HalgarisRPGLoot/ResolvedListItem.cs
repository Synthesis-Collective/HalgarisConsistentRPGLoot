using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

namespace HalgarisRPGLoot
{
    public class ResolvedListItem<TEnchantedItemTypeGetter>
        where TEnchantedItemTypeGetter : class, IMajorRecordGetter
    {
        public ILeveledItemGetter List { get; set; }
        public TEnchantedItemTypeGetter Resolved { get; set; }
        public ILeveledItemEntryGetter Entry { get; set; }
    }
}
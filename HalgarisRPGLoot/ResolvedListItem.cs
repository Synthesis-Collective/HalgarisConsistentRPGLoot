using Mutagen.Bethesda;
using Mutagen.Bethesda.Skyrim;

namespace HalgarisRPGLoot
{
    public class ResolvedListItem<TEnchantedItemType, TEnchantedItemTypeGetter>
        where TEnchantedItemType : class, IMajorRecord
        where TEnchantedItemTypeGetter : class, IMajorRecordGetter
    {
        public ILeveledItemGetter List { get; set; }
        public TEnchantedItemTypeGetter Resolved { get; set; }
        public ILeveledItemEntryGetter Entry { get; set; }
    }
}
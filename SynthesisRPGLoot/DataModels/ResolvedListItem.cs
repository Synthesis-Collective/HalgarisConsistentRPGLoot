using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

namespace SynthesisRPGLoot.DataModels
{
    public class ResolvedListItem<TEnchantedItemTypeGetter>
        where TEnchantedItemTypeGetter : class, IMajorRecordGetter
    {
        public ILeveledItemGetter List { get; init; }
        public TEnchantedItemTypeGetter Resolved { get; init; }
        public ILeveledItemEntryGetter Entry { get; init; }
    }
}
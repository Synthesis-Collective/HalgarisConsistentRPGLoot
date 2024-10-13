using Mutagen.Bethesda.Skyrim;

namespace SynthesisRPGLoot.DataModels
{
    public class ResolvedEnchantment
    {
        public short Level { get; set; }
        public IObjectEffectGetter Enchantment { get; set; }
        public ushort? Amount { get; set; }
    }
}
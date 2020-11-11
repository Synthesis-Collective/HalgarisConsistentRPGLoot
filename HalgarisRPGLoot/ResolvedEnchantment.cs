using Mutagen.Bethesda;
using Mutagen.Bethesda.Skyrim;

namespace HalgarisRPGLoot
{
    public class ResolvedEnchantment
    {
        public short Level { get; set; }
        public IObjectEffectGetter Enchantment { get; set; }
        public ushort? Amount { get; set; }
    }
}
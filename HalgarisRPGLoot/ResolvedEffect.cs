using Mutagen.Bethesda.Skyrim;

namespace HalgarisRPGLoot
{
    public class ResolvedEffect
    {
        public ResolvedEnchantment Enchantment { get; set; }
        public IEffectGetter EffectData { get; set; }
        public IMagicEffectGetter MagicEffect { get; set; }
    }
}
using Mutagen.Bethesda.WPF.Reflection.Attributes;

// ReSharper disable ConvertToConstant.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable CollectionNeverUpdated.Global

namespace SynthesisRPGLoot.Settings
{
    public class Settings
    {
        [MaintainOrder] public GeneralSettings GeneralSettings = new();

        [MaintainOrder] public EnchantmentSettings EnchantmentSettings = new();
        
        [MaintainOrder] public NamingGeneratorSettings NamingGeneratorSettings = new();

        [MaintainOrder] public RarityAndVariationDistributionSettings RarityAndVariationDistributionSettings = new();
    }
    
}
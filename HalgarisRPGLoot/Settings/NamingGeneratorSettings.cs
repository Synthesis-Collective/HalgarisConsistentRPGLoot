using Mutagen.Bethesda.Synthesis.Settings;
using Mutagen.Bethesda.WPF.Reflection.Attributes;

// ReSharper disable ConvertToConstant.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable CollectionNeverUpdated.Global


namespace HalgarisRPGLoot.Settings;

public class NamingGeneratorSettings
{
    [MaintainOrder]
    [SynthesisSettingName("Enchantment Separator")]
    [SynthesisDescription("This is the string(text) that will be written between the enchantments listed in enchantments and item names.")]
    [SynthesisTooltip("This is the string(text) that will be written between the enchantments listed in enchantments and item names.")]
    public string EnchantmentSeparator = ", ";
        
    [MaintainOrder]
    [SynthesisSettingName("Last Enchantment Separator")]
    [SynthesisDescription("This is the string(text) that will be written between the two last enchantments listed in enchantments and item names.")]
    [SynthesisTooltip("This is the string(text) that will be written between the two last enchantments listed in enchantments and item names.")]
    public string LastEnchantmentSeparator = " and ";
    
    [MaintainOrder]
    [SynthesisSettingName("Syllabore Settings")]
    [SynthesisDescription("The Settings used for Random Name Generation.")]
    [SynthesisTooltip("The Settings used for Random Name Generation.")]
    public SyllaboreSettings SyllaboreSettings = new SyllaboreSettings();
}
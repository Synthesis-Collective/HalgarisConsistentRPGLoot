

// ReSharper disable ConvertToConstant.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable CollectionNeverUpdated.Global


using Mutagen.Bethesda.Synthesis.Settings;
using Mutagen.Bethesda.WPF.Reflection.Attributes;

namespace SynthesisRPGLoot.Settings.SyllaboreSettings;

public class SyllaboreSettings
{
    [MaintainOrder]
    [SynthesisSettingName("Syllable Character Settings")]
    public SyllableSettings SyllableSettings = new();
    
    [MaintainOrder]
    [SynthesisSettingName("Probabilities")]
    public Probabilities Probabilities = new();
    
    [MaintainOrder]
    [SynthesisSettingName("Filters")]
    public Filters Filters = new();
}
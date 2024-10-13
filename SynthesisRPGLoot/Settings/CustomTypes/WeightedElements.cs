using Mutagen.Bethesda.Synthesis.Settings;


// ReSharper disable ConvertToConstant.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable CollectionNeverUpdated.Global


namespace SynthesisRPGLoot.Settings.CustomTypes;

public class WeightedElements<TType> where TType : class
{
    [SynthesisSettingName("Weighted List Element")]
    [SynthesisDescription("The content of the weighted list element.")]
    [SynthesisTooltip("The content of the weighted list element.")]
    public TType Element { get; set; } = default!;
    
    [SynthesisSettingName("Element Weight")]
    [SynthesisDescription("The weight of the weighted list element.")]
    [SynthesisTooltip("The weight of the weighted list element.")]
    public int Weight { get; set; } = 1;
}
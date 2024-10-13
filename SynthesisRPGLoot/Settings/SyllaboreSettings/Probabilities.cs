using Mutagen.Bethesda.WPF.Reflection.Attributes;
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace SynthesisRPGLoot.Settings.SyllaboreSettings;

public class Probabilities
{
    //For Each Syllable
    [MaintainOrder]
    public double OfLeadingConsonants { get; set; } = 0.95;
    [MaintainOrder]
    public double OfVowelsExits { get; set; } = 1.00;
    [MaintainOrder]
    public double OfTrailingConsonants { get; set; } = 0.10;
    [MaintainOrder]
    public double OfFinalConsonants { get; set; } = 0.50;
    
    //For Each Starting Syllable
    [MaintainOrder]
    public double OfLeadingVowelsInStartingSyllable { get; set; } = 0.0;
    
}
using Mutagen.Bethesda.WPF.Reflection.Attributes;

namespace HalgarisRPGLoot.Settings.SyllaboreSettings;

public class Probabilities
{
    //For Each Syllable
    [MaintainOrder]
    public double OfLeadingConsonants { get; set; } = 0.95;
    [MaintainOrder]
    public double OfLeadingConsonantIsSequence { get; set; } = 0.25;
    [MaintainOrder]
    public double OfVowelsExits { get; set; } = 1.00;
    [MaintainOrder]
    public double OfVowelsExitIsSequence { get; set; } = 0.25;
    [MaintainOrder]
    public double OfTrailingConsonants { get; set; } = 0.10;
    [MaintainOrder]
    public double OfTrailingConsonantIsSequence { get; set; } = 0.25;
    
    //For Each Starting Syllable
    [MaintainOrder]
    public double OfLeadingVowelIsInStartingSyllable { get; set; } = 0.0;
    [MaintainOrder]
    public double OfLeadingVowelIsSequenceInStartingSyllable { get; set; } = 0.0;
    
}
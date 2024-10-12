using System.Collections.Generic;
using Mutagen.Bethesda.Synthesis.Settings;
using HalgarisRPGLoot.Settings.CustomTypes;
using HalgarisRPGLoot.Settings.Enums;
using Mutagen.Bethesda.WPF.Reflection.Attributes;


// ReSharper disable ConvertToConstant.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable CollectionNeverUpdated.Global

namespace HalgarisRPGLoot.Settings.SyllaboreSettings;

public class SyllableSettings
{
    [MaintainOrder]
    [SynthesisSettingName("Usable Vowels")]
    [SynthesisDescription("Vowels used for the Syllable generation.")]
    [SynthesisTooltip("Vowels used for the Syllable generation.")]
    public List<WeightedElements<string>> WithVowels = new()
    {
        new(){Element = "a"},
        new(){Element = "e"},
        new(){Element = "i"},
        new(){Element = "o"},
        new(){Element = "u"},
    };

    [MaintainOrder]
    [SynthesisSettingName("Consonants Mode")]
    [SynthesisDescription(
        "Modifies if just one consonants list gets used or if you define the onsets and codas in AdvancedMode.")]
    [SynthesisTooltip(
        "Modifies if just one consonants list gets used or if you define the onsets and codas in AdvancedMode.")]
    public ConsonantRulesMode ConsonantRulesMode = ConsonantRulesMode.AdvancedMode;
    
    [MaintainOrder]
    [SynthesisSettingName("Usable Consonants (Basic)")]
    [SynthesisDescription("Consonants used for the Syllable generation. Ignored in advanced mode.")]
    [SynthesisTooltip(" Consonants used for the Syllable generation. Ignored in advanced mode.")]
    public List<WeightedElements<string>> WithConsonants = new ()
    {
        new(){Element = "b"}, new(){Element = "c"}, new(){Element = "d"}, new(){Element = "f"},
        new(){Element = "g"}, new(){Element = "h"}, new(){Element = "j"}, new(){Element = "k"}, 
        new(){Element = "l"}, new(){Element = "m"}, new(){Element = "n"}, new(){Element = "p"},
        new(){Element = "q"}, new(){Element = "r"}, new(){Element = "s"}, new(){Element = "t"},
        new(){Element = "v"}, new(){Element = "w"}, new(){Element = "x"}, new(){Element = "y"},
        new(){Element = "z"}
    };
    
    [MaintainOrder]
    [SynthesisSettingName("Onsets (Advanced)")]
    [SynthesisDescription("Onsets (leading consonants) used for the Syllable generation.")]
    [SynthesisTooltip("Onsets (leading consonants) used for the Syllable generation.)")]
    public List<WeightedElements<string>> WithLeadingConsonants = new ()
    {
        new(){Element = "b"}, new(){Element = "c"}, new(){Element = "d"}, new(){Element = "f"},
        new(){Element = "g"}, new(){Element = "h"}, new(){Element = "j"}, new(){Element = "k"}, 
        new(){Element = "l"}, new(){Element = "m"}, new(){Element = "n"}, new(){Element = "p"},
        new(){Element = "q"}, new(){Element = "r"}, new(){Element = "s"}, new(){Element = "t"},
        new(){Element = "v"}, new(){Element = "w"}, new(){Element = "x"}, new(){Element = "y"},
        new(){Element = "z"}
    };// Onsets (Grammatically Speaking)
    
    [MaintainOrder]
    [SynthesisSettingName("Codas (Advanced)")]
    [SynthesisDescription("Codas (trailing consonants) used for the Syllable generation.")]
    [SynthesisTooltip("Codas (trailing consonants) used for the Syllable generation.)")]
    public List<WeightedElements<string>> WithTrailingConsonants = new()
    {
        new(){Element = "b"}, new(){Element = "c"}, new(){Element = "d"}, new(){Element = "f"},
        new(){Element = "g"}, new(){Element = "h"}, new(){Element = "j"}, new(){Element = "k"}, 
        new(){Element = "l"}, new(){Element = "m"}, new(){Element = "n"}, new(){Element = "p"},
        new(){Element = "q"}, new(){Element = "r"}, new(){Element = "s"}, new(){Element = "t"},
        new(){Element = "v"}, new(){Element = "w"}, new(){Element = "x"}, new(){Element = "y"},
        new(){Element = "z"}
    }; // Codas (Grammatically Speaking)

    [MaintainOrder]
    [SynthesisSettingName("Vowel Sequences")]
    [SynthesisDescription("Vowel sequences that should be used together for the Syllable generation.")]
    [SynthesisTooltip("Vowel sequences that should be used together for the Syllable generation.)")]
    public List<WeightedElements<List<string>>> VowelSequences = new()
    {
        new(){Element = ["ai","oi"]},
        new(){Element = ["ou","ui"]},
        new(){Element = ["ae","ue"]},
    }; //Vowels Commonly used Together
    
    [MaintainOrder]
    [SynthesisSettingName("Onset Sequences")]
    [SynthesisDescription("Onset sequences that should be used for the Syllable generation.")]
    [SynthesisTooltip("Onset sequences that should be used for the Syllable generation.)")]
    public List<WeightedElements<List<string>>> LeadingConsonantSequences = new()
    {
        new(){Element = ["wh", "fr"]},
        new(){Element = ["st", "br","ph"]},
    }; //Onset clusters
    
    [MaintainOrder]
    [SynthesisSettingName("Coda Sequences")]
    [SynthesisDescription("Coda sequences that should be used for the Syllable generation.")]
    [SynthesisTooltip("Coda sequences that should be used for the Syllable generation.)")]
    public List<WeightedElements<List<string>>> TrailingConsonantSequences = new()
    {
        new(){Element = ["ld","rd"]},
        new(){Element = ["st", "rn", "ln"]},
    }; //Coda clusters
}
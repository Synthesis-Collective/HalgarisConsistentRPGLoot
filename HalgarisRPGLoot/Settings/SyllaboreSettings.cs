using System;
using System.Collections.Generic;
using System.Linq;
using Mutagen.Bethesda.Synthesis.Settings;
using HalgarisRPGLoot.DataModels;
using HalgarisRPGLoot.Settings.CustomTypes;
using HalgarisRPGLoot.Settings.Enums;
using Mutagen.Bethesda.WPF.Reflection.Attributes;
using Syllabore;

// ReSharper disable ConvertToConstant.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable CollectionNeverUpdated.Global


namespace HalgarisRPGLoot.Settings;

public class SyllaboreSettings
{
    
    //TODO: Rewrite to account for https://github.com/kesac/Syllabore/wiki/Guide-1.1.4%EA%9E%89-Frequencies
    
    [MaintainOrder]
    [SynthesisSettingName("Usable Vowels")]
    [SynthesisDescription("Vowels used for the Syllable generation.")]
    [SynthesisTooltip("Vowels used for the Syllable generation.")]
    public List<WeightedObject<string>> WithVowels = new()
    {
        new(){Object = "a", Weight = 1},
        new(){Object = "e", Weight = 1},
        new(){Object = "i", Weight = 1},
        new(){Object = "o", Weight = 1},
        new(){Object = "u", Weight = 1},
    };

    [MaintainOrder]
    public List<(string, int)> WithVowels2 = [("a",1), ("e",1), ("i",1), ("o",1), ("u",1)];

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
    public string WithConsonants;
    
    [MaintainOrder]
    [SynthesisSettingName("Onsets (Advanced)")]
    [SynthesisDescription("Onsets (leading consonants) used for the Syllable generation.")]
    [SynthesisTooltip("Onsets (leading consonants) used for the Syllable generation.)")]
    public string WithLeadingConsonants; // Onsets (Grammatically Speaking)
    
    [MaintainOrder]
    [SynthesisSettingName("Codas (Advanced)")]
    [SynthesisDescription("Codas (trailing consonants) used for the Syllable generation.")]
    [SynthesisTooltip("Codas (trailing consonants) used for the Syllable generation.)")]
    public string WithTrailingConsonants; // Codas (Grammatically Speaking)
    
    [MaintainOrder]
    [SynthesisSettingName("Vowel Sequences")]
    [SynthesisDescription("Vowel sequences that should be used together for the Syllable generation.")]
    [SynthesisTooltip("Vowel sequences that should be used together for the Syllable generation.)")]
    public string[] VowelSequences; //Vowels Commonly used Together
    
    [MaintainOrder]
    [SynthesisSettingName("Onset Sequences")]
    [SynthesisDescription("Onset sequences that should be used for the Syllable generation.")]
    [SynthesisTooltip("Onset sequences that should be used for the Syllable generation.)")]
    public string[] LeadingConsonantSequences; //Onset clusters
    
    [MaintainOrder]
    [SynthesisSettingName("Coda Sequences")]
    [SynthesisDescription("Coda sequences that should be used for the Syllable generation.")]
    [SynthesisTooltip("Coda sequences that should be used for the Syllable generation.)")]
    public string[] TrailingConsonantSequences; //Coda clusters
    
}
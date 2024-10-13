using System;
using HalgarisRPGLoot.Settings.Enums;
using Syllabore;

namespace HalgarisRPGLoot.Generators;

public class ConfiguredNameGenerator : NameGenerator
{
    //TODO: Implement the Name Generator in the Code
    
    public ConfiguredNameGenerator(int seedSalt)
    {
        Random random = new(Program.Settings.GeneralSettings.RandomGenerationSeed+seedSalt);
        var syllaboreSettings = Program.Settings.NamingGeneratorSettings.SyllaboreSettings;
        
        //Add Syllable Settings
        var syllableGenerator = new DefaultSyllableGenerator();
        syllableGenerator.WithRandom(random);
        
        foreach (var weightedVowels in syllaboreSettings.SyllableSettings.WithVowels)
        {
            syllableGenerator.WithVowels(weightedVowels.Element).Weight(weightedVowels.Weight);
        }

        switch (syllaboreSettings.SyllableSettings.ConsonantRulesMode)
        {
            case ConsonantRulesMode.BasicMode:
            {
                foreach (var weightedConsonants in syllaboreSettings.SyllableSettings.WithConsonants)
                {
                    syllableGenerator.WithConsonants(weightedConsonants.Element).Weight(weightedConsonants.Weight);
                }
                break;
            }
            case ConsonantRulesMode.AdvancedMode:
            {
                foreach (var weightedLeadingConsonant in syllaboreSettings.SyllableSettings.WithLeadingConsonants)
                {
                    syllableGenerator.WithLeadingConsonants(weightedLeadingConsonant.Element)
                        .Weight(weightedLeadingConsonant.Weight);
                }

                foreach (var weightedTrailingConsonant in syllaboreSettings.SyllableSettings.WithTrailingConsonants)
                {
                    syllableGenerator.WithTrailingConsonants(weightedTrailingConsonant.Element)
                        .Weight(weightedTrailingConsonant.Weight);
                }
                
                break;
            }
        }

        foreach (var weightedVowelSequence in syllaboreSettings.SyllableSettings.VowelSequences)
        {
            syllableGenerator.WithVowelSequences(weightedVowelSequence.Element.ToArray())
                .Weight(weightedVowelSequence.Weight);
        }

        foreach (var weightedLeadingConsonantSequence in syllaboreSettings.SyllableSettings.LeadingConsonantSequences)
        {
            syllableGenerator.WithLeadingConsonantSequences(weightedLeadingConsonantSequence.Element.ToArray())
                .Weight(weightedLeadingConsonantSequence.Weight);
        }

        foreach (var weightedTrailingConsonantSequence in syllaboreSettings.SyllableSettings.TrailingConsonantSequences)
        {
            syllableGenerator.WithTrailingConsonants(weightedTrailingConsonantSequence.Element.ToArray())
                .Weight(weightedTrailingConsonantSequence.Weight);
        }
        
        UsingSyllables(syllableGenerator);

        //Add Probability Settings
        UsingProbability(p => p
            .OfLeadingConsonants(syllaboreSettings.Probabilities.OfLeadingConsonants)
            .OfTrailingConsonants(syllaboreSettings.Probabilities.OfTrailingConsonants)
            .OfFinalConsonants(syllaboreSettings.Probabilities.OfFinalConsonants)
            .OfVowels(syllaboreSettings.Probabilities.OfVowelsExits)
            .OfLeadingVowelsInStartingSyllable(syllaboreSettings.Probabilities.OfLeadingVowelsInStartingSyllable)
        );
        //Add the Seeded Random
        UsingRandom(random);

        //Add Filter Settings
        var filter = new NameFilter();

        foreach (var notAllowed in syllaboreSettings.Filters.DoNotAllow)
        {
            filter.DoNotAllow(notAllowed);
        }

        foreach (var notAllowed in syllaboreSettings.Filters.DoNotAllowSubstring)
        {
            filter.DoNotAllowSubstring(notAllowed);
        }

        foreach (var notAllowed in syllaboreSettings.Filters.DoNotAllowEnd)
        {
            filter.DoNotAllowEnding(notAllowed);
        }
        
        foreach (var notAllowed in syllaboreSettings.Filters.DoNotAllowStart)
        {
            filter.DoNotAllowStart(notAllowed);
        }

        UsingFilter(filter);
        
    }
    
}
using System.Collections.Generic;
using Mutagen.Bethesda.Synthesis.Settings;
using Mutagen.Bethesda.WPF.Reflection.Attributes;

namespace SynthesisRPGLoot.Settings.SyllaboreSettings;

public class Filters
{
    [MaintainOrder]
    [SynthesisDescription("This allows the use of regular expression matching filters.")]
    [SynthesisTooltip("This allows the use of regular expression matching filters.")]
    public List<string> DoNotAllow = [
        "([^aieou]{3})",
        "(q[^u])",
        "(y[^aeiou])",
        "([^tsao]w)",
        "(p[^aeioustrlh])"
    ];
    [MaintainOrder]
    public List<string> DoNotAllowStart = [];
    [MaintainOrder]
    public List<string> DoNotAllowSubstring = ["pn", "zz", "yy", "xx"];
    [MaintainOrder]
    public List<string> DoNotAllowEnd = [];
}
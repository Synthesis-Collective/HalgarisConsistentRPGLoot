using System.Collections.Generic;
using Mutagen.Bethesda.FormKeys.SkyrimSE;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis.Settings;
using Mutagen.Bethesda.WPF.Reflection.Attributes;
// ReSharper disable ConvertToConstant.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable CollectionNeverUpdated.Global


namespace SynthesisRPGLoot.Settings;

public class GeneralSettings
{
    [MaintainOrder] public int RandomGenerationSeed = 42;
        
    [MaintainOrder]
    [SynthesisSettingName("LeveledList Flags")]
    [SynthesisDescription("Flags that will be set on generated LeveledLists")]
    [SynthesisTooltip("Flags that will be set on generated LeveledLists")]
    public LeveledListFlagSettings LeveledListFlagSettings = new();

    [MaintainOrder]
    [SynthesisSettingName("Untouchable Equipment Keywords")]
    [SynthesisDescription("Keywords that define Items you don't want processed.")]
    [SynthesisTooltip("Keywords that define Items you don't want processed.")]
    public HashSet<IFormLinkGetter<IKeywordGetter>> UntouchableEquipmentKeywords =
        new()
        {
            Skyrim.Keyword.MagicDisallowEnchanting,
            Skyrim.Keyword.DaedricArtifact,
            Skyrim.Keyword.WeapTypeStaff
        };
}
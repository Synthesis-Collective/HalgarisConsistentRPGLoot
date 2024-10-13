using System.Collections.Generic;
using SynthesisRPGLoot.Settings.Enums;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.FormKeys.SkyrimSE;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Synthesis.Settings;
using Mutagen.Bethesda.WPF.Reflection.Attributes;
// ReSharper disable ConvertToConstant.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable CollectionNeverUpdated.Global


namespace SynthesisRPGLoot.Settings;

public class EnchantmentSettings
{
    [MaintainOrder]
    [SynthesisSettingName("Enchantment List Mode")]
    [SynthesisDescription(
        "Blacklist: Selected Enchantments wont be distributed.\nWhitelist: Only the selected Enchantments get Distributed.")]
    [SynthesisTooltip(
        "Blacklist: Selected Enchantments wont be distributed.\nWhitelist: Only the selected Enchantments get Distributed.")]
    public ListMode EnchantmentListMode = ListMode.Blacklist;

    [MaintainOrder] [SynthesisSettingName("Enchantment List")] [SynthesisDescription("List of Enchantments")]
    public HashSet<IFormLinkGetter<IObjectEffectGetter>> EnchantmentList = new()
    {
        Skyrim.ObjectEffect.BoundBattleaxeEnchantment,
        Skyrim.ObjectEffect.BoundBowEnchantment,
        Skyrim.ObjectEffect.BoundSwordEnchantment,
        Dragonborn.ObjectEffect.BoundDaggerEnchantment
    };

    [MaintainOrder]
    [SynthesisSettingName("Plugin List Mode")]
    [SynthesisDescription(
        "Blacklist: Enchantments of selected Plugins wont be distributed." +
        "\nWhitelist: Only the Enchantments of selected Plugins  get Distributed.")]
    [SynthesisTooltip(
        "Blacklist: Enchantments of selected Plugins wont be distributed." +
        "\nWhitelist: Only the Enchantments of selected Plugins  get Distributed.")]
    public ListMode PluginListMode = ListMode.Blacklist;

    [MaintainOrder] [SynthesisSettingName("Plugin List")] [SynthesisDescription("List of Plugins")]
    public HashSet<ModKey> PluginList = new();
}

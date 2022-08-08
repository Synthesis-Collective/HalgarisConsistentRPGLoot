using System.Collections.Generic;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using IArmorGetter = Mutagen.Bethesda.Skyrim.IArmorGetter;

namespace HalgarisRPGLoot.Analyzers
{
    public class ConstructibleObjectAnalyzer
    {
        public Dictionary<IFormLinkGetter<IWeaponGetter>, IConstructibleObjectGetter> WeaponDictionary { get; } = new ();

        public Dictionary<IFormLinkGetter<IArmorGetter>, IConstructibleObjectGetter> ArmorDictionary { get; } = new();


        public ConstructibleObjectAnalyzer(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            foreach (var constructibleObjectGetter in state.LoadOrder.PriorityOrder.OnlyEnabled().ConstructibleObject()
                         .WinningOverrides())
            {
                if (constructibleObjectGetter.CreatedObject.TryResolve<IArmorGetter>(state.LinkCache,
                        out var armorGetter))
                {
                    ArmorDictionary.TryAdd(armorGetter.ToLink(), constructibleObjectGetter);
                }

                if (constructibleObjectGetter.CreatedObject.TryResolve<IWeaponGetter>(state.LinkCache,
                        out var weaponGetter))
                {
                    WeaponDictionary.TryAdd(weaponGetter.ToLink(), constructibleObjectGetter);
                }
            }

        }
    }
}
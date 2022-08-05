using System.Collections.Generic;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using IArmorGetter = Mutagen.Bethesda.Skyrim.IArmorGetter;

namespace HalgarisRPGLoot.Analyzers
{
    public class ConstructibleObjectAnalyzer
    {
        public Dictionary<IWeaponGetter,IConstructibleObjectGetter> WeaponDictionary { get; } =
            new Dictionary<IWeaponGetter,IConstructibleObjectGetter>();

        public Dictionary<IArmorGetter,IConstructibleObjectGetter> ArmorDictionary { get; } =
            new Dictionary<IArmorGetter,IConstructibleObjectGetter>();

        public ConstructibleObjectAnalyzer(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
           foreach (var constructibleObjectGetter in state.LoadOrder.PriorityOrder.OnlyEnabled().ConstructibleObject().WinningOverrides())
           {
               if (constructibleObjectGetter.CreatedObject.TryResolve<IArmorGetter>(state.LinkCache, out var armorGetter))
               {
                   ArmorDictionary.Add(armorGetter,constructibleObjectGetter);
               }
               if (constructibleObjectGetter.CreatedObject.TryResolve<IWeaponGetter>(state.LinkCache,out var weaponGetter))
               {
                   WeaponDictionary.Add(weaponGetter,constructibleObjectGetter);
               }

           }
        }

    }
}
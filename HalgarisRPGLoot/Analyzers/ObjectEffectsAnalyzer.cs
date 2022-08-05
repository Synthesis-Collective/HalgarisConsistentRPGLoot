using System.Collections.Generic;
using System.Linq;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;

namespace HalgarisRPGLoot.Analyzers
{
    public class ObjectEffectsAnalyzer
    {
        public Dictionary<FormKey, IObjectEffectGetter> AllObjectEffects { get; set; }
        
        private IPatcherState<ISkyrimMod, ISkyrimModGetter> State { get; set; }

        public ObjectEffectsAnalyzer(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            State = state;
            AllObjectEffects = State.LoadOrder.PriorityOrder.ObjectEffect().WinningOverrides()
                .Where(x => x.Name != null)
                .Where(k => !k.Name.ToString()!
                    .EndsWith("FX")) // Excluding Bound Weapon FX Object Effects as they don't do a thing on non bound weapons. 
                .ToDictionary(k => k.FormKey);
        }
    }
}
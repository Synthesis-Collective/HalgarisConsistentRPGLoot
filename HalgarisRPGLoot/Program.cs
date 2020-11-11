using System;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;

namespace HalgarisRPGLoot
{
    class Program
    {
        static int Main(string[] args)
        {
            return SynthesisPipeline.Instance.Patch<ISkyrimMod, ISkyrimModGetter>(
                args: args,
                patcher: RunPatch,
                new UserPreferences
                {
                    ActionsForEmptyArgs = new RunDefaultPatcher
                    {
                        IdentifyingModKey = "ENBLightPatcher.esp",
                        TargetRelease = GameRelease.SkyrimSE
                    }
                });
        }

        private static void RunPatch(SynthesisState<ISkyrimMod, ISkyrimModGetter> state)
        {
            var armor  = new ArmorAnalyzer(state);
            Console.WriteLine("Analyzing armor");
            armor.Analyze();
            Console.WriteLine("Generating armor enchantments");
            armor.Generate();
            
            var weapon = new WeaponAnalyzer(state);
            Console.WriteLine("Analyzing weapons");
            weapon.Analyze();
            Console.WriteLine("Generating weapon enchantments");
            weapon.Generate();
            
        }
    }
}
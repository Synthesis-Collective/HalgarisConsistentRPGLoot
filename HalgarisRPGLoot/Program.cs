using System;
using System.Threading;
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
            var weapon = new WeaponAnalyzer(state);
            
            Console.WriteLine("Analyzing mod list");
            var th1 = new Thread(() => armor.Analyze());
            var th2 = new Thread(() => weapon.Analyze());
            
            th1.Start();
            th2.Start();
            th1.Join();
            th2.Join();
            
            Console.WriteLine("Generating armor enchantments");
            armor.Generate();
            
            Console.WriteLine("Generating weapon enchantments");
            weapon.Generate();
            
        }
    }
}
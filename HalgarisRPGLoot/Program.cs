using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;

namespace HalgarisRPGLoot
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            return await SynthesisPipeline.Instance
                .AddPatch<ISkyrimMod, ISkyrimModGetter>(RunPatch)
                .Run(args, new RunPreferences()
                {
                    ActionsForEmptyArgs = new RunDefaultPatcher()
                    {
                        IdentifyingModKey = "HalgariRpgLoot.esp",
                        TargetRelease = GameRelease.SkyrimSE,
                    }
                });
        }

        private static void RunPatch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
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
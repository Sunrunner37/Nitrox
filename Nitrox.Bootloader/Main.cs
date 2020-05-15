﻿using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Nitrox.Bootloader
{
    public static class Main
    {
        private static readonly Lazy<string> nitroxLauncherDir = new Lazy<string>(() =>
        {
            string nitroxAppData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Nitrox");
            if (!Directory.Exists(nitroxAppData))
            {
                return null;
            }
            string nitroxLauncherPathFile = Path.Combine(nitroxAppData, "launcherpath.txt");
            if (!File.Exists(nitroxLauncherPathFile))
            {
                return null;
            }

            try
            {
                string valueInFile = File.ReadAllText(nitroxLauncherPathFile).Trim();
                return Directory.Exists(valueInFile) ? valueInFile : null;
            }
            catch
            {
                // ignored
            }
            return null;
        });
        
        public static void Execute()
        {
            if (nitroxLauncherDir.Value == null)
            {
                Console.WriteLine("Nitrox launcher path not set in AppData. Nitrox will not start.");
                return;
            }

            Environment.SetEnvironmentVariable("NITROX_LAUNCHER_PATH", nitroxLauncherDir.Value);
            Task.Factory.StartNew(() =>
            {
                try
                {
                    // Delete the path so that the launcher should be used to launch Nitrox
                    File.Delete(Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Nitrox"), "launcherpath.txt"));
                }
                catch (Exception)
                {
                    // ignored
                }
            });
            
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainOnAssemblyResolve;
            AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += CurrentDomainOnAssemblyResolve;

            BootstrapNitrox();
        }
        
        private static void BootstrapNitrox()
        {
            Assembly core = Assembly.Load(new AssemblyName("NitroxPatcher"));
            Type mainType = core.GetType("NitroxPatcher.Main");
            mainType.InvokeMember("Execute", BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod, null, null, null);
        }

        private static Assembly CurrentDomainOnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            string dllFileName = args.Name.Split(',')[0];
            if (!dllFileName.EndsWith(".dll"))
            {
                dllFileName += ".dll";
            }

            // Load DLLs where Nitrox launcher is first, if not found, use Subnautica's DLLs.
            string dllPath = Path.Combine(nitroxLauncherDir.Value, dllFileName);
            if (!File.Exists(dllPath))
            {
                dllPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), dllFileName);
            }
            
            if (!File.Exists(dllPath))
            {
                Console.WriteLine($"Nitrox dll missing: {dllPath}");
            }
            return Assembly.LoadFile(dllPath);
        }
    }
}

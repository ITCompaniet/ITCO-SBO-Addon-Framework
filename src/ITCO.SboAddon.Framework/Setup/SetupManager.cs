using ITCO.SboAddon.Framework.Services;
using System;
using System.Linq;
using System.Reflection;

namespace ITCO.SboAddon.Framework.Setup
{
    /// <summary>
    /// Setup Manager
    /// Uses setup version for faster startup
    /// </summary>
    public static class SetupManager
    {
        /// <summary>
        /// Find ISetup classes and Run Setup
        /// </summary>
        /// <param name="assembly">Assembly to search in</param>
        public static void FindAndRunSetups(Assembly assembly)
        {
            var setups = (from t in assembly.GetTypes()
                          where !t.IsInterface && t.GetInterfaces().Contains(typeof(ISetup))
                          select t);

            if (setups.Count() == 0)
                return;

            SettingService.Init();

            foreach (var setup in setups)
            {
                var setupInstance = Activator.CreateInstance(setup) as ISetup;
                var key = $"setup.lastversion.{setup.Name.Replace("Setup", string.Empty)}";
                var lastVersionInstalled = SettingService.GetSettingByKey(key, 0);
                if (lastVersionInstalled < setupInstance.Version)
                {
                    try
                    {
                        setupInstance.Run();
                        SettingService.SaveSetting(key, setupInstance.Version);
                    }
                    catch(Exception ex)
                    {
                        SboApp.Application.MessageBox($"Error in {setup.Name}: {ex.Message}");
                    }
                }
            }
        }
    }
}

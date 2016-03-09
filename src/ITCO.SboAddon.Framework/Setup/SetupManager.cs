using ITCO.SboAddon.Framework.Services;
using System;
using System.Linq;
using System.Reflection;
using SAPbouiCOM;

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
                          select t).ToArray();

            if (!setups.Any())
                return;

            SettingService.Init();

            foreach (var setup in setups)
            {
                var setupInstance = Activator.CreateInstance(setup) as ISetup;
                var key = $"setup.lastversion.{setup.Name.Replace("Setup", string.Empty)}";
                var lastVersionInstalled = SettingService.GetSettingByKey(key, 0);
                if (setupInstance != null && lastVersionInstalled < setupInstance.Version)
                {
                    try
                    {
                        SboApp.Application.StatusBar.SetText($"Running setup for {setup.Name}, current installed version is {lastVersionInstalled}, new version is {lastVersionInstalled})"
                            ,BoMessageTime.bmt_Medium, BoStatusBarMessageType.smt_Warning);

                        setupInstance.Run();
                        SettingService.SaveSetting(key, setupInstance.Version);
                    }
                    catch(Exception ex)
                    {
                        SboApp.Application.MessageBox($"Setup error in {setup.Name}: {ex.Message}");
                    }
                }

                SboApp.Application.StatusBar.SetText($"Setup for {setup.Name} is up-to-date! (v.{lastVersionInstalled})",
                    SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
            }
        }
    }
}

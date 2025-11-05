using System;
using System.Diagnostics;

namespace AdvancedVirus
{
    public class SystemManager
    {
        public void RebootSystem()
        {
            var reboot = new ProcessStartInfo("shutdown", "/r /f /t 0");
            reboot.CreateNoWindow = true;
            reboot.UseShellExecute = false;

            try
            {
                Process.Start(reboot);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Reboot error: {ex.Message}");
            }
        }
    }
}
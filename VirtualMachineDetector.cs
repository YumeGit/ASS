using System;
using System.Diagnostics;
using System.IO;

namespace AdvancedVirus
{
    public class VirtualMachineDetector
    {
        public bool IsRunningInVirtualMachine()
        {
            string model = GetComputerSystemModel();
            return IsVirtualMachine(model) && CheckVirtualBoxGuestAdditions();
        }

        private string GetComputerSystemModel()
        {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = "/c wmic computersystem get model",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            process.StartInfo = startInfo;
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            return output;
        }

        private bool IsVirtualMachine(string model)
        {
            if (model.Contains("VirtualBox") ||
                model.Contains("VMware") ||
                model.Contains("Virtual Machine") ||
                model.Contains("Hypervisor"))
            {
                return true;
            }
            return false;
        }

        private bool CheckVirtualBoxGuestAdditions()
        {
            string programFilesPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            programFilesPath += "\\Oracle\\VirtualBox Guest Additions";
            return Directory.Exists(programFilesPath);
        }
    }
}
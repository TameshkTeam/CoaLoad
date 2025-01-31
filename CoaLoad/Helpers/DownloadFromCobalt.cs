using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CoaLoad.Helpers;
// TODO: Add error handling and add video quality
public class DownloadFromCobalt
{
    public static async Task<string> GetDownloadUrl(string _cobaltInstance, string _mediaUrl)
    {
        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            var content = new StringContent("{\"url\":\"" + _mediaUrl + "\"}", Encoding.UTF8, "application/json");
            var response = await client.PostAsync(_cobaltInstance, content);
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<Dictionary<string, string>>(responseContent);
                Console.WriteLine(result["url"]);
                return result["url"];
            }
        }

        return string.Empty;
    }

    public static async Task<bool> DownloadFile(string _downloadUrl, string _filepath, IProgress<double> progress)
    {
        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            var response = await client.GetAsync(_downloadUrl, HttpCompletionOption.ResponseHeadersRead);
            if (response.IsSuccessStatusCode)
            {
                using (var contentStream = await response.Content.ReadAsStreamAsync())
                using (var fileStream = new FileStream(_filepath, FileMode.Create, FileAccess.Write, FileShare.None,
                           8192, true))
                {
                    var totalRead = 0L;
                    var buffer = new byte[8192];
                    var isMoreToRead = true;

                    do
                    {
                        var read = await contentStream.ReadAsync(buffer, 0, buffer.Length);
                        if (read == 0)
                        {
                            isMoreToRead = false;
                            progress.Report(totalRead / (1024d * 1024d));
                            continue;
                        }

                        await fileStream.WriteAsync(buffer, 0, read);
                        totalRead += read;
                        progress.Report(Math.Round(totalRead / (1024d * 1024d), 1));
                    } while (isMoreToRead);
                }

                return true;
            }
        }

        return false;
    }
}
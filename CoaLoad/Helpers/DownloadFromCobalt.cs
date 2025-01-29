using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CoaLoad.Helpers
{
    public class DownloadFromCobalt
    {
        public static async Task<bool> Download(string contentUrl, string downloadPath, string CobadltApiUrl)
        {
            if (string.IsNullOrEmpty(contentUrl) || string.IsNullOrEmpty(downloadPath))
            {
                throw new ArgumentNullException("Both contentUrl and downloadPath must be provided.");
            }

            Console.WriteLine($"Downloading file from {contentUrl} to {downloadPath}");

            // Create the request body
            var requestBody = new { url = contentUrl };
            using StringContent jsonContent = new(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json");

            using (var client = new HttpClient())
            {
                // Add required headers
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                //client.DefaultRequestHeaders.Add("Content-Type","application/json");

                try
                {
                    Console.WriteLine("Sending POST request to Cobalt API...");

                    // Send POST request
                    var response = await client.PostAsync(CobadltApiUrl, jsonContent);

                    Console.WriteLine($"Response status code: {response.StatusCode}");

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        return await HandleResponse(responseContent, downloadPath);
                    }
                    else
                    {
                        Console.WriteLine($"Download failed with status code: {response.StatusCode}");
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Download error: {ex.Message}");
                    return false;
                }
            }
        }

        private static async Task<bool> HandleResponse(string responseContent, string downloadPath)
        {
            try
            {
                using (JsonDocument document = JsonDocument.Parse(responseContent))
                {
                    JsonElement root = document.RootElement;
                    if (root.TryGetProperty("status", out JsonElement statusElement))
                    {
                        string status = statusElement.GetString();
                        if (status == "tunnel" || status == "redirect")
                        {
                            if (root.TryGetProperty("url", out JsonElement urlElement))
                            {
                                string downloadUrl = urlElement.GetString();
                                return await DownloadFile(downloadUrl, downloadPath);
                            }
                            else
                            {
                                Console.WriteLine("Cobalt API response missing 'url' property.");
                                return false;
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Cobalt API returned status: {status}. Full response: {responseContent}");
                            return false;
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Cobalt API response missing 'status' property. Full response: {responseContent}");
                        return false;
                    }
                }
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error parsing JSON response: {ex.Message}. Full response: {responseContent}");
                return false;
            }
        }

        private static async Task<bool> DownloadFile(string downloadUrl, string downloadPath)
        {
            Console.WriteLine($"Downloading file from {downloadUrl} to {downloadPath}");

            using (var client = new HttpClient())
            {
                using (var response = await client.GetAsync(downloadUrl))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        using (var fileStream = new FileStream(downloadPath, FileMode.Create))
                        {
                            await response.Content.CopyToAsync(fileStream);
                            return true;
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Failed to download file from {downloadUrl}");
                        return false;
                    }
                }
            }
        }
    }
}

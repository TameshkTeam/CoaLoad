using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace CoaLoad.Helpers
{
    public class DownloadFromCobalt
    {
        private const string CobaltApiUrl = "https://api.cobalt.tools";  // Corrected API URL

        public static async Task<bool> Download(string contentUrl, string downloadPath, string CobadltApiUrl)
        {
            // Validate arguments
            if (string.IsNullOrEmpty(contentUrl) || string.IsNullOrEmpty(downloadPath))
            {
                throw new ArgumentNullException("Both contentUrl and downloadPath must be provided.");
            }

            Console.WriteLine($"Downloading file from {contentUrl} to {downloadPath}");

            // Create the request body object for POST request
            var requestBody = new Dictionary<string, string>()
            {
                ["url"] = contentUrl
            };

            var jsonContent = JsonSerializer.Serialize(requestBody);
            var buffer = System.Text.Encoding.UTF8.GetBytes(jsonContent);

            using (var client = new HttpClient())
            {
                // Add necessary headers for POST request
                client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "application/json");
                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");

                // If authorization token is provided, add it to the headers
    

                try
                {
                    Console.WriteLine("Sending POST request to Cobalt API...");

                    // Send the POST request asynchronously
                    var response = await client.PostAsync(CobaltApiUrl, new ByteArrayContent(buffer));

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

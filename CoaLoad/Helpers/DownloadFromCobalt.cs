using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace CoaLoad.Helpers;

// AI GENERATED CODE
public class DownloadFromCobalt
{
public static async Task<bool> Download(string instance, string videoQuality, string contentUrl, string downloadPath)
{
    // Validate arguments
    if (string.IsNullOrEmpty(instance) || string.IsNullOrEmpty(videoQuality) || string.IsNullOrEmpty(contentUrl) || string.IsNullOrEmpty(downloadPath))
    {
        throw new ArgumentNullException("All arguments must be provided.");
    }
    Console.WriteLine("Downloading file from " + contentUrl + " to " + downloadPath + " with instance: " + instance + " and video quality: " + videoQuality);
    // Build the Cobalt API endpoint URL
    string url = $"{instance}/";
    // Create the request body object
    var requestBody = new Dictionary<string, string>()
    {
        ["url"] = contentUrl,
        ["videoQuality"] = videoQuality,
    };

    //  Prepare JSON content for request body
    var jsonContent = JsonSerializer.Serialize(requestBody);
    var buffer = System.Text.Encoding.UTF8.GetBytes(jsonContent);

    // Create the HTTP request with required headers
    using (var client = new HttpClient())
    {
        

        using (var request = new HttpRequestMessage(HttpMethod.Post, url) { Content = new ByteArrayContent(buffer) })
        {
            try
            {
                request.Headers.TryAddWithoutValidation("Content-Type", "application/json");
                request.Headers.TryAddWithoutValidation("Accept", "application/json");
                
                Console.WriteLine("Sending request to Cobalt API...");
                // Send the request asynchronously
                var response = await client.SendAsync(request);
                Console.WriteLine("Response status code: " + response.StatusCode.ToString());
                // Check for successful response
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();

                    // Deserialize using System.Text.Json
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
}

private static async Task<bool> DownloadFile(string downloadUrl, string downloadPath)
{
    Console.WriteLine("Downloading file from " + downloadUrl + " to " + downloadPath);
    // Download the file using HttpClient
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
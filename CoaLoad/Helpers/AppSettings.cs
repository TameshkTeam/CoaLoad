using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using CoaLoad.Views;

namespace CoaLoad.Helpers;

public class AppSettings
{
    public static async Task<bool> SetupSettingsDb(IStorageProvider storage)
    {
        try
        {
            // Get the app data path
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string appFolderPath = Path.Combine(appDataPath, "CoaLoad");

            // Ensure the directory exists
            if (!Directory.Exists(appFolderPath))
            {
                Directory.CreateDirectory(appFolderPath);
            }

            // Define the settings file path
            string settingsFilePath = Path.Combine(appFolderPath, "settings.json");

            // Check if the file exists; if it does, do nothing
            if (File.Exists(settingsFilePath))
            {
                return true;
            }

            // File doesn't exist; create it with default settings
            var defaultSettings = new SettingsValues
            {
                Instance = "",
                VideoQuality = ""
            };

            // Serialize using the source generator
            string jsonString = JsonSerializer.Serialize(defaultSettings);

            // Use the storage provider to create the file and write the default settings
            var bookmark = await storage.OpenFileBookmarkAsync(settingsFilePath);

            await File.WriteAllTextAsync(settingsFilePath,jsonString);
            

            return true;
        }
        catch (Exception ex)
        {
            // Log the exception or handle it as needed
            Console.WriteLine($"An error occurred: {ex.Message}");
            return false;
        }
    }




    public static async Task<bool> SaveSettings(SettingsValues s)
    {
        try
        {
            // Get the application data path (cross-platform support)
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            // Define the folder and file name
            string appFolderPath = Path.Combine(appDataPath, "CoaLoad");
            string settingsFilePath = Path.Combine(appFolderPath, "settings.json");

            // Ensure the directory exists
            if (!Directory.Exists(appFolderPath))
            {
                Directory.CreateDirectory(appFolderPath);
            }

            // Serialize the settings object to JSON
            string jsonString = JsonSerializer.Serialize(s, new JsonSerializerOptions { WriteIndented = true });

            // Write the JSON string to the file
            await File.WriteAllTextAsync(settingsFilePath, jsonString, Encoding.UTF8);

            return true; // Indicate success
        }
        catch (Exception ex)
        {
            // Log or handle the error as needed
            Console.WriteLine($"An error occurred while saving settings: {ex.Message}");
            return false; // Indicate failure
        }
    }

    
    public static async Task<SettingsValues> LoadSettings()
    {
        try
        {
            // Get app data path
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string appFolderPath = Path.Combine(appDataPath, "CoaLoad");
            string settingsFilePath = Path.Combine(appFolderPath, "settings.json");

            // Check if the file exists
            if (!File.Exists(settingsFilePath))
            {
                // If the file doesn't exist, return default settings
                return new SettingsValues { Instance = "", VideoQuality = "" };
            }

            // Read the JSON from the file
            string jsonString = await File.ReadAllTextAsync(settingsFilePath);

            // Deserialize the JSON string into a SettingsValues object
            var settings = JsonSerializer.Deserialize<SettingsValues>(jsonString);

            // Return the loaded settings or default if null
            return settings ?? new SettingsValues { Instance = "", VideoQuality = "" };
        }
        catch (Exception ex)
        {
            // Log the exception (replace with your logging mechanism)
            Console.WriteLine($"Error loading settings: {ex.Message}");

            // Return default settings in case of an error
            return new SettingsValues { Instance = "", VideoQuality = "" };
        }
    }

    
    public class SettingsValues
    {
        public  string Instance { get; set; }
        public  string VideoQuality { get; set; }
    }
}
using Flux.Encryption;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Flux.Save
{
    public static class PreferenceSaver
    {
        private const int CypherOffset = 5;

        public const string PrefsFolderName = "prefs";
        public const string PrefsFileName = "preferences.sav";
        public static readonly string PrefsFolderPath;
        public static readonly string PrefsFilePath;

        private static readonly Dictionary<string, object> preferences;
        private static readonly JsonSerializerSettings jsonSerializerSettings;

        private static CancellationTokenSource cancellationTokenSource;
        private static bool hasUnsavedChanges = false;

        static PreferenceSaver()
        {
            // Build file path
            PrefsFolderPath = Path.Combine(Application.persistentDataPath, PrefsFolderName);
            PrefsFilePath = Path.Combine(PrefsFolderPath, PrefsFileName);

            // Setup serializer settings
            jsonSerializerSettings = new()
            {
                // Adds type names to the json, so can easily deserialize to correct types
                TypeNameHandling = TypeNameHandling.All,

                // Ignore recursive fields that would result in infinite loop
                // (eg. Vector3 contains one of these)
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            // Read any cached preferences from file
            preferences = ReadFromFile();

            // Write any changes to file when quitting, if we have unsaved changes
            // (Note: This won't work with force-quits, and is not called on certain
            // platforms eg. mac, android etc.)
            Application.quitting += WriteToFile;
        }

        public static void Save<T>(string key, T data, float writeDelay = 0)
        {
            // Add new data to preferences
            preferences[key] = data;

            // Update unsaved changes flag
            hasUnsavedChanges = true;

            if (writeDelay > 0)
            {
                DelayedWriteToFile(writeDelay);
            }
            else
            {
                WriteToFile();
            }
        }

        public static bool Load<T>(string key, out T data)
        {
            if (preferences.TryGetValue(key, out var untypedData) && untypedData is T typedData)
            {
                data = typedData;
                return true;
            }
            else
            {
                data = default;
                return false;
            }
        }

        private static void CancelCurrentTask()
        {
            // If there's an existing cancellationTokenSource, cancel it
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
                cancellationTokenSource = null;
            }
        }

        private static async void DelayedWriteToFile(float delaySeconds)
        {
            // Cancel the current task, if we have one
            CancelCurrentTask();

            // Setup cancellation token for the new task
            cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            try
            {
                // Await the delay
                await Task.Delay(TimeSpan.FromSeconds(delaySeconds), cancellationToken);
            }
            catch (TaskCanceledException)
            {
                // Task was canceled, do nothing
                return;
            }

            // Finally, write to file
            WriteToFile();
        }

        private static void WriteToFile()
        {
            // Guard if we have no changes to write to file.
            if (!hasUnsavedChanges)
                return;

            // Cancel the current task, if we have one
            CancelCurrentTask();

            // Serialise prefs to string
            var serializedData = JsonConvert.SerializeObject(preferences, Formatting.Indented, jsonSerializerSettings);

            // Obfuscate serialized data before writing to file
            var obfuscatedData = Encryptor.Obfuscate(serializedData, CypherOffset);

            // Create directory (if needed)
            Directory.CreateDirectory(PrefsFolderPath);

            // Write the string data to file
            File.WriteAllText(PrefsFilePath, obfuscatedData);
            Debug.Log($"Wrote to : {PrefsFilePath}.");
        }

        private static Dictionary<string, object> ReadFromFile()
        {
            Dictionary<string, object> preferences;

            // Build preferences from file, if it exists
            if (File.Exists(PrefsFilePath))
            {
                // Read the serialized data from file
                string serializedData = File.ReadAllText(PrefsFilePath);

                // Deobfuscate serialized data after reading from file
                var deobfuscatedData = Encryptor.Deobfuscate(serializedData, CypherOffset);

                // Deserialize into preferences
                preferences = JsonConvert.DeserializeObject<Dictionary<string, object>>(deobfuscatedData, jsonSerializerSettings);
            }
            else
            {
                preferences = new();
            }

            return preferences;
        }
    }
}
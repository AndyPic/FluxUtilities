using Flux.Encryption;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace Flux.Save
{
    public static class GameSaver
    {
        private const string Key = "cre$-SToSp_Qibr6kEs4p_L-p-gosIk!";
        private const string IV = "@Ut0xln*#E&esu5I";
        private const string SaveFileExt = ".sav";
        private const string SavesFolderName = "saves";
        private static readonly JsonSerializerSettings jsonSerializerSettings;
        private static readonly string SavesFolderPath;

        static GameSaver()
        {
            SavesFolderPath = Path.Combine(Application.persistentDataPath, SavesFolderName);

            // Setup serializer settings
            jsonSerializerSettings = new()
            {
                // Adds type names to the json, so can easily deserialize to correct types
                TypeNameHandling = TypeNameHandling.All,

                // Ignore recursive fields that would result in infinite loop
                // (Vector3 contains one of these)
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
        }

        public static void Save<T>(string saveName, T saveData)
        {
            string filePath = Path.Combine(SavesFolderPath, saveName + SaveFileExt);

            // Serialise prefs to string
            var serializedData = JsonConvert.SerializeObject(saveData, jsonSerializerSettings);

            // Encrypt the json string
            var encryptedData = Encryptor.AesEncrypt(serializedData, IV, Key);

            // Encode to bytes
            var binaryData = Encoding.UTF8.GetBytes(encryptedData);

            // Create saves directory (if needed)
            Directory.CreateDirectory(SavesFolderPath);

            // Write the data to file
            File.WriteAllBytes(filePath, binaryData);
        }

        public static bool Load<T>(string saveName, out T saveData)
        {
            string filePath = Path.Combine(Application.persistentDataPath, saveName + SaveFileExt);

            // Guard if file doesn't exist
            if (!File.Exists(filePath))
            {
                Debug.LogError($"File doesn't exist: {filePath}");
                saveData = default;
                return false;
            }

            try
            {
                // Read binary data from file
                var binaryData = File.ReadAllBytes(filePath);

                // Decode to string
                var encodedData = Encoding.UTF8.GetString(binaryData);

                // Decrypt to json
                var serializedData = Encryptor.AesDecrypt(encodedData, IV, Key);

                // Deserialize from json
                saveData = JsonConvert.DeserializeObject<T>(serializedData, jsonSerializerSettings);

                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to load file: {filePath} \nError: {ex.Message}");
                saveData = default;
                return false;
            }
        }

        public static string[] GetSaves()
        {
            string[] saveFiles = Directory.GetFiles(Application.persistentDataPath, "*" + SaveFileExt);

            // Remove the path and extension, leaving only the file names
            for (int i = 0; i < saveFiles.Length; i++)
            {
                saveFiles[i] = Path.GetFileNameWithoutExtension(saveFiles[i]);
            }

            return saveFiles;
        }
    }
}
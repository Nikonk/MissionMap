using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

namespace MissionMap.Bootstrap
{
    public class JsonDataService : IDataService
    {
        private readonly string _basePath = Application.dataPath;

        public bool Save<T>(string relativePath, T data)
        {
            string path = _basePath + relativePath;

            try
            {
                if (File.Exists(path))
                {
                    Debug.Log("Data exists. Deleting old file and writing a new one.");
                    File.Delete(path);
                }
                else
                {
                    Debug.Log("Writing file for the first time");
                }

                using FileStream stream = File.Create(path);
                stream.Close();
                File.WriteAllText(path, JsonConvert.SerializeObject(data, Formatting.Indented, new StringEnumConverter()));

                return true;
            }
            catch (IOException e)
            {
                Debug.LogError($"Unable to save data due to: {e.Message} {e.StackTrace}");
                return false;
            }
        }

        public T Load<T>(string relativePath)
        {
            string path = _basePath + relativePath;

            if (File.Exists(path) == false)
            {
                Debug.LogError($"Cannot load file at {path}. File does not exist");
                throw new FileNotFoundException($"{path} does not exist");
            }

            try
            {
                var data = JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
                return data;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load data due to: {e.Message} {e.StackTrace}");
                throw;
            }
        }
    }
}
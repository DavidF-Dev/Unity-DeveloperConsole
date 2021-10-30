// File: DevConsoleData.cs
// Purpose: Static class for accessing saved developer console preferences.
// Created by: DavidFDev

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace DavidFDev.DevConsole
{
    internal static class DevConsoleData
    {
        #region Static fields

        private static Dictionary<string, object> _data;

        #endregion

        #region Static properties

        public static string FilePath { get; private set; }

        #endregion

        #region Static constructor

        static DevConsoleData()
        {
            _data = new Dictionary<string, object>();
            FilePath = Path.Combine(Application.persistentDataPath, "devconsole.dat");
        }

        #endregion

        #region Static methods

        public static void SetObject(string key, object value)
        {
            _data[key] = value;
        }

        public static T GetObject<T>(string key, T defaultValue)
        {
            try
            {
                return !_data.ContainsKey(key) ? defaultValue : (T)_data[key];
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        public static void Save()
        {
            FileStream fs = new FileStream(FilePath, FileMode.Create);
            BinaryFormatter bf = new BinaryFormatter();

            try
            {
                bf.Serialize(fs, _data);
            }
            catch (Exception e)
            {
                DevConsole.LogException(e);
                DevConsole.LogError("Failed to save developer console preferences due to an exception.");
            }
            finally
            {
                fs.Close();
            }
        }

        public static void Load()
        {
            if (!File.Exists(FilePath))
            {
                return;
            }

            FileStream fs = new FileStream(FilePath, FileMode.Open);
            BinaryFormatter bf = new BinaryFormatter();
            bool delete = false;

            try
            {
                _data = (Dictionary<string, object>)bf.Deserialize(fs);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                delete = true;
            }
            finally
            {
                fs.Close();
            }

            if (delete)
            {
                File.Delete(FilePath);
            }
        }

        public static void Clear()
        {
            _data.Clear();
        }

        #endregion
    }
}

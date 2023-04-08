﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace MP3GainMT
{
    public class MP3GainSettings
    {
        public readonly string SettingsFileLocation = @".\settings.json";
        private static readonly string HeightSizeLabel = "Heigth Size";
        private static readonly string LeftPositionLabel = "Left Position";
        private static readonly string ParentFolderLabel = "Last Used Folder";
        private static readonly string TargetDbLabel = "Target dB";
        private static readonly string TopPositionLabel = "Top Position";
        private static readonly string WidthSizeLabel = "Width Size";
        private JObject _json = null;

        public MP3GainSettings()
        {
            this.ReadSettings();
        }

        public int HeightSize
        {
            get
            {
                return NoZero(ReadKey<int>(HeightSizeLabel));
            }
            set
            {
                WriteKey<int>(value, HeightSizeLabel);
            }
        }

        public int LeftPosition
        {
            get
            {
                return ReadKey<int>(LeftPositionLabel);
            }
            set
            {
                WriteKey<int>(value, LeftPositionLabel);
            }
        }

        public string ParentFolder
        {
            get
            {
                return ReadKey<string>(ParentFolderLabel);
            }
            set
            {
                WriteKey<string>(value, ParentFolderLabel);
            }
        }

        public double TargetDb
        {
            get
            {
                return ReadKey<double>(TargetDbLabel);
            }
            set
            {
                WriteKey<double>(value, TargetDbLabel);
            }
        }

        public int TopPosition
        {
            get
            {
                return ReadKey<int>(TopPositionLabel);
            }
            set
            {
                WriteKey<int>(value, TopPositionLabel);
            }
        }

        public int WidthSize
        {
            get
            {
                return NoZero(ReadKey<int>(WidthSizeLabel));
            }
            set
            {
                WriteKey<int>(value, WidthSizeLabel);
            }
        }

        public void WriteSettingsFile()
        {
            using (JsonTextWriter writer = new JsonTextWriter(File.CreateText(SettingsFileLocation)))
            {
                this._json.WriteTo(writer);
            }
        }

        private static int NoZero(int result)
        {
            if (result == 0)
            {
                result = 1;
            }

            return result;
        }

        private void CheckFile()
        {
            if (_json == null)
            {
                this.ReadSettings();
            }
        }

        private void PrepareKey<T>(string key)
        {
            if (!_json.ContainsKey(key))
            {
                T obj = default(T);

                if (obj == null)
                {
                    obj = (T)Convert.ChangeType(string.Empty, typeof(T));
                }

                _json.Add(key, JToken.FromObject(obj));
            }
        }

        private T ReadKey<T>(string key)
        {
            CheckFile();

            var result = (T)Convert.ChangeType(_json[key], typeof(T));

            return result;
        }

        private void ReadSettings()
        {
            JsonTextReader reader = null;

            if (File.Exists(SettingsFileLocation))
            {
                reader = new JsonTextReader(File.OpenText(SettingsFileLocation));

                reader.Read();

                this._json = (JObject)JToken.ReadFrom(reader);
            }
            else
            {
                this._json = new JObject();
            }

            PrepareKey<string>(ParentFolderLabel);
            PrepareKey<int>(LeftPositionLabel);
            PrepareKey<int>(TopPositionLabel);
            PrepareKey<int>(WidthSizeLabel);
            PrepareKey<int>(HeightSizeLabel);
            PrepareKey<double>(TargetDbLabel);

            if (reader != null)
            {
                reader.Close();
            }
        }

        private void WriteKey<T>(T value, string key)
        {
            CheckFile();

            this._json[key] = JToken.FromObject(value);
        }
    }
}
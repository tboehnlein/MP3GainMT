using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace MP3GainMT
{
    internal class MP3GainSettings
    {
        private static readonly string ParentFolderLabel = "Last Used Folder";
        private static readonly string LeftPositionLabel = "Left Position";
        private static readonly string TopPositionLabel = "Top Position";
        private static readonly string HeightSizeLabel = "Heigth Size";
        private static readonly string WidthSizeLabel = "Width Size";
        private static readonly string TargetDbLabel = "Target dB";
        private JObject _json = null;

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

        private static int NoZero(int result)
        {
            if (result == 0)
            {
                result = 1;
            }

            return result;
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

        private void WriteKey<T>(T value, string key)
        {
            CheckFile();

            this._json[key] = JToken.FromObject(value);
        }

        private T ReadKey<T>(string key)
        {
            CheckFile();

            var result = (T)Convert.ChangeType(_json[key], typeof(T));

            return result;
        }

        private void CheckFile()
        {
            if (_json == null)
            {
                this.ReadSettingsFile();
            }
        }

        public readonly string SettingsFileLocation = @".\settings.json";

        public MP3GainSettings()
        {
            this.ReadSettingsFile();
        }

        private void ReadSettingsFile()
        {
            if (File.Exists(SettingsFileLocation))
            {
                using (var reader = new JsonTextReader(File.OpenText(SettingsFileLocation)))
                {
                    reader.Read();

                    this._json = (JObject)JToken.ReadFrom(reader);

                    PrepareKey<string>(ParentFolderLabel);
                    PrepareKey<int>(LeftPositionLabel);
                    PrepareKey<int>(TopPositionLabel);
                    PrepareKey<int>(WidthSizeLabel);
                    PrepareKey<int>(HeightSizeLabel);
                    PrepareKey<double>(TargetDbLabel);
                }
            }
            else
            {
                this._json = new JObject();
                _json.Add(ParentFolderLabel, string.Empty);
            }
        }

        private void PrepareKey<T>(string key)
        {
            if (!_json.ContainsKey(key))
            {
                _json.Add(key, JToken.FromObject(default(T)));
            }
        }

        public void WriteSettingsFile()
        {
            using (JsonTextWriter writer = new JsonTextWriter(File.CreateText(SettingsFileLocation)))
            {
                this._json.WriteTo(writer);
            }
        }
    }
}
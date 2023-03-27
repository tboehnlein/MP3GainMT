using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace MP3GainMT
{
    internal class MP3GainSettings
    {
        private static readonly string FolderPathLabel = "Last Used Folder";
        private JObject _json = null;

        public string ParentFolder
        {
            get
            {
                CheckFile();

                return _json[FolderPathLabel].ToString();
            }
            set
            {
                CheckFile();

                this._json[FolderPathLabel] = value;
            }
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

                    if (!_json.ContainsKey(FolderPathLabel))
                    {
                        _json.Add(FolderPathLabel, string.Empty);
                    }
                }
            }
            else
            {
                this._json = new JObject();
                _json.Add(FolderPathLabel, string.Empty);
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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MP3GainMT
{
    internal class MP3GainSettings
    {
        private static readonly string LastUsedLabel = "Last Used";
        private JObject _json = null;

        public string LastUsedParentFolder
        {
            get
            {
                CheckFile();

                return _json[LastUsedLabel].ToString();
            }
            set
            {
                CheckFile();

                this._json[LastUsedLabel] = value;
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

                    if (!_json.ContainsKey(LastUsedLabel))
                    {
                        _json.Add(LastUsedLabel, string.Empty);
                    }
                }
            }
            else
            {
                this._json = new JObject();
                _json.Add(LastUsedLabel, string.Empty);

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

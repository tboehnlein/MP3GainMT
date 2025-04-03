// Copyright (c) 2025 Thomas Boehnlein
// 
// This software is provided 'as-is', without any express or implied
// warranty. In no event will the authors be held liable for any damages
// arising from the use of this software.
// 
// Permission is granted to anyone to use this software for any purpose,
// including commercial applications, and to alter it and redistribute it
// freely, subject to the following restrictions:
// 
// 1. The origin of this software must not be misrepresented; you must not
//    claim that you wrote the original software. If you use this software
//    in a product, an acknowledgment in the product documentation would be
//    appreciated but is not required.
// 2. Altered source versions must be plainly marked as such, and must not be
//    misrepresented as being the original software.
// 3. This notice may not be removed or altered from any source distribution.

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace MP3GainMT
{
    public class MP3GainSettings
    {
        public readonly string SettingsFileLocation = @".\settings.json";
        private static readonly string ExecutableLabel = "Executable";
        private static readonly string HeightSizeLabel = "Heigth Length";
        private static readonly string LeftPositionLabel = "Left Position";
        private static readonly string ParentFolderLabel = "Last Used Folder";
        private static readonly string PathWidthLabel = "Path Column Width";
        private static readonly string TargetDbLabel = "Target dB";
        private static readonly string TopPositionLabel = "Top Position";
        private static readonly string UseAndLabel = "Use And";
        private static readonly string WidthSizeLabel = "Width Length";
        private static readonly string DoubleClickTableLabel = "Double Click Table";
        private static readonly string ThemeLabel = "Theme";
        private static readonly string TableFontSizeLabel = "Table Font";
        private JObject _json = null;

        public MP3GainSettings()
        {
            this.ReadSettings();
        }

        public string Executable
        {
            get
            {
                return ReadKey<string>(ExecutableLabel);
            }
            set
            {
                WriteKey<string>(value, ExecutableLabel);
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

        public int PathWidth
        {
            get
            {
                return NoZero(ReadKey<int>(PathWidthLabel));
            }
            set
            {
                WriteKey<int>(value, PathWidthLabel);
            }
        }

        public int TableFontSize
        {
            get
            {
                return ReadKey<int>(TableFontSizeLabel);
            }
            set
            {
                WriteKey<int>(value, TableFontSizeLabel);
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

        public bool UseAnd
        {
            get
            {
                return ReadKey<bool>(UseAndLabel);
            }
            set
            {
                WriteKey<bool>(value, UseAndLabel);
            }
        }

        public string Theme
        {
            get
            {
                return ReadKey<string>(ThemeLabel);
            }
            set
            {
                WriteKey<string>(value, ThemeLabel);
            }
        }

        public string DoubleClickTable
        {
            get
            {
                return ReadKey<string>(DoubleClickTableLabel);
            }
            set
            {
                WriteKey<string>(value, DoubleClickTableLabel);
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
            PrepareKey<int>(PathWidthLabel);
            PrepareKey<bool>(UseAndLabel);
            PrepareKey<string>(ExecutableLabel);
            PrepareKey<string>(DoubleClickTableLabel);
            PrepareKey<string>(ThemeLabel);
            PrepareKey<int>(TableFontSizeLabel);

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
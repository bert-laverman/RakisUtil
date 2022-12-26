/*
 * Copyright (c) 2021, 2022. Bert Laverman
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *    http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rakis.Logging;
using Rakis.Settings.Dictionary;
using System;
using System.IO;

namespace Rakis.Settings.Files
{

    public class JsonFileSettings : DictionarySettings
    {
        private readonly static ILogger log = Logger.GetLogger(typeof(JsonFileSettings));

        public SettingsType Type { get; init; }
        public Context Context { get; init; }
        public SettingsDir SettingsDir { get; set; }
        public string Filename { get; init; }

        public JsonFileSettings(string filename, Context context, SettingsType type =SettingsType.AppDataRoaming) : base()
        {
            Type = type;
            Context = context;
            SettingsDir = new(context, type);
            Filename = filename;
            Load();
        }

        public JsonFileSettings(Context context, SettingsType type = SettingsType.AppDataRoaming) : base()
        {
            Type = type;
            Context = context;
            SettingsDir = new(context, type);
            Filename = "settings.json";
            Load();
        }
 
        private void CopyField(ISettings tgt, JProperty prop)
        {
            if (prop.Value.Type == JTokenType.Object)
            {
                var sub = tgt[prop.Name];
                foreach (JProperty subProp in  prop.Value.Value<JToken>())
                {
                    CopyField(sub.AsSettings, subProp);
                }
            }
            else if (prop.Value.Type == JTokenType.Boolean)
            {
                tgt[prop.Name] = ((bool)prop.Value);
            }
            else if (prop.Value.Type == JTokenType.Integer)
            {
                tgt[prop.Name] = ((int)prop.Value);
            }
            else
            {
                tgt[prop.Name] = prop.Value.ToString();
            }
        }

        public bool Load()
        {
            bool result = false;

            var filePath = SettingsDir.SettingFile(Filename);
            if (File.Exists(filePath))
            {
                var text = File.ReadAllText(filePath);

                try
                {
                    var loadedSettings = JsonConvert.DeserializeObject<JObject>(text) as JObject;

                    if (loadedSettings != null)
                    {
                        Dict.Clear();
                        foreach (JProperty prop in (JToken)loadedSettings)
                        {
                            CopyField(this, prop);
                        }
                        result = true;
                    }
                    else
                    {
                        log.Error?.Log($"Failed to parse settings in '{filePath}' as a Dictionary");
                    }
                }
                catch (Exception e)
                {
                    log.Error?.Log($"Failed to parse settings in '{filePath}': {e.Message}");
                }
            } else
            {
                log.Warn?.Log($"No settings in '{filePath}'.");
            }
            return result;
        }

        public bool Save()
        {
            bool result = false;

            var filePath = SettingsDir.SettingFile(Filename);
            try
            {
                var text = JsonConvert.SerializeObject(Dict);
                File.WriteAllText(filePath, text);

                result = true;
            }
            catch (Exception e)
            {
                log.Error?.Log($"Failed to store settings in '{filePath}': {e.Message}");
            }

            return result;
        }
    }
}

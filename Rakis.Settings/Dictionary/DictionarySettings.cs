/*
 * Copyright (c) 2021. Bert Laverman
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

using System;
using System.Collections.Generic;

namespace Rakis.Settings.Dictionary
{
    public class DictionarySettings : SettingsValue, ISettings
    {
        public DictionarySettings Parent { get; set; }
        public string Name { get; init; }
        public string Key { get; init; }
        internal IDictionary<string, object> Dict;

        public DictionarySettings()
        {
            Name = "<root>";
            Dict = new SortedDictionary<string, object>();
        }

        public DictionarySettings(string name, IDictionary<string, object> sub)
        {
            Parent = null;
            Name = name;
            Key = name;
            Dict = sub;
        }

        public DictionarySettings(DictionarySettings parent, string name, IDictionary<string, object> sub)
        {
            Parent = parent;
            Name = name;
            Key = parent.Key + "." + name;
            Dict = sub;
        }

        public DictionarySettings(DictionarySettings parent, string name)
        {
            Parent = parent;
            Name = name;
            Key = parent.Key + "." + name;
            if (Parent.Dict.ContainsKey(name) && Parent.Dict [name] is SortedDictionary<string, object> subDict)
            {
                Dict = subDict;
            }
            else
            {
                Dict = new SortedDictionary<string, object>();
                Parent.Dict.Add(name, Dict);
            }
        }

        private SettingsValue GetValue(string key)
        {
            if (Dict.ContainsKey(key))
            {
                var value = Dict[key];
                if (value is IDictionary<string,object> sub)
                {
                    return new DictionarySettings(this, key, sub);
                }
                return new DictionaryValue(this, key, value);
            }
            return new DictionaryNullValue(this, key);
        }

        private void SetValue(string key, SettingsValue value)
        {
            if (value.IsNull)
            {
                if (Dict.ContainsKey(key))
                {
                    Dict.Remove(key);
                }
            }
            else if (value.IsValue)
            {
                if (Dict.ContainsKey(key))
                {
                    Dict[key] = value.AsObject;
                }
                else
                {
                    Dict.Add(key, value.AsObject);
                }
            }
            else if (value.IsSettings)
            {
                var sub = value as DictionarySettings;
                if (sub == null)
                {
                    throw new NotImplementedException();
                }
                if (Dict.ContainsKey(key))
                {
                    Dict[key] = sub.Dict;
                    sub.Parent = this;
                }
                else
                {
                    Dict.Add(key, sub.Dict);
                    sub.Parent = this;
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public virtual SettingsValue this[string key]
        {
            get => GetValue(key);
            set => SetValue(key, value);
        }
        public override ISettings AsSettings => this;
        public override bool IsNull => false;
    }
}

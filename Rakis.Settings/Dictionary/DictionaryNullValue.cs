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

namespace Rakis.Settings.Dictionary
{
    internal class DictionaryNullValue : SettingsValue, ISettings
    {
        public DictionarySettings Parent { get; init; }
        public string Name { get; init; }

        internal DictionaryNullValue(DictionarySettings parent, string name)
        {
            Parent = parent;
            Name = name;
        }

        internal string Key => Parent.Key + "." + Name;

        private SettingsValue GetValue(string key)
        {
            return new DictionarySettings(Parent, Name)[key];
        }

        private void SetValue(string key, SettingsValue value)
        {
            if (Parent.Dict.ContainsKey(Name))
            {
                Parent[Name].AsSettings[key] = value;
            }
            else
            {
                new DictionarySettings(Parent, Name)[key] = value;
            }
        }

        public SettingsValue this[string key]
        {
            get => GetValue(key);
            set => SetValue(key, value);
        }

        public override ISettings AsSettings => new DictionarySettings(Parent, Name);
        public override bool IsNull => false;
    }
}

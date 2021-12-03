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

using Microsoft.Win32;
using System;
using System.Collections.Generic;

namespace Rakis.Settings.WindowsRegistry
{
    public class WindowsRegistryKey : SettingsValue, ISettings
    {
        public WindowsRegistryKey Parent { get; init; }
        public string Name { get; init; }
        public RegistryKey Key { get; init; }

        public ISet<string> Values;
        public ISet<string> SubKeys;

        public WindowsRegistryKey()
        {

        }

        public WindowsRegistryKey(string name, RegistryKey key)
        {
            Parent = null;
            Name = name;
            Key = key;
            Values = new SortedSet<string>(Key.GetValueNames());
            SubKeys = new SortedSet<string>(Key.GetSubKeyNames());
        }

        public WindowsRegistryKey(WindowsRegistryKey parent, string name)
        {
            Parent = parent;
            Name = name;
            Key = parent.Key.OpenSubKey(name);
            Values = new SortedSet<string>(Key.GetValueNames());
            SubKeys = new SortedSet<string>(Key.GetSubKeyNames());
        }

        private SettingsValue GetValue(string key)
        {
            if (SubKeys.Contains(key))
            {
                return new WindowsRegistryKey(this, key);
            }
            else if (Values.Contains(key))
            {
                return new WindowsRegistryValue(this, key, Key.GetValue(key));
            }
            throw new NotImplementedException();
        }

        private void SetValue(string key, SettingsValue value)
        {
            if (!SubKeys.Contains(key) && value.IsValue)
            {
                if (value.IsNull)
                {
                    if (Values.Contains(key))
                    {
                        Key.DeleteValue(key);
                    }
                }
                else
                {
                    Key.SetValue(key, value.AsObject);
                }
            }
            throw new NotImplementedException();
        }

        public virtual SettingsValue this[string key]
        {
            get => GetValue(key);
            set => SetValue(key, value);
        }
    }
}

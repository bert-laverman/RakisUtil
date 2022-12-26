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
using Rakis.Logging;
using System;

namespace Rakis.Settings.WindowsRegistry
{
    public class WindowsRegistrySettings : WindowsRegistryKey
    {

        private static readonly ILogger log = Logger.GetLogger(typeof(WindowsRegistrySettings));

        private const string HKLM_BASE = "HKEY_LOCAL_MACHINE\\SOFTWARE";
        private const string HKCU_BASE = "HKEY_CURRENT_USER\\Software";

        private RegistryKey OpenOrCreate(string key)
        {
            if (SubKeys.Contains(key))
            {
                return Key.OpenSubKey(key);
            }
            else if (Parent != null)
            {
                using RegistryKey writableKey = Parent.Key.OpenSubKey(Name, true);
                return writableKey.CreateSubKey(key);
            }
            else if (Name == HKLM_BASE)
            {
                using RegistryKey writableKey = Registry.LocalMachine.OpenSubKey("SOFTWARE", true);
                return writableKey.CreateSubKey(key);
            }
            else if (Name == HKCU_BASE)
            {
                using RegistryKey writableKey = Registry.CurrentUser.OpenSubKey("Software", true);
                return writableKey.CreateSubKey(key);
            }
            throw new NotImplementedException();
        }

        public WindowsRegistrySettings(SettingsType type, Context context)
        {
           if ((type != SettingsType.SystemRegistry) && (type != SettingsType.UserRegistry))
           {
               throw new NotImplementedException();
           }

           if (type == SettingsType.SystemRegistry)
            {
                Name = HKLM_BASE;
                Key = Registry.LocalMachine.OpenSubKey("SOFTWARE");
            }
           else
            {
                Name = HKCU_BASE;
                Key = Registry.CurrentUser.OpenSubKey("Software");
            }
            if (context.Group != null)
            {
                var groupKey = OpenOrCreate(context.Group);
                if (groupKey is null)
                {
                    log.Info?.Log($"Creating registry key for \"{context.Group}\" in \"{Name}\".");
                    groupKey = Key.CreateSubKey(context.Group);
                }
                Key = groupKey;
            }
            if (context.Application != null)
            {
                var appKey = OpenOrCreate(context.Application);
                if (appKey is null)
                {
                    log.Info?.Log($"Creating registry key for \"{context.Application}\" in \"{Name}\".");
                    appKey = Key.CreateSubKey(context.Application);
                }
                Key = appKey;
            }
        }

    }
}

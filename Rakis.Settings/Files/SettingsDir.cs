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

using System;
using System.IO;
using static System.Environment;
using Rakis.Logging;

namespace Rakis.Settings
{

    public class SettingsDir
    {

        private static readonly ILogger log = Logger.GetLogger(typeof(SettingsDir));

        public Context Context { get; set; }
        public SettingsType Type { get; set; }
        public bool Hidden { get; set; }
        public bool UseGroup { get; set; }
        public bool UseApplication { get; set; }

        public SettingsDir(Context context, SettingsType type =SettingsType.AppDataRoaming, bool hidden =false, bool useGroup =true, bool useApplication =true)
        {
            Context = context;
            Type = type;
            Hidden = hidden;
            UseGroup = useGroup;
            UseApplication = useApplication;
        }

        private static string UserHome => GetEnvironmentVariable("HOMEDRIVE") + GetEnvironmentVariable("HOMEPATH");
        private static string AppData(string category) => Path.Combine(UserHome, "AppData", category);
        private static string ProgramData => Path.Combine(GetEnvironmentVariable("HOMEDRIVE") + "\\ProgramData");
        private static string UserDocuments => Path.Combine(UserHome, "Documents");

        public string Combine(string baseDir, string filename)
        {
            if (UseGroup)
            {
                string group = Hidden ? ("." + Context.Group) : Context.Group;
                return UseApplication ? Path.Combine(baseDir, group, Context.Application, filename) : Path.Combine(baseDir, group, filename);
            }
            return UseApplication ? Path.Combine(baseDir, Context.Application, filename) : Path.Combine(baseDir, filename);
        }

        public string SettingFile(string filename)
        {
            string result;

            switch (Type) {
                case SettingsType.ProgramData:
                    result = Combine(ProgramData, filename);
                    break;
                case SettingsType.AppDataLocal:
                    result = Combine(AppData("Local"), filename);
                    break;
                case SettingsType.AppDataLocalLow:
                    result = Combine(AppData("LocalLow"), filename);
                    break;
                case SettingsType.AppDataRoaming:
                    result = Combine(AppData("Roaming"), filename);
                    break;
                case SettingsType.UserHome:
                    result = Combine(UserHome, filename);
                    break;
                case SettingsType.UserDocuments:
                    result = Combine(UserDocuments, filename);
                    break;
                case SettingsType.CurrentWorkingDirectory:
                    result = Combine(".", filename);
                    break;
                default:
                    throw new NotImplementedException();
            }
            log.Debug?.Log($"SettingsFile '{filename}' mapped to '{result}'");
            string dirPath = Path.GetDirectoryName(result);
            if (!File.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            return result;
        }

    }
}

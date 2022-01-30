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

using System.Collections.Generic;

namespace Rakis.Args
{
    public class Args
    {
        public List<string> Parameters { get; init; } = new();
        public HashSet<string> BoolOpts { get; init; } = new();
        public Dictionary<string, string> ArgOpts { get; init; } = new();

        private void SetOpt(string longOpt, string value =null)
        {
            if (longOpt != null)
            {
                if ((value != null) && !ArgOpts.ContainsKey(longOpt))
                {
                    ArgOpts.Add(longOpt, value);
                }
                else if (!BoolOpts.Contains(longOpt))
                {
                    BoolOpts.Add(longOpt);
                }
            }
        }

        private void SetOpt(char shortOpt, string value =null)
        {
            if (shortOpt != '\0')
            {
                SetOpt(shortOpt.ToString(), value);
            }
        }

        internal void SetOpt(Option opt, string value =null)
        {
            if (opt.HasArg ^ (value == null))
            {
                SetOpt(opt.ShortOpt, value);
                SetOpt(opt.LongOpt, value);
            }
        }

        public bool Has(string key) => BoolOpts.Contains(key);
        public bool Has(char key) => BoolOpts.Contains(key.ToString());
        public string this[string key] { get => ArgOpts.GetValueOrDefault(key, null); }

    }
}

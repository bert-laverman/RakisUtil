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

using Rakis.Logging;
using System;
using System.Collections.Generic;

namespace Rakis.Args
{
    public class ArgParser
    {
        private static readonly Logger log = Logger.GetLogger(typeof(ArgParser));

        public string[] CommandLineArgs { get; init; }
        private Dictionary<char, Option> charOptions = new();
        private Dictionary<string, Option> stringOptions = new();

        public ArgParser(string[] args)
        {
            CommandLineArgs = args;
        }

        public ArgParser WithOption(char shortOpt, string longOpt, bool hasArg =false)
        {
            var o = new Option(shortOpt, longOpt, hasArg);
            if (shortOpt != '\0')
            {
                if (!charOptions.ContainsKey(shortOpt))
                {
                    log.Debug?.Log($"Adding option '{shortOpt}', HasArg={o.HasArg}");
                    charOptions.Add(shortOpt, o);
                }
                else
                {
                    charOptions[shortOpt] = o;
                }
            }
            if (longOpt != null)
            {
                if (!stringOptions.ContainsKey(longOpt))
                {
                    log.Debug?.Log($"Adding option \"{longOpt}\", HasArg={o.HasArg}");
                    stringOptions.Add(longOpt, o);
                }
                else
                {
                    stringOptions[longOpt] = o;
                }
            }
            return this;
        }

        public ArgParser WithOption(char optChar, bool hasArg = false)
        {
            return WithOption(optChar, null, hasArg);
        }
        public ArgParser WithOption(string optString, bool hasArg = false)
        {
            return WithOption('\0', optString, hasArg);
        }

        public Args Parse()
        {
            Args result = new();

            uint i = 0;
            while (i < CommandLineArgs.Length)
            {
                string thisArg = CommandLineArgs[i];
                if (!thisArg.StartsWith("-"))
                {
                    break;
                }

                log.Debug?.Log($"Parsing \"{thisArg}\"");
                if (thisArg.StartsWith("--"))
                {
                    int index = thisArg.IndexOf('=');
                    if (index == -1) index = thisArg.Length;
                    string optName = thisArg.Substring(2, index - 2);
                    string value = (index == thisArg.Length) ? null : thisArg.Substring(index + 1);

                    if (stringOptions.ContainsKey(optName))
                    {
                        log.Debug?.Log($"Adding \"{optName}\" with value \"{value ?? ""}\"");
                        result.SetOpt(stringOptions[optName], value);
                    }
                    else
                    {
                        log.Warn?.Log($"Unknown option \"{optName}\"");
                    }
                }
                else
                {
                    foreach (char c in thisArg.Substring(1))
                    {
                        if (charOptions.ContainsKey(c))
                        {
                            Option opt = charOptions[c];
                            if (opt.HasArg && (i < CommandLineArgs.Length))
                            {
                                log.Debug?.Log($"Adding '{opt}' with value \"{CommandLineArgs[i]}\"");
                                result.SetOpt(opt, CommandLineArgs[++i]);
                            }
                            else
                            {
                                log.Debug?.Log($"Adding '{opt}'");
                                result.SetOpt(opt);
                            }
                        }
                    }
                }
                i++;
            }
            while (i < CommandLineArgs.Length)
            {
                log.Debug?.Log($"Adding parameter \"{CommandLineArgs [i]}\".");
                result.Parameters.Add(CommandLineArgs[i++]);
            }

            return result;
        }
    }
}

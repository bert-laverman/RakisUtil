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

namespace Rakis.Settings
{
    public abstract class SettingsValue
    {
        public virtual bool? AsBool => null;
        public bool IsBool => AsBool!= null;

        public virtual int? AsInt => null;
        public bool IsInt => AsInt!= null;

        public virtual string AsString => null;
        public bool IsString => AsString!= null;

        public virtual ISettings AsSettings => null;
        public bool IsSettings => AsSettings!= null;
        public bool IsValue => !IsSettings;

        public virtual object AsObject => null;
        public virtual bool IsNull => (AsObject == null);

        public static implicit operator SettingsValue(bool value) => new BoolValue(value);
        public static implicit operator SettingsValue(int value) => new IntValue(value);
        public static implicit operator SettingsValue(string value) => new StringValue(value);
    }

    internal class BoolValue : SettingsValue
    {
        private readonly bool boolValue;

        public BoolValue(bool value)
        {
            boolValue = value;
        }

        public override bool? AsBool => boolValue;
        public override object AsObject => boolValue;
    }

    internal class IntValue : SettingsValue
    {
        private readonly int intValue;

        public IntValue(int value)
        {
            intValue = value;
        }

        public override int? AsInt => intValue;
        public override object AsObject => intValue;
    }

    internal class StringValue : SettingsValue
    {
        private readonly string stringValue;

        public StringValue(string value)
        {
            stringValue = value;
        }

        public override string AsString => stringValue;
        public override object AsObject => stringValue;
    }

    internal class SubSettingsValue : SettingsValue
    {
        private readonly ISettings settingsValue;

        public SubSettingsValue(ISettings value)
        {
            settingsValue = value;
        }

        public override ISettings AsSettings => settingsValue;
    }

    internal class NullValue : SettingsValue
    {
    }
}

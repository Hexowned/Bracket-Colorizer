using BracketPairColorizer.Settings.Settings;
using System.Collections.Generic;

namespace BracketPairColorizer.Languages
{
    public abstract class LanguageSettings : SettingsBase, ILanguageSettings, ICustomExport
    {
        protected static readonly string[] EMPTY = { };
        protected abstract string[] ControlFlowDefaults { get; }
        protected abstract string[] LinqDefaults { get; }
        protected abstract string[] VisibilityDefaults { get; }
        public string KeyName { get; private set; }

        public LanguageSettings(string key, ITypedSettingsStore store)
            : base(store)
        {
            this.KeyName = key;
        }

        public string[] ControlFlow
        {
            get { return this.Store.GetList(KeyName + "_ControlFlow", ControlFlowDefaults); }
            set { this.Store.SetValue(KeyName + "_ControlFlow", value); }
        }

        public string[] Linq
        {
            get { return this.Store.GetList(KeyName + "_Linq", LinqDefaults); }
            set { this.Store.SetValue(KeyName + "_Linq", value); }
        }

        public string[] Visibility
        {
            get { return this.Store.GetList(KeyName + "_Visibility", VisibilityDefaults); }
            set { this.Store.SetValue(KeyName + "_Visibility", value); }
        }

        public bool Enabled
        {
            get { return this.Store.GetBoolean(KeyName + "_Enabled", true); }
            set { this.Store.SetValue(KeyName + "_Enabled", value); }
        }

        public IDictionary<string, object> Export()
        {
            return new Dictionary<string, object>
            {
                { KeyName + "_ControlFlow", ControlFlow },
                { KeyName + "_Linq", Linq},
                { KeyName + "_Visibility", Visibility },
                { KeyName + "_Enabled", Enabled }
            };
        }
    }
}

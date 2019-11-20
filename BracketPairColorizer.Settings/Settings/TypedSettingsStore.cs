using System;
using System.ComponentModel.Composition;

namespace BracketPairColorizer.Settings.Settings
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(ITypedSettingsStore))]
    public class TypedSettingsStore : ITypedSettingsStore
    {
        private ISettingsStore store;
        private IStorageConversions converter;

        public event EventHandler SettingsChanged;

        [ImportingConstructor]
        public TypedSettingsStore(ISettingsStore store, IStorageConversions converter)
        {
            this.store = store;
            this.converter = converter;
        }

        public string Get(string name)
        {
            return this.store.Get(name);
        }

        public void Set(string name, object value)
        {
            this.store.Set(name, value);
        }

        public void SetValue(string name, object value)
        {
            if (value != null)
            {
                this.store.Set(name, converter.ToString(value));
            } else
            {
                this.store.Set(name, null);
            }
        }

        public bool GetBoolean(string name, bool defaultValue)
        {
            string value = this.store.Get(name);

            return string.IsNullOrEmpty(value) ? defaultValue : this.converter.ToBoolean(value);
        }

        public double GetDouble(string name, double defaultValue)
        {
            string value = this.store.Get(name);

            return string.IsNullOrEmpty(value) ? defaultValue : this.converter.ToDouble(value);
        }

        public T GetEnum<T>(string name, T defaultValue) where T : struct
        {
            string value = this.store.Get(name);
            T actual;
            if (this.converter.ToEnum<T>(value, out actual))
            {
                return actual;
            }

            return defaultValue;
        }

        public int GetInt32(string name, int defaultValue)
        {
            string value = this.store.Get(name);

            return string.IsNullOrEmpty(value) ? defaultValue : this.converter.ToInt32(value);
        }

        public long GetInt64(string name, long defaultValue)
        {
            string value = this.store.Get(name);

            return string.IsNullOrEmpty(value) ? defaultValue : this.converter.ToInt64(value);
        }

        public string[] GetList(string name, string[] defaultValue)
        {
            string value = GetString(name, "");
            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }

            var list = this.converter.ToList(value);

            return list.Length > 0 ? list : defaultValue;
        }

        public string GetString(string name, string defaultValue)
        {
            string value = this.store.Get(name);

            return string.IsNullOrEmpty(value) ? defaultValue : value;
        }

        public void Load()
        {
            this.store.Load();
        }

        public void Save()
        {
            this.store.Save();
            SettingsChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace BracketPairColorizer.Settings.Settings
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class GlobalXmlSettingsStore : ISettingsStore
    {
        private const string FILE_NAME = "BracketPairColorizer.xml";
        private string filePath;
        private Dictionary<string, string> settings = new Dictionary<string, string>();

        public GlobalXmlSettingsStore() : this(null)
        {
        }

        public GlobalXmlSettingsStore(string file)
        {
            ConfigurePath(file);
        }

        public string Get(string name)
        {
            if (this.settings.TryGetValue(name, out string val))
            {
                return val;
            }

            return null;
        }

        public void Set(string name, string value)
        {
            this.settings[name] = value;
        }

        public void Load()
        {
            var info = new FileInfo(this.filePath);
            if (info.Exists && info.Length > 0)
            {
                XDocument doc = XDocument.Load(this.filePath);
                foreach (var element in doc.Root.Elements())
                {
                    this.settings[element.Name.LocalName] = element.Value;
                }
            }
        }

        public void Save()
        {
            using (var xw = XmlWriter.Create(this.filePath))
            {
                xw.WriteStartElement("BracketPairColorizer");
                foreach (string key in this.settings.Keys)
                {
                    string value = this.settings[key];
                    if (value != null)
                    {
                        xw.WriteElementString(key, this.settings[key]);
                    }
                }

                xw.WriteEndElement();
            }
        }

        private void ConfigurePath(string filePath)
        {
            string folder = null;
            if (string.IsNullOrEmpty(filePath))
            {
                filePath = GetDefaultFilePath();
            }

            folder = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            this.filePath = filePath;
        }

        private static string GetDefaultFilePath()
        {
            var environmentValue = Environment.GetEnvironmentVariable("BRACKETPAIRCOLORIZER_SETTINGS");
            if (!string.IsNullOrEmpty(environmentValue))
            {
                return environmentValue;
            } else
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "BracketPairColorizer", FILE_NAME);
            }
        }
    }
}

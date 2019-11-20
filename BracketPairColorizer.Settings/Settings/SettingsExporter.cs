using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;

namespace BracketPairColorizer.Settings.Settings
{
    [Export(typeof(ISettingsExport))]
    public class SettingsExporter : ISettingsExport
    {
        private IDictionary<string, string> settings = new Dictionary<string, string>();
        private IStorageConversions converter;

        [ImportingConstructor]
        public SettingsExporter(IStorageConversions converter)
        {
            this.converter = converter;
        }

        public void Load(string sourcePath)
        {
            settings.Clear();
            XDocument doc = XDocument.Load(sourcePath);
            foreach (var element in doc.Root.Elements())
            {
                settings[element.Name.LocalName] = element.Value;
            }
        }

        public void Save(string targetPath)
        {
            using (var xw = XmlWriter.Create(targetPath))
            {
                xw.WriteStartElement("BracketPairColorizer");
                foreach (string key in settings.Keys)
                {
                    string value = settings[key];
                    if (value != null)
                    {
                        xw.WriteElementString(key, settings[key]);
                    }
                }

                xw.WriteEndElement();
            }
        }

        public void Export<T>(T settingsObject) where T : class
        {
            var exporter = settingsObject as ICustomExport;
            if (exporter != null)
            {
                CustomExport(exporter);
            } else
            {
                var type = typeof(T);
                var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (var prop in props)
                {
                    var value = prop.GetValue(settingsObject);

                    if (value == null)
                    {
                        this.settings[prop.Name] = null;
                    } else
                    {
                        this.settings[prop.Name] = this.converter.ToString(value);
                    }
                }
            }
        }

        public void Import(ISettingsStore store)
        {
            foreach (var key in this.settings.Keys)
            {
                store.Set(key, this.settings[key]);
            }
        }

        private void CustomExport(ICustomExport exporter)
        {
            var props = exporter.Export();
            foreach (var key in props.Keys)
            {
                var value = props[key];
                if (value == null)
                {
                    this.settings[key] = null;
                } else
                {
                    this.settings[key] = this.converter.ToString(value);
                }
            }
        }
    }
}

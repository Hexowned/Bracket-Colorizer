using BracketPairColorizer.Core.Settings;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Utilities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;

namespace BracketPairColorizer.Options
{
    public class ClassificationList
    {
        private readonly ColorStorage storage;
        private readonly IDictionary<string, ClassificationColors> classifications;
        private const string AUTOMATIC_COLOR = "Automatic";

        public ClassificationList(ColorStorage colorStorage)
        {
            this.storage = colorStorage;
            this.classifications = new Dictionary<string, ClassificationColors>();
        }

        public void Load(params Type[] classificationDefinitions)
        {
            var classificationNames = ExtractClassificationNames(classificationDefinitions);
            Load(classificationNames);
        }

        public void Load(params string[] classificationNames)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            this.classifications.Clear();
            var category = new Guid(FontsAndColorsCategories.TextEditorCategory);
            uint flags = (uint)(__FCSTORAGEFLAGS.FCSF_LOADDEFAULTS | __FCSTORAGEFLAGS.FCSF_READONLY);
            var hr = this.storage.Storage.OpenCategory(ref category, flags);
            ErrorHandler.ThrowOnFailure(hr);

            try
            {
                foreach (var classification in classificationNames)
                {
                    var colors = new ClassificationColors(classification);
                    colors.Load(this.storage);
                    this.classifications.Add(classification, colors);
                }
            } finally
            {
                this.storage.Storage.CloseCategory();
            }
        }

        public void Save()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var category = new Guid(FontsAndColorsCategories.TextEditorCategory);
            uint flags = (uint)(__FCSTORAGEFLAGS.FCSF_LOADDEFAULTS | __FCSTORAGEFLAGS.FCSF_PROPAGATECHANGES);
            var hr = this.storage.Storage.OpenCategory(ref category, flags);
            ErrorHandler.ThrowOnFailure(hr);

            try
            {
                foreach (var colors in this.classifications.Values)
                {
                    colors.Save(this.storage);
                }
            } finally
            {
                this.storage.Storage.CloseCategory();
            }
        }

        public Color Get(string classificationName, bool foreground)
        {
            if (this.classifications.TryGetValue(classificationName, out var entry))
            {
                return entry.Get(foreground);
            }

            return default(Color);
        }

        public void Set(string classificationName, bool foreground, Color color)
        {
            var obj = this.classifications[classificationName];
            obj.Set(color, foreground);
        }

        public void Export(string filePath)
        {
            var list = new JObject();

            foreach (var key in this.classifications.Keys)
            {
                var entry = this.classifications[key];
                var item = new JObject
                {
                    ["foreground"] = ColorToHtml(entry.Foreground),
                    ["background"] = ColorToHtml(entry.Background),
                    ["style"] = JToken.FromObject(entry.Style)
                };

                list[key] = item;
            }

            File.WriteAllText(filePath, list.ToString());
        }

        public void Import(string filePath)
        {
            var list = JObject.Parse(File.ReadAllText(filePath));

            foreach (var obj in list.Children())
            {
                var item = obj as JProperty;

                if (item == null)
                    continue;

                string key = item.Name;

                if (!this.classifications.TryGetValue(key, out ClassificationColors classification))
                {
                    continue;
                }

                JsonToClassification(item, classification);
            }

            this.Save();
        }

        private static string ColorToHtml(Color color)
        {
            return color == Color.Transparent ? AUTOMATIC_COLOR : ColorTranslator.ToHtml(color);
        }

        private static Color ColorFromHtml(string text)
        {
            if (text.Equals(AUTOMATIC_COLOR, StringComparison.InvariantCultureIgnoreCase))
            {
                return Color.Transparent;
            }

            return ColorTranslator.FromHtml(text);
        }

        private static void JsonToClassification(JProperty item, ClassificationColors classification)
        {
            var value = item.Value as JObject;
            var foreground = value["foreground"];

            if (foreground != null)
            {
                classification.Foreground = ColorFromHtml(foreground.Value<string>());
            }

            var background = value["background"];

            if (background != null)
            {
                classification.Background = ColorFromHtml(background.Value<string>());
            }

            var style = value["style"];

            if (style != null)
            {
                if (Enum.TryParse<FontStyles>(style.Value<string>(), out var parsedStyle))
                    classification.Style = parsedStyle;
            }
        }

        private string[] ExtractClassificationNames(Type[] classificationDefinitions)
        {
            var names = new List<string>();

            foreach (var def in classificationDefinitions)
            {
                var fields = def.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

                foreach (var field in fields)
                {
                    var name = GetDefinitionName(field);

                    if (!string.IsNullOrEmpty(name))
                    {
                        names.Add(name);
                    }
                }
            }

            return names.ToArray();
        }

        private string GetDefinitionName(FieldInfo field)
        {
            var nameAttr = field.GetCustomAttribute<NameAttribute>();

            if (nameAttr != null)
            {
                return nameAttr.Name;
            }

            return null;
        }
    }
}

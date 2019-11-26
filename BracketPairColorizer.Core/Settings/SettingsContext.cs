using BracketPairColorizer.Core.Compatibility;
using BracketPairColorizer.Languages;

namespace BracketPairColorizer.Core.Settings
{
    public class SettingsContext
    {
        public static IBpcSettings GetSettings()
        {
            var model = new SComponentModel();

            return model.GetService<IBpcSettings>();
        }

        public static T GetService<T>()
        {
            var model = new SComponentModel();

            return model.GetService<T>();
        }

        public static ILanguage GetLanguage(string key)
        {
            var model = new SComponentModel();
            var factory = model.GetService<ILanguageFactory>();

            return factory.TryCreateLanguage(key);
        }
    }
}

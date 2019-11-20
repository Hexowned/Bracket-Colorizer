using BracketPairColorizer.Languages.BraceScanners;
using BracketPairColorizer.Languages.CommentParsers;
using BracketPairColorizer.Languages.Utilities;
using System;

namespace BracketPairColorizer.Languages
{
    public abstract class LanguageInfo
    {
        public Func<string, string> NormalizationFunction { get; protected set; }

        protected abstract string[] SupportedContentTypes { get; }

        public LanguageInfo()
        {
            this.NormalizationFunction = x => x;
        }

        public T GetService<T>()
        {
            if (typeof(T) == typeof(IBraceScanner))
            {
                return (T)NewBraceScanner();
            } else if (typeof(T) == typeof(IFirstLineCommentParser))
            {
                return (T)NewFirstLineCommentParser();
            }

            return default(T);
        }

        protected abstract IBraceScanner NewBraceScanner();

        protected virtual IFirstLineCommentParser NewFirstLineCommentParser()
        {
            return new GenericCommentParser();
        }

        public virtual IStringScanner NewStringScanner(string classificationName, string text)
        {
            return null;
        }

        public virtual bool MatchesContentType(Func<string, bool> contentTypeMatches)
        {
            foreach (string str in this.SupportedContentTypes)
            {
                if (contentTypeMatches(str))
                    return true;
            }

            return false;
        }

        public virtual bool IsKeywordClassification(string classificationType)
        {
            return CompareClassification(classificationType, "Keyword");
        }

        protected bool CompareClassification(string classificationType, string name)
        {
            return classificationType.Equals(name, StringComparison.OrdinalIgnoreCase);
        }
    }
}

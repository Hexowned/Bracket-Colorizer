using Microsoft.VisualStudio.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace BracketPairColorizer.Core.Utilities
{
    public class RegexEntry : INotifyPropertyChanged
    {
        private string name;
        private string regex;
        private ExpressionKind kind;
        private ExpressionOptions options;
        private Regex compiledExpression;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Name
        {
            get { return this.name; }
            set { this.name = value; RaiseChanged(nameof(Name)); }
        }

        public string RegularExpression
        {
            get { return this.regex; }
            set { this.regex = value; RaiseChanged(nameof(RegularExpression)); }
        }

        public ExpressionKind Kind
        {
            get { return this.kind; }
            set { this.kind = value; RaiseChanged(nameof(Kind)); }
        }

        public ExpressionOptions Options
        {
            get { return this.options; }
            set { this.options = value; RaiseChanged(nameof(Options)); }
        }

        private void RaiseChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        public Regex GetRegex()
        {
            if (string.IsNullOrEmpty(this.RegularExpression)) { return null; }
            if (this.compiledExpression == null)
            {
                this.compiledExpression = new Regex(this.RegularExpression, RegexOptions.Compiled);
            }

            return this.compiledExpression;
        }

        public IEnumerable<SnapshotSpan> Match(ITextSnapshotLine line)
        {
            var regex = GetRegex();
            if (line.Length == 0 || regex == null) { yield break; }

            var snapshot = line.Snapshot;
            var matches = GetRegex().Matches(line.GetText());
            foreach (Match m in matches)
            {
                switch (Options)
                {
                    case ExpressionOptions.HideMatch:
                        yield return new SnapshotSpan(snapshot, line.Start + m.Index, m.Length);
                        break;

                    case ExpressionOptions.HideGroups:
                        for (int g = 1; g < m.Groups.Count; g++)
                        {
                            yield return new SnapshotSpan(snapshot, line.Start + m.Groups[g].Index, m.Groups[g].Length);
                        }

                        break;
                }
            }
        }

        public bool IsValid()
        {
            if (string.IsNullOrEmpty(this.RegularExpression)) { return false; }
            try
            {
                var temp = new Regex(this.RegularExpression);
                return true;
            } catch
            {
                return false;
            }
        }
    }

    public enum ExpressionKind
    {
        RegularExpression = 0
    }

    public enum ExpressionOptions
    {
        HideMatch = 0,
        HideGroups = 1
    }
}

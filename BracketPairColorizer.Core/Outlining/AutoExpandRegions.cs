using BracketPairColorizer.Core.Settings;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Outlining;
using Microsoft.VisualStudio.Utilities;
using System;
using System.ComponentModel.Composition;
using IVsOutliningManager = Microsoft.VisualStudio.Text.Outlining.IOutliningManager;
using IVsOutliningManagerService = Microsoft.VisualStudio.Text.Outlining.IOutliningManagerService;

namespace BracketPairColorizer.Core.Outlining
{
    [Export(typeof(IWpfTextViewCreationListener))]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    [Name("BracketPairColorizer.auto-expand-regions")]
    [ContentType(ContentTypes.Text)]
    public class AutoExpandRegions : IWpfTextViewCreationListener
    {
        [Import]
        private readonly IVsOutliningManagerService outlining = null;

        [Import]
        private readonly IBpcSettings settings = null;

        public void TextViewCreated(IWpfTextView textView)
        {
            var manager = this.outlining.GetOutliningManager(textView);
            if (manager != null)
            {
                textView.Properties.GetOrCreateSingletonProperty(()
                    => new AutoExpander(textView, manager, this.settings));
            }
        }
    }

    public class AutoExpander
    {
        private IWpfTextView theView;
        private IVsOutliningManager outliningManager;
        private readonly AutoExpandMode expandMode;
        private IBpcSettings settings;

        public AutoExpander(IWpfTextView textView, IVsOutliningManager outlining, IBpcSettings settings)
        {
            this.settings = settings;
            this.theView = textView;
            this.outliningManager = outlining;
            this.expandMode = settings.AutoExpandRegions;

            this.theView.Closed += OnViewClosed;
            this.settings.SettingsChanged += OnSettingsChanged;

            if (this.expandMode == AutoExpandMode.Disable)
            {
                outlining.Enabled = false;
            } else if (this.expandMode == AutoExpandMode.Expand)
            {
                this.theView.LayoutChanged += OnLayoutChanged;
                this.outliningManager.RegionsCollapsed += OnRegionsCollapsed;
                this.theView.GotAggregateFocus += OnGotFocus;
            }
        }

        private void OnGotFocus(object sender, EventArgs e)
        {
            this.theView.GotAggregateFocus -= OnGotFocus;
            ExpandAll();
        }

        private void OnSettingsChanged(object sender, EventArgs e)
        {
            if (this.settings.AutoExpandRegions == AutoExpandMode.Disable)
            {
                this.outliningManager.Enabled = false;
            } else
            {
                this.outliningManager.Enabled = true;
            }
        }

        private void OnViewClosed(object sender, EventArgs e)
        {
            this.settings.SettingsChanged -= OnSettingsChanged;
            this.settings = null;
            this.outliningManager.RegionsCollapsed -= OnRegionsCollapsed;
            this.theView.LayoutChanged -= OnLayoutChanged;
            this.theView.GotAggregateFocus -= OnGotFocus;
            this.theView.Closed -= OnViewClosed;
            this.theView = null;
            this.outliningManager = null;
        }

        private void OnRegionsCollapsed(object sender, RegionsCollapsedEventArgs e)
        {
            this.outliningManager.RegionsCollapsed -= OnRegionsCollapsed;
            ExpandAll();
        }

        private void OnLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            this.theView.LayoutChanged -= OnLayoutChanged;
            ExpandAll();
        }

        private void ExpandAll()
        {
            var snapshot = this.theView.TextSnapshot;
            if (snapshot != null)
            {
                var span = snapshot.GetSpan();
                this.outliningManager.ExpandAll(span, collapsed => true);
            }
        }
    }
}

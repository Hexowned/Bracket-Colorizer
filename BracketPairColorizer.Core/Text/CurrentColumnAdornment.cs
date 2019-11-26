using BracketPairColorizer.Core.Settings;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace BracketPairColorizer.Core.Text
{
    public class CurrentColumnAdornment
    {
        public const string CURRENT_COLUMN_TAG = "currentColumn";
        private IAdornmentLayer layer;
        private IWpfTextView view;
        private IClassificationFormatMap formatMap;
        private IClassificationType formatType;
        private IBpcSettings settings;
        private Border highlight;
        private Dispatcher dispatcher;

        public CurrentColumnAdornment(IWpfTextView view, IClassificationFormatMap formatMap, IClassificationType formatType, IBpcSettings settings)
        {
            this.view = view;
            this.formatMap = formatMap;
            this.formatType = formatType;
            this.settings = settings;
            this.dispatcher = Dispatcher.CurrentDispatcher;
            this.highlight = new Border();
            this.layer = view.GetAdornmentLayer(Constants.COLUMN_HIGHLIGHT);

            view.Caret.PositionChanged += OnCaretPositionChanged;
            view.ViewportWidthChanged += OnViewportChanged;
            view.ViewportHeightChanged += OnViewportChanged;
            view.LayoutChanged += OnViewLayoutChanged;
            view.TextViewModel.EditBuffer.PostChanged += OnBufferPostChanged;
            view.Closed += OnViewClosed;
            view.Options.OptionChanged += OnSettingsChanged;

            this.settings.SettingsChanged += OnSettingsChanged;
            formatMap.ClassificationFormatMappingChanged += OnClassificationFormatMappingChanged;

            CreateDrawingObjects();
        }

        private void OnSettingsChanged(object sender, EventArgs e)
        {
            if (this.view != null)
            {
                this.UpdateViewOnUIThread();
            }
        }

        private void UpdateViewOnUIThread()
        {
            var dispatcher = Dispatcher.CurrentDispatcher;
            if (!dispatcher.CheckAccess())
            {
                Action action = this.UpdateView;
                dispatcher.Invoke(action);
            } else
            {
                this.UpdateView();
            }
        }

        private void UpdateView()
        {
            CreateDrawingObjects();
            RedrawAdornments();
        }

        private void OnClassificationFormatMappingChanged(object sender, EventArgs e)
        {
            if (this.view != null)
            {
                CreateDrawingObjects();
                RedrawAdornments();
            }
        }

        private void OnViewClosed(object sender, EventArgs e)
        {
            if (this.settings != null)
            {
                this.settings.SettingsChanged -= OnSettingsChanged;
            }

            if (this.view != null)
            {
                this.view.Caret.PositionChanged -= OnCaretPositionChanged;
                if (this.view.TextViewModel?.EditBuffer != null)
                {
                    this.view.TextViewModel.EditBuffer.PostChanged -= OnBufferPostChanged;
                }

                this.view.ViewportWidthChanged -= OnViewportChanged;
                this.view.ViewportHeightChanged -= OnViewportChanged;
                this.view.Closed -= OnViewClosed;
                this.view.LayoutChanged -= OnViewLayoutChanged;
                this.view = null;
            }

            if (this.formatMap != null)
            {
                this.formatMap.ClassificationFormatMappingChanged -= OnClassificationFormatMappingChanged;
                this.formatMap = null;
            }

            this.formatType = null;
        }

        private void OnViewportChanged(object sender, EventArgs e)
        {
            if (this.view != null)
            {
                RedrawAdornments();
            }
        }

        private void OnCaretPositionChanged(object sender, CaretPositionChangedEventArgs e)
        {
            if (e.NewPosition != e.OldPosition && this.view != null)
            {
                this.layer.RemoveAllAdornments();
                this.CreateVisuals(e.NewPosition.VirtualBufferPosition);
            }
        }

        private void OnBufferPostChanged(object sender, EventArgs e)
        {
            if (this.view != null)
            {
                this.layer.RemoveAllAdornments();
                this.CreateVisuals(this.view.Caret.Position.VirtualBufferPosition);
            }
        }

        private void OnViewLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            if (this.view != null && e.VerticalTranslation)
            {
                this.layer.RemoveAllAdornments();
                this.CreateVisuals(this.view.Caret.Position.VirtualBufferPosition);
            }
        }

        private void CreateDrawingObjects()
        {
            var format = this.formatMap.GetExplicitTextProperties(this.formatType);

            this.highlight.BorderBrush = format.ForegroundBrush;
            switch (this.settings.CurrentColumnHighlightStyle)
            {
                case ColumnStyle.LeftBorder:
                    this.highlight.BorderThickness = new Thickness(this.settings.HighlightLineWidth, 0, 0, 0);
                    break;

                case ColumnStyle.RightBorder:
                    this.highlight.BorderThickness = new Thickness(0, 0, this.settings.HighlightLineWidth, 0);
                    break;

                default:
                    this.highlight.BorderThickness = new Thickness(this.settings.HighlightLineWidth);
                    break;
            }

            var fill = new Rectangle();
            this.highlight.Child = fill;
            fill.Fill = format.BackgroundBrush;
            fill.StrokeThickness = 0;
        }

        private void RedrawAdornments()
        {
            if (this.view.TextViewLines != null)
            {
                this.layer.RemoveAllAdornments();
                var caret = this.view.Caret.Position;
                this.CreateVisuals(caret.VirtualBufferPosition);
            }
        }

        private void CreateVisuals(VirtualSnapshotPoint caretPosition)
        {
            if (!this.settings.CurrentColumnHighlightEnabled) { return; }

            var textViewLines = this.view.TextViewLines;
            if (textViewLines == null)
                return;
            if (caretPosition.Position.Snapshot != this.view.TextBuffer.CurrentSnapshot)
                return;

            var line = this.view.GetTextViewLineContainingBufferPosition(caretPosition.Position);
            var charBounds = line.GetCharacterBounds(caretPosition);

            this.highlight.Width = charBounds.Width;
            this.highlight.Height = this.view.ViewportHeight;
            if (this.highlight.Height > 2)
            {
                this.highlight.Height -= 2;
            }

            Canvas.SetLeft(this.highlight, charBounds.Left);
            Canvas.SetTop(this.highlight, this.view.ViewportTop);

            this.layer.AddAdornment(AdornmentPositioningBehavior.OwnerControlled, null, CURRENT_COLUMN_TAG, this.highlight, null);
        }
    }
}

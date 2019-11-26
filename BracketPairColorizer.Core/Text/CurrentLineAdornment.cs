﻿using BracketPairColorizer.Core.Settings;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace BracketPairColorizer.Core.Text
{
    public class CurrentLineAdornment
    {
        public const string CURRENT_LINE_TAG = "currentLine";
        private IAdornmentLayer layer;
        private IWpfTextView view;
        private IClassificationFormatMap formatMap;
        private IClassificationType formatType;
        private IBpcSettings settings;
        private Rectangle lineRect;
        private Dispatcher dispatcher;

        public CurrentLineAdornment(IWpfTextView view, IClassificationFormatMap formatMap, IClassificationType formatType, IBpcSettings settings)
        {
            this.view = view;
            this.formatMap = formatMap;
            this.settings = settings;
            this.layer = view.GetAdornmentLayer(ZoomConstants.LINE_HIGHLIGHT);
            this.lineRect = new Rectangle();
            this.dispatcher = Dispatcher.CurrentDispatcher;

            view.Caret.PositionChanged += OnCaretPositionChanged;
            view.ViewportWidthChanged += OnViewportWidthChanged;
            view.LayoutChanged += OnLayoutChanged;
            view.ViewportLeftChanged += OnViewportLeftChanged;
            view.Closed += OnViewClosed;
            view.Options.OptionChanged += OnSettingsChanged;

            this.settings.SettingsChanged += OnSettingsChanged;
            formatMap.ClassificationFormatMappingChanged += OnClassificationFormatMappingChanged;

            CreateDrawingObjects();
        }

        private void OnViewClosed(object sender, EventArgs e)
        {
            if (this.settings != null)
            {
                this.settings.SettingsChanged -= OnSettingsChanged;
                this.settings = null;
            }

            if (this.view != null)
            {
                this.view.Options.OptionChanged -= OnSettingsChanged;
                this.view.Caret.PositionChanged -= OnCaretPositionChanged;
                this.view.ViewportWidthChanged -= OnViewportWidthChanged;
                this.view.LayoutChanged -= OnLayoutChanged;
                this.view.ViewportLeftChanged -= OnViewportLeftChanged;
                this.view.Closed -= OnViewClosed;
                this.view = null;
            }

            if (this.formatMap != null)
            {
                this.formatMap.ClassificationFormatMappingChanged -= OnClassificationFormatMappingChanged;
                this.formatMap = null;
            }

            this.layer = null;
            this.formatType = null;
        }

        private void OnSettingsChanged(object sender, EventArgs e)
        {
            void UpdateUI()
            {
                CreateDrawingObjects();
                RedrawAdornments();
            }

            if (!this.dispatcher.CheckAccess())
            {
                this.dispatcher.Invoke(UpdateUI);
            } else
            {
                UpdateUI();
            }
        }

        private void OnViewportLeftChanged(object sender, EventArgs e)
        {
            RedrawAdornments();
        }

        private void OnViewportWidthChanged(object sender, EventArgs e)
        {
            RedrawAdornments();
        }

        private void OnClassificationFormatMappingChanged(object sender, EventArgs e)
        {
            CreateDrawingObjects();
        }

        private void OnCaretPositionChanged(object sender, CaretPositionChangedEventArgs e)
        {
            var newLine = GetLineByPos(e.NewPosition);
            var oldLine = GetLineByPos(e.OldPosition);
            if (newLine != oldLine)
            {
                this.layer.RemoveAdornmentsByTag(CURRENT_LINE_TAG);
                this.CreateVisuals(newLine);
            }
        }

        private void OnLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            var caret = this.view.Caret.Position.BufferPosition;
            foreach (var line in e.NewOrReformattedLines)
            {
                if (line.ContainsBufferPosition(caret))
                {
                    this.CreateVisuals(line);
                    break;
                }
            }
        }

        private void CreateDrawingObjects()
        {
            var format = this.formatMap.GetTextProperties(this.formatType);

            this.lineRect.Fill = format.BackgroundBrush;
            this.lineRect.Stroke = format.ForegroundBrush;
            this.lineRect.StrokeThickness = this.settings.HighlightLineWidth;
        }

        private void RedrawAdornments()
        {
            if (this.view.TextViewLines != null)
            {
                this.layer.RemoveAllAdornments();
                var caret = this.view.Caret.Position;
                var line = GetLineByPos(caret);
                this.CreateVisuals(line);
            }
        }

        private ITextViewLine GetLineByPos(CaretPosition pos)
        {
            var point = pos.BufferPosition;
            if (point.Snapshot != this.view.TextSnapshot)
            {
                point = point.TranslateTo(this.view.TextSnapshot, PointTrackingMode.Positive);
            }

            return this.view.GetTextViewLineContainingBufferPosition(point);
        }

        private bool IsEnabled()
        {
            return this.settings.CurrentLineHighlightEnabled
                && !this.view.Options.GetOptionValue<bool>(ViewOptions.HighlightCurrentLineOption);
        }

        private void CreateVisuals(ITextViewLine line)
        {
            if (!IsEnabled()) { return; }

            var textViewLines = view.TextViewLines;
            if (textViewLines == null)
                return;
            var span = line.Extent;
            var rc = new Rect(new Point(this.view.ViewportLeft, line.TextTop),
                new Point(Math.Max(this.view.ViewportRight - 2, line.TextRight), line.TextBottom));

            this.lineRect.Width = rc.Width;
            this.lineRect.Height = rc.Height;

            Canvas.SetLeft(this.lineRect, rc.Left);
            Canvas.SetTop(this.lineRect, rc.Top);

            this.layer.AddAdornment(AdornmentPositioningBehavior.TextRelative, span, CURRENT_LINE_TAG, this.lineRect, null);
        }
    }
}

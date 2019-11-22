using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Projection;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace BracketPairColorizer.Core.Margins
{
    public class DevMarginViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<BufferInfoViewModel> bufferGraph = new ObservableCollection<BufferInfoViewModel>();
        private ObservableCollection<string> textViewRoles = new ObservableCollection<string>();
        private string bufferPosition;
        private BufferInfoViewModel selectedBuffer;

        public ReadOnlyObservableCollection<BufferInfoViewModel> BufferGraph
        {
            get { return new ReadOnlyObservableCollection<BufferInfoViewModel>(this.bufferGraph); }
        }

        public string BufferPosition
        {
            get { return this.bufferPosition; }
            set { this.bufferPosition = value; NotifyChanged("BufferPosition"); }
        }

        public BufferInfoViewModel SelectedBuffer
        {
            get { return this.selectedBuffer; }
            set { this.selectedBuffer = value; NotifyChanged("SelectedBuffer"); }
        }

        public ObservableCollection<string> TextViewRoles => this.textViewRoles;

        public event PropertyChangedEventHandler PropertyChanged;

        public void RefreshView(ITextView view)
        {
            this.TextViewRoles.Clear();
            foreach (var role in view.Roles)
            {
                this.TextViewRoles.Add(role);
            }
        }

        public void RefreshBuffers(IBufferGraph graph)
        {
            this.bufferGraph.Clear();
            var buffers = graph.GetTextBuffers(b => true);
            int index = 0;
            foreach (var buffer in buffers)
            {
                this.bufferGraph.Add(new BufferInfoViewModel
                {
                    ContentType = buffer.ContentType.DisplayName,
                    BufferType = buffer.GetType(),
                    ActualContentType = new ContentTypeViewModel(buffer.ContentType),
                    Index = index++
                });
            }
            NotifyChanged("BufferGraph");
            this.SelectedBuffer = this.bufferGraph.FirstOrDefault(
              b => TextEditor.IsNonProjectionOrElisionBufferType(b.BufferType));
        }

        private void NotifyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }

    public class BufferInfoViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string contentType;
        private Type bufferType;
        private int index;
        private ContentTypeViewModel ctViewModel;

        public string ContentType
        {
            get { return this.contentType; }
            set { this.contentType = value; NotifyChanged("ContentType"); }
        }

        public Type BufferType
        {
            get { return this.bufferType; }
            set { this.bufferType = value; NotifyChanged("BufferType"); }
        }

        public int Index
        {
            get { return this.index; }
            set { this.index = value; NotifyChanged("Index"); }
        }

        public ContentTypeViewModel ActualContentType
        {
            get { return this.ctViewModel; }
            set { this.ctViewModel = value; NotifyChanged("ActualContentType"); }
        }

        public string DisplayName
        {
            get { return string.Format("{0} ({1})", ContentType, BufferType.Name); }
        }

        private void NotifyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }

    public class ContentTypeViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string displayName;
        private ObservableCollection<ContentTypeViewModel> baseTypes;

        public string DisplayName
        {
            get { return this.displayName; }
            set { this.displayName = value; NotifyChanged("DisplayName"); }
        }

        public ObservableCollection<ContentTypeViewModel> BaseTypes
        {
            get { return this.baseTypes; }
            set { this.baseTypes = value; NotifyChanged("BaseTypes"); }
        }

        public ContentTypeViewModel(IContentType type)
        {
            this.DisplayName = this.DisplayName;
            var btCollection = new ObservableCollection<ContentTypeViewModel>();
            foreach (var bt in type.BaseTypes)
            {
                btCollection.Add(new ContentTypeViewModel(bt));
            }

            this.BaseTypes = btCollection;
        }

        private void NotifyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}

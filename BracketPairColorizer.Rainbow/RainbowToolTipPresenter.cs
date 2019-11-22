using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Adornments;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using System;
using System.ComponentModel.Composition;
using System.Windows;

namespace BracketPairColorizer.Rainbow
{
    [Export(typeof(IIntellisensePresenterProvider))]
    [Name("BracketPairColorizer.tooltip.presenter")]
    [Order(Before = "Default Quick Info Presenter")]
    [ContentType(ContentTypes.Text)]
    public class RainbowToolTipPresenterProvider : IIntellisensePresenterProvider
    {
        [Import]
        public ITextEditorFactoryService EditorFactory { get; set; }

        [Import]
        public IEditorOptionsFactoryService OptionsFactory { get; set; }

        public IIntellisensePresenter TryCreateIntellisensePresenter(IIntellisenseSession session)
        {
            IQuickInfoSession infoSession = session as IQuickInfoSession;
            if (infoSession != null)
            {
                if (infoSession.Get<RainbowToolTipContext>() != null)
                {
                    return new RainbowToolTipPresenter(infoSession, this);
                }
            }

            return null;
        }
    }

    public class RainbowToolTipPresenter : IPopupIntellisensePresenter, IIntellisenseCommandTarget
    {
        private IQuickInfoSession session;
        private ITrackingSpan trackingSpan;
        private QuickInfoPresenter presenter;

        public IIntellisenseSession Session => this.session;

        public double Opacity
        {
            get { return this.presenter.Opacity; }
            set { this.presenter.Opacity = value; }
        }

        public PopupStyles PopupStyles { get; private set; }

        public ITrackingSpan PresentationSpan
        {
            get
            {
                if (this.trackingSpan == null)
                {
                    this.trackingSpan = GetPresentationSpan();
                }

                return this.trackingSpan;
            }
        }

        public string SpaceReservationManagerName { get; private set; }

        public UIElement SurfaceElement => this.presenter;

        public event EventHandler SurfacElementChanged;

        public event EventHandler PresentationSpanChanged;

        public event EventHandler<ValueChangedEventArgs<PopupStyles>> PopupStylesChanged;

        public RainbowToolTipPresenter(IQuickInfoSession infoSession, RainbowToolTipPresenterProvider provider)
        {
            this.session = infoSession;
            this.session.Dismiss += OnSessionDismissed;
            this.presenter = new QuickInfoPresenter();
            this.presenter.EditorFactory = provider.EditorFactory;
            this.presenter.OptionsFactory = provider.OptionsFactory;
            this.presenter.Opacity = 1.0;
            this.presenter.SnapsToDevicePixels = true;
            this.presenter.BindToSource(infoSession.QuickInfoContent);
            this.PopupStyles = PopupStyles.DismissOnMouseLeaveText | PopupStyles.PositionClosest;
            this.SpaceReservationManagerName = "quickinfo";
        }

        private ITrackingSpan GetPresentationSpan()
        {
            return this.session.ApplicableToSpan;
        }

        public bool ExecuteKeyboardCommand(IntellisenseKeyboardCommand command)
        {
            switch (command)
            {
                case IntellisenseKeyboardCommand.Escape:
                    if (this.session != null)
                    {
                        this.session.Dismiss();
                        return true;
                    }
                    break;
            }

            return false;
        }

        private void OnSessionDismissed(object sender, EventArgs e)
        {
            this.session.Dismiss -= this.OnSessionDismissed;
            if (this.presenter != null)
            {
                this.presenter.Close();
            }
        }
    }
}

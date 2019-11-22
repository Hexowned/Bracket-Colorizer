using BracketPairColorizer.Settings.Settings;
using System;
using System.Windows;

namespace BracketPairColorizer.Core.Settings
{
    public class VsfSettingEventManager : WeakEventManager
    {
        public static void AddListener(IUpdatableSettings source, IWeakEventListener handler)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (handler == null)
                throw new ArgumentNullException("handler");

            CurrentManager.ProtectedAddListener(source, handler);
        }

        public static void RemoveListener(IUpdatableSettings source, IWeakEventListener handler)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (handler == null)
                throw new ArgumentNullException("handler");

            CurrentManager.ProtectedRemoveListener(source, handler);
        }

        private static VsfSettingEventManager CurrentManager
        {
            get
            {
                var managerType = typeof(VsfSettingEventManager);
                var manager = (VsfSettingEventManager)GetCurrentManager(managerType);

                if (manager == null)
                {
                    manager = new VsfSettingEventManager();
                    SetCurrentManager(managerType, manager);
                }

                return manager;
            }
        }

        protected override void StartListening(object source)
        {
            var typedSource = (IUpdatableSettings)source;
            typedSource.SettingsChanged += DeliverEvent;
        }

        protected override void StopListening(object source)
        {
            var typedSource = (IUpdatableSettings)source;
            typedSource.SettingsChanged -= DeliverEvent;
        }
    }
}

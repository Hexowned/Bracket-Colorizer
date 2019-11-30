using System;
using System.ComponentModel;
using BracketPairColorizer.Contracts;
using BracketPairColorizer.Settings;

namespace BracketPairColorizer.Xml
{
    [ExtenderProvidedProperty(typeof(IXmlSettings))]
    public class XmlSettings : SettingsBase, IXmlSettings
    {
        public bool XmlnsPrefixEnabled
        {
            get { return this.Store.GetBoolean(nameof(XmlnsPrefixEnabled), true); }
            set { this.Store.SetValue(nameof(XmlnsPrefixEnabled), value); }
        }

        public bool XmlCloseTagEnabled
        {
            get { return this.Store.GetBoolean(nameof(XmlCloseTagEnabled), true); }
            set { this.Store.SetValue(nameof(XmlCloseTagEnabled), value); }
        }

        public bool XmlMatchTagsEnabled
        {
            get { return this.Store.GetBoolean(nameof(XmlMatchTagsEnabled), true); }
            set { this.Store.SetValue(nameof(XmlMatchTagsEnabled), value); }
        }

        [ImportingConstructor]
        public XmlSettings(ITypedSettingsStore store, IBpcTelemetry telemetry)
            : base(store)
        {
            telemetry.FeatureStatus("XmlnsPrefix", XmlnsPrefixEnabled);
            telemetry.FeatureStatus("XmlCloseTag", XmlCloseTagEnabled);
            telemetry.FeatureStatus("XmlMatchTag", XmlMatchTagsEnabled);
        }
    }
}
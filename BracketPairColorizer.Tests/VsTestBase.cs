using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace BracketPairColorizer.Tests
{
    [Collection("DependsOnVS")]
    public class VsTestBase
    {
        private readonly VsEditorHost editorHost;
        private static VsEditorHost cachedEditorHost;
        public const string CSharpContentType = "CSharp";

        public VsEditorHost EditorHost => this.editorHost;

        public VsTestBase()
        {
            if ( Application.Current == null )
            {
                new Application();
            }

            this.editorHost = GetOrCreateEditorHost();
        }

        public string[] ReadResource(String name)
        {
            Assembly assembly = this.GetType().Assembly;
            var stream = assembly.GetManifestResourceStream(name);
            IList<string> lines = new List<string>();
            using ( var reader = new StreamReader(stream) )
            {
                string line = null;
                while ( (line = reader.ReadLine()) != null )
                {
                    lines.Add(line);
                }
            }

            return lines.ToArray();
        }

        public ILanguage GetLanguage(ITextBuffer buffer)
        {
            var factory = EditorHost.CompositionContainer.GetExportedValue<ILanguageFactory>();

            return factory.TryCreateLanguage(buffer);
        }

        public ITextBuffer GetCSharpTextBuffer(string file)
        {
            var contentType = this.EditorHost.GetOrCreateContentType(CSharpContentType, "text");

            return this.EditorHost.CreateTextBuffer(contentType, ReadResource(GetType().Namespace + "." + file));
        }

        public ITextBuffer GetPlainTextBuffer(string file)
        {
            var contentType = this.EditorHost.GetOrCreateContentType("text", "any");

            return this.EditorHost.CreateTextBuffer(contentType, ReadResource(GetType().Name + "." + file));
        }

        private VsEditorHost GetOrCreateEditorHost()
        {
            if ( cachedEditorHost == null )
            {
                var editorHostFactory = new EditorHostFactory();
                var catalog = new AggregateCatalog(
                  new AssemblyCatalog(typeof(IUpdatableSettings).Assembly),
                  new AssemblyCatalog(typeof(LanguageFactory).Assembly), // BracketPairColorizer.Languages
                  new AssemblyCatalog(typeof(Guids).Assembly), // BracketPairColorizer.Core
                  new AssemblyCatalog(typeof(TextBufferBraces).Assembly), // BracketPairColorizer.Rainbow
                  new AssemblyCatalog(typeof(XmlTaggerProvider).Assembly) // BracketPairColorizer.Xml
                  );
                editorHostFactory.Add(catalog);
                var compositionContainer = editorHostFactory.CreateCompositionContainer();
                cachedEditorHost = new VsEditorHost(compositionContainer);
            }

            return cachedEditorHost;
        }
    }
}

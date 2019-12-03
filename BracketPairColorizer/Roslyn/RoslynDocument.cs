#region USING_DIRECTIVES

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Text;
using System.Threading.Tasks;

#endregion USING_DIRECTIVES

namespace BracketPairColorizer.Roslyn
{
    public class RoslynDocument
    {
        public Workspace Workspace { get; private set; }
        public Document Document { get; private set; }
        public SemanticModel SemanticModel { get; private set; }
        public SyntaxNode SyntaxRoot { get; private set; }
        public ITextSnapshot Snapshot { get; private set; }

        private RoslynDocument()
        {
        }

        public static async Task<RoslynDocument> Resolve(ITextBuffer buffer, ITextSnapshot snapshot)
        {
            var workspace = buffer.GetWorkspace();
            var document = snapshot.GetOpenDocumentInCurrentContextWithChanges();
            var semanticModel = await document.GetSemanticModelAsync().ConfigureAwait(false);
            var syntaxRoot = await document.GetSyntaxRootAsync().ConfigureAwait(false);

            return new RoslynDocument
            {
                Workspace = workspace,
                Document = document,
                SemanticModel = semanticModel,
                SyntaxRoot = syntaxRoot,
                Snapshot = snapshot
            };
        }
    }
}
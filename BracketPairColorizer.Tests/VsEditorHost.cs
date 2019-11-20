namespace BracketPairColorizer.Tests
{
    public class VsEditorHost : EditorHost
    {
        public VsEditorHost(CompositionContainer compositionContainer)
            : base(compositionContainer)
        {
        }

        public IBufferTagAggregatorFactoryService GetBufferTagAggregatorFactory
        {
            get { return this.CompositionContainer.GetExportedValue<IBufferTagAggregatorFactoryService>(); }
        }
    }
}

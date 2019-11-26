using Microsoft.VisualStudio.Shell;
using System;

namespace BracketPairColorizer.Core.Compatibility
{
    public class SComponentModel
    {
        public const string SComponentModelHost = "FD57C398-FDE3-42c2-A358-660F269CBE43";
        private object sComponentModel;

        public SComponentModel()
        {
            this.sComponentModel = ServiceProvider.GlobalProvider.GetService(new Guid(SComponentModelHost));
        }

        public T GetService<T>()
        {
            var generic = this.sComponentModel.GetType().GetMethod("GetService");
            var concrete = generic.MakeGenericMethod(typeof(T));

            return (T)concrete.Invoke(this.sComponentModel, null);
        }
    }
}

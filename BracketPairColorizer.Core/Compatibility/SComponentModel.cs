using System;

namespace BracketPairColorizer.Core.Compatibility
{
    public class SComponentModel
    {
        public const string SComponentModelHost = ""; // TODO:
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

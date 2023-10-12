using Autofac;
using PA.QuoteSubmission.SharedKernel;
using System.Reflection;

namespace PA.QuoteSubmission.Web
{
    public class ApiContainerModule : AssemblyScanModule
    {
        protected override Assembly Assembly => Assembly.GetExecutingAssembly();

        /*
         * Implement below method to customize the Dependency loading
        protected override void Load(Autofac.ContainerBuilder builder)
        {
            
        }*/
    }
}

using System.Reflection;
using PA.QuoteSubmission.SharedKernel;

namespace PA.QuoteSubmission.Core
{
    /// <summary>
    /// Injecting Dependent Objects into the IoC.
    /// </summary>
    public class CoreContainerModule : AssemblyScanModule 
    {
        protected override Assembly Assembly => Assembly.GetExecutingAssembly();

        /*
         * Implement below method to customize the Dependency loading
        protected override void Load(Autofac.ContainerBuilder builder)
        {
            
        }*/
    }
}

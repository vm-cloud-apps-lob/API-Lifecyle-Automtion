using System.Reflection;

namespace PA.QuoteSubmission.SharedKernel
{
    /// <summary>
    /// Injecting Dependent Objects into the IoC.
    /// </summary>
    public class SharedKernelContainerModule : AssemblyScanModule
    {
        protected override Assembly Assembly => Assembly.GetExecutingAssembly();

        /*
         * Implement below method to customize the Dependency loading
        protected override void Load(Autofac.ContainerBuilder builder)
        {
            
        }*/

    }
}

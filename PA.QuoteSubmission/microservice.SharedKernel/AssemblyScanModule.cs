using Autofac;
using System.Reflection;

namespace PA.QuoteSubmission.SharedKernel
{
    public abstract class AssemblyScanModule : Autofac.Module
    {
        protected abstract Assembly Assembly { get; }
        protected override void Load(ContainerBuilder builder) =>
            builder.RegisterAssemblyTypes(Assembly)
                   .AsImplementedInterfaces();
    }
}

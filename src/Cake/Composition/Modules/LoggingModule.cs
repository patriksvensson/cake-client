using Cake.Core.Composition;
using Cake.Core.Diagnostics;
using Cake.Diagnostics;

namespace Cake.Modules
{
    public sealed class LoggingModule : ICakeModule
    {
        private readonly Verbosity _verbosity;

        public LoggingModule(Verbosity verbosity)
        {
            _verbosity = verbosity;
        }

        public void Register(ICakeContainerRegistrar registrar)
        {
            registrar.RegisterType<ConsoleLog>().As<ICakeLog>().Singleton();
            registrar.RegisterInstance(new ConsoleLogSettings(_verbosity)).AsSelf().Singleton();
        }
    }
}

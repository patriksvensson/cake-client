using Cake.Core;
using Cake.Core.Diagnostics;

namespace Cake.Internal.Diagnostics
{
    internal sealed class CakeLog : ICakeLog
    {
        private readonly CakeBuildLog _log;

        public Verbosity Verbosity
        {
            get => _log.Verbosity;
            set => _log.Verbosity = value;
        }

        public CakeLog(IConsole console, LogSettings settings)
        {
            _log = new CakeBuildLog(console, settings.Verbosity);
        }

        public void Write(Verbosity verbosity, LogLevel level, string format, params object[] args)
        {
            _log.Write(verbosity, level, format, args);
        }
    }
}

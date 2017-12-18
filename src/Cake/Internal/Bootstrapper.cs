using System.Collections.Generic;
using System.Linq;
using Cake.Commands;
using Cake.Core;
using Cake.Core.Composition;
using Cake.Core.Configuration;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Internal.Temp;

namespace Cake.Internal
{
    internal sealed class Bootstrapper : IBootstrapper
    {
        private readonly IFileSystem _fileSystem;
        private readonly ICakeEnvironment _environment;
        private readonly ICakeLog _log;

        public Bootstrapper(IFileSystem fileSystem, ICakeEnvironment environment, ICakeLog log)
        {
            _fileSystem = fileSystem;
            _environment = environment;
            _log = log;
        }

        public ICakeConfiguration GetConfiguration(BuildSettings settings, ILookup<string, string> arguments)
        {
            var provider = new CakeConfigurationProvider(_fileSystem, _environment);
            return provider.CreateConfiguration(
                settings.Script.GetDirectory(),
                arguments.ToDictionary(x => x.Key, x => x.FirstOrDefault() ?? string.Empty));
        }

        public IEnumerable<ICakeModule> LoadModules(BuildSettings settings, ICakeConfiguration configuration)
        {
            var moduleLoader = new ModuleLoader(
                new ModuleSearcher(_fileSystem, _environment, 
                    new AssemblyLoader(_environment, _fileSystem, _log), _log), 
                _log, _environment);

            return moduleLoader.LoadModules(settings, configuration);
        }
    }
}

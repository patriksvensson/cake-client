using System.Linq;
using Cake.Core;
using Cake.Core.Configuration;
using Cake.Core.IO;

namespace Cake
{
    public sealed class ConfigReader
    {
        private readonly IFileSystem _fileSystem;
        private readonly ICakeEnvironment _environment;

        public ConfigReader(IFileSystem fileSystem, ICakeEnvironment environment)
        {
            _fileSystem = fileSystem;
            _environment = environment;
        }

        public ICakeConfiguration ReadConfiguration(DirectoryPath root, ILookup<string, string> arguments)
        {
            var provider = new CakeConfigurationProvider(_fileSystem, _environment);
            return provider.CreateConfiguration(
                root,
                arguments.ToDictionary(x => x.Key, x => x.FirstOrDefault() ?? string.Empty));
        }
    }
}

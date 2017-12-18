using System.Collections.Generic;
using System.Linq;
using Cake.Commands;
using Cake.Core.Composition;
using Cake.Core.Configuration;

namespace Cake
{
    public interface IBootstrapper
    {
        ICakeConfiguration GetConfiguration(BuildSettings settings, ILookup<string, string> arguments);
        IEnumerable<ICakeModule> LoadModules(BuildSettings settings, ICakeConfiguration configuration);
    }
}
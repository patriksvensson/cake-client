using System.Collections.Generic;
using System.Linq;
using Cake.Core;

namespace Cake.Internal
{
    internal sealed class CakeArguments : ICakeArguments
    {
        public Dictionary<string, string> Arguments { get; }

        public CakeArguments(ILookup<string, string> arguments)
        {
            Arguments = arguments.ToDictionary(x => x.Key, x => x.FirstOrDefault() ?? string.Empty);
        }

        public bool HasArgument(string name)
        {
            return Arguments.ContainsKey(name);
        }

        public string GetArgument(string name)
        {
            return Arguments.ContainsKey(name)
                ? Arguments[name] : null;
        }
    }
}
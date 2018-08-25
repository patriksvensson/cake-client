using System;
using System.Linq;
using Cake.Core;
using Cake.Core.Composition;

namespace Cake.Modules
{
    internal sealed class ArgumentsModule : ICakeModule
    {
        private readonly ILookup<string, string> _arguments;

        public ArgumentsModule(ILookup<string, string> arguments)
        {
            _arguments = arguments;
        }

        public void Register(ICakeContainerRegistrar registrar)
        {
            if (registrar == null)
            {
                throw new ArgumentNullException(nameof(registrar));
            }
            registrar.RegisterInstance(new CakeArguments(_arguments)).As<ICakeArguments>();
        }
    }
}

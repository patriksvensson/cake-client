using System;
using System.Linq;
using Cake.Core;
using Cake.Core.Composition;
using Spectre.Cli;

namespace Cake.Modules
{
    internal sealed class ArgumentsModule : ICakeModule
    {
        private readonly IRemainingArguments _arguments;

        public ArgumentsModule(IRemainingArguments arguments)
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

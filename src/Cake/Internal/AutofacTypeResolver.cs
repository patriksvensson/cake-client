using System;
using Autofac;
using Spectre.CommandLine;

namespace Cake.Internal
{
    internal sealed class AutofacTypeResolver : ITypeResolver
    {
        private readonly ILifetimeScope _scope;

        public AutofacTypeResolver(ILifetimeScope scope)
        {
            _scope = scope;
        }

        public object Resolve(Type type)
        {
            return _scope.Resolve(type);
        }
    }
}
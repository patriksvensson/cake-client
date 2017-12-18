// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Autofac;
using Autofac.Builder;
using Cake.Core.Composition;

namespace Cake.Internal.Composition
{
    internal sealed class ContainerRegistrar : ICakeContainerRegistrar
    {
        public ContainerBuilder Builder { get; }

        public ContainerRegistrar()
        {
            Builder = new ContainerBuilder();
        }

        public void RegisterModule(ICakeModule module)
        {
            module.Register(this);
        }

        public ICakeRegistrationBuilder RegisterType(Type type)
        {
            var registration = Builder.RegisterType(type);
            return new ContainerRegistrationBuilder<object, ConcreteReflectionActivatorData>(registration);
        }

        public ICakeRegistrationBuilder RegisterInstance<TImplementationType>(TImplementationType instance)
            where TImplementationType : class
        {
            var registration = Builder.RegisterInstance(instance);
            return new ContainerRegistrationBuilder<TImplementationType, SimpleActivatorData>(registration);
        }

        public IContainer Build()
        {
            return Builder.Build();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Cake.Commands.Run;
using Cake.Core;
using Cake.Core.Composition;
using Cake.Core.Configuration;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Temp;

namespace Cake.Utilities
{
    public sealed class ModuleLoader
    {
        private readonly ICakeEnvironment _environment;
        private readonly ICakeLog _log;
        private readonly ModuleSearcher _searcher;

        public ModuleLoader(IFileSystem filesystem, ICakeEnvironment environment, ICakeLog log)
        {
            _environment = environment;
            _log = log;
            _searcher = new ModuleSearcher(filesystem, environment, new AssemblyLoader(_environment, filesystem, new AssemblyVerifier()), log);
        }

        public ICakeModule LoadModule(IContainer container, FilePath path)
        {
            var moduleType = _searcher.LoadModule(path);
            if (moduleType != null)
            {
                var constructor = GetGreediestConstructor(moduleType);
                var parameters = constructor.GetParameters();

                var arguments = new object[parameters.Length];
                for (int index = 0; index < parameters.Length; index++)
                {
                    var parameter = parameters[index];
                    arguments[index] = container.Resolve(parameter.ParameterType);
                }

                if (Activator.CreateInstance(moduleType, arguments) is ICakeModule module)
                {
                    return module;
                }
            }

            return null;
        }

        public IEnumerable<ICakeModule> LoadModule(RunCommandSettings settings, ICakeConfiguration configuration, IContainer container)
        {
            var root = GetModulePath(configuration, settings.Script.GetDirectory());

            var moduleTypes = _searcher.Search(root);
            if (moduleTypes.Count > 0)
            {
                var result = new List<ICakeModule>();
                foreach (var moduleType in moduleTypes)
                {
                    var constructor = GetGreediestConstructor(moduleType);
                    var parameters = constructor.GetParameters();

                    var arguments = new object[parameters.Length];
                    for (int index = 0; index < parameters.Length; index++)
                    {
                        var parameter = parameters[index];
                        arguments[index] = container.Resolve(parameter.ParameterType);
                    }

                    if (Activator.CreateInstance(moduleType, arguments) is ICakeModule module)
                    {
                        result.Add(module);
                    }
                }

                return result;
            }

            return Enumerable.Empty<ICakeModule>();
        }

        private void RegisterExternalModules(IEnumerable<Type> moduleTypes, ILifetimeScope scope)
        {
            var builder = new ContainerBuilder();
            foreach (var moduleType in moduleTypes)
            {
                _log.Debug("Registering module {0}...", moduleType.FullName);
                builder.RegisterType(moduleType).As<ICakeModule>().SingleInstance();
            }
        }

        private DirectoryPath GetToolPath(ICakeConfiguration configuration, DirectoryPath root)
        {
            var toolPath = configuration.GetValue("Paths_Tools");
            if (!string.IsNullOrWhiteSpace(toolPath))
            {
                return new DirectoryPath(toolPath).MakeAbsolute(_environment);
            }
            return root.Combine("tools");
        }

        private DirectoryPath GetModulePath(ICakeConfiguration configuration, DirectoryPath root)
        {
            var modulePath = configuration.GetValue("Paths_Modules");
            if (!string.IsNullOrWhiteSpace(modulePath))
            {
                return new DirectoryPath(modulePath).MakeAbsolute(_environment);
            }
            var toolPath = GetToolPath(configuration, root);
            return toolPath.Combine("Modules").Collapse();
        }

        private static ConstructorInfo GetGreediestConstructor(Type type)
        {
            ConstructorInfo current = null;
            var count = -1;
            foreach (var constructor in type.GetTypeInfo().GetConstructors())
            {
                var parameters = constructor.GetParameters();
                if (parameters.Length > count)
                {
                    count = parameters.Length;
                    current = constructor;
                }
            }
            return current;
        }
    }
}

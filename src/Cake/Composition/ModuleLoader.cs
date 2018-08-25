using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Cake.Commands;
using Cake.Core;
using Cake.Core.Composition;
using Cake.Core.Configuration;
using Cake.Core.Diagnostics;
using Cake.Core.IO;

namespace Cake.Composition
{
    internal sealed class ModuleLoader
    {
        private readonly ModuleSearcher _searcher;
        private readonly ICakeLog _log;
        private readonly ICakeEnvironment _environment;

        public ModuleLoader(ModuleSearcher searcher, ICakeLog log, ICakeEnvironment environment)
        {
            _searcher = searcher;
            _log = log;
            _environment = environment;
        }

        public IEnumerable<ICakeModule> LoadModules(RunSettings settings, ICakeConfiguration configuration)
        {
            var root = GetModulePath(configuration, settings.Script.GetDirectory());

            var moduleTypes = _searcher.Search(root);
            if (moduleTypes.Count > 0)
            {
            }

            return Enumerable.Empty<ICakeModule>();
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

        private void RegisterExternalModules(IEnumerable<Type> moduleTypes, ILifetimeScope scope)
        {
            var builder = new ContainerBuilder();
            foreach (var moduleType in moduleTypes)
            {
                _log.Debug("Registering module {0}...", moduleType.FullName);
                builder.RegisterType(moduleType).As<ICakeModule>().SingleInstance();
            }
        }
    }
}

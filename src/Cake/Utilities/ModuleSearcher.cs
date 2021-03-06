﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.Composition;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Core.Reflection;

namespace Cake.Utilities
{
    public sealed class ModuleSearcher
    {
        private readonly IFileSystem _fileSystem;
        private readonly ICakeEnvironment _environment;
        private readonly IAssemblyLoader _assemblyLoader;
        private readonly ICakeLog _log;

        public ModuleSearcher(
            IFileSystem fileSystem,
            ICakeEnvironment environment,
            IAssemblyLoader assemblyLoader,
            ICakeLog log)
        {
            _fileSystem = fileSystem;
            _environment = environment;
            _assemblyLoader = assemblyLoader;
            _log = log;
        }

        public IReadOnlyList<Type> Search(DirectoryPath path)
        {
            path = path.MakeAbsolute(_environment);
            var root = _fileSystem.GetDirectory(path);
            if (!root.Exists)
            {
                _log.Debug("Module directory does not exist.");
                return new Type[] { };
            }

            var result = new List<Type>();
            var files = root.GetFiles("Cake.*.Module.dll", SearchScope.Recursive);
            foreach (var file in files)
            {
                var module = LoadModule(file.Path);
                if (module != null)
                {
                    result.Add(module);
                }
            }

            return result;
        }

        public Type LoadModule(FilePath path)
        {
            try
            {
                var assembly = _assemblyLoader.Load(path, true);

                var attribute = assembly.GetCustomAttributes<CakeModuleAttribute>().FirstOrDefault();
                if (attribute == null)
                {
                    _log.Warning("The assembly '{0}' does not have module metadata.", path.FullPath);
                    return null;
                }

                if (!typeof(ICakeModule).IsAssignableFrom(attribute.ModuleType))
                {
                    _log.Warning("The module type '{0}' is not an actual module.", attribute.ModuleType.FullName);
                    return null;
                }

                return attribute.ModuleType;
            }
            catch (CakeException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _log.Warning("Could not load module '{0}'. {1}", path.FullPath, ex);
                return null;
            }
        }
    }
}

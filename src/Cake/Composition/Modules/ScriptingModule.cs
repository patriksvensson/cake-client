using Cake.Core.Composition;
using Cake.Core.Scripting;
using Cake.Scripting;
using Cake.Scripting.Roslyn;

namespace Cake.Modules
{
    public sealed class ScriptingModule : ICakeModule
    {
        private readonly bool _debug;

        public ScriptingModule(bool debug)
        {
            _debug = debug;
        }

        public void Register(ICakeContainerRegistrar registrar)
        {
            // Roslyn script engine.
            registrar.RegisterType<RoslynScriptEngine>().As<IScriptEngine>().Singleton();

            // Roslyn script options.
            var scriptOptions = new RoslynScriptOptions();
            if (_debug)
            {
                scriptOptions.PerformDebug = true;
            }
            registrar.RegisterInstance(scriptOptions).As<RoslynScriptOptions>().Singleton();

            // Script hosts.
            registrar.RegisterType<BuildScriptHost>().Singleton();
            registrar.RegisterType<DescriptionScriptHost>().Singleton();
            registrar.RegisterType<DryRunScriptHost>().Singleton();
        }
    }
}

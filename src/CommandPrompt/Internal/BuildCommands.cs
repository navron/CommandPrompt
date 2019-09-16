using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CommandPrompt.Internal
{
    /// <summary>
    /// Build Command Prompts by using refection
    /// </summary>
    internal class BuildCommands
    {
        internal static List<PromptCommand> ScanForPrompt(IPromptConfiguration configuration)
        {
            var list = new List<PromptCommand>();

            var assemblies = configuration.GetScanForAssemblies;
            foreach (var assembly in assemblies.AsParallel()) // in case of a large number of assemblies, do this in parallel
            {
                if (assembly.GlobalAssemblyCache) continue; // Don't want to scan Assembly in the global cache, this may become a defect 

                foreach (var type in assembly.GetTypes())
                {
                    foreach (var methodInfo in type.GetMethods())
                    {
                        if (!(methodInfo.GetCustomAttribute(typeof(PromptAttribute), true) is PromptAttribute attribute))
                            continue;

                        list.Add(GetPromptInformation(type, methodInfo, attribute));
                    }
                }
            }

            return list;
        }

        private static PromptCommand GetPromptInformation(Type classType, MethodInfo methodInfo, PromptAttribute attribute)
        {
            var command = new PromptCommand
            {
                HelpText = attribute.HelpText,
                CommandText = attribute.Command,
                MethodInfo = methodInfo,
                ClassType = classType,
                // If there are Any Parameters then set the Starts with flag -- TODO Should be able to remove this
                StartWith = methodInfo.GetParameters().Length > 0
            };


            // Check if the class of this Method has PromptClass Attribute and add that information to the command
            if (command.ClassType.GetCustomAttributes(typeof(PromptClassAttribute), true).FirstOrDefault() is PromptClassAttribute promptClassAttribute)
            {
                command.KeepClassInstance = promptClassAttribute.Keep;
                command.Folder = promptClassAttribute.Folder;
            }

            return command;
        }

        internal static List<PromptClass> ScanForPromptClasses(IPromptConfiguration configuration)
        {
            var list = new List<PromptClass>();
            var assemblies = configuration.GetScanForAssemblies;
            foreach (var assembly in assemblies.AsParallel()) // in case of a large number of assemblies, do this in parallel
            {
                if (assembly.GlobalAssemblyCache) continue; // Don't want to scan Assembly in the global cache, this may become a defect 

                foreach (var type in assembly.GetTypes())
                {
                    if (!(type.GetCustomAttributes(typeof(PromptClassAttribute), true).FirstOrDefault() is
                        PromptClassAttribute promptClassAttribute)) continue;

                    var folder = new PromptClass
                    {
                        TypeId = promptClassAttribute.TypeId,
                        Folder = promptClassAttribute.Folder,
                        KeepClassInstance = promptClassAttribute.Keep,
                        Help = promptClassAttribute.Help
                    };
                    list.Add(folder);
                }
            }

            return list;
        }
    }
}

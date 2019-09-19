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
        internal static void ScanForPrompt(Prompt prompt)
        {
            prompt.PromptClasses = new List<PromptClass>(); // TODO Rework this code so the assemblies can be scanned in Parallel,  List<PromptClass>() is not concurrent
            prompt.PromptCommands = new List<PromptCommand>();

            var assemblies = prompt.Configuration.GetScanForAssemblies;
            foreach (var assembly in assemblies) // in case of a large number of assemblies, do this in parallel
            {
                if (assembly.GlobalAssemblyCache) continue; // Don't want to scan Assembly in the global cache, this may become a defect 

                foreach (var type in assembly.GetTypes())
                {
                    var test = prompt.PromptCommands.Count;
                    foreach (var methodInfo in type.GetMethods())
                    {
                        var attributes = methodInfo.GetCustomAttributes();
                        prompt.PromptCommands.AddRange(from attribute in attributes.OfType<PromptAttribute>()
                                                    let info = GetPromptInformation(prompt, type, methodInfo, attribute)
                                                    select GetPromptInformation(prompt, type, methodInfo, attribute));
                    }

                    if (test != prompt.PromptCommands.Count) // Change, Stupid need better test
                        prompt.PromptClasses.Add(GetPromptClassInformation(type));
                }
            }
        }

        private static PromptCommand GetPromptInformation(Prompt prompt, Type classType, MethodInfo methodInfo, PromptAttribute attribute)
        {
            var command = new PromptCommand
            {
                HelpText = attribute.Help,
                CommandText = attribute.Command,
                MethodInfo = methodInfo,
                ClassType = classType,
                // If there are Any Parameters then set the Starts with flag -- TODO Should be able to remove this
                StartWith = methodInfo.GetParameters().Length > 0,
                Hide = attribute.Hide
            };

            // Check if the class of this Method has PromptClass Attribute and add that information to the command
            if (classType.GetCustomAttributes(typeof(PromptClassAttribute), true).FirstOrDefault() is
                PromptClassAttribute promptClassAttribute)
            {
                command.KeepClassInstance = promptClassAttribute.Keep;
                command.Folder = promptClassAttribute.Folder;
            }

            return command;
        }


        private static PromptClass GetPromptClassInformation(Type classType)
        {
            // Create a 
            var promptClass = new PromptClass
            {
                TypeId = classType
            };

            // Check if the class of this Method has PromptClass Attribute and add that information to the command
            if (classType.GetCustomAttributes(typeof(PromptClassAttribute), true).FirstOrDefault() is
                PromptClassAttribute promptClassAttribute)
            {
                promptClass.KeepClassInstance = promptClassAttribute.Keep;
                promptClass.Folder = promptClassAttribute.Folder;
                promptClass.Help = promptClassAttribute.Description;
            }

            return promptClass;
        }
    }
}

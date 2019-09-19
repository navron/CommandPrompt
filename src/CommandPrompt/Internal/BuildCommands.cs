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
            prompt.CommandClass = new List<PromptClass>(); // TODO Rework this code so the assemblies can be scanned in Parallel,  List<PromptClass>() is not concurrent
            prompt.CommandList = new List<PromptCommand>();

            var assemblies = prompt.Configuration.GetScanForAssemblies;
            foreach (var assembly in assemblies) // in case of a large number of assemblies, do this in parallel
            {
                if (assembly.GlobalAssemblyCache) continue; // Don't want to scan Assembly in the global cache, this may become a defect 

                foreach (var type in assembly.GetTypes())
                {
                    var test = prompt.CommandList.Count;
                    foreach (var methodInfo in type.GetMethods())
                    {
                        var attributes = methodInfo.GetCustomAttributes();
                        prompt.CommandList.AddRange(from attribute in attributes.OfType<PromptAttribute>()
                                                    let info = GetPromptInformation(prompt, type, methodInfo, attribute)
                                                    select GetPromptInformation(prompt, type, methodInfo, attribute));
                    }

                    if (test != prompt.CommandList.Count) // Change, Stupid need better test
                        prompt.CommandClass.Add(GetPromptClassInformation(type));
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
                promptClass.Help = promptClassAttribute.Help;
            }

            return promptClass;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using CommandPrompt.Internal;

[assembly: InternalsVisibleTo("CommandPrompt.Tests")]
namespace CommandPrompt
{
    /// <summary>
    /// A CommandText Prompt with an console REPL interface 
    /// </summary>
    public class Prompt
    {
        /// <summary>
        /// A asynchronous Run method for an REPL 
        /// </summary>
        /// <param name="configuration">Prompt Configuration, null for using default settings</param>
        /// <param name="token">Cancellation Token, allows the task to exit nicely</param>
        /// <returns></returns>
        public static async Task RunAsync(IPromptConfiguration configuration = null, CancellationToken token = default)
        {
            var prompt = new Prompt(configuration);
            var cmd = string.Empty;
            while (!string.Equals(cmd, "exit", StringComparison.OrdinalIgnoreCase))
            {
                // ReadLine is not await able, hence running as a Task.Run. Improve foreign project to 
                // include ReadLine.ReadAsync( with CancellationToken)
                await Task.Run(() => cmd = ReadLine.Read($"{configuration?.GetOption("Prompt")}"), token);
                try
                {
                    //  _ = prompt.ProcessPrompt(cmd, prompt.commandList, prompt.configuration);
                    prompt.ProcessPrompt(cmd);
                }
                catch (Exception exception)
                {
                    // Don't allow an exception to crash the application by a throw
                    Console.WriteLine(exception);
                }
            }

            HistoryFile.Save(configuration);
        }

        internal readonly IPromptConfiguration Configuration;
        internal readonly List<PromptCommand> CommandList;

        public Prompt(IPromptConfiguration configuration)
        {
            try
            {
                Configuration = configuration;
                ReadLine.HistoryEnabled = string.Equals(Configuration.GetOption("HistoryEnabled"), "True", StringComparison.OrdinalIgnoreCase);

                CommandList = BuildCommands.ScanForPrompt(configuration);

                HistoryFile.Load(configuration);
            }
            catch (Exception e)
            {
                // Don't allow an exception to crash the application by a throw
                // Show the error so that an developer can fix the problem. 
                Console.WriteLine(e);
            }
        }
       
        /// <summary>
        /// Run the given command
        /// </summary>
        /// <param name="command"></param>
        /// <returns>true if an command was found and ran</returns>
        internal bool ProcessPrompt(string command) => ProcessPrompt(command, CommandList, Configuration);
        private bool ProcessPrompt(string cmd, List<PromptCommand> cmdList, IPromptConfiguration config)
        {
            var testCmd = cmd.ToLower();
            foreach (var command in cmdList)
            {
                if (command.StartWith)
                {
                    var test = $"{command.CommandText.ToLower().TrimEnd()} ";
                    if (testCmd.StartsWith(test)) // Check with a space at the end. 
                    {
                        return RunCommand(command, cmd, config);
                    }
                }

                if (command.CommandText.ToLower() == testCmd)
                {
                    return RunCommand(command, cmd, config);
                }
            }
            return false;
        }

        // Run the given command
        private bool RunCommand(PromptCommand pCmd, string cmd, IPromptConfiguration config)
        {
            var classInstance = GetCommandClassInstance(pCmd, config);
            if (classInstance == null) return false; // Something when wrong

            if (pCmd.MethodInfo == null) return false;

            var parameters = pCmd.MethodInfo.GetParameters();
            if (parameters.Length == 0)
            {
                // This works fine
                var result = pCmd.MethodInfo.Invoke(classInstance, null);
            }
            else
            {
                List<object> objectList = new List<object>();
                var test = cmd.Remove(0, pCmd.CommandText.Length).Trim();
                if (parameters.Length == 1)
                {
                    if (ParameterConversion.Convert(test, parameters[0].ParameterType, out var ob)) // BUG HERE
                    {
                        objectList.Add(ob);
                    }

                }
                else
                {
                    var parametersTest = ParameterConversion.SplitCommand(test).ToArray(); // TODO FIX THIS CODE
                    if (parameters.Length != parametersTest.Length)
                    {
                        Console.WriteLine($"Error parsing command, incorrect parameter count");
                        return false;
                    }

                    for (int i = 0; i < parameters.Length; i++)
                    {
                        if (ParameterConversion.Convert(parametersTest[i], parameters[i].ParameterType, out var ob))
                        {
                            objectList.Add(ob);
                        }
                        else
                        {
                            Console.WriteLine($"Error parsing command, incorrect parameter type");
                            return false;
                        }
                    }
                }

                var result = pCmd.MethodInfo.Invoke(classInstance, objectList.ToArray());
            }

            return true;
        }

        #region Prompt Command Class Instance

        // Internal storage of previous class instance that was used to run commands
        internal List<object> CmdPromptClasses = new List<object>();

        // Get the Class Instance for the command that is to be run
        private object GetCommandClassInstance(PromptCommand pCmd, IPromptConfiguration config)
        {
            // If this class instance exists in the configuration store then return it
            var classInstance = CmdPromptClasses.FirstOrDefault(promptClass => promptClass.GetType() == pCmd.ClassType);
            if (classInstance != null)
            {
                // Great Work Class Instance Already created
                return classInstance;
            }

            // We only Deal with the first Constructor (Restriction on current design) 
            var constructorInfo = pCmd.ClassType.GetConstructors(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public).FirstOrDefault();
            if (constructorInfo == null)
            {
                Console.WriteLine($"Cannot create of type {pCmd.ClassType} due to no public constructor");
                return null; // Cannot create class Whoops
            }

            // OtherWise, Create
            var constructorParameters = constructorInfo.GetParameters();

            classInstance = constructorParameters.Length == 0
                ? Activator.CreateInstance(pCmd.ClassType)
                // The order is important here, Lets hope Linq gets it right
                : Activator.CreateInstance(pCmd.ClassType,
                    constructorInfo.GetParameters().Select(parameterInfo => InjectPromptClass(config, parameterInfo.ParameterType)).ToArray());

            // If the Class has a Custom Attribute to keep this between commands, then store it in the configuration 
            if (pCmd.KeepClassInstance)
            {
                CmdPromptClasses.Add(classInstance);
            }

            return classInstance;
        }

        // Inject this Prompt class, use by internal commands like 'help'
        private object InjectPromptClass(IPromptConfiguration config, Type type)
        {
            return type == typeof(Prompt) ? this : config.GetObjectOfType(type);
        }
        #endregion
    }
}

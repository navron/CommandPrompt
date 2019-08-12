using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommandPrompt.Internal;

[assembly: InternalsVisibleTo("CommandPrompt.Tests")]
namespace CommandPrompt
{
    /// <summary>
    /// A Command Prompt with an console REPL interface 
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
                await Task.Run(() => cmd = ReadLine.Read($"{configuration?.PromptPreFix}"), token);
                try
                {
                    //  _ = prompt.ProcessPrompt(cmd, prompt.commandList, prompt.configuration);
                    prompt.RunCommand(cmd);
                }
                catch (Exception exception)
                {
                    // Don't Allow an exception to crash the application by a throw;
                    Console.WriteLine(exception);
                }
            }

            prompt.History(configuration, false);
        }

        private readonly IPromptConfiguration configuration;
        private readonly List<PromptCommand> commandList;

        public Prompt(IPromptConfiguration configuration)
        {
            ReadLine.HistoryEnabled = true; // TEMP SETTING
            this.configuration = configuration;

            commandList = BuildPromptCommand.ScanForPrompt();

            History(configuration, true);
        }

        private void History(IPromptConfiguration config, bool load)
        {
            var historyFile = configuration.GetOption("HistoryFile");
            if (string.IsNullOrEmpty(historyFile)) return;

            if (load)
            {
                ReadLine.HistoryEnabled = true;
                if (File.Exists(historyFile))
                {
                    var history = File.ReadAllLines(historyFile);
                    ReadLine.AddHistory(history);
                }
            }
            else
            {
                var myHistory = ReadLine.GetHistory();
                File.WriteAllLines(historyFile, myHistory);
            }
        }

        internal void RunCommand(string command)
        {
            if (IsCommandACommandPromptCommand(command))
            {
                ProcessInternalCommandPrompt(command, commandList, configuration);
            }
            else
            {
                ProcessPrompt(command, commandList, configuration);

            }
        }

        private bool IsCommandACommandPromptCommand(string command)
        {
            var cmd = command.ToLower();
            if (cmd == "help" || cmd == "rescan") return true;

            if (cmd.StartsWith("cd")) return true;
            return false;
        }

        private bool ProcessInternalCommandPrompt(string command, List<PromptCommand> cmdList, IPromptConfiguration config)
        {
            var cmd = command.ToLower();
            if (cmd == "help") return ShowHelp(cmdList);
            if (cmd == "rescan")
            {
                cmdList = BuildPromptCommand.ScanForPrompt();
            }

            if (cmd.StartsWith("cd"))
            {
                Console.WriteLine($"Switching to {cmd} TODO");
                return true;
            }

            return false;
        }

        private bool ProcessPrompt(string cmd, List<PromptCommand> cmdList, IPromptConfiguration config)
        {
            var testCmd = cmd.ToLower();
            foreach (var command in cmdList)
            {
                if (command.StartWith)
                {
                    var test = $"{command.CommandLower.TrimEnd()} ";
                    if (testCmd.StartsWith(test)) // Check with a space at the end. 
                    {
                        return RunCommand(command, cmd, config);
                    }
                }


                if (command.CommandLower == testCmd)
                {
                    return RunCommand(command, cmd, config);
                }
            }
            return false;
        }

        private object GetCmdClass(PromptCommand pCmd, IPromptConfiguration config)
        {
            // If this class instance exists in the configuration store then return it
            var classInstance = config.GetClassInstance(pCmd.ClassType);
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
                // The order is important here, Lets hope Linq gets it write
                : Activator.CreateInstance(pCmd.ClassType,
                    constructorInfo.GetParameters().Select(parameterInfo => config.GetObjectOfType(parameterInfo.ParameterType)).ToArray());

            // If the Class has a Custom Attribute to keep this between commands, then store it in the configuration 
            if (pCmd.ClassKeep)
            {
                config.StoreClassInstance(classInstance);
            }

            // If Keep add to Store
            return classInstance;
        }
        private bool RunCommand(PromptCommand pCmd, string cmd, IPromptConfiguration config)
        {
            //if (config.Container != null)
            //{
            //    using (var container = config.Container)
            //    {
            //        var classInstance2 = container.Resolve(pCmd.ClassType);
            //        var methodInfo2 = classInstance2.GetType().GetMethod(pCmd.MethodInfo);
            //        if (methodInfo2 == null) return false;
            //        var result = methodInfo2.Invoke(classInstance2, null);
            //    }

            //    return true;
            //}

            // 
            // if(pCmd.ReUseCommandClass)
            var classInstance = GetCmdClass(pCmd, config);
            if (classInstance == null) return false; // Something when wrong


            var methodInfo = classInstance.GetType().GetMethod(pCmd.MethodInfo);
            if (methodInfo == null) return false;

            if (pCmd.Parameters.Length == 0)
            {
                // This works fine
                var result = methodInfo.Invoke(classInstance, null);
            }
            else
            {
                List<object> objectList = new List<object>();
                var test = cmd.Remove(0, pCmd.Command.Length).Trim();
                if (pCmd.Parameters.Length == 1)
                {
                    if (ParameterConvert(test, pCmd.Parameters[0].ParameterType, out var ob))
                    {
                        objectList.Add(ob);
                    }
                   
                }
                else
                {
                    var parameters = SplitCommand(test).ToArray();
                    if (pCmd.Parameters.Length != parameters.Length)
                    {
                        Console.WriteLine($"Error parsing command, incorrect parameter count");
                        return false;
                    }

                    for (int i = 0; i < parameters.Length; i++)
                    {
                        if (ParameterConvert(parameters[i], pCmd.Parameters[i].ParameterType, out var ob))
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

                var result = methodInfo.Invoke(classInstance, objectList.ToArray());
            }

            return true;
        }

        private bool ParameterConvert(string text, Type parameterType, out object result)
        {
            result = null;
            if (parameterType == typeof(string))
            {
                result = text;
            }
            if (parameterType == typeof(int) && int.TryParse(text, out var resultInt))
            {
                result = resultInt;
            }
            if (parameterType == typeof(ulong) && ulong.TryParse(text, out var resultULong))
            {
                result = resultULong;
            }
            if (parameterType == typeof(int[]))
            {
                var listString = SplitCommand(text);
                var listInt = new List<int>();
                foreach (var s in listString)
                {
                    int.TryParse(s, out var resultIntA);
                    listInt.Add(resultIntA);
                }
                result = listInt.ToArray();
            }

            return result != null;
        }

        private List<string> SplitCommand(string text)
        {
            var result = text.Split('"')
                .Select((element, index) => index % 2 == 0  // If even index
                    ? element.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)  // Split the item
                    : new string[] { element })  // Keep the entire item
                .SelectMany(element => element).ToList();
            return result;
        }

        private static bool ShowHelp(List<PromptCommand> commandList)
        {
            foreach (var command in commandList)
            {
                var str = new StringBuilder();
                str.Append(command.Command);
                foreach (ParameterInfo info in command.Parameters)
                {
                    str.Append($" {info.Name}");

                }
                Console.WriteLine($"{str,-30} {command.HelpText}");
            }

            return true;
        }
    }
}

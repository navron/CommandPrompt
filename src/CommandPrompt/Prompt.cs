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
            var cmd = String.Empty;
            while (!String.Equals(cmd, "exit", StringComparison.OrdinalIgnoreCase))
            {
                var promptText = $"{prompt.Configuration.GetOption("PromptPreFix")}{prompt.CurrentFolder}{prompt.Configuration.GetOption("PromptPostFix")}";
                // ReadLine is not await able, hence running as a Task.Run. Improve foreign project to 
                // include ReadLine.ReadAsync( with CancellationToken)
                await Task.Run(() => cmd = ReadLine.Read(promptText), token);
                try
                {
                    //  _ = pCmd.ProcessPrompt(commandText, pCmd.commandList, pCmd.configuration);
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

        internal string CurrentFolder { get; set; }
        internal readonly IPromptConfiguration Configuration;
        internal List<PromptCommand> CommandList;
        internal List<PromptClass> CommandClass;

        public Prompt(IPromptConfiguration configuration)
        {
            try
            {
                // If Configuration is null, use default settings
                Configuration = configuration ?? new PromptConfiguration();

                ReadLine.HistoryEnabled = String.Equals(Configuration.GetOption("HistoryEnabled"), true.ToString());

                BuildCommands.ScanForPrompt(this);

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
            Warning = string.Empty;
            
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
            if(!string.IsNullOrEmpty(Warning)) Console.WriteLine(Warning);
            return false;
        }

        // Run the given command
        private bool RunCommand(PromptCommand pCmd, string commandText, IPromptConfiguration config)
        {
            var classInstance = GetCommandClassInstance(pCmd, config);
            if (classInstance == null) return false; // Something when wrong

            if (pCmd.MethodInfo == null) return false;

            var parameters = pCmd.MethodInfo.GetParameters();
            if (parameters.Length == 0)
            {
                // Invoke with 'null' as the parameter array
                pCmd.MethodInfo.Invoke(classInstance, null);
                return true;
            }

            // Remove the Command Text from the 
            var parameterText = commandText.Remove(0, pCmd.CommandText.Length).Trim();

            // Build a Parameter List to match the Parameters types of the Method
            // If the Last parameter is a string, the reminding text after the split can be considered part of that parameter

            // Split the text but all for escaped strings
            var parametersSplit = GetParameterString(parameters, parameterText);
            if (parametersSplit == null) return false;

            var parameterList = new List<object>();
            for (int i = 0; i < parameters.Length; i++)
            {
                var ob = Configuration.ParameterConvert(parametersSplit[i], parameters[i].ParameterType);
                if (ob == null)
                {
                    Warning = "Failed parsing command, incorrect parameter type";
                    return false;
                }

                parameterList.Add(ob);
            }

            pCmd.MethodInfo.Invoke(classInstance, parameterList.ToArray());
            return true;
        }

        internal string[] GetParameterString(ParameterInfo[] parameters, string text)
        {
            // Split the text but all for escaped strings
            var parametersSplit = SplitCommand(text).ToArray();

            // Perfect
            if (parameters.Length == parametersSplit.Length) return parametersSplit;

            if (parameters.Length < parametersSplit.Length)
            {
                if (parameters.Last().ParameterType == typeof(string))
                {
                    var last = string.Join(" ", parametersSplit.ToArray(), parameters.Length, parametersSplit.Length - parameters.Length);

                    var list = new List<string>();
                    for (int i = 0; i < parameters.Length - 1; i++)
                    {
                        list.Add(parametersSplit[i]);
                    }

                    list.Add(string.Join(" ", new[] {parametersSplit[parameters.Length - 1], last}));
                    return list.ToArray();
                }

                if (Configuration.ParameterConvert(text, parameters.Last().ParameterType) != null)
                {
                    return new string[] {text};
                }

                Warning = "parameters count for method does not match given command";
                return null; // Can't process text
            }

            if (parameters.Length > parametersSplit.Length)
            {
                for (int i = parametersSplit.Length; i < parameters.Length; i++)
                {
                    if (parameters[i].ParameterType == typeof(string) || IsNullable(parameters[i].ParameterType))
                    {
                        parametersSplit.Append(string.Empty);
                    }
                    else
                    {
                        Warning = "Non null-able type in parameter";
                        return null;
                    }
                }

                return parametersSplit;
            }
            Warning = "Missing required parameters for given command";
            return null; // Can't process text
        }

        bool IsNullable(Type type) => Nullable.GetUnderlyingType(type) != null;

        // Split the command into different parts, 
        internal static List<string> SplitCommand(string text)  //TODO Write Unit Tests
        {
            var result = text.Split('"')
                .Select((element, index) => index % 2 == 0  // If even index
                    ? element.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)  // Split the item
                    : new string[] { element })  // Keep the entire item
                .SelectMany(element => element).ToList();
            return result;
        }

        /// <summary>
        /// If there is a Error processing a command, a message will be available here
        /// </summary>
        public string Warning { get; set; }

        #region Prompt Command Class Instance

        // Internal storage of previous class instance that was used to run commands
        internal List<object> StoreCommandPromptClasses = new List<object>();

        // Get the Class Instance for the command that is to be run
        private object GetCommandClassInstance(PromptCommand pCmd, IPromptConfiguration config)
        {
            // If this class instance exists in the configuration store then return it
            var classInstance = StoreCommandPromptClasses.FirstOrDefault(promptClass => promptClass.GetType() == pCmd.ClassType);
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
                StoreCommandPromptClasses.Add(classInstance);
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

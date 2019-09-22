using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandPrompt.Internal;

namespace CommandPrompt.Commands
{
    /// <summary>
    /// Help Commands for the Command Prompt Library
    /// </summary>
    internal class HelpCmd
    {
        private readonly Prompt prompt;
        private const string LineBreak = "---------------------------------";
        private const int Indent = 2;// two spaces
        private const int PadCommand = 20;

        public HelpCmd(Prompt prompt)
        {
            this.prompt = prompt;
        }

        [Prompt("help", Help = "Shows help", Hide = true)]
        public void Help(string command)
        {
            // 1. If there is no command given then show the help for all the application.
            //   Show the summary help of each command group by folder and sorted 
            // 2. If the command give is a command then show the detail help for that command

            var promptCommands = prompt.PromptCommands;
            var promptClasses = prompt.PromptClasses;
            if (!string.IsNullOrEmpty(prompt.CurrentFolder))
            {
                promptCommands = promptCommands.Where(c =>string.Equals(c.Folder, prompt.CurrentFolder, StringComparison.OrdinalIgnoreCase)).ToList();
                promptClasses = promptClasses.Where(c => string.Equals(c.Folder, prompt.CurrentFolder, StringComparison.OrdinalIgnoreCase)).ToList();
                // If the help for the command is null and because we are in a folder, only show the folder help
                if (string.IsNullOrEmpty(command))
                {
                    command = prompt.CurrentFolder;
                }
            }


            var str = new StringBuilder();
            // Does the given 'command' match any commands
            if (promptCommands.Any(c => string.Equals(c.CommandText, command.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                str.Append(PrintCommandDetailHelp(command, promptCommands));
            }
            // Does the given 'command' match any Folder names
            else if (promptClasses.Any(c => string.Equals(c.Folder, command.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                str.Append(PrintFolderHelp(command, promptClasses, promptCommands));
            }
            // Other wise Print the Application help 
            else
            {
                var appHelp = prompt.Configuration.GetOption("ApplicationHelp");
                str.Append(PrintApplicationHelp(appHelp, promptClasses, promptCommands));
            }

            Console.WriteLine(str.ToString());
        }

        #region Command Description

        private static StringBuilder PrintCommandDetailHelp(string command, List<PromptCommand> commandList)
        {
            var cmdList = commandList.Where(c => string.Equals(c.CommandText, command.Trim(), StringComparison.OrdinalIgnoreCase)).ToList();

            var str = new StringBuilder();
            foreach (var promptCommand in cmdList)
            {
                str.Append(PrintCommandHelpDetails(promptCommand, 0));
            }
            return str;
        }

        private static StringBuilder PrintCommandSummaryHelp(PromptCommand cmd, int indent = Indent)
        {
            return new StringBuilder().AppendLine($"{string.Empty.PadRight(indent)}{cmd.CommandText.PadRight(PadCommand)} {cmd.HelpText,-30}");
        }

        private static StringBuilder PrintCommandHelpDetails(PromptCommand cmd, int indent = Indent)
        {
            var str = new StringBuilder().Append(PrintCommandSummaryHelp(cmd, indent));
            var list = cmd.MethodInfo.GetParameters();
            foreach (var info in list)
            {
                str.AppendLine($"{string.Empty.PadRight(indent+2)}{info.Name.PadRight(PadCommand - 2)} {info.ParameterType}");
            }
            return str;
        }

        #endregion

        #region Folder Description

        private static StringBuilder PrintFolderHelp(string folder, List<PromptClass> promptClasses, List<PromptCommand> commandList)
        {
            var cmds = new List<PromptCommand>();
            var folders = promptClasses.Where(c => string.Equals(c.Folder, folder, StringComparison.OrdinalIgnoreCase)).ToList();
            folders.ForEach(f=>cmds.AddRange(commandList.Where(c => (c.ClassType == f.TypeId || c.ClassType == null) && !c.Hide)));

            var str = new StringBuilder();
            var folderName = string.IsNullOrEmpty(folder) ? "Root" : folder;
            var helps = folders.Where(f => !string.IsNullOrWhiteSpace(f.Help)).Select(f=>f.Help).Distinct().ToList();
            var hlp = string.Join(Environment.NewLine, helps);
            str.AppendLine($"{folderName}: {hlp}").AppendLine(LineBreak);

            cmds.Sort((x, y) => string.Compare(x.CommandText, y.CommandText, StringComparison.Ordinal));
            cmds.ForEach(c=> str.Append(PrintCommandSummaryHelp(c)));
            return str;
        }

        #endregion

        #region ApplicationH Description
        private static StringBuilder PrintApplicationHelp(string applicationHelp, List<PromptClass> promptClasses, List<PromptCommand> commandList)
        {
            if (string.IsNullOrEmpty(applicationHelp)) return null;

            var folders = promptClasses;
            folders.Sort((x, y) => string.Compare(x.Folder, y.Folder, StringComparison.Ordinal));
            var groupFolders = folders.GroupBy(g => g.Folder);

            var str = new StringBuilder();
            str.AppendLine(LineBreak).AppendLine(applicationHelp).AppendLine(LineBreak);

            foreach (IGrouping<string, PromptClass> grouping in groupFolders)
            {
                str.AppendLine().Append(PrintFolderHelp(grouping.Key, promptClasses, commandList));
            }
            return str;
        }
        #endregion
    }
}

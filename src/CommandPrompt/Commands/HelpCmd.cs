using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandPrompt.Internal;

namespace CommandPrompt.Commands
{
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

            var str = new StringBuilder();
            // Does the given 'command' match any commands
            if (prompt.CommandList.Any(c => string.Equals(c.CommandText, command.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                str.Append(PrintCommandDetailHelp(command));
            }
            // Does the given 'command' match any Folder names
            else if (prompt.CommandClass.Any(c => string.Equals(c.Folder, command.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                str.Append(PrintFolderHelp(command));
            }
            // Other wise Print the Application help 
            else
            {
                str.Append(PrintApplicationHelp());
            }

            Console.WriteLine(str.ToString());
        }

        #region Command Help

        private StringBuilder PrintCommandDetailHelp(string command)
        {
            var cmdList = prompt.CommandList.Where(c => string.Equals(c.CommandText, command.Trim(), StringComparison.OrdinalIgnoreCase)).ToList();

            var str = new StringBuilder();
            foreach (var promptCommand in cmdList)
            {
                str.Append(PrintCommandHelpDetails(promptCommand, 0));
            }
            return str;
        }

        private StringBuilder PrintCommandSummaryHelp(PromptCommand cmd, int indent = Indent)
        {
            return new StringBuilder().AppendLine($"{string.Empty.PadRight(indent)}{cmd.CommandText.PadRight(PadCommand)} {cmd.HelpText,-30}");
        }

        private StringBuilder PrintCommandHelpDetails(PromptCommand cmd, int indent = Indent)
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

        #region Folder Help

        private StringBuilder PrintFolderHelp(string folder)
        {
            var cmds = new List<PromptCommand>();
            var folders = prompt.CommandClass.Where(c => string.Equals(c.Folder, folder, StringComparison.OrdinalIgnoreCase)).ToList();
            folders.ForEach(f=>cmds.AddRange(prompt.CommandList.Where(c => (c.ClassType == f.TypeId || c.ClassType == null) && !c.Hide)));

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

        #region ApplicationH Help
        private StringBuilder PrintApplicationHelp()
        {
            var appHelp = prompt.Configuration.GetOption("ApplicationHelp");
            if (string.IsNullOrEmpty(appHelp)) return null;

            var folders = prompt.CommandClass;
            folders.Sort((x, y) => string.Compare(x.Folder, y.Folder, StringComparison.Ordinal));
            var groupFolders = folders.GroupBy(g => g.Folder);

            var str = new StringBuilder();
            str.AppendLine(LineBreak).AppendLine(appHelp).AppendLine(LineBreak);

            foreach (IGrouping<string, PromptClass> grouping in groupFolders)
            {
                str.AppendLine().Append(PrintFolderHelp(grouping.Key));
            }
            return str;
        }
        #endregion
    }
}

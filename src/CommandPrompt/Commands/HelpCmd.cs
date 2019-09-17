using System;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CommandPrompt.Commands
{
    [PromptClass ("Hidden")]
    internal class HelpCmd
    {
        private readonly Prompt prompt;
        private const string LineBreak = "--------------------------";

        public HelpCmd(Prompt prompt)
        {
            this.prompt = prompt;
        }

        [Prompt("help", Help = "Shows help", Hide = true)]
        public void Help(string cmdOrFolder)
        {
            var appHelp = prompt.Configuration.GetOption("ApplicationHelp");
            if (!string.IsNullOrEmpty(appHelp))
            {
                Console.WriteLine(appHelp);
                Console.WriteLine(LineBreak);
            }
            var isFolder = prompt.CommandList.Any(c => c.Folder == cmdOrFolder);
            if (isFolder)
            {
                var folderHelp = prompt.CommandClass.Where(c => c.Folder == cmdOrFolder).ToList();
                foreach (var classFolder in folderHelp)
                {
                    Console.WriteLine($"{classFolder.Folder}: {classFolder.Help}");
                    Console.WriteLine(LineBreak);


                }
                Console.WriteLine(appHelp);
                Console.WriteLine(LineBreak);
            }

            //   Console.WriteLine($"help {cmd,-30} ");
            Console.WriteLine($"Help Help");
            // Show all the help 
            var commandList = prompt.CommandList.Where(c=> c.Folder != "Hidden");

            //
      //      var isfolder = prompt.CommandList.Where(c => c.Folder == cmdOrFolder);
            var isCmd = prompt.CommandList.Where(c => c.CommandText == cmdOrFolder);



            // 1. Print Application Help First

            // ----------------------
            // 2. Then Print Folder Help, 
            // ----------------------

            foreach (var command in commandList)
            {
                var str = new StringBuilder();
                str.Append(command.CommandText);
                foreach (ParameterInfo info in command.MethodInfo.GetParameters())
                {
                    str.Append($" {info.Name}");

                }
                Console.WriteLine($"{str,-30} {command.HelpText}");
            }
        }

        //[Prompt("help", Help = "Shows help", Hide = true)]
        //public void Help()
        //{
        //    Console.WriteLine($"Help Help");
        //    // Show all the help 
        //    var commandList = prompt.CommandList;

        //    foreach (var command in commandList)
        //    {
        //        var str = new StringBuilder();
        //        str.Append(command.CommandText);
        //        foreach (ParameterInfo info in command.MethodInfo.GetParameters())
        //        {
        //            str.Append($" {info.Name}");

        //        }
        //        Console.WriteLine($"{str,-30} {command.Help}");
        //    }

        //    // Limit to current Folder

        //}

    }
}

using System;
using System.Reflection;
using System.Text;

namespace CommandPrompt.Commands
{
    [PromptClass()]
    internal class HelpCmd
    {
        private readonly Prompt prompt;

        public HelpCmd(Prompt prompt)
        {
            this.prompt = prompt;
        }

        [Prompt("help", HelpText = "Shows help", Hide = true)]
        public void Help(string cmd)
        {
            Console.WriteLine($"help {cmd,-30} ");
            Console.WriteLine($"Help Help");
            // Show all the help 
            var commandList = prompt.CommandList;

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

        //[Prompt("help", HelpText = "Shows help", Hide = true)]
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
        //        Console.WriteLine($"{str,-30} {command.HelpText}");
        //    }

        //    // Limit to current Folder

        //}

    }
}

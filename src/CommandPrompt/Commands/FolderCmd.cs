using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandPrompt.Commands
{
    internal class FolderCmd
    {
        private readonly Prompt prompt;

        public FolderCmd(Prompt prompt)
        {
            this.prompt = prompt;
        }

        readonly List<string> rootFolderCmd = new List<string> { ".", "..", "/", "\\" };

        [Prompt("cd", Help = "Change folder", HelpDetail = "A folder is use to limit the help commands\n dots or slashes can be used to go back to the root folder\nMore that one class can have the same folder and thus can be used to make a super set of commands")]
        public void ChangeFolder(string folder)
        {
            if (rootFolderCmd.Contains(folder))
            {
                prompt.CurrentFolder = string.Empty;
                return;
            }

            // use dos or unix format?
            var isFolderValid = prompt.CommandClass.FirstOrDefault(c => string.Equals(c.Folder, folder, StringComparison.OrdinalIgnoreCase));
            if (isFolderValid != null)
            {
                prompt.CurrentFolder = isFolderValid.Folder;
            }
            else
            {
                Console.WriteLine($"Unknown folder:{folder}");
            }
        }

        [Prompt("ls Folders", Help = "List All Folders")]
        [Prompt("List Folders", Help = "List All Folders")]
        public void ListFolders()
        {
            var list = prompt.CommandClass.GroupBy(g => g.Folder).Where(grouping => grouping.Key != null).ToList();

            Console.WriteLine($"Folders");
            foreach (var grouping in list)
            {
                Console.WriteLine($"  {grouping.Key}");
            }
        }
    }
}

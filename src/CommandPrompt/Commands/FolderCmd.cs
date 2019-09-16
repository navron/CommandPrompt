using System;
using System.Linq;

namespace CommandPrompt.Commands
{

    [PromptClass()]
    internal class FolderCmd
    {
        private readonly Prompt prompt;

        public FolderCmd(Prompt prompt)
        {
            this.prompt = prompt;
        }

        [Prompt("cd", HelpText = "Change folder")]
        public void ChangeFolder(string folder)
        {
            // use dos or unix format?
            var isFolderValid = prompt.CommandClass.Any(c => string.Equals(c.Folder, folder, StringComparison.OrdinalIgnoreCase));
            if (isFolderValid)
            {
                prompt.CurrentFolder = folder.ToUpperInvariant();

            }

            // .. back one
            // / route folder
        }
    }
}

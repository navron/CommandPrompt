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

            // .. back one
            // / route folder
        }
    }
}

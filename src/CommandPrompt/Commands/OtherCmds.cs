namespace CommandPrompt.Commands
{
    [PromptClass()]
    internal class OtherCmds
    {
        private readonly Prompt prompt;

        public OtherCmds(Prompt prompt)
        {
            this.prompt = prompt;
        }


        [Prompt("rescan", Help = "Re-scan the assemblies for prompt configurations", Hide = true)]
        public void Rescan(string folder)
        {

        }
    }
}

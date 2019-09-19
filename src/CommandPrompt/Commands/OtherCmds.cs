using CommandPrompt.Internal;

namespace CommandPrompt.Commands
{
    internal class OtherCmds
    {
        private readonly Prompt prompt;

        public OtherCmds(Prompt prompt)
        {
            this.prompt = prompt;
        }


        [Prompt("rescan", Help = "Re-scan the assemblies for prompt configurations", Hide = true)]
        public void Rescan()
        {
            BuildCommands.ScanForPrompt(prompt);
        }
    }
}

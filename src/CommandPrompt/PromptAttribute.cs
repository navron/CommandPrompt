using System;

namespace CommandPrompt
{
    public class PromptAttribute : Attribute
    {
        public PromptAttribute(string cmd)
        {
            Command = cmd;
        }
        public string Command { get; set; }

        public string HelpText { get; set; }
    }
}

using System;

namespace CommandPrompt.Internal
{
    internal class PromptClass
    {
        public string Folder { get; set; }
        public bool KeepClassInstance { get; set; }
        public Type TypeId { get; set; }
        public string Help { get; set; }
    }
}
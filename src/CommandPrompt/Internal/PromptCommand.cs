using System;
using System.Reflection;

namespace CommandPrompt.Internal
{
    internal class PromptCommand
    {
        public Type PromptMethodType;
        public object[] test;
        public string HelpText { get; set; }
        public string Command { get; set; }
        public object PromptAttributeTypeId { get; set; }
        public string MethodInfo { get; set; }
        public Type ClassType { get; set; }

        // Method Parameter Info
        public ParameterInfo[] Parameters { get; set; }

        // If the Method has parameters then the 'StartWith' Flag will be true
        public bool StartWith { get; set; }
        public string CommandLower { get; set; }

        // Keep the Instance of the Class between prompt commands
        public bool ClassKeep { get; set; }
        public string Folder { get; set; }
    }
}

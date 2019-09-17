using System;

namespace CommandPrompt
{
    /// <summary>
    ///  CommandText prompt attribute for an method 
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class PromptAttribute : Attribute
    {
        /// <summary>
        /// The CommandText prompt method attribute, for describing the
        /// </summary>
        /// <param name="command">The command that</param>
        public PromptAttribute(string command)
        {
            Command = command;
        }

        /// <summary>
        /// The first word of the text entered on the command prompt. 
        /// </summary>
        /// <remarks>The command is the </remarks>
        public string Command { get; } // Set via constructors, required property

        /// <summary>
        /// Help Topic for this command
        /// </summary>
        public string Help { get; set; }

        /// <summary>
        /// Detail help of the command, may be multi-line
        /// </summary>
        public string HelpDetail { get; set; }

        /// <summary>
        /// Hide the CommandText from the Help, but allow it to be executed
        /// </summary>
        public bool Hide { get; set; }
    }
}

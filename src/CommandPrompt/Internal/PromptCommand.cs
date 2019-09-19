using System;
using System.Reflection;

namespace CommandPrompt.Internal
{
    internal class PromptCommand
    {
        /// <summary>
        /// The CommandText Text
        /// </summary>
        public string CommandText { get; set; }

        /// <summary>
        /// If the Method has parameters then the 'StartWith' Flag will be true
        /// </summary>
        public bool StartWith { get; set; }

        /// <summary>
        /// Help Text for this command
        /// </summary>
        public string HelpText { get; set; }    
        
        /// <summary>
        /// The Method of this command
        /// </summary>
        public MethodInfo MethodInfo { get; set; }

        /// <summary>
        /// The Class where this method is located
        /// </summary>
        public Type ClassType { get; set; }

        /// <summary>
        /// If the Class instance is to be kept between prompt commands, then set this flag
        /// </summary>
        /// <remarks>From Prompt Class Attribute</remarks>
        public bool KeepClassInstance { get; set; }

        /// <summary>
        ///  The folder of that this command belongs to
        /// </summary>
        /// <remarks>From Prompt Class Attribute</remarks>
        public string Folder { get; set; }

        /// <summary>
        /// Hide this command from the help and list systems
        /// </summary>
        public bool Hide { get; set; }
    }
}

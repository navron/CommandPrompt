using System;

namespace CommandPrompt
{
    /// <summary>
    /// CommandText prompt attribute for an class 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class PromptClassAttribute : Attribute
    {
        /// <summary>
        /// CommandText Prompt Class Attribute
        /// </summary>
        /// <param name="folder">The folder that can group prompt commands</param>
        /// <remarks>folder=null is valid for a command that is in the root folder</remarks>
        public PromptClassAttribute(string folder = null) 
        {
            Folder = folder;
        }

        /// <summary>
        /// The folder that can be used to group prompt commands
        /// </summary>
        /// <remarks>Multiple class may share the same folder</remarks>
        public string Folder { get; } // Set via constructors, required property

        /// <summary>
        /// Keep the class instance between command usages
        /// </summary>
        /// <remarks>Stored in the configuration class, so may be instated before usage</remarks>
        public bool Keep { get; set; } = false;

        /// <summary>
        /// Description summary for commands within this class
        /// </summary>
        public string Description { get; set; }
    }
}
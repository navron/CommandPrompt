using System;

namespace CommandPrompt
{
    public class PromptClassAttribute : Attribute
    {
        /// <summary>
        /// Command Prompt Class Attribute
        /// </summary>
        /// <param name="folder">The folder that can group prompt commands</param>
        public PromptClassAttribute(string folder = null)
        {
            Folder = folder;
        }

        /// <summary>
        /// The folder that can group prompt commands
        /// </summary>
        public string Folder { get; set; }

        /// <summary>
        /// Keep the Class Instance between command usages, Only create once on first use if not in the Prompt Configuration 
        /// </summary>
        public bool Keep { get; set; }


        /// <summary>
        /// Help Summary for commands within this class group
        /// </summary>
        public string Help { get; set; }

    }
}
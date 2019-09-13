﻿
using System;
using System.Linq;
using System.Reflection;

namespace CommandPrompt
{
    /// <summary>
    /// Configuration for the Prompt
    /// </summary>
    /// <remarks>May be overridden to for to provide custom actions</remarks>
    public class PromptConfiguration : IPromptConfiguration
    {
        #region Prompt Options

        public string PromptPostFix { get; set; } = ">";
        public string PromptPreFix { get; set; } = string.Empty;

        public string HistoryFile { get; set; } = string.Empty;

        public string HistoryEnabled { get; set; } = "True";
        
        #endregion

        public string CurrentFolder { get; set; } = string.Empty;

        #region IPromptConfiguration

        /// <summary>
        /// Gets an 
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        public string GetOption(string option)
        {
            switch (option)
            {
                case "Prompt": return $"{PromptPreFix}{CurrentFolder}{PromptPostFix}";
                case "HistoryFile": return HistoryFile;
                case "HistoryEnabled": return HistoryEnabled;
            }

            return string.Empty;
        }

        /// <summary>
        /// Returns a array of Assemblies to scan for
        /// </summary>
        /// <remarks>Don't scan Assemblies that begin with 'System' these will not contain Prompt CommandText attributes </remarks>
        public Assembly[] GetScanForAssemblies => Assemblies ?? AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.FullName.StartsWith("System")).ToArray();

        /// <summary>
        /// Gets an Object that may be passed to an CommandText Prompt method or class
        /// </summary>
        public object GetObjectOfType(Type type)
        {
            var ob = new object();
            // First Scan the Objects List for any matching type and return it
            if (type.IsInterface)
            {
                ob = Objects.FirstOrDefault(type.IsInstanceOfType);
            }
            // Scan the Objects List for an concrete matching type and return it
            return ob == null ? Objects.FirstOrDefault(ob2 => ob2.GetType() == type) : null;
        }
        #endregion

        /// <summary>
        /// Specify the Assemblies that should be scanned for Prompt Commands
        /// </summary>
        public Assembly[] Assemblies { get; set; }

        /// <summary>
        /// An array of initialise objects to be passed to either an CommandText Prompt method or class
        /// </summary>
        public object[] Objects { get; set; }
    }
}


using System;
using System.Collections.Generic;
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

        /// <summary>
        /// If configured the prompt history will be saved to given file name on save and loaded at start up
        /// </summary>
        public string HistoryFile { get; set; } = string.Empty;

        /// <summary>
        /// Enable using history on the prompt, i.e. the up and down arrows
        /// </summary>
        public bool HistoryEnabled { get; set; } = true;

        /// <summary>
        /// Sets the Application Help
        /// </summary>
        public string ApplicationHelp { get; set; }

        #endregion

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
                case "PromptPostFix": return PromptPostFix;
                case "PromptPreFix": return PromptPreFix;
                case "HistoryFile": return HistoryFile;
                case "HistoryEnabled": return HistoryEnabled.ToString();
                case "ApplicationHelp": return ApplicationHelp;
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
            // First Scan the Objects List for any matching type and return it
            if (type.IsInterface)
                return Objects.FirstOrDefault(type.IsInstanceOfType) ??
                       Objects.FirstOrDefault(ob => ob.GetType() == type);

            // Scan the Objects List for an concrete matching type and return it
            return Objects.FirstOrDefault(ob => ob.GetType() == type);
        }

        public object ParameterConvert(string text, Type parameterType)
        {
            if (parameterType == typeof(string))
            {
                return text;
            }
            if (parameterType == typeof(int) && int.TryParse(text, out var resultInt))
            {
                return resultInt;
            }
            if (parameterType == typeof(ulong) && ulong.TryParse(text, out var resultULong))
            {
                return resultULong;
            }
            if (parameterType == typeof(int[]))
            {
                var listString = text.Split(new []{' '}, StringSplitOptions.None);
                var listInt = new List<int>();
                foreach (var s in listString)
                {
                    int.TryParse(s, out var resultIntA);
                    listInt.Add(resultIntA);
                }
                return listInt.ToArray();
            }
            if (parameterType == typeof(string[]))
            {
                var listString = text.Split(new[] { ' ' }, StringSplitOptions.None);
                var listInt = new List<int>();
                foreach (var s in listString)
                {
                    int.TryParse(s, out var resultIntA);
                    listInt.Add(resultIntA);
                }
                return listInt.ToArray();
            }

            // Parameter Type not coded for, either subclass this Configuration class or ask for type to be handled
            return null;
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

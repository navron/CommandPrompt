using System;
using System.IO;
using System.Linq;

namespace CommandPrompt.Internal
{
    /// <summary>
    /// Manages the loading and saving of an command history file 
    /// </summary>
    internal static class HistoryFile
    {
        /// <summary>
        /// Load Command Text History
        /// </summary>
        public static void Load(IPromptConfiguration config)
        {
            var historyFile = config.GetOption("HistoryFile");
            if (string.IsNullOrEmpty(historyFile)) return;


            ReadLine.HistoryEnabled = true;
            if (!File.Exists(historyFile)) return;
            var history = File.ReadAllLines(historyFile);
            ReadLine.AddHistory(history);
        }

        /// <summary>
        /// Saves Command Text History
        /// </summary>
        public static void Save(IPromptConfiguration config)
        {
            var historyFile = config.GetOption("HistoryFile");
            if (string.IsNullOrEmpty(historyFile)) return;

            var history = ReadLine.GetHistory();
            history = history.Distinct().ToList(); // Remove duplicates
            File.WriteAllLines(historyFile, history);
        }
    }
}

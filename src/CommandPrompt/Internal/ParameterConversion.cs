using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandPrompt.Internal
{
    /// <summary>
    /// Parameter Conversion 
    /// </summary>
    internal static class ParameterConversion
    {
        // Converts the Text (with the command removed) to the parameter type
        public static bool Convert(string text, Type parameterType, out object result)
        {
            result = null;
            if (parameterType == typeof(string))
            {
                result = text;
            }
            if (parameterType == typeof(int) && int.TryParse(text, out var resultInt))
            {
                result = resultInt;
            }
            if (parameterType == typeof(ulong) && ulong.TryParse(text, out var resultULong))
            {
                result = resultULong;
            }
            if (parameterType == typeof(int[]))
            {
                var listString = SplitCommand(text);
                var listInt = new List<int>();
                foreach (var s in listString)
                {
                    int.TryParse(s, out var resultIntA);
                    listInt.Add(resultIntA);
                }
                result = listInt.ToArray();
            }
            if (parameterType == typeof(string[]))
            {
                var listString = SplitCommand(text);
                var listInt = new List<int>();
                foreach (var s in listString)
                {
                    int.TryParse(s, out var resultIntA);
                    listInt.Add(resultIntA);
                }
                result = listInt.ToArray();
            }

            return result != null;
        }

        // Split the command into different parts, 
        internal static List<string> SplitCommand(string text)  //TODO Write Unit Tests
        {
            var result = text.Split('"')
                .Select((element, index) => index % 2 == 0  // If even index
                    ? element.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)  // Split the item
                    : new string[] { element })  // Keep the entire item
                .SelectMany(element => element).ToList();
            return result;
        }
    }
}

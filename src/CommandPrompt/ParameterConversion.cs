using System;
using System.Collections.Generic;

namespace CommandPrompt
{
    public static class ParameterConversion
    {
        public static object Convert(string text, Type parameterType)
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
                var listString = text.Split(new[] { ' ' }, StringSplitOptions.None);
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
    }
}

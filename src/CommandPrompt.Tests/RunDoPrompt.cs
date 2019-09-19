using System;

namespace CommandPrompt.Tests
{
    public class CanDoPrompt
    {
        // Static value to hold what Method was Run 
        public static string MethodRun;
        public static string MethodValue;

        [Prompt("CanDo", Help = "I Can Do it")]
        public void CanDo()
        {
            Console.WriteLine("Running Method 'CanDo'");
            MethodRun = "CanDo";
            MethodValue = string.Empty;
        }

        [Prompt("DoString", Help = "Parse an String")]
        public void DoString(string pram)
        {
            Console.WriteLine($"Running Method 'DoString' with and parameter:{pram}");
            MethodRun = "DoString";
            MethodValue = pram;
        }

        [Prompt("DoInt", Help = "Parse an integer")]
        public void DoInt(int pram)
        {
            Console.WriteLine($"Running Method 'DoInt' with and parameter:{pram}");
            MethodRun = "DoInt";
            MethodValue = $"{pram}";
        }

        [Prompt("DoULong", Help = "Parse an ULong")]
        public void DoULong(ulong pram)
        {
            Console.WriteLine($"Running Method 'DoULong' with and parameter:{pram}");
            MethodRun = "DoULong";
            MethodValue = $"{pram}";
        }

        [Prompt("DoStringArray", Help = "Parse an String Array")]
        public void DoStringArray(string[] pram)
        {
            Console.WriteLine($"Running Method 'DoStringArray' with and parameter:{pram}");
            MethodRun = "DoStringArray";
            MethodValue = $"{string.Join(" ", pram)}";
        }

        [Prompt("DoIntArray", Help = "Parse an Integer Array")]
        public void DoIntArray(int[] pram)
        {
            Console.WriteLine($"Running Method 'DoIntArray' with and parameter:{pram}");
            MethodRun = "DoIntArray";
            MethodValue = $"{string.Join(" ", pram)}";
        }

        [Prompt("DoStringInt", Help = "Parse an String then an Integer Array")]
        public void DoStringInt(string myString, int myInt)
        {
            Console.WriteLine($"Running Method 'DoStringInt' with and myString:{myString}, myInt:{myInt}");
            MethodRun = "DoStringInt";
            MethodValue = $"{myString} {myInt}";
        }

        [Prompt("DoIntString", Help = "Parse an String then an Integer Array")]
        public void DoIntString(int myInt, string myString)
        {
            Console.WriteLine($"Running Method 'DoIntString' with and myInt:{myInt} myString:{myString}");
            MethodRun = "DoIntString";
            MethodValue = $"{myInt} {myString}";
        }
    }
}

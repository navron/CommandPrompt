using System;
using System.Linq;
using FluentAssertions;
using Xunit;
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

namespace CommandPrompt.Tests
{
    public class CanDoPromptTests
    {
        // Static value to hold what Method was Run 
        private static string methodRun;
        private static string methodValue;

        [Prompt("CanDo", HelpText = "I Can Do it")]
        public void CanDo()
        {
            Console.WriteLine("Running Method 'CanDo'");
            methodRun = "CanDo";
            methodValue = string.Empty;
        }

        [Theory]
        [InlineData("CanDo", "")]
        [InlineData("DoString","Test String")]
        [InlineData("DoInt", "123")]
        [InlineData("DoULong", "123123")]
        [InlineData("DoIntArray", "123 321")]
        [InlineData("DoStringInt", "NumberIs 321")]
        public void CanDoTest(string promptCmd, string pramText)
        {
            methodRun = methodValue = "Before Test";
            var config = new PromptConfiguration(); // Scan this file only
            var prompt = new Prompt(config);

            var cmd = $"{promptCmd} {pramText}".Trim();
            prompt.RunCommand(cmd);
            methodRun.Should().Be(promptCmd);
            methodValue.Should().Be(pramText);
        }

        [Prompt("DoString", HelpText = "Parse an String")]
        public void DoString(string pram)
        {
            Console.WriteLine($"Running Method 'DoString' with and parameter:{pram}");
            methodRun = "DoString";
            methodValue = pram;
        }

        [Prompt("DoInt", HelpText = "Parse an integer")]
        public void DoInt(int pram)
        {
            Console.WriteLine($"Running Method 'DoInt' with and parameter:{pram}");
            methodRun = "DoInt";
            methodValue = $"{pram}";
        }

        [Prompt("DoULong", HelpText = "Parse an ULong")]
        public void DoULong(ulong pram)
        {
            Console.WriteLine($"Running Method 'DoULong' with and parameter:{pram}");
            methodRun = "DoULong";
            methodValue = $"{pram}";
        }

        [Prompt("DoStringArray", HelpText = "Parse an String Array")]
        public void DoStringArray(string[] pram)
        {
            Console.WriteLine($"Running Method 'DoStringArray' with and parameter:{pram}");
            methodRun = "DoStringArray";
            methodValue = $"{string.Join(" ",pram)}";
        }

        [Prompt("DoIntArray", HelpText = "Parse an Integer Array")]
        public void DoIntArray(int[] pram)
        {
            Console.WriteLine($"Running Method 'DoIntArray' with and parameter:{pram}");
            methodRun = "DoIntArray";
            methodValue = $"{string.Join(" ", pram)}";
        }

        [Prompt("DoStringInt", HelpText = "Parse an String then an Integer Array")]
        public void DoStringInt(string myString, int myInt)
        {
            Console.WriteLine($"Running Method 'DoStringInt' with and myString:{myString}, myInt:{myInt}");
            methodRun = "DoStringInt";
            methodValue = $"{myString} {myInt}";
        }

        // Use this to spilt string with '


        //string myString = "WordOne \"Word Two\"";
        //var result = myString.Split(''')
        //    .Select((element, index) => index % 2 == 0  // If even index
        //        ? element.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)  // Split the item
        //        : new string[] { element })  // Keep the entire item
        //    .SelectMany(element => element).ToList();


    }
}

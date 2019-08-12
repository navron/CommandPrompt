using System;

namespace CommandPrompt.Tests.TestData
{
    [PromptClass]
    public class Class1Simple
    {

        [Prompt("SimpleCmd1", HelpText = "SimpleCmd's first command method ")]
        public void SimpleCmd1()
        {
            Console.WriteLine($"Running Method 'SimpleCmd1'");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CommandPrompt.Tests;

namespace CommandPrompt.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            PromptTests.IncludeThisAss();
            var tasks = new List<Task>();
            var configuration = new PromptConfiguration {ApplicationHelp = "Command Prompt Demo"};

            using (var tokenSource = new CancellationTokenSource())
            {
                tasks.Add(Prompt.RunAsync(configuration, tokenSource.Token));
  
                // Add other Tasks i.e. service task 

                // Wait for any of the task to exit, this would indicate that the application is shutting down
                Task.WaitAny(tasks.ToArray());
            }
            Console.WriteLine("Good Bye World!");
        }
    }
}

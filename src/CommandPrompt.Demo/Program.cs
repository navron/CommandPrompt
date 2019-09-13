using System;
using System.Collections.Generic;
using System.Threading.Tasks;
// ReSharper disable once RedundantUsingDirective
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
            var pConfig = new PromptConfiguration();

            using (var cancellationTokenSource = new System.Threading.CancellationTokenSource())
            {
                tasks.Add(Prompt.RunAsync(pConfig, cancellationTokenSource.Token));
  
                // Add other Tasks i.e. service task 

                // Wait for any of the task to exit, this would indicate that the application is shutting down
                Task.WaitAny(tasks.ToArray());
            }
            Console.WriteLine("Good Bye World!");
        }
    }
}

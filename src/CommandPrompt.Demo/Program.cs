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

                Task.WaitAny(tasks.ToArray());
            }
        }
    }
}

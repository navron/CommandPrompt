using System;

namespace CommandPrompt.Tests.RunTestData
{
    // This class is not kept between Commands 
    [PromptClass("Class2a", Description = "Class2a's Commands", Keep = false)]
    public class Class2AWithObjectClass
    {
        public Class2AWithObjectClass(Data2a dataObject)
        {
            DataObject = dataObject;
            dataObject.Name = "WithClass2";
            dataObject.ClassCount = ++ClassCount;
        }

        public Data2a DataObject { get; set; }

        public int ClassCount { get; set; } = 0;

        [Prompt("2aCmd1", Help = "Class2's first command method ")]
        public void Cmd1()
        {
            Console.WriteLine($"Running Method 'Class2a Cmd1'");
            DataObject.Name = "2aCmd1";
            DataObject.UsageCount++;
        }

        [Prompt("2aCmd2", Help = "Class2's Second command method ")]
        public void Cmd2()
        {
            Console.WriteLine($"Running Method 'Class2a Cmd2'");
            DataObject.Name = "2aCmd2";
            DataObject.UsageCount++;
        }
    }

    public class Data2a
    {
        public string Name { get; set; } = string.Empty;
        public int UsageCount { get; set; } = 0;
        public int ClassCount { get; set; }
    }
}

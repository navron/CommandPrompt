using System;
using System.Threading;


namespace CommandPrompt.Tests.TestData
{
    // This class is kept between Commands 
    [PromptClass(Folder = "Class2b", Help = "Class2b's Commands", Keep = true)]
    public class Class2BWithObjectClass
    {
        public Class2BWithObjectClass(Data2b dataObject)
        {
            DataObject = dataObject;
            dataObject.Name = "WithClass2";
            dataObject.ClassCount = ++ClassCount;
        }

        public int ClassCount { get; set; } = 0;

        public Data2b DataObject { get; set; }

        [Prompt("2bCmd1", HelpText = "Class2's first command method ")]
        public void Cmd1()
        {
            Console.WriteLine($"Running Method 'Class2b Cmd1'");
            DataObject.Name = "2bCmd1";
            DataObject.UsageCount++;
        }

        [Prompt("2bCmd2", HelpText = "Class2's Second command method ")]
        public void Cmd2()
        {
            Console.WriteLine($"Running Method 'Class2b Cmd2'");
            DataObject.Name = "2bCmd2";
            DataObject.UsageCount++;
        }
    }

    public class Data2b
    {
        public string Name { get; set; } = string.Empty;
        public int UsageCount { get; set; } = 0;
        public int ClassCount { get; set; }
    }
}

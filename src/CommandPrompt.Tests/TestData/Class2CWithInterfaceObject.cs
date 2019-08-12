using System;
using System.Data;

namespace CommandPrompt.Tests.TestData
{
    [PromptClass(Keep = true, Help = "Testing that a class can be constructor with an object from interface")]
    public class Class2CWithInterfaceObject
    {
        public Class2CWithInterfaceObject(IData2 data)
        {
            DataObject = data as Data2C;
        }

        public int ClassCount { get; set; } = 0;

        public Data2C DataObject { get; set; }

        [Prompt("2cCmd1", HelpText = "Class2c's first command method ")]
        public void Cmd1()
        {
            Console.WriteLine($"Running Method 'Class2b Cmd1'");
            DataObject.Name = "2cCmd1";
            DataObject.UsageCount++;
        }
    }

    public interface IData2
    {
        string Name { get; set; }
        int UsageCount { get; set; }
        int ClassCount { get; set; }
    }

    public class Data2C : IData2
    {
        public string Name { get; set; } = string.Empty;
        public int UsageCount { get; set; } = 0;
        public int ClassCount { get; set; }
    }
}

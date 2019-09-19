# Command Prompt
An Command Prompt simple configuration method for running REPL (Read, Eval, Print, and Loop) actions with an console application

This library has been design to make running commands in either development or production code for diagnostic reasons easy.

The library has been designed to use in asynchronous service program environment, where the prompt is run asynchronous to other task.

Sample code of an Entry point

```C#
    static void Main(string[] args)
    {
        Console.WriteLine("Hello World!");
        var tasks = new List<Task>();
        var configuration = new PromptConfiguration();
        using (var tokenSource = new CancellationTokenSource())
        {
            tasks.Add(Prompt.RunAsync(configuration, tokenSource.Token));
  
            // Add other Tasks i.e. service task 

            // Wait for any of the task to exit, 
            // this would indicate that the application is shutting down
            Task.WaitAny(tasks.ToArray());
        }
        Console.WriteLine("Good Bye World!");
    }
```


Sample Prompt Code, the Prompt Class Attribute is optional and only used to group prompt commands

```c#
[PromptClass("CanDo", Description = "Folder Description")]
public class CanDo
{
    [Prompt("CanDo", Help = "I Can Do it")]
    public void CanDo()
    {
        Console.WriteLine("Running Method 'CanDo'");
    }

    [Prompt("DoString", Help = "Parse an String")]
    public void DoString(string pram)
    {
        Console.WriteLine($"Running Method 'DoString' with and parameter:{pram}");
    }
    [Prompt("DoInt", Help = "Parse an integer")]
    public void DoInt(int pram)
    {
        Console.WriteLine($"Running Method 'DoInt' with and parameter:{pram}");
    }
}
```
Prompts can be configured to automatically convert the text input into any object for easy of use.

Prompt class instances can be passed into the constructor for access to establish objects

```c#
public class MyObjectCmds
{
    MyObject myObject;
    public ClassWithObject(MyObject myObject)
    {
        this.myObject = myObject;
    }
    [Prompt("MyAdd", Help = "Do something with MyObject")]
    public void MyAdd(int pram)
    {
        myObject.Add(parm); // Assume MyObject has a method to add a integer to it
    }
}
```
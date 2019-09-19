# Command Prompt
An Command Prompt simple configuration method for running REPL (Read, Eval, Print, and Loop) actions with an console application

All configuration of the prompt settings can be extended from the IPromptConfiguration Interface

This prompt has been designed to work as async await Task. 

- The code uses refection to build a internal list of commands.
- Folders can be used to limit the command list
- Assemblies can be given so that only a set list is scanned 

​	
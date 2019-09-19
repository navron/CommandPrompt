namespace CommandPrompt.Extension.MemoryDump
{
    class MemoryDumpCmd
    {
        [Prompt("CreateMemoryDump", Help = "Write a memory dump of the current process to disk")]
        public void CreateMemoryDump()
        {
            // Pseudocode because I don't actually know how to do this  

            // Get the Process Name and executable 

            // Launch this extension as a separate process, so that it can get a memory dump of (i think this is required)


        }
    }
}

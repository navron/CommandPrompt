namespace CommandPrompt.Tests.FolderTestData
{
    [PromptClass("Bill", Description = "Bills Folder")]
    public class BillFolder
    {
        [Prompt("DoBill")]
        public void DoBill()
        {
        }
    }
}

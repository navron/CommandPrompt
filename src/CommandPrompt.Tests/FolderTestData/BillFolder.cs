using System;
using System.Collections.Generic;
using System.Text;

namespace CommandPrompt.Tests.FolderTestData
{
    [PromptClass("Bill")]
    public class BillFolder
    {
        [Prompt("DoBill")]
        public void DoBill()
        {
        }
    }
}

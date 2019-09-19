using System;
using System.Collections.Generic;
using System.Text;

namespace CommandPrompt.Tests.FolderTestData
{
    [PromptClass("Fred")]
    public class FredFolder
    {
        [Prompt("DoFred")]
        public void DoFred()
        {
        }
    }
}

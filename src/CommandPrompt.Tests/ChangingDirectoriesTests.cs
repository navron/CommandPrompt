using Xunit;

namespace CommandPrompt.Tests
{
    public class ChangingDirectoriesTests
    {


        [Fact]
        public void ChangeToFredFolder()
        {
            // Setup Configuration 
            var config = new PromptConfiguration();
            config.PromptPreFix = ">";

            var p = new Prompt(config);

        }
    }
}

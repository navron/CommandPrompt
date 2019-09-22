using FluentAssertions;
using Xunit;

namespace CommandPrompt.Tests
{
    public class ChangingDirectoriesTests
    {
        [Fact]
        public void ChangeToFredFolder()
        {
            // Set up Configuration 
            var config = new PromptConfiguration(); // Scan this file only
            config.PromptPostFix = ">";
            var prompt = new Prompt(config);

            var cmd = $"cd bill";
            prompt.Run(cmd);
            prompt.CurrentFolder.Should().Be("Bill");

            cmd = $"cd fred";
            prompt.Run(cmd);
            prompt.CurrentFolder.Should().Be("Fred");

            cmd = $"cd ..";
            prompt.Run(cmd);
            prompt.CurrentFolder.Should().BeEmpty();
        }
    }
}

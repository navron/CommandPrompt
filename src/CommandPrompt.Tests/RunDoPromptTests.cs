using FluentAssertions;
using Xunit;
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

namespace CommandPrompt.Tests
{
    public class CanDoPromptTests
    {
        [Theory]
        [InlineData("CanDo", "")]
        [InlineData("DoString", "")]
        [InlineData("DoString", "Test")]
        [InlineData("DoString","Test String")]
        [InlineData("DoString", "Test String Number3")]
        [InlineData("DoInt", "123")]
        [InlineData("DoULong", "123123")]
        [InlineData("DoIntArray", "123 321")]
        [InlineData("DoStringInt", "NumberIs 321")]
        [InlineData("DoIntString", "4321 String")]
        [InlineData("DoIntString", "4321 String Test")]
        public void CanDoTest(string promptCmd, string pramText)
        {
            CanDoPrompt.MethodRun = CanDoPrompt.MethodValue = "Before Test";
            var config = new PromptConfiguration(); // Scan this file only
            var prompt = new Prompt(config);

            var cmd = $"{promptCmd} {pramText}".Trim();
            prompt.Run(cmd);
            CanDoPrompt.MethodRun.Should().Be(promptCmd);
            CanDoPrompt.MethodValue.Should().Be(pramText);
        }
    }
}

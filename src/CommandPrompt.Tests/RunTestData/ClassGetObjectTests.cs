using FluentAssertions;
using Xunit;

namespace CommandPrompt.Tests.RunTestData
{
    public class ClassGetObjectTests
    {
        [Fact]
        public void GetClassInstanceSingleObjectTest()
        {
            var config = new PromptConfiguration(); // Scan this file only
            var data2A = new Data2a();
            config.Objects = new object[] { data2A };
            var prompt = new Prompt(config);

            prompt.Run("2aCmd1");
            data2A.Name.Should().Be("2aCmd1");
        }

        [Fact]
        public void GetClassInstanceManyParametersObjectTest()
        {
            var config = new PromptConfiguration(); // Scan this file only
            var data2A = new Data2a();
            var data2B = new Data2b();
            config.Objects = new object[] { data2A, data2B };
            var prompt = new Prompt(config);

            prompt.Run("2aCmd1");
            data2A.Name.Should().Be("2aCmd1");
        }

        [Fact]
        public void GetClassInstanceManyParametersObjectOrderDifferentTest()
        {
            var config = new PromptConfiguration(); // Scan this file only
            var data2A = new Data2a();
            var data2B = new Data2b();
            config.Objects = new object[] { data2B, data2A };
            var prompt = new Prompt(config);

            prompt.Run("2aCmd1");
            data2A.Name.Should().Be("2aCmd1");
        }

        [Fact]
        public void GetClassInstanceKeptIsTest()
        {
            var config = new PromptConfiguration(); // Scan this file only
            var data2B = new Data2b();
            config.Objects = new object[] { data2B };
            var prompt = new Prompt(config);

            prompt.Run("2bCmd1");  // 2b New Class Instance between Commands
            data2B.Name.Should().Be("2bCmd1");
            data2B.ClassCount.Should().Be(1);
            data2B.UsageCount.Should().Be(1);

            prompt.Run("2bCmd1");
            data2B.ClassCount.Should().Be(1); // Keep Class Reused between Commands, that same count
            data2B.UsageCount.Should().Be(2);
        }

        [Fact]
        public void GetClassInstanceKeptNotTest()
        {
            var config = new PromptConfiguration(); // Scan this file only
            var data2A = new Data2a();
            config.Objects = new object[] { data2A };
            var prompt = new Prompt(config);

            prompt.Run("2aCmd1"); // 2a Keep Class between Commands
            data2A.Name.Should().Be("2aCmd1");
            data2A.ClassCount.Should().Be(1);

            prompt.Run("2aCmd1");
            data2A.ClassCount.Should().Be(1); 
            data2A.UsageCount.Should().Be(2);
        }

        [Fact]
        public void GetClassInstanceSingleObjectFromInterfaceTest()
        {
            var config = new PromptConfiguration(); // Scan this file only
            var data2C = new Data2C();
            config.Objects = new object[] { data2C };
            var prompt = new Prompt(config);

            prompt.Run("2cCmd1");
            data2C.Name.Should().Be("2cCmd1");
        }
    }
}

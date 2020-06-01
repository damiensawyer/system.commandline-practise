using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;
using Xunit;

namespace basiccommandline.test
{
    public class UnitTest1
    {
        [Fact]
        public async Task Test1()
        {
            var command = new RootCommand
            {
                new Option("--a-string") { Argument = new Argument<string>() },
                new Option("--an-int") { Argument = new Argument<int>() }
            };

            command.Handler = CommandHandler.Create(
                (string aString, int anInt) =>
                {
                    Console.WriteLine($"{aString}");
                    Console.WriteLine($"{anInt}");
                });

            var result = await command.InvokeAsync("--an-int 123 --a-string \"Hello world!\" ");

        }
    }
}

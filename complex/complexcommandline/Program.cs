using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
using System.CommandLine.Parsing;
using System.Threading.Tasks;

namespace complexcommandline
{
    public static class asdasd
    {
        	static public void AddFour(string label, int first, int second, int third, int fourth)
            {
                var s = $"{label}: {first + second + third + fourth}";
            }
    }

    class Program
    {
        static async Task Main()
        {


            var command = new RootCommand
            {
                new Option("--a-string") { Argument = new Argument<string>() },
                new Option("--an-int") { Argument = new Argument<int>() },
                new Option("--an-enum") { Argument = new Argument<System.IO.FileAttributes>() },
            };

            command.Handler = CommandHandler.Create(
                (ParseResult parseResult, IConsole console) =>
                {
                    console.Out.WriteLine($"{parseResult}");
                });

            await command.InvokeAsync("--an-int 123 --a-string \"Hello world!\" --an-enum compressed");

            Console.ReadLine();
        }
    }
}

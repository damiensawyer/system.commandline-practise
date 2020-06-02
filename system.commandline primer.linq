<Query Kind="Program">
  <NuGetReference Prerelease="true">System.CommandLine</NuGetReference>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.CommandLine</Namespace>
  <Namespace>System.CommandLine.Invocation</Namespace>
</Query>

async Task Main()
{
		await Demos.one_helloCommandLine();
		await Demos.two_complexType();
	
}


public static class Demos
{
	public static async Task one_helloCommandLine()
	{
		var command = new RootCommand
			{
				new Option("--a-string") { Argument = new Argument<string>() },
				new Option("--an-int") { Argument = new Argument<int>() }
			};

		command.Handler = CommandHandler.Create(
			(string aString, int anInt) =>
			{
				aString.Dump("a string");
				anInt.Dump("a number");
			});

		await command.InvokeAsync("--an-int 423 --a-string \"Hello world!\" ");
	}

	public static async Task two_complexType()
	{
		var command = new RootCommand
			{
				new Option("--a-string") { Argument = new Argument<string>() },
				new Option("--an-int") { Argument = new Argument<int>() }
			};

		 command.Handler = CommandHandler.Create(
        (ComplexType complexType) =>
		{
			$"string: {complexType.AString}, int: {complexType.AnInt}".Dump("complex type");
		});
		await command.InvokeAsync("--an-int 423 --a-string \"Hello world!\" ");
	}

}

public class ComplexType
{
	public int AnInt { get; set; }
	public string AString { get; set; }
}
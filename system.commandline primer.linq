<Query Kind="Program">
  <NuGetReference Prerelease="true">System.CommandLine</NuGetReference>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.CommandLine</Namespace>
  <Namespace>System.CommandLine.Invocation</Namespace>
</Query>

async Task Main()
{
	var currentFolder = $"{Path.GetDirectoryName(Util.CurrentQueryPath)}";
	currentFolder.Dump();
	
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
				new Option("--an-int") { Argument = new Argument<int>() },
				new Option("--an-enum") { Argument = new Argument<System.IO.FileAttributes>() },
				new Option("--a-bool") { Argument = new Argument<bool>() },
				new Option("--items") { Argument = new Argument<string[]>() },
				new Option(new []{"--an-int-with-Aliases","x","cat","aa"}) { Argument = new Argument<int>() }, // notice that cat didn't have --. You have to be deliberate about it.
			};

		command.Handler = CommandHandler.Create(
			(string aString, int anInt, System.IO.FileAttributes anEnum, bool aBool, string[] items, int anIntWithAliases ) =>
			{
				var itemString = items.Aggregate(new StringBuilder(), (a, b) => a.Append(", " + b));
				$"{aString}, {anInt}, {anEnum}, {aBool}, items: {itemString} aliased int: {anIntWithAliases}".Dump("One");

			});

		await command.InvokeAsync("--a-bool true --a-string \"Hello world!\" --an-enum compressed --items first second third --an-int 423 cat 99999");
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

	public static async Task three_fileSystemTypes()
	{
		var command = new RootCommand
			{
				new Option("--a-string") { Argument = new Argument<string>() },
				new Option("--an-int") { Argument = new Argument<int>() }
			};

		command.Handler = CommandHandler.Create(
	   (ComplexType complexType) =>
	   {
		   $"string: {complexType.AString}, int: {complexType.AnInt}".Dump("Two");
	   });
		await command.InvokeAsync("--an-int 423 --a-string \"Hello world!\" ");
	}

}

public class ComplexType
{
	public int AnInt { get; set; }
	public string AString { get; set; }
}
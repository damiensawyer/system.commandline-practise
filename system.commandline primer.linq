<Query Kind="Program">
  <NuGetReference Prerelease="true">System.CommandLine</NuGetReference>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.CommandLine</Namespace>
  <Namespace>System.CommandLine.Invocation</Namespace>
</Query>

async Task Main()
{
	// https://github.com/dotnet/command-line-api
	// https://docs.microsoft.com/en-us/archive/msdn-magazine/2019/march/net-parse-the-command-line-with-system-commandline
	
	await Demos.one_helloCommandLine();
	await Demos.two_complexType();
	await Demos.three_typesWithStringConstructor();
	
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
				$"{aString}, {anInt}, {anEnum}, {aBool}, items: {itemString} aliased int: {anIntWithAliases}".Dump("One: hello");

			});

		// await command.InvokeAsync("[debug] --a-bool true --a-string \"Hello world!\" --an-enum compressed --items first second third --an-int 423 cat 99999");  // if you run this one, you can attach a debugger to the console!! 
		// await command.InvokeAsync("[parse] --a-bool true --a-string \"Hello world!\" --an-enum compressed --items first second third --an-int 423 cat 99999");  // if you run this one, you can displays a preview of how tokens are parsed
		
		await command.InvokeAsync(" --a-bool true --a-string \"Hello world!\" --an-enum compressed --items first second third --an-int 423 cat 99999");
	}

	public static async Task two_complexType()
	{
		var command = new RootCommand
			{
				new Option(new []{"--a-string","a"}, "This is the string description") { Argument = new Argument<string>() }, // make a mistake on the parameter string you pass in and you see these descriptions et al.
				new Option(new []{"--an-int","b"}, "This is the int description") { Argument = new Argument<int>() }
			};

		 command.Handler = CommandHandler.Create(
        (ComplexType complexType) =>
		{
			$"string: {complexType.AString}, int: {complexType.AnInt}".Dump("Two (aliased): complex type");
		});
		await command.InvokeAsync("--an-int 423 --a-string \"Hello world!\" ");
		await command.InvokeAsync("-b 423 -a \"Hello world 2!\" ");
	}

	public static async Task three_typesWithStringConstructor()
	{
		var command = new RootCommand
			{
				 new Option("-d") { Argument = new Argument<DirectoryInfo>().ExistingOnly() },
				 	 new Option("--custom-object-message") { Argument = new Argument<MyCustomClass>() }
			};

		command.Handler = CommandHandler.Create(
	   (DirectoryInfo d, MyCustomClass customObjectMessage) =>
	   {
		   Directory.GetFiles(d.FullName).Dump("Three 1: files in directory");
		   customObjectMessage.Message.Dump("Three 2: string passed as constructor to custom object");
	   });

		var currentFolder = $"{Path.GetDirectoryName(Util.CurrentQueryPath)}";
		await command.InvokeAsync($"-d {currentFolder} --custom-object-message \"hello how are you today?\"");
		
		
		//await command.InvokeAsync(@"-d c://temp/");
	}

}

public class ComplexType
{
	public int AnInt { get; set; }
	public string AString { get; set; }
}

public class MyCustomClass
{ 
	public string Message { get; set; }
	
	public MyCustomClass(string message)
	{
		this.Message = message;
	}
}



/* 
Some extra notes:
- One of my favorite features is support for tab completion, which end users can opt into by running a command to activate it (see bit.ly/2sSRsQq). This is an opt-in scenario because users tend to be protective of implicit changes to the shell. Tab completion for options and command names happens automatically, but there’s also tab completion for arguments via suggestions. When configuring a command or option, the tab completion values can come from a static list of values, such as the q, m, n, d or diagnostic values of --verbosity. Or they can be dynamically provided at run time, such as from REST invocation that returns a list of available NuGet packages when the argument is a NuGet reference.


*/
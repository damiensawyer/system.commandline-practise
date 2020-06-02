<Query Kind="Program">
  <NuGetReference Prerelease="true">System.CommandLine</NuGetReference>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.CommandLine</Namespace>
  <Namespace>System.CommandLine.Invocation</Namespace>
  <Namespace>System.CommandLine.Binding</Namespace>
  <Namespace>System.CommandLine.Parsing</Namespace>
</Query>

async Task Main()
{
	// https://github.com/dotnet/command-line-api
	// https://docs.microsoft.com/en-us/archive/msdn-magazine/2019/march/net-parse-the-command-line-with-system-commandline
	
	//await Demos.one_helloCommandLine();
	//await Demos.two_complexType();
	//await Demos.three_typesWithStringConstructor();
	await Demos.four_moreComplex();
	await Demos.five_methodFirstApproach();
	
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
			(string aString, int anInt, System.IO.FileAttributes anEnum, bool aBool, string[] items, int anIntWithAliases) =>
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


	/// <summary>https://docs.microsoft.com/en-us/archive/msdn-magazine/2019/march/net-parse-the-command-line-with-system-commandline#making-the-complex-possible</summary>
	public static async Task four_moreComplex()
	{

		// todo. Add sub commands. 
		var rootCommand = new RootCommand() { Description = "Converts an image file from one format to another.", TreatUnmatchedTokensAsErrors = true};
		var nameOption = new Option(aliases: new string[] { "--name", "-n" }) { Description = "Your Name.", Argument = new Argument<string>() };
		rootCommand.AddOption(nameOption);

		var ageOption = new Option(aliases: new string[] { "--age", "-a" }) { Description = "Your Age", Argument = new Argument<int>() };
		rootCommand.AddOption(ageOption);

		rootCommand.Handler = CommandHandler.Create<string, int>(Convert);

		var r = await rootCommand.InvokeAsync("-n damien -a 46");
	}

	static public void Convert(string name, int age)
	{
		$"{name} {age}".Dump("Four: more complex");
	}


	public static async Task five_methodFirstApproach()
	{
		var rootCommand = new RootCommand() { Description = "Converts an image file from one format to another.", TreatUnmatchedTokensAsErrors = false};
		var method = typeof(Demos).GetMethod(nameof(AddFour));
		
 		//rootCommand.Handler =  HandlerDescriptor.FromMethodInfo(method).GetCommandHandler();
		rootCommand.ConfigureFromMethod(method);
		rootCommand.Aliases.Dump();
		//rootCommand.Arguments. ["--first"].
		//rootCommand.Children["--output"].AddAlias("-o");
		await rootCommand.InvokeAsync("--label damien --first 10 --second 20 --third 30 --fourth 40");
	}

	static public void AddFour(string label, int first, int second, int third, int fourth) => $"{label}: {first+second+third+fourth}".Dump("Five: Method First");
	

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

public static class Helpers
{
	internal static string BuildAlias(string parameterName)
	{
		if (String.IsNullOrWhiteSpace(parameterName))
		{
			throw new ArgumentException("Value cannot be null or whitespace.", nameof(parameterName));
		}

		return parameterName.Length > 1
				   ? $"--{parameterName.ToKebabCase()}"
				   : $"-{parameterName.ToLowerInvariant()}";
	}


	public static string BuildAlias(this IValueDescriptor descriptor)
	{
		if (descriptor == null)
		{
			throw new ArgumentNullException(nameof(descriptor));
		}

		return BuildAlias(descriptor.ValueName);
	}


	public static Option BuildOption(this ParameterDescriptor parameter)
	{
		var argument = new Argument
		{
			ArgumentType = parameter.ValueType
		};

		if (parameter.HasDefaultValue)
		{
			argument.SetDefaultValueFactory(parameter.GetDefaultValue);
		}

		var option = new Option(
			parameter.BuildAlias(),
			parameter.ValueName)
		{
			Argument = argument
		};

		return option;
	}


	private static readonly string[] _argumentParameterNames =
			{
			"arguments",
			"argument",
			"args"
		};

	public static IEnumerable<Option> BuildOptions(this MethodInfo method)
	{
		var descriptor = HandlerDescriptor.FromMethodInfo(method);

		var omittedTypes = new[]
						   {
								   typeof(IConsole),
								   typeof(InvocationContext),
								   typeof(BindingContext),
								   typeof(ParseResult),
								   typeof(CancellationToken),
							   };

		foreach (var option in descriptor.ParameterDescriptors
										 .Where(d => !omittedTypes.Contains(d.ValueType))
										 .Where(d => !_argumentParameterNames.Contains(d.ValueName))
										 .Select(p => p.BuildOption()))
		{
			yield return option;
		}
	}

	public static void ConfigureFromMethod(
		   this Command command,
		   MethodInfo method,
		   object target = null)
	{
		if (command == null)
		{
			throw new ArgumentNullException(nameof(command));
		}

		if (method == null)
		{
			throw new ArgumentNullException(nameof(method));
		}

		foreach (var option in method.BuildOptions())
		{
			command.AddOption(option);
		}

		if (method.GetParameters()
				  .FirstOrDefault(p => _argumentParameterNames.Contains(p.Name)) is ParameterInfo argsParam)
		{
			var argument = new Argument
			{
				ArgumentType = argsParam.ParameterType,
				Name = argsParam.Name
			};

			if (argsParam.HasDefaultValue)
			{
				if (argsParam.DefaultValue != null)
				{
					argument.SetDefaultValue(argsParam.DefaultValue);
				}
				else
				{
					argument.SetDefaultValueFactory(() => null);
				}
			}

			command.AddArgument(argument);
		}

		command.Handler = CommandHandler.Create(method, target);
	}

}



/* 
Some extra notes:
- One of my favorite features is support for tab completion, which end users can opt into by running a command to activate it (see bit.ly/2sSRsQq). This is an opt-in scenario because users tend to be protective of implicit changes to the shell. Tab completion for options and command names happens automatically, but there’s also tab completion for arguments via suggestions. When configuring a command or option, the tab completion values can come from a static list of values, such as the q, m, n, d or diagnostic values of --verbosity. Or they can be dynamically provided at run time, such as from REST invocation that returns a list of available NuGet packages when the argument is a NuGet reference.


*/
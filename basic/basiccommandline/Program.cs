using System;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.IO;
using System.Reflection;

namespace basiccommandline
{
    class Program
    {
        /// <summary>
        /// Run with:
        /// dotnet basic/basiccommandline/bin/Debug/netcoreapp3.1/basiccommandline.dll -h
        /// or
        /// dotnet basic/basiccommandline/bin/Debug/netcoreapp3.1/basiccommandline.dll --int-option 20
        /// </summary>
        /// <param name="intOption">This is an integer</param>
        /// <returns></returns>
        static int Main(int intOption)
        {
            Console.WriteLine($"option {intOption}");
            return 0;
        }
    }
}

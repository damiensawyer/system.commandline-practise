using System;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.IO;
using System.Reflection;

namespace basiccommandline
{
    class Program
    {
        /*
         To run this you need to reference System.CommandLine.DragonFruit from nuget. Without it, this app won't compile because there is no
         suitable main(). I'm not exactly sure what that dll is doing to intercept the startup. I downloaded the code from 
         https://github.com/dotnet/command-line-api/tree/master/src/System.CommandLine.DragonFruit but it wasn't obvious to me exactly 
         what was doing the interception. 

         */


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

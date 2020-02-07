using System;
using System.IO;
using System.Text.RegularExpressions;
using DumpFiles.Utils;

namespace DumpFiles
{
    using static DirectoryInfoUtils;

    public class Storage
    {

    }

    /// <summary>
    ///     Programmatically embed resources in csharp dll
    /// </summary>
    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("Begin process...");

            // TODO: Get content file path from the provided arguments.
            var currentDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());

            // Get project file
            var csprojRegex =new Regex("\\.csproj"); 
            var currentProjectFile =
                currentDirectory.GetFullnameForFileInHigherDirectory(csprojRegex, "*.csproj");

            // - OR - 
            // var currentProjectFile = currentDirectory.GetCurrentProjectFile();
            Console.WriteLine(currentProjectFile);

            // Read and enhance project file.
            // TODO
            // TODO
            // TODO

            // Run csharp compiler.
            var compiler = new Standard("C:\\Windows\\Microsoft.NET\\Framework\\v4.0.30319\\csc.exe");
            // compiler.RunAndWaitForExit("");

            Console.ReadLine();
        }
    }
}

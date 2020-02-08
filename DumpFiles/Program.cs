using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using DumpFiles.Utils;
using SQLite;
using Microsoft.Extensions.Configuration;

namespace DumpFiles
{
    using static DirectoryInfoUtils;

    public enum FileType
    {
        Text
    }

    public class FileDb
    {
        public string Name;
        private readonly SQLiteAsyncConnection DbConnection;

        public FileDb(string fileDbPathname)
        {
            this.Name = fileDbPathname;
            this.DbConnection = new SQLiteAsyncConnection(fileDbPathname);
            this.DbConnection.CreateTable<File>();
        }
        public static FileDb NewFile(string databasePath) => new FileDb(databasePath);

        public class File
        {
            [PrimaryKey]
            public string Filename { get; set; }
            public FileType FileType { get; set; }
            public Byte[] Content { get; set; }
        }

        public FileDb CollectFile(string filename, FileType type)
        {
            var _file = new File()
            {
                Filename = filename,
                FileType = type,

                // using filename and type to get content
                Content = System.IO.File.ReadAllBytes(filename)
            };

            this.DbConnection.Insert(_file);
            return this;
        }

        public void Print()
        {
            var _tblQuery = this.DbConnection.Table<File>();

            foreach (var entry in _tblQuery.ToList())
            {
                Console.WriteLine(entry.Filename);
                if (entry.FileType != FileType.Text)
                {
                    Console.WriteLine(Encoding.UTF8.GetString(entry.Content));
                    continue;
                }
            }
        }

        public void Build() => this.DbConnection.Close();
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

            // Read project file.
            var xml = new XmlDocument();
            xml.Load(currentProjectFile);

            var parsedXml = XDocument.Parse(xml.OuterXml);
            Console.WriteLine(parsedXml);

            // Enhance project file.
            // TODO: https://stackoverflow.com/questions/49781946/programmatically-embed-resource-in-net-assembly

            // <Project>
            //   <ItemGroup>
            //     <EmbeddedResource Include="Content\Item1.png" />
            //     <EmbeddedResource Include="Content\Item2.png" />
            //   </ItemGroup>
            // </Project>

            // Initial test to see how sqlite holds up with adding files
            var test = new Testing();
            test.CollectFilesFromProvidedDirectoryAndSaveAsEmbed();

            // Run csharp compiler.
            var compiler = new Standard("C:\\Windows\\Microsoft.NET\\Framework\\v4.0.30319\\csc.exe");
            // compiler.RunAndWaitForExit("");

            Console.ReadLine();
        }
    }
}

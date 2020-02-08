﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using DumpFiles.Utils;
using SQLite;

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
        public Env EnvironmentInstance;

        public FileDb(string fileDbPathname, string fullpath)
        {
            this.Name = fileDbPathname;
            this.DbConnection = new SQLiteAsyncConnection(fileDbPathname);
            this.DbConnection.CreateTable<File>();
            this.EnvironmentInstance = new Env(fullpath);
        }

        public static FileDb NewFile(string databasePath)
        {
            var fullpath = Path.Combine(Environment.CurrentDirectory, databasePath);
            return new FileDb(databasePath, fullpath);
        }
      
        public static FileDb NewFile(string databasePath, string parentDirectory)
        {
            var fullpath = Path.Combine(parentDirectory, databasePath);
            return new FileDb(databasePath, fullpath);
        }

        public class Env
        {
            public string Fullpath;
            public Env(string fullpath)
            {
                this.Fullpath = fullpath;
            }
        }

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

        public string Pathname => this.EnvironmentInstance.Fullpath;

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

            // Initial test to see how sqlite holds up with adding files
            var procedures = new Procedures();
            var embed = procedures.CollectFilesFromDirectoryProvidedBySecretsAndSaveAsEmbed();

            // Read project file.
            var xml = new XmlDocument();
            xml.Load(currentProjectFile);

            var parsedXml = new XDocument(XDocument.Parse(xml.OuterXml));

            var embedElement = procedures.AddEmbedItemGroup(parsedXml, embed.Pathname);
            parsedXml.Root.Add(embedElement);

            Console.WriteLine(parsedXml.Document);

            // TODO: Backup the current project file and overwrite with new project file.

            // Run csharp compiler.
            var compiler = new Standard("C:\\Windows\\Microsoft.NET\\Framework\\v4.0.30319\\csc.exe");
            // compiler.RunAndWaitForExit("");

            Console.ReadLine();
        }
    }
}

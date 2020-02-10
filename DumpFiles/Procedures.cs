using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using DumpFiles.Utils;
using Microsoft.Extensions.Configuration;

namespace DumpFiles
{

    public class Procedures
    {

        private IConfiguration Configuration { get; set; }

        public Procedures() : this(new ConfigurationBuilder()
                .AddUserSecrets("04201f84-8b4c-494d-8445-6942271ad77f")
                .Build())
        { }

        internal Procedures(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public FileDb CollectFilesFromProvidedDirectoryAndSaveAsEmbed(
            string directoryPath)
        {
            var embed = FileDb.NewFile("first.embed");

            var txtFiles = Directory.GetFiles(directoryPath, "*");

            foreach (var file_ in txtFiles)
            {
                embed.CollectFile(file_, FileType.Text);
            }

            embed.Build();

            return embed;
        }


        public FileDb CollectFilesFromDirectoryProvidedBySecretsAndSaveAsEmbed()
        {
            var dirName = this.Configuration["directoryName"];

            var specialDirPath =
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\" + dirName;

            var fileExists = File.Exists("first.embed");
            if (fileExists) { File.Delete("first.embed");  }

            var embed = FileDb.NewFile("first.embed");

            var txtFiles = Directory.GetFiles(specialDirPath, "*");

            foreach (var file_ in txtFiles)
            {
                embed.CollectFile(file_, FileType.Text);
            }
            embed.Build();

            return embed;
        }

        public XElement AddEmbedItemGroup(XDocument projectXml, string embedFilePath)
        {
            var itemGroup = new XElement(XName.Get("ItemGroup"));

            var embedResource = ProjectXmlUtils.EmbedResourceWrapper(embedFilePath);
            itemGroup.Add(embedResource);

            return itemGroup;
        }

        public void DuplicateAllFilesIncludingProjAndEmbedExcludingGitIgnored(
            string dirName,
            FileInfo newProjectFileInfo,
            FileInfo embedFileInfo)
        {
            var relevantFiles = GetFilesViaGit(dirName);

            // Create new directory and copy relevant files
            if (Directory.Exists("temp")) { Directory.Delete("temp", true); }
            var tempDir = Directory.CreateDirectory("temp");

            foreach (var _file in relevantFiles)
            {
                if (!_file.Name.Contains(".csproj") && !_file.Name.Contains("gitignore"))
                {
                    var newFileName = Path.Combine(tempDir.FullName, _file.Name);
                    _file.CopyTo(newFileName);
                }
            }

            var newProjectFileName =
                Path.Combine(tempDir.FullName, newProjectFileInfo.Name.Replace(".csproj.TOUPDATE", ".csproj"));

            newProjectFileInfo.CopyTo(newProjectFileName);

            var newEmbedFileName =
                Path.Combine(tempDir.FullName, embedFileInfo.Name);

            embedFileInfo.CopyTo(newEmbedFileName);
        }

        public List<FileInfo> GetFilesViaGit(string dirName)
        {
            var gitClient = new Standard("C:\\Users\\u1d246\\AppData\\Local\\Programs\\Git\\cmd\\git.exe");

            gitClient.Proc.StartInfo.WorkingDirectory = dirName;

            return gitClient
                .RunAndWaitForExit("ls-files")
                .TakeContent()
                .Split('\n')
                .Where(x => !string.IsNullOrEmpty(x))
                .Select(x => new FileInfo(dirName + "\\" + x))
                .ToList();
        }
    }

}

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;
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

            if (File.Exists("first.embed")) { File.Delete("first.embd");  }

            var embed = FileDb.NewFile("first.embed");

            var txtFiles = Directory.GetFiles(specialDirPath, "*");

            foreach (var file_ in txtFiles)
            {
                embed.CollectFile(file_, FileType.Text);
            }
            embed.Build();

            return embed;
        }

        public void AddEmbedItemGroup(XDocument projectXml, string embedFilePath)
        {

            // Enhance project file.
            // TODO: https://stackoverflow.com/questions/49781946/programmatically-embed-resource-in-net-assembly

            // <Project>
            //   <ItemGroup>
            //     <EmbeddedResource Include="Content\Item1.png" />
            //     <EmbeddedResource Include="Content\Item2.png" />
            //   </ItemGroup>
            // </Project>

        }
    }

}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DumpFiles.Utils
{
    public static class DirectoryInfoUtils
    {
        public static bool DirectoryContains(this DirectoryInfo directory, Regex searchRegex)
        {
            var dirFiles = directory
                .GetFiles("*", SearchOption.TopDirectoryOnly)
                .ToList()
                .Select(x => x.ToString());

            return dirFiles.Any(x => searchRegex.IsMatch(x));
        }

        public static string GetParentOfFileInHigherDirectory(this DirectoryInfo directory, Regex searchRegex)
        {
            var i = 0;
            while(!directory.DirectoryContains(searchRegex))
            {
                directory = directory.Parent;

                // Throw argument exception
                i++;
                if (i == 10) { throw new ArgumentException(); }
            }

            return directory.ToString();
        }

        public static string GetFullnameForFileInHigherDirectory(
            this DirectoryInfo directory,
            Regex searchRegex,
            string searchText)
        {
            var parent = directory.GetParentOfFileInHigherDirectory(searchRegex);

            return Directory.GetFiles(parent.ToString(), searchText)[0];
        }

        public static string GetCurrentProjectFile(this DirectoryInfo directory)
        {
            var projectFileRegex = new Regex("\\.csproj$");

            var parent = GetParentOfFileInHigherDirectory(directory, projectFileRegex);

            // Get project file name
            var matches = Directory.GetFiles(parent.ToString(), "*.csproj");

            return matches[0];;
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ZiZhuJY.FileSystemHelpers
{
    public class FileSystemHelper
    {
        public static string[] SearchFileNames(string directoryName, string pattern)
        {
            var regex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

            return SearchFileNames(directoryName, regex);
        }

        public static string[] SearchFileNames(string directoryName, Regex regex)
        {
            return Directory.GetFiles(directoryName).Where(fileName => fileName != null && regex.IsMatch(Path.GetFileName(fileName))).Select(Path.GetFileName).ToArray();
        }
    }
}

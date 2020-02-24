using System;
using System.IO;

namespace Library
{
    public static class DirectoryInfos
    {
        public static readonly string TxtFolder = @"..\..\..\..\Library\Txt\";

        public static readonly string XmlFolder = @"..\..\..\..\Library\Xml\";

        public static string GetPath(string fileName)
        {
            var extension = Path.GetExtension(fileName);
            if (extension.Equals(".txt") && File.Exists(Path.Combine(TxtFolder, fileName)))
                return TxtFolder + fileName;
            else if (extension.Equals(".xml") && File.Exists(Path.Combine(XmlFolder, fileName)))
                return XmlFolder + fileName;
            else
                throw new ArgumentException("File does not exist or has the wrong extension: " + fileName);
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Library
{
    public static class Serializer
    {
        public static T Deserialize<T>(string input)
        {
            XmlSerializer ser = new XmlSerializer(typeof(T));

            using (StringReader sr = new StringReader(input))
            {
                return (T)ser.Deserialize(sr);
            }
        }

        public static string Serialize<T>(T ObjectToSerialize)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(ObjectToSerialize.GetType());

            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, ObjectToSerialize);
                return textWriter.ToString();
            }
        }

        private static FileStream WaitForFile(string fullPath, FileMode mode, FileAccess access)
        {
            for (int numTries = 0; numTries < 10; numTries++)
            {
                FileStream fs = null;
                try
                {
                    fs = new FileStream(fullPath, mode, access);
                    return fs;
                }
                catch (IOException e)
                {
                    if (fs != null)
                        fs.Dispose();

                    Thread.Sleep(50);
                }
            }
            return null;
        }

        public static T FromXml<T>(string path)
        {
            string result = null;
            var stream = WaitForFile(path, FileMode.Open, FileAccess.Read);
            using (StreamReader inputFile = new StreamReader(stream))
            {
                result = inputFile.ReadToEnd();
                inputFile.Close();
            }
            return Deserialize<T>(result);
        }

        public static void ToXml<T>(this T sender, string path)
        {
            var stream = WaitForFile(path, FileMode.OpenOrCreate, FileAccess.Write);
            using (StreamWriter outputFile = new StreamWriter(stream))
            {
                outputFile.Write(Serialize<T>(sender));
                outputFile.Close();
            }
        }
    }
}

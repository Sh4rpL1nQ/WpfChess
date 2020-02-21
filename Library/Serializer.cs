using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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

        public static Assembly GetAssemblyByName(string name)
        {
            return AppDomain.CurrentDomain.GetAssemblies().
                   SingleOrDefault(assembly => assembly.GetName().Name == name);
        }

        public static Dictionary<int, string> LookupTable()
        {
            var dic = new Dictionary<int, string>();
            using (StreamReader reader = new StreamReader(@"..\..\..\..\Library\Txt\LookupTable.txt"))
            {
                var counter = 0;
                var currentLine = string.Empty;
                while ((currentLine = reader.ReadLine()) != null)
                {
                    dic.Add(counter + 1, currentLine);

                    counter++;
                }
            }

            return dic;
        }

        public static object ImportFromTxt(string path)
        {
            var board = new Board();
            board.MakeBoard();
            var lookup = LookupTable();
            var colors = new List<string>();
            int initialStep1 = 0;
            int initialStep2 = 0;
            try
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    string currentLine;
                    int counter = 0;
                    int countSpace = 0;
                    while ((currentLine = reader.ReadLine()) != null)
                    {
                        if (string.IsNullOrEmpty(currentLine))
                        {
                            countSpace++;
                            continue;
                        }

                        if (countSpace == 0)
                            colors.Add(currentLine);

                        if (countSpace == 1)
                        {
                            int[] array = Array.ConvertAll(currentLine.Split(' '), s => int.Parse(s));
                            board.TopColor = (Color)Enum.Parse(typeof(Color), colors[0]);
                            for (int i = 0; i < array.Length; i++)
                            {
                                if (array[i] != 0)
                                {
                                    board.Squares[initialStep1 + i].Piece = (Piece)Activator.CreateInstance(GetAssemblyByName("Library").GetType("Library.Pieces." + lookup[array[i]]));
                                }

                            }

                            initialStep1 += 8;
                        }

                        if (countSpace == 2)
                        {
                            int[] array = Array.ConvertAll(currentLine.Split(' '), s => int.Parse(s));
                            for (int i = 0; i < array.Length; i++)
                            {
                                if (array[i] == 1)
                                {
                                    if (board.Squares[initialStep2 + i].Piece == null) throw new Exception("color was set, but there's no piece: reason txt second matrix");
                                    board.Squares[initialStep2 + i].Piece.Color = board.TopColor;
                                    board.Squares[initialStep2 + i].Piece.PartOfTopBoard = true;
                                }
                                else if (array[i] == 2)
                                {
                                    if (board.Squares[initialStep2 + i].Piece == null) throw new Exception("color was set, but there's no piece: reason txt second matrix");
                                    board.Squares[initialStep2 + i].Piece.Color = board.GetOtherColor(board.TopColor);
                                    board.Squares[initialStep2 + i].Piece.PartOfTopBoard = false;
                                }
                            }

                            initialStep2 += 8;
                        }

                        counter++;
                    }
                }
            } catch (Exception e) { 
                return e.Message; }

            return board;
        }
    }
}

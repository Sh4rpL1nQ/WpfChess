using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
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
                catch (IOException)
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

        private static Assembly GetAssemblyByName(string name)
        {
            return AppDomain.CurrentDomain.GetAssemblies().
                   SingleOrDefault(assembly => assembly.GetName().Name == name);
        }

        public static Dictionary<int, string> LookupTable()
        {
            var dic = new Dictionary<int, string>();
            using (StreamReader reader = new StreamReader(DirectoryInfos.GetPath("LookupTable.txt")))
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

        public static Board ImportFromTxt(string path)
        {
            var board = new Board();
            var lookup = LookupTable();
            var innerCounter = 0;
            var currentLine = string.Empty;
            var counter = 0;
            var countSpace = 0;
            using (StreamReader reader = new StreamReader(path))
            {
                while ((currentLine = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrEmpty(currentLine))
                    {
                        innerCounter = 0;
                        countSpace++;
                        continue;
                    }

                    if (countSpace == 0 && counter == 0)
                        board.TopColor = currentLine.ToEnum<Color>();

                    if (countSpace == 1)
                    {
                        int[] array = Array.ConvertAll(currentLine.Split(' '), s => int.Parse(s));
                        for (int i = 0; i < array.Length; i++)
                            if (array[i] != 0)
                                board.Squares[innerCounter + i].Piece = (Piece)Activator.CreateInstance(GetAssemblyByName("Library").GetType("Library.Pieces." + lookup[array[i]]));

                        innerCounter += Board.BoardSize;
                    }

                    if (countSpace == 2)
                    {
                        int[] array = Array.ConvertAll(currentLine.Split(' '), s => int.Parse(s));
                        for (int i = 0; i < array.Length; i++)
                        {
                            if (array[i] == 1)
                            {
                                if (board.Squares[innerCounter + i].Piece == null)
                                    throw new Exception("color was set, but there's no piece: reason txt second matrix");

                                board.Squares[innerCounter + i].Piece.Color = board.TopColor;
                                board.Squares[innerCounter + i].Piece.PartOfTopBoard = true;
                            }
                            else if (array[i] == 2)
                            {
                                if (board.Squares[innerCounter + i].Piece == null)
                                    throw new Exception("color was set, but there's no piece: reason txt second matrix");

                                board.Squares[innerCounter + i].Piece.Color = board.GetOtherColor(board.TopColor);
                                board.Squares[innerCounter + i].Piece.PartOfTopBoard = false;
                            }
                            else
                                if (board.Squares[innerCounter + i].Piece != null)
                                    throw new Exception("color wasn't set, but there's a piece: reason txt second matrix");                           
                        }

                        innerCounter += Board.BoardSize;
                    }

                    counter++;
                }
            }
            return board;
        }
    }
}

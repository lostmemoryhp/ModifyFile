using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ModifyFile
{
    public class AppendContent
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Color { get; set; }

        public override string ToString()
        {
            return $"[{X},{Y},{Color}]";
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            var cnt = obj as AppendContent;
            if (cnt == null)
                return false;
            return cnt.X == X && cnt.Y == Y;
        }

        public override int GetHashCode()
        {
            return $"{X}:{Y}".GetHashCode();
        }
    }

    public class AppendContentBuilder
    {
        static Regex reg = new Regex(@"^==APPEND==(\[[+-]*\d+,[+-]*\d+,[+-]*\d+\])+==APPEND\d{6}==$");

        public ISet<AppendContent> Contents { get; set; } = new HashSet<AppendContent>();

        public int Length
        {
            get
            {
                var content = string.Join("", Contents.Select(x => x.ToString()));
                var length = content.Length + 20 + 6;
                return length;
            }
        }

        public override string ToString()
        {
            var content = string.Join("", Contents.Select(x => x.ToString()));
            return $"==APPEND=={content}==APPEND{Length.ToString("000000")}==";
        }



        public static AppendContentBuilder FromString(string str)
        {
            var match = reg.Match(str);
            if (!match.Success)
                return null;

            var builder = new AppendContentBuilder();
            if (match.Groups.Count == 2)
            {
                var captureCollection = match.Groups[1].Captures;
                for (int i = 0; i < captureCollection.Count; i++)
                {
                   var itemStr =  captureCollection[i].Value;
                    var items = itemStr.Substring(1, itemStr.Length - 2).Split(new char[] { ',' });
                    var content = new AppendContent
                    {
                        X = int.Parse(items[0]),
                        Y = int.Parse(items[1]),
                        Color = int.Parse(items[2])
                    };
                    builder.Contents.Add(content);
                }
            }
            return builder;
        }

        public static AppendContentBuilder ParseFile(String file)
        {
            var buffer = File.ReadAllBytes(file);
            return ParseFile(buffer);
        }

        public static AppendContentBuilder ParseFile(FileInfo file)
        {
            var buffer = File.ReadAllBytes(file.FullName);
            return ParseFile(buffer);
        }

        public static AppendContentBuilder ParseFile(FileStream fs)
        {
            var buffer = new byte[fs.Length];
            return ParseFile(buffer);
        }


        public static AppendContentBuilder ParseFile(byte[] fileBuffer)
        {
            var buffer = new byte[16];
            Array.Copy(fileBuffer, fileBuffer.Length - buffer.Length, buffer, 0, buffer.Length);
            var str = Encoding.ASCII.GetString(buffer);
            var reg = new Regex(@"^==APPEND(\d{6})==$");
            var m = reg.Match(str);
            if (!m.Success)
                return null;

            var strLength = int.Parse(m.Groups[1].Value);
            var buffer2 = new byte[strLength];
            Array.Copy(fileBuffer, fileBuffer.Length - buffer2.Length, buffer2, 0, buffer2.Length);
            var appendStr = Encoding.ASCII.GetString(buffer2);
            return FromString(appendStr);
        }


        public static void AddContent2File(String file, ISet<AppendContent> contents)
        {
            var fileBuffer = File.ReadAllBytes(file);

            var builder = ParseFile(fileBuffer);
            if (builder == null)
            {
                builder = new AppendContentBuilder();
                foreach (var content in contents)
                {
                    builder.Contents.Add(content);
                }
               
                using (var fs = new FileStream(file, FileMode.Append, FileAccess.Write))
                {
                    fs.Write(Encoding.ASCII.GetBytes(builder.ToString()), 0, builder.Length);
                }

            }
            else
            {
                int oldlen = builder.Length;
                foreach (var content in contents)
                {
                    builder.Contents.Add(content);
                }
                var contentBuffer = Encoding.ASCII.GetBytes(builder.ToString());
                var resultbuffer = new byte[fileBuffer.Length + builder.Length];
                Array.Copy(fileBuffer, 0, resultbuffer, 0, fileBuffer.Length - oldlen);
                Array.Copy(contentBuffer, 0, resultbuffer, fileBuffer.Length, contentBuffer.Length);
                using (var fs = File.Create(file))
                {
                    fs.Write(resultbuffer, 0, resultbuffer.Length);
                }

            }

        }


        public static void RemoveContentfromFile(string file)
        {
            var fileBuffer = File.ReadAllBytes(file);

            var builder = ParseFile(fileBuffer);
            if (builder == null)
                return;

            var resultbuffer = new byte[fileBuffer.Length - builder.Length];
            Array.Copy(fileBuffer, 0, resultbuffer, 0, resultbuffer.Length);
            using (var fs = File.Create(file))
            {
                fs.Write(resultbuffer, 0, resultbuffer.Length);
            }

        }
    }

}

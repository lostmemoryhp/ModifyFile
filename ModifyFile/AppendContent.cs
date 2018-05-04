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
        public RGBTypeEnum RGBType { get; set; }
        public byte Value { get; set; }

        public override string ToString()
        {
            return $"[{X},{Y},{(int)RGBType},{Value}]";
        }
    }

    public class AppendContentBuilder
    {
        static Regex reg = new Regex(@"^==APPEND==((\[\d+,\d+,\d+,\d+])+)==APPEND\d{6}==$");

        public List<AppendContent> Contents { get; set; } = new List<AppendContent>();


        public override string ToString()
        {
            var content = string.Join("", Contents.Select(x => x.ToString()));
            var length = content.Length + 20 + 6;
            return $"==APPEND=={content}==APPEND{length.ToString("000000")}==";
        }



        public static AppendContentBuilder FromString(string str)
        {
            var match = reg.Match(str);
            if (!match.Success)
                return null;

            var builder = new AppendContentBuilder();
            for (int i = 2; i < match.Groups.Count; i++)
            {
                var groupValue = match.Groups[i].Value; // [12,31,2,31]

                var items = groupValue.Substring(1, groupValue.Length - 2).Split(new char[] { ',' });
                var content = new AppendContent
                {
                    X = int.Parse(items[0]),
                    Y = int.Parse(items[1]),
                    RGBType = (RGBTypeEnum)int.Parse(items[2]),
                    Value = byte.Parse(items[3])
                };
                builder.Contents.Add(content);
            }
            return builder;
        }


        public static AppendContentBuilder parseFile(FileStream fs)
        {
            var lenth = fs.Length;
            var buffer = new byte[16];
            fs.Read(buffer, (int)lenth - 16, 16);
            var str = Encoding.Default.GetString(buffer);
            var reg = new Regex(@"^==APPEND(\d{6})==$");
            var m = reg.Match(str);
            if (!m.Success)
                return null;

            return null;
        }





    }

    public enum RGBTypeEnum
    {
        A, R, G, B
    }
}

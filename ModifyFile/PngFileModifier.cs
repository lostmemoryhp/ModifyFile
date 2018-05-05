using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModifyFile
{
    public class PngFileModifier : IFileModifier
    {
        public bool canModify(FileInfo file)
        {
            if (file.Name.EndsWith(".9.png"))
            {
                return false;
            }
            String ext = file.Extension.ToLower();
            switch (ext)
            {
                case ".bmp":
                //case ".gif":
                case ".png":
                    return true;
                default:
                    return false;
            }
        }

        public void modify(FileInfo file)
        {
            Bitmap bitmap = null;
            AppendContent appendContent = null;
            AppendContentBuilder builder = AppendContentBuilder.ParseFile(file)??new AppendContentBuilder();
            try
            {
                using (var fs = file.OpenRead())
                {
                    bitmap = new Bitmap(fs);
                }

                Random random = new Random((int)DateTime.Now.Ticks);
                for (int i = 0; i < 2048 * 2048; i++)
                {
                    var x = random.Next(bitmap.Width);
                    var y = random.Next(bitmap.Height);
                    var pixel = bitmap.GetPixel(x, y);
                    if (pixel.ToArgb() != Color.Transparent.ToArgb())
                    {
                        if (pixel.A > 125)
                        {
                            bitmap.SetPixel(x, y, Color.FromArgb(pixel.A - random.Next(1, 3), pixel));
                        }
                        else
                        {
                            bitmap.SetPixel(x, y, Color.FromArgb(pixel.A + random.Next(1, 3), pixel));
                        }
                        appendContent = new AppendContent { X = x, Y = y, Color = pixel.ToArgb() };
                        break;
                    }
                }

                bitmap.Save(file.FullName);
            }
            catch (Exception ex)
            {
            }
            finally
            {
                if (bitmap != null)
                {
                    bitmap.Dispose();
                }
            }
            if (appendContent != null)
            {
                builder.Contents.Add(appendContent);
                AppendContentBuilder.AddContent2File(file.FullName, builder.Contents);
            }

            //var png = new Png(file);
            //png.AddInternationalText("comment", "hello " + DateTime.Now.Ticks);
            //png.SaveFile();
        }

        public void recovery(FileInfo file)
        {
            Bitmap bitmap = null;
            var filebuffer = File.ReadAllBytes(file.FullName);
            var builder = AppendContentBuilder.ParseFile(filebuffer);
            if (builder == null)
                return;

            bool success = false;
            try
            {
                using (var fs = file.OpenRead())
                {
                    bitmap = new Bitmap(fs);
                }

                foreach (var apcnt in builder.Contents)
                {
                    bitmap.SetPixel(apcnt.X, apcnt.Y, Color.FromArgb(apcnt.Color));
                }

                bitmap.Save(file.FullName);
                success = true;
            }
            catch (Exception ex)
            {
            }
            finally
            {
                if (bitmap != null)
                {
                    bitmap.Dispose();
                }
            }

            if (success)
            {
                AppendContentBuilder.RemoveContentfromFile(file.FullName);
            }
        }
    }
}

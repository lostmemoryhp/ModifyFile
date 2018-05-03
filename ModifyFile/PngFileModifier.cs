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
            String ext = file.Extension.ToLower();
            switch (ext)
            {
                case ".jpg":
                case ".jpeg":
                case ".bmp":
                case ".gif":
                case ".png":
                    return true;
                default:
                    return false;
            }
        }

        public void modify(FileInfo file)
        {
            Bitmap bitmap = null;
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
                            bitmap.SetPixel(x, y, Color.FromArgb(pixel.A - random.Next(1,5), pixel));
                        }
                        else
                        {
                            bitmap.SetPixel(x, y, Color.FromArgb(pixel.A + random.Next(1,5), pixel));
                        }
                        break;
                    }
                }

                bitmap.Save(file.FullName);
            }
            catch (Exception)
            {
            }
            finally
            {
                if (bitmap != null)
                {
                    bitmap.Dispose();
                }
            }


            //var png = new Png(file);
            //png.AddInternationalText("comment", "hello " + DateTime.Now.Ticks);
            //png.SaveFile();
        }


    }
}

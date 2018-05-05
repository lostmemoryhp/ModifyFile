using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModifyFile
{
    public class JPGFileModifier : IFileModifier
    {
        public bool canModify(FileInfo file)
        {
            String ext = file.Extension.ToLower();
            switch (ext)
            {
                case ".jpg":
                case ".jpeg":
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

                bitmap.SetPixel(0, 0, bitmap.GetPixel(0, 0));
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
        }

        public void recovery(FileInfo file)
        {
            // do nothing
        }
    }
}

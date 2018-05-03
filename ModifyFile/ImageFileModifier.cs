using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ModifyFile
{
    public class ImageFileModifier : IFileModifier
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
                    //case ".tiff":
                    //case ".tif":
                    return true;
                default:
                    return false;
            }
        }

        public void modify(FileInfo file)
        {
            BitmapDecoder decoder = null;
            BitmapFrame bitmapFrame = null;
            BitmapMetadata metadata = null;


            decoder = GetBitmapDecoder(file);


            bitmapFrame = decoder.Frames[0];
            metadata = (BitmapMetadata)bitmapFrame.Metadata;

            var cmt = metadata.GetQuery("/app1/ifd/exif:{uint=40092}");

            if (bitmapFrame != null)
            {
                BitmapMetadata metaData = (BitmapMetadata)bitmapFrame.Metadata.Clone();

                if (metaData != null)
                {
                    // modify the metadata   
                    //metaData.SetQuery("/app1/ifd/exif:{uint=40092}", "hello world!");

                    setMetaData(file, metadata);

                    // get an encoder to create a new jpg file with the new metadata.      
                    var encoder = GetBitmapEncoder(file);
                    encoder.Frames.Add(BitmapFrame.Create(bitmapFrame, bitmapFrame.Thumbnail, metaData, bitmapFrame.ColorContexts));
                    //string jpegNewFileName = Path.Combine(jpegDirectory, "JpegTemp.jpg");

                    // Delete the original
                    file.Delete();

                    // Save the new image 
                    using (Stream jpegStreamOut = File.Open(file.FullName, FileMode.CreateNew, FileAccess.ReadWrite))
                    {
                        encoder.Save(jpegStreamOut);
                    }
                }
            }

        }

        void setMetaData(FileInfo f, BitmapMetadata metadata)
        {
            switch (f.Extension.ToLower())
            {
                case ".jpg":
                case ".jpeg":
                    metadata.SetQuery("/app1/ifd/exif:{uint=40092}", "hello " + DateTime.Now.Ticks);
                    break;
                case ".png":
                    metadata.SetQuery("/iTXt/Description", "hello world!" + DateTime.Now.Ticks);
                    break;
                case ".tiff":
                case ".tif":
                    break;
                case ".gif":
                    metadata.SetQuery("/commentext/", "hello gif" + DateTime.Now.Ticks);
                    break;
                default:
                    break;
            }
        }


        BitmapDecoder GetBitmapDecoder(FileInfo f)
        {
            //BmpBitmapDecoder s;
            //JpegBitmapDecoder s1;
            //IconBitmapDecoder s2;
            //GifBitmapDecoder s3;
            //LateBoundBitmapDecoder s4;
            //PngBitmapDecoder s5;
            //TiffBitmapDecoder s6;
            //WmpBitmapDecoder s7;

            using (Stream jpegStreamIn = File.Open(f.FullName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                switch (f.Extension.ToLower())
                {
                    case ".jpg":
                    case ".jpeg":
                        return new JpegBitmapDecoder(jpegStreamIn, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                    case ".png":
                        return new PngBitmapDecoder(jpegStreamIn, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                    case ".tif":
                    case ".tiff":
                        return new TiffBitmapDecoder(jpegStreamIn, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                    case ".gif":
                        return new GifBitmapDecoder(jpegStreamIn, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                    case ".bmp":
                        return new BmpBitmapDecoder(jpegStreamIn, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                    default:
                        return new JpegBitmapDecoder(jpegStreamIn, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                }
            }
        }

        BitmapEncoder GetBitmapEncoder(FileInfo f)
        {
            //BmpBitmapDecoder s;
            //JpegBitmapDecoder s1;
            //IconBitmapDecoder s2;
            //GifBitmapDecoder s3;
            //LateBoundBitmapDecoder s4;
            //PngBitmapDecoder s5;
            //TiffBitmapDecoder s6;
            //WmpBitmapDecoder s7;

            using (Stream jpegStreamIn = File.Open(f.FullName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                switch (f.Extension.ToLower())
                {
                    case ".jpg":
                    case ".jpeg":
                        return new JpegBitmapEncoder();
                    case ".png":
                        return new PngBitmapEncoder();
                    case ".tif":
                    case ".tiff":
                        return new TiffBitmapEncoder();
                    case ".gif":
                        return new GifBitmapEncoder();
                    case ".bmp":
                        return new BmpBitmapEncoder();
                    default:
                        return new JpegBitmapEncoder();
                }
            }
        }
    }
}

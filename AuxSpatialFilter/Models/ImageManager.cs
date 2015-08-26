using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Livet;
using OpenCvSharp.CPlusPlus;
using OpenCvSharp.Extensions;
using System.IO;

namespace AuxSpatialFilter.Models
{
    public class ImageManager : NotificationObject
    {
        public static ImageData Filter(string fileName, SpatialFilterKernel kernel)
        {
            try
            {
                var src = new Mat(fileName);
                var bmp = kernel.FilterWriteableBitmap(src);
                var images = new ImageData();
                images.SourceImage = src.ToWriteableBitmap();
                images.ResultImage = bmp;
                return images;
            }
            finally
            {
            }
        }
    }
}

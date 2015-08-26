using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Livet;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.IO;

namespace AuxSpatialFilter.Models
{
    public class ImageData : NotificationObject
    {

        #region SourceImage変更通知プロパティ
        private WriteableBitmap _SourceImage;

        public WriteableBitmap SourceImage
        {
            get
            { return _SourceImage; }
            set
            {
                if (_SourceImage == value)
                    return;
                _SourceImage = value;
                RaisePropertyChanged("SourceImage");
            }
        }
        #endregion


        #region ResultImage変更通知プロパティ
        private WriteableBitmap _ResultImage;

        public WriteableBitmap ResultImage
        {
            get
            { return _ResultImage; }
            set
            {
                if (_ResultImage == value)
                    return;
                _ResultImage = value;
                RaisePropertyChanged("ResultImage");
            }
        }
        #endregion

        public void SaveSource(string fileName)
        {
            SaveImage(SourceImage, fileName);
        }

        public void SaveResult(string fileName)
        {
            SaveImage(ResultImage, fileName);

        }

        public void SaveImage(WriteableBitmap bitmap, string fileName)
        {
            using (var stream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmap));
                encoder.Save(stream);
            }
        }


        public void Freeze()
        {
            SourceImage.Freeze();
            ResultImage.Freeze();
        }

    }
}

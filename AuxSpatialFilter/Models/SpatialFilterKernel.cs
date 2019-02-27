using OpenCvSharp.CPlusPlus;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace AuxSpatialFilter.Models
{
    public class SpatialFilterKernel
    {
        public SpatialFilterKernel()
        {

        }

        public SpatialFilterKernel(double[] kernel, int kernelSize)
        {
            Values = kernel;
            KernelSize = kernelSize;
        }

        public SpatialFilterKernel(double[][] kernel, int kernelSize)
        {
            Values = GetKernel(kernel, kernelSize);
            KernelSize = kernelSize;
        }

        public double[] GetKernel(double[][] kernelSource, int kernelSize)
        {
            var kernel = new double[kernelSize * kernelSize];
            int count = 0;
            for (int i = 0; i < kernelSize; i++)
            {
                for (int j = 0; j < kernelSize; j++)
                {
                    kernel[count] = kernelSource[i][j];
                    count++;
                }
            }
            return kernel;
        }


        public int KernelSize { get; set; }
        public double[] Values { get; set; }
        public bool IsNormalized { get; set; }

        public Mat GetMat()
        {
            if (KernelSize * KernelSize != Values.Count())
            {
                throw new InvalidOperationException("you must fulfill KernelSize^2 == Values.Count");
            }

            var mat = new MatOfDouble(KernelSize, KernelSize,Values);

            if (IsNormalized)
            {
                Cv2.Normalize(mat, mat, 1, 0, OpenCvSharp.NormType.L1);
            }
            return mat;
        }


        public Bitmap FilterBitmap(Mat src)
        {
            using (var k = GetMat())
            using (var dst = new Mat())
            {
                Cv2.Filter2D(src, dst, MatType.CV_8U, k);
                return dst.ToBitmap();
            }
        }

        public Task<Bitmap> FilterBitmapAsync(Mat src)
        {
            return Task.Run(() =>
            {
                using (var k = GetMat())
                using (var dst = new Mat())
                {
                    Cv2.Filter2D(src, dst, MatType.CV_8U, k);
                    return dst.ToBitmap();
                }
            });
        }


        public WriteableBitmap FilterWriteableBitmap(Mat src)
        {
            using (var k = GetMat())
            using (var dst = new Mat())
            {
                Cv2.Filter2D(src, dst, MatType.CV_8U, k);
                return dst.ToWriteableBitmap();
            }
        }

        public Task<WriteableBitmap> FilterWriteableBitmapAsync(Mat src)
        {
            return Task.Run(() =>
            {
                using (var k = GetMat())
                using (var dst = new Mat())
                {
                    Cv2.Filter2D(src, dst, MatType.CV_8U, k);
                    return dst.ToWriteableBitmap();
                }
            });
        }

        public Mat Filter(Mat src)
        {
            var dst = new Mat();
            using (var k = GetMat())
            {
                Cv2.Filter2D(src, dst, MatType.CV_8U, k);
                return dst;
            }
        }

        public Task<Mat> FilterAsync(Mat src)
        {
            return Task.Run(() =>
            {
                var dst = new Mat();
                using (var k = GetMat())
                {
                    Cv2.Filter2D(src, dst, MatType.CV_8U, k);
                    return dst;
                }
            });
        }


        public Bitmap FilterBitmap(Bitmap src)
        {
            using (var mat = src.ToMat())
            {
                return FilterBitmap(src);
            }
        }
        public Task<Bitmap> FilterBitmapAsync(Bitmap src)
        {
            using (var mat = src.ToMat())
            {
                return FilterBitmapAsync(src);
            }
        }
        public Mat Filter(Bitmap src)
        {
            using (var mat = src.ToMat())
            {
                return Filter(src);
            }
        }
        public Task<Mat> FilterAsync(Bitmap src)
        {
            using (var mat = src.ToMat())
            {
                return FilterAsync(src);
            }
        }



    }
}

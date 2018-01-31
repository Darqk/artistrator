using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Artistrator {

    public class Kernel {

        public double sum = 1f;
        public int[,] mask;

        public int SmallWidth
        {
            get
            {
                return (Width - 1) / 2;
            }
        }

        public int SmallHeight
        {
            get
            {
                return (Height - 1) / 2;
            }
        }

        public int Width
        {
            get
            {
                return mask.GetLength(0);
            }
        }

        public int Height
        {
            get
            {
                return mask.GetLength(1);
            }
        }

        public Kernel (int[,] mask,double sum) {

            this.mask = mask;
            this.sum = sum;
        }

        public Kernel (int[,] mask) {

            this.mask = mask;
            sum = GetMaskSum(mask);
        }

        public static Kernel sobelHorizontal3x3 = new Kernel(
            new int[3,3] {
                {1,0,-1},
                {2,0,-2},
                {1,0,-1}
            },
            4.0
        );

        public static Kernel sobelVertical3x3 = new Kernel(
            new int[3,3] {
                { 1, 2, 1},
                { 0, 0, 0},
                {-1,-2,-1}
            },
            4.0
        );

        public static Kernel gaussianBlur5x5 = new Kernel(
            new int[5,5] {
                {1,4 ,6 ,4 ,1},
                {4,16,24,16,4},
                {6,24,36,24,6},
                {4,16,24,16,4},
                {1,4 ,6 ,4 ,1}
            },
            256.0
        );

        public static Kernel boxBlur3x3 = new Kernel(
            new int[3,3] {
                {1,1,1},
                {1,1,1},
                {1,1,1}
            },
            9.0
        );

        public static Kernel boxBlur5x5 = new Kernel(
            new int[5,5] {
                {1,1,1,1,1},
                {1,1,1,1,1},
                {1,1,1,1,1},
                {1,1,1,1,1},
                {1,1,1,1,1}
            },
            25.0
        );

        public static Kernel sharpen3x3 = new Kernel(
            new int[3,3] {
                { 0,-1, 0},
                {-1, 5,-1},
                { 0,-1, 0}
            },
            9.0
        );

        public static Kernel unsharpMasking5x5 = new Kernel(
            new int[5,5] {
                {1,4 , 6  ,4 ,1},
                {4,16, 24 ,16,4},
                {6,24,-476,24,6},
                {4,16, 24 ,16,4},
                {1,4 , 6  ,4 ,1}
            },
            -256.0
        );

        public static double GetMaskSum (int[,] mask) {

            double posSum = 0f;
            double negSum = 0f;

            for (int x = 0; x < mask.GetLength(0); x++) {
                for (int y = 0; y < mask.GetLength(1); y++) {
                    if (mask[x,y] > 0)
                        posSum += mask[x,y];
                    else
                        negSum += mask[x,y];
                }
            }

            return Math.Max(posSum,Math.Abs(negSum));
        }

        public static double[,] Apply (double[,] im,Kernel kernel) {

            int width = im.GetLength(0) - kernel.SmallWidth * 2;
            int height = im.GetLength(1) - kernel.SmallHeight * 2;

            double[,] newIm = new double[width,height];

            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {

                    double sum = 0.0;

                    for (int kx = 0; kx < kernel.Width; kx++) {
                        for (int ky = 0; ky < kernel.Height; ky++) {

                            double value = im[x + kx,y + ky];

                            sum += value * kernel.mask[kx,ky];
                        }
                    }

                    newIm[x,y] = sum / kernel.sum;
                }
            }

            return newIm;
        }

        public static double[,] ApplySeries (double[,] im,params Kernel[] kernels) {

            double[,] newIm = im;

            for (int i = 0; i < kernels.Length; i++) {
                newIm = Apply(newIm,kernels[i]);
            }

            return newIm;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Artistrator {

    public static class ImageTools {

        public enum Operation { Add, Sub, Mult, Div, Max, Min, Mag, Abs };

        public static double[][] BitmapToArray (Bitmap im) {

            int totalPoints = im.Width * im.Height;
            double[][] points = new double[totalPoints][];

            for (int x = 0, i = 0; x < im.Width; x++) {
                for (int y = 0; y < im.Height; y++, i++) {

                    Color pixel = im.GetPixel(x,y);
                    points[i] = new double[3] { pixel.R,pixel.G,pixel.B };
                }
            }

            return points;
        }

        public static Bitmap ArrayToBitmap (double[,] array) {

            int width = array.GetLength(0);
            int height = array.GetLength(1);

            Bitmap image = new Bitmap(width,height);

            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    int value = (int)(array[x,y] * 255);
                    image.SetPixel(x,y,Color.FromArgb(value,value,value));
                }
            }

            return image;
        }

        public static Bitmap ArrayToBitmap (int[,] array) {

            int width = array.GetLength(0);
            int height = array.GetLength(1);

            Bitmap image = new Bitmap(width,height);

            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    int value = array[x,y] * 255;
                    image.SetPixel(x,y,Color.FromArgb(value,value,value));
                }
            }

            return image;
        }

        public static int GetMaxValue (int[,] map) {

            int count = 0;

            for (int x = 0; x < map.GetLength(0); x++) {
                for (int y = 0; y < map.GetLength(1); y++) {
                    if (map[x,y] > count)
                        count = map[x,y];
                }
            }

            return count;
        }

        public static double[,] ReturnRange (double[,] image,double max,double min) {

            int width = image.GetLength(0);
            int height = image.GetLength(1);

            double[,] newImage = new double[width,height];

            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {

                    if (image[x,y] > min) {
                        if (image[x,y] <= max) {
                            newImage[x,y] = image[x,y];
                            continue;
                        }
                    }

                    newImage[x,y] = 0.0;
                }
            }

            return newImage;
        }

        public static void ApplyOperation (double[,] image,Operation operation,double value) {

            int width = image.GetLength(0);
            int height = image.GetLength(1);

            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {

                    switch (operation) {
                        case Operation.Add:
                            image[x,y] += value;
                            break;
                        case Operation.Sub:
                            image[x,y] -= value;
                            break;
                        case Operation.Mult:
                            image[x,y] *= value;
                            break;
                        case Operation.Div:
                            image[x,y] /= value;
                            break;
                        case Operation.Max:
                            image[x,y] = Math.Max(image[x,y],value);
                            break;
                        case Operation.Min:
                            image[x,y] = Math.Min(image[x,y],value);
                            break;
                        case Operation.Abs:
                            image[x,y] = Math.Abs(image[x,y]);
                            break;
                    }
                }
            }
        }

        public static double[,] Combine (double[,] imageA,double[,] imageB,Operation operation) {

            int width = imageA.GetLength(0);
            int height = imageA.GetLength(1);

            double[,] newImage = new double[width,height];

            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {

                    switch (operation) {
                        case Operation.Add:
                            newImage[x,y] = imageA[x,y] + imageB[x,y];
                            break;
                        case Operation.Sub:
                            newImage[x,y] = imageA[x,y] - imageB[x,y];
                            break;
                        case Operation.Mult:
                            newImage[x,y] = imageA[x,y] * imageB[x,y];
                            break;
                        case Operation.Div:
                            newImage[x,y] = imageA[x,y] / imageB[x,y];
                            break;
                        case Operation.Max:
                            newImage[x,y] = Math.Max(imageA[x,y],imageB[x,y]);
                            break;
                        case Operation.Min:
                            newImage[x,y] = Math.Min(imageA[x,y],imageB[x,y]);
                            break;
                        case Operation.Mag:
                            newImage[x,y] = Math.Sqrt(imageA[x,y] * imageA[x,y] + imageB[x,y] * imageB[x,y]);
                            break;
                    }
                }
            }

            return newImage;
        }

        public static void Normalize (double[,] image) {

            int width = image.GetLength(0);
            int height = image.GetLength(1);

            double maxValue = image[0,0];
            double minValue = image[0,0];

            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    if (image[x,y] > maxValue)
                        maxValue = image[x,y];
                    else if (image[x,y] < minValue)
                        minValue = image[x,y];
                }
            }

            ApplyOperation(image,Operation.Sub,minValue);
            ApplyOperation(image,Operation.Div,maxValue - minValue);
        }

        public static double[,] GetBrightnessMap (Bitmap im) {

            double[,] newIm = new double[im.Width,im.Height];

            for (int x = 0; x < im.Width; x++) {
                for (int y = 0; y < im.Height; y++) {
                    newIm[x,y] = im.GetPixel(x,y).GetBrightness();
                }
            }

            return newIm;
        }
    }
}

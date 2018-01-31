using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;
using Artistrator.Drawing;

namespace Artistrator {

    public partial class MainWindow : Window {

        private const string rootPath = @"C:\Users\Ciaran Hogan\Desktop\";
        private const string imagePath = @"C:\Users\Ciaran Hogan\Desktop\Artistrator\Images\";

        private string[] fileNames = new string[] {
            "IMG_1519.JPG",
            "IMG_1483.JPG",
            "IMG_1092.JPG",
            "IMG_1085.JPG",
            "IMG_2135.JPG",
            "IMG_4995.JPG",
            "Rose.JPEG",
            "Flowers.JPG",
            "Grass.JPG",
            "GrassBlock32x32.png",
            "WaterFlowingAroundRock.JPG",
            "Stream.JPG",
            "Floorboards.JPG",
            "LinesWarped.PNG",
            "Flow Test Image.PNG"
        };

        private Random random = new Random();

        public MainWindow () {

            InitializeComponent();

            Run();
        }

        public Bitmap GetImage () {

            Bitmap bitmap = (Bitmap)Image.FromFile(imagePath + fileNames[8]);

            int width = bitmap.Width / 2;
            int height = bitmap.Height / 2;

            Bitmap newBitmap = new Bitmap(width,height);

            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    newBitmap.SetPixel(x,y,bitmap.GetPixel(x * 2,y * 2));
                }
            }

            return newBitmap;
        }

        public void Save (int[,] map,Clustering.KMeans kMeans,string name) {

            Bitmap newImage = new Bitmap(map.GetLength(0),map.GetLength(1));

            Color[] colors = new Color[kMeans.clusters.Length];

            for (int i = 0; i < colors.Length; i++) {
                double[] mean = kMeans.clusters[i].GetMean();
                colors[i] = Color.FromArgb((int)mean[0],(int)mean[1],(int)mean[2]);
            }

            for (int x = 0; x < map.GetLength(0); x++) {
                for (int y = 0; y < map.GetLength(1); y++) {
                    newImage.SetPixel(x,y,colors[map[x,y]]);
                }
            }

            newImage.Save(rootPath + name + " - " + random.Next() + ".png",ImageFormat.Png);
        }

        public void Run () {

            Bitmap bitmap = GetImage();

            int width = bitmap.Width;
            int height = bitmap.Height;

            double[][] image = ImageTools.BitmapToArray(bitmap);

            var kMeans = new Clustering.KMeans(20,6,0,255,image,SCDE);

            int[,] map = new int[width,height];

            int currentCluster = 0;

            foreach (var cluster in kMeans.clusters) {
                foreach (int index in cluster.indeces) {

                    int x = (int)Math.Floor(index / (double)bitmap.Height);
                    int y = index % bitmap.Height;

                    map[x,y] = currentCluster;
                }

                currentCluster++;
            }

            Save(FeatureExtraction.CellularAutomata(map,12,0,0,6),kMeans,"4-0-0-6");

            //map = FloodFill.Fill(map);

            /*
            foreach (var cluster in kMeans.clusters) {

                double[] mean = cluster.GetMean();
                Color color = Color.FromArgb((int)mean[0],(int)mean[1],(int)mean[2]);

                foreach (int index in cluster.indeces) {

                    int x = (int)Math.Floor(index / (double)bitmap.Height);
                    int y = index % bitmap.Height;

                    newImage.SetPixel(x,y,color);
                }
            }
            */
        }

        public double SCDE (double[] a,double[] b) {

            double r = (a[0] + b[0]) / 2.0;

            double R = a[0] - b[0];
            double G = a[1] - b[1];
            double B = a[2] - b[2];

            return (2.0 + (r / 256.0) * R * R + 4.0 * G * G + (2.0 + (255.0 - r) / 256.0) * B * B);
        }
    }
}

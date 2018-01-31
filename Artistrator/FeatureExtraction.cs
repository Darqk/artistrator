using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Artistrator.Drawing;

namespace Artistrator {

    public static class FeatureExtraction {

        public static double[,] EdgeDetection (double[,] im) {

            double[,] gradientX = Kernel.Apply(im,Kernel.sobelHorizontal3x3);
            double[,] gradientY = Kernel.Apply(im,Kernel.sobelVertical3x3);

            double[,] gradient = ImageTools.Combine(gradientX,gradientY,ImageTools.Operation.Mag);
            ImageTools.ApplyOperation(gradient,ImageTools.Operation.Div,Math.Sqrt(2.0));

            double[,] edges = NonMaxSuppression(gradient);

            return edges;
        }

        public static int[,] CannyEdgeDetection (double[,] im,double strongThreshold,double weakThreshold) {

            double[,] edges = EdgeDetection(im);

            int width = edges.GetLength(0);
            int height = edges.GetLength(1);

            int[,] lines = new int[width,height];

            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    if (edges[x,y] > strongThreshold)
                        lines[x,y] = 2;
                    else if (edges[x,y] > weakThreshold)
                        lines[x,y] = 1;
                    else
                        lines[x,y] = 0;
                }
            }

            return Hysteresis(lines);
        }

        public static int[,] Hysteresis (int[,] lines) {

            int width = lines.GetLength(0);
            int height = lines.GetLength(1);

            int[,] visited = new int[width,height];
            int[,] newLines = new int[width,height];

            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    if (lines[x,y] == 2) {
                        FloodFill(newLines,lines,visited,x,y);
                    }
                }
            }

            return newLines;
        }

        public static void FloodFill (int[,] newLines,int[,] lines,int[,] visited,int x,int y) {

            if (visited[x,y] == 1)
                return;

            visited[x,y] = 1;
            newLines[x,y] = 1;

            if (lines[x,y + 1] != 0)
                FloodFill(newLines,lines,visited,x,y + 1);
            if (lines[x + 1,y + 1] != 0)
                FloodFill(newLines,lines,visited,x + 1,y + 1);
            if (lines[x + 1,y] != 0)
                FloodFill(newLines,lines,visited,x + 1,y);
            if (lines[x + 1,y - 1] != 0)
                FloodFill(newLines,lines,visited,x + 1,y - 1);
            if (lines[x,y - 1] != 0)
                FloodFill(newLines,lines,visited,x,y - 1);
            if (lines[x - 1,y - 1] != 0)
                FloodFill(newLines,lines,visited,x - 1,y - 1);
            if (lines[x - 1,y] != 0)
                FloodFill(newLines,lines,visited,x - 1,y);
            if (lines[x - 1,y + 1] != 0)
                FloodFill(newLines,lines,visited,x - 1,y + 1);
        }

        public static double[,] NonMaxSuppression (double[,] im) {

            double[,] edges = new double[im.GetLength(0),im.GetLength(1)];

            for (int x = 1; x < im.GetLength(0) - 1; x++) {
                for (int y = 1; y < im.GetLength(1) - 1; y++) {
                    if (im[x,y] > im[x + 1,y] && im[x,y] > im[x - 1,y]) {
                        edges[x,y] = im[x,y];
                    }
                    else if (im[x,y] > im[x,y + 1] && im[x,y] > im[x,y - 1]) {
                        edges[x,y] = im[x,y];
                    }
                    else if (im[x,y] > im[x + 1,y + 1] && im[x,y] > im[x - 1,y - 1]) {
                        edges[x,y] = im[x,y];
                    }
                    else if (im[x,y] > im[x - 1,y + 1] && im[x,y] > im[x + 1,y - 1]) {
                        edges[x,y] = im[x,y];
                    }
                }
            }

            return edges;
        }

        public static double[,] Gradient (double[,] im) {

            double[,] gradientX = Kernel.Apply(im,Kernel.sobelHorizontal3x3);
            double[,] gradientY = Kernel.Apply(im,Kernel.sobelVertical3x3);

            double[,] gradient = ImageTools.Combine(gradientX,gradientY,ImageTools.Operation.Mag);
            ImageTools.ApplyOperation(gradient,ImageTools.Operation.Div,Math.Sqrt(2.0));

            return gradient;
        }

        public static int[,] CellularAutomata (int[,] map,int iterations,int minNeighbours,int minHeadStart,int values) {

            int newWidth = map.GetLength(0);
            int newHeight = map.GetLength(1);

            int[,] newMap = new int[newWidth,newHeight];

            int[] neighbourCount = new int[values];

            for (int i = 0; i < iterations; i++) {
                for (int x = 0; x < newWidth; x++) {
                    for (int y = 0; y < newHeight; y++) {

                        for (int c = 0; c < values; c++) {
                            neighbourCount[c] = 0;
                        }

                        for (int dx = -1; dx <= 1; dx++) {
                            for (int dy = -1; dy <= 1; dy++) {

                                if (dx == 0 && dy == 0)
                                    continue;

                                int neighbourX = x + dx;
                                int neighbourY = y + dy;

                                if (neighbourX >= 0 && neighbourX < newWidth) {
                                    if (neighbourY >= 0 && neighbourY < newHeight) {

                                        int val = map[neighbourX,neighbourY];
                                        neighbourCount[val] += 1;

                                        continue;
                                    }
                                }

                                int cent = map[x,y];
                                neighbourCount[cent] += 1;
                            }
                        }

                        int maxIndex = 0;

                        for (int c = 1; c < values; c++) {
                            if (neighbourCount[c] > neighbourCount[maxIndex]) {
                                maxIndex = c;
                            }
                        }

                        int secMaxIndex = maxIndex > 0 ? maxIndex - 1 : maxIndex + 1;

                        for (int c = 0; c < values; c++) {
                            if (c != maxIndex) {
                                if (neighbourCount[c] > neighbourCount[secMaxIndex]) {
                                    secMaxIndex = c;
                                }
                            }
                        }

                        int maxNeighbours = neighbourCount[maxIndex];
                        int secMaxNeighbours = neighbourCount[secMaxIndex];

                        if (maxNeighbours > minNeighbours && maxNeighbours - secMaxNeighbours > minHeadStart)
                            newMap[x,y] = maxIndex;
                        else
                            newMap[x,y] = map[x,y];
                    }
                }
            }

            return newMap;
        }

        public static List<Area> GetAreas (int[,] map) {

            return null;
        }
    }
}

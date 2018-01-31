using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Artistrator.Drawing;

namespace Artistrator {

    public static class FloodFill {

        public static int[,] Fill (int[,] map) {

            int width = map.GetLength(0);
            int height = map.GetLength(1);

            int[,] visited = new int[width,height];
            int[,] newMap = new int[width,height];

            int count = 1;

            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    if (visited[x,y] == 0) {
                        Flood(map,newMap,visited,count,x,y,width * height);
                        count++;
                    }
                }
            }

            return newMap;
        }

        private static void Flood (int[,] map,int[,] newMap,int[,] visited,int value,int x,int y,int capacity) {

            Queue<Point> queue = new Queue<Point>();

            queue.Enqueue(new Point(x,y));
            visited[x,y] = 1;

            while (queue.Count > 0) {

                Point p = queue.Dequeue();

                newMap[p.x,p.y] = value;
                visited[p.x,p.y] = 1;

                if (CanFlood(map,visited,p.x + 1,p.y + 0,map[p.x,p.y])) {
                    visited[p.x + 1,p.y] = 1;
                    queue.Enqueue(new Point(p.x + 1,p.y + 0));
                }
                if (CanFlood(map,visited,p.x - 1,p.y + 0,map[p.x,p.y])) {
                    visited[p.x - 1,p.y] = 1;
                    queue.Enqueue(new Point(p.x - 1,p.y + 0));
                }
                if (CanFlood(map,visited,p.x + 0,p.y + 1,map[p.x,p.y])) {
                    visited[p.x,p.y + 1] = 1;
                    queue.Enqueue(new Point(p.x + 0,p.y + 1));
                }
                if (CanFlood(map,visited,p.x + 0,p.y - 1,map[p.x,p.y])) {
                    visited[p.x,p.y - 1] = 1;
                    queue.Enqueue(new Point(p.x + 0,p.y - 1));
                }
            }

            queue.Clear();
        }

        private static bool CanFlood (int[,] map,int[,] visited,int x,int y,int value) {

            if (0 <= x && x < map.GetLength(0)) {
                if (0 <= y && y < map.GetLength(1)) {
                    if (visited[x,y] == 0 && map[x,y] == value) {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}

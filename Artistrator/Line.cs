using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Artistrator.Drawing {

    public class Line {

        public bool isEnclosed = false;

        public List<Point> points = new List<Point>();

        public int Length { get { return points.Count; } }

        public void AddPoint (int x,int y) {

            points.Add(new Point(x,y));
        }

        public double GetRoughness (int startPoint,int endPoint) {

            return 0.0;
        }

        public double GetHardness (int startPoint,int endPoint) {

            return 0.0;
        }
    }
}

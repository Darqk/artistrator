using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Artistrator {

    public class LayeredImage {

        public int width, height;
        private Dictionary<string,double[,]> layers = new Dictionary<string,double[,]>();

        public void AddLayer (string name,double[,] layer) {

            if (layer.GetLength(0) < width)
                width = layer.GetLength(0);
            if (layer.GetLength(1) < height)
                height = layer.GetLength(1);

            layers.Add(name,layer);
        }

        public double[,] GetLayer (string name) {

            return layers[name];
        }
    }
}

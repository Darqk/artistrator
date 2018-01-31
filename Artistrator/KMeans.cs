using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Artistrator.Clustering {

    public class KMeans {

        private static Random random = new Random();

        public int iterations, centroids, dimensions;
        public double minBound, maxBound;
        public double[][] points;
        public Cluster[] clusters;

        public delegate double HeuristicMethod (double[] a,double[] b);
        public HeuristicMethod Heuristic;

        public KMeans (int iterations,int centroids,double minBound,double maxBound,double[][] points) {

            this.iterations = iterations;
            this.centroids = centroids;
            this.dimensions = points[0].GetLength(0);

            this.minBound = minBound;
            this.maxBound = maxBound;

            this.points = points;

            Heuristic = DistanceSquared;

            DetermineClusters();
        }

        public KMeans (int iterations,int centroids,double minBound,double maxBound,double[][] points,HeuristicMethod Heuristic) {

            this.iterations = iterations;
            this.centroids = centroids;
            this.dimensions = points[0].GetLength(0);

            this.minBound = minBound;
            this.maxBound = maxBound;

            this.points = points;

            this.Heuristic = Heuristic;

            DetermineClusters();
        }

        public class Cluster {

            private double[][] points;

            public double[] centroid;
            public List<int> indeces;
            public int dimensions;

            public Cluster (double[][] points,int dimensions,double[] centroid) {

                this.points = points;
                indeces = new List<int>();
                this.dimensions = dimensions;
                this.centroid = centroid;
            }

            public void AddIndex (int i) {

                indeces.Add(i);
            }

            public int GetIndex (int i) {

                return indeces[i];
            }

            public double[] GetPoint (int i) {

                return points[indeces[i]];
            }

            public double[] GetPoint (double[][] points,int i) {

                return points[indeces[i]];
            }

            public void RecalculateCentroid () {

                centroid = GetMean();
            }

            public double[] GetMean () {

                double[] mean = new double[dimensions];

                if (indeces.Count == 0)
                    return mean;

                for (int i = 0; i < indeces.Count; i++) {

                    double[] point = GetPoint(i);

                    for (int d = 0; d < dimensions; d++) {
                        mean[d] += point[d];
                    }
                }

                for (int d = 0; d < dimensions; d++) {
                    mean[d] /= indeces.Count;
                }

                return mean;
            }

            public double[] GetMean (double[][] points) {

                double[] mean = new double[dimensions];

                if (indeces.Count == 0)
                    return mean;

                for (int i = 0; i < indeces.Count; i++) {

                    double[] point = GetPoint(points,i);

                    for (int d = 0; d < dimensions; d++) {
                        mean[d] += point[d];
                    }
                }

                for (int d = 0; d < dimensions; d++) {
                    mean[d] /= indeces.Count;
                }

                return mean;
            }

            public void Clear () {

                indeces.Clear();
            }
        }

        private void DetermineClusters () {

            clusters = CreateRandomClusters();

            for (int i = 0; i < iterations; i++) {

                //Resets all of the clusters
                for (int c = 0; c < clusters.Length; c++) {
                    clusters[c].Clear();
                }

                //Adds each point to their closest cluster
                for (int p = 0; p < points.Length; p++) {

                    Cluster closestCluster = GetClosestCluster(points[p]);
                    closestCluster.AddIndex(p);
                }

                //Repositions each cluster's centroid to the average position of all of the points under it
                for (int c = 0; c < clusters.Length; c++) {
                    clusters[c].RecalculateCentroid();
                }
            }
        }

        private Cluster GetClosestCluster (double[] point) {

            int closestCentroid = 0;
            double closestDist = Heuristic(clusters[0].centroid,point);

            for (int c = 1; c < clusters.Length; c++) {

                double dist = Heuristic(clusters[c].centroid,point);

                if (dist < closestDist) {

                    closestDist = dist;
                    closestCentroid = c;
                }
            }

            return clusters[closestCentroid];
        }

        private Cluster[] CreateRandomClusters () {

            Cluster[] clusters = new Cluster[centroids];

            for (int c = 0; c < centroids; c++) {
                clusters[c] = new Cluster(points,dimensions,GetRandomPosition());
            }

            return clusters;
        }

        private double[] GetRandomPosition () {

            //return points[random.Next() % points.Length];

            double[] position = new double[dimensions];

            for (int d = 0; d < dimensions; d++) {

                int randomIndex = random.Next() % points.Length;
                position[d] = points[randomIndex][d];

                //position[d] = random.NextDouble() * (maxBound - minBound) + minBound;
            }

            return position;
        }

        private static double DistanceSquared (double[] a,double[] b) {

            double distSqr = 0.0;

            for (int i = 0; i < a.Length; i++) {

                double displacement = a[i] - b[i];

                distSqr += displacement * displacement;
            }

            return distSqr;
        }
    }
}

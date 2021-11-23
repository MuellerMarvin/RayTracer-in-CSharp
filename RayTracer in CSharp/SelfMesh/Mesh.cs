using System;
using System.IO;
using System.Collections.Generic;

using Raytracing.Hittables;
using Raytracing.DataStructures;

namespace Raytracing.SelfMesh
{
    public class Mesh : IHittable
    {
        private Point3 _Origin = new Point3(0, 0, 0);
        public Point3 Origin {
            get
            {
                return _Origin;
            }
            set
            {
                _Origin = value;
                RejectionSphere.Origin = value;
            }
        }
        public Triangle[] Triangles { get; private set; }
        public Point3[] Vertices { get; private set; }
        public Materials.IMaterial Material { get; set; }
        public Sphere RejectionSphere { get; private set; }

        public Mesh(Triangle[] triangles, Point3[] vertices)
        {
            Triangles = triangles;
            Vertices = vertices;

            // Set rejectionsphere
            this.UpdateRejectionsphere();
        }

        public void UpdateRejectionsphere()
        {
            Sphere rs = new Sphere(this.Origin, 0, this.Material);
            this.Origin = rs.Origin;

            double maxLength = 0;

            for (int i = 0; i < this.Vertices.Length; i++)
            {
                if (((Vector3)this.Vertices[i]).Length > maxLength)
                {
                    maxLength = ((Vector3)this.Vertices[i]).Length;
                }
            }

            rs.Radius = maxLength;
        }

        public void OverwriteRejectionSphere(Sphere sphere)
        {
            sphere.Origin = this.Origin;
            RejectionSphere = sphere;
        }

        /// <summary>
        /// Create a mesh from a specified .obj filepath
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static Mesh FromObjFile(string filePath)
        {
            // read file
            StreamReader r = new(filePath);
            string fileContent = r.ReadToEnd();
            r.Close();
            r.Dispose();

            // split file into lines
            string[] lines = fileContent.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            // Set up variables
            List<Point3> Vertices = new();
            List<Vector3> normals = new();
            List<int[]> faceIndices = new();
            List<int> normalIndices = new();

            foreach (string line in lines)
            {
                string[] particles = line.Split(' ');


                switch (particles[0])
                {
                    case "v": // Vertices
                        Vertices.Add(new Point3(double.Parse(particles[1]), double.Parse(particles[2]), double.Parse(particles[3])));
                        break;
                    case "vt": // texture coordinates
                        break;
                    case "vn": // vertex normals
                        normals.Add(new Vector3(double.Parse(particles[1]), double.Parse(particles[2]), double.Parse(particles[3])));
                        break;
                    case "vp": // parameter space Vertices
                        break;
                    case "f": // faces
                        {
                            // if the face doesn't consist of 3 Vertices, ignore it
                            if (particles.Length != 4)
                                break;

                            int[] face = new int[3];
                            for (int i = 1; i < particles.Length; i++)
                            {
                                face[i - 1] = int.Parse(particles[i].Split('/')[0]) - 1; // lower the index by 1 to correct for the .obj waveform 1-based indexing
                            }
                            faceIndices.Add(face);

                            // add vertex-normal index
                            normalIndices.Add(int.Parse(particles[1].Split('/')[2]) - 1); // correct for 1-based index
                        }
                        break;
                    case "l": // lines
                        break;
                    default:
                        break;
                }
            }

            // resolve faces to triangles
            // no automatic triangulisation yet, as non-triangulized faces are auto-rejected on-interpretion above
            // todo: auto-triangulate n-gons into triangle faces
            Triangle[] triangles = new Triangle[faceIndices.Count];
            for (int i = 0; i < faceIndices.Count; i++)
            {
                Point3[] points = new Point3[3];

                // retrieve Vertices according to indexing in the file
                for (int j = 0; j < 3; j++)
                {
                    points[j] = Vertices[faceIndices[i][j]];
                }

                // build triangle
                triangles[i] = new Triangle(points[0], points[1], points[2], normals[normalIndices[i]]);
            }

            return new Mesh(triangles, Vertices.ToArray());
        }

        public bool Hit(Ray ray, double minDist, double maxDist, out HitRecord hitRecord)
        {
            bool hit = false;
            hitRecord = new();
            hitRecord.Distance = double.MaxValue;
            hitRecord.Material = this.Material;

            //if(this.RejectionSphere.Hit(ray, minDist, maxDist, out HitRecord rejectionRecord))
            //{
                for (int i = 0; i < Triangles.Length; i++)
                {
                    // hit it
                    if (Triangles[i].Hit(ray, minDist, maxDist, out HitRecord currentRecord, this.Origin))
                    {
                        // if it hit, say so
                        hit = true;

                        // if its the lowest distance yet, save it
                        if (currentRecord.Distance < hitRecord.Distance)
                            hitRecord = currentRecord;
                    }
                }
            //}
            return hit;
        }
    }
}
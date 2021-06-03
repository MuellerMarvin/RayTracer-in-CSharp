using Raytracing.DataStructures;
using System;

namespace Raytracing.Materials
{
    public class Dielectric : IMaterial
    {
        public double IndexOfRefraction { get; set; }
        public Color3 Color { get; set; } = new Color3(1, 1, 1);

        public Dielectric(double indexOfRefraction)
        {
            this.IndexOfRefraction = indexOfRefraction;
        }

        public Dielectric(Color3 color, double indexOfRefraction)
        {
            this.Color = color;
            this.IndexOfRefraction = indexOfRefraction;
        }

        public bool Scatter(Ray rayIn, HitRecord hitRecord, out Color3 colorAttenuation, out Ray scatteredRay)
        {
            colorAttenuation = this.Color;
            Vector3 unitDirection = rayIn.Direction.UnitVector;

            double refractionRatio = hitRecord.HitFront ? (1.0d / this.IndexOfRefraction) : this.IndexOfRefraction;
            double cosTheta = Math.Min(Vector3.Dot(-unitDirection, hitRecord.Normal), 1.0f);
            double sinTheta = Math.Sqrt(1.0 - Math.Pow(cosTheta, 2));

            if (refractionRatio * sinTheta > 1.0)
            {
                Console.WriteLine("owo");
                scatteredRay = new Ray(hitRecord.Point, Vector3.Reflect(rayIn.Direction, hitRecord.Normal));
            }
            else
            {
                Vector3 rayOutPerpendicular = refractionRatio * (unitDirection + cosTheta * hitRecord.Normal);
                Vector3 rayOutParallel = -Math.Sqrt(Math.Abs(1.0d - rayOutPerpendicular.LengthSquared)) * hitRecord.Normal;
                scatteredRay = new Ray(hitRecord.Point, rayOutPerpendicular + rayOutParallel);
            }

            return true;
        }

        #region Defaults
        public static Dielectric Glass {
            get
            {
                return new Dielectric(1.5);
            }
        }
        #endregion
    }
}

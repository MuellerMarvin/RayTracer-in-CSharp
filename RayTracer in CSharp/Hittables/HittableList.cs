using System.Collections.Generic;
using Raytracing.DataStructures;


namespace Raytracing.Hittables
{
    public class HittableList : List<IHittable>, IHittable
    {
        public bool Hit(Ray ray, double minDist, double maxDist, out HitRecord hitRecord)
        {
            hitRecord = new HitRecord();
            double closest = double.MaxValue;
            bool hit = false;

            for (int i = 0; i < this.Count; i++)
            {
                // create record and hit target
                if (this[i].Hit(ray, minDist, maxDist, out HitRecord currentRecord))
                { // if hit
                    hit = true;

                    if (currentRecord.Distance < closest) // if it's closer than a previous hit
                    {
                        hitRecord = currentRecord; // then save it's record
                        closest = currentRecord.Distance;
                    }
                }
                // else
            }
            return hit;
        }
    }
}

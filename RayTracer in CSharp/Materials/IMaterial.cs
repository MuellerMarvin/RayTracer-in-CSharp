using Raytracing.DataStructures;

public interface IMaterial
{
    public bool Scatter(Ray rayIn, HitRecord hitRecord, Color3 colorAttenuation, Ray scatteredRay);
}
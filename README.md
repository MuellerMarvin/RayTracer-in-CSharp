
# RayTracer-in-CSharp

![spheres](https://user-images.githubusercontent.com/8641639/119091756-d53fc700-ba0d-11eb-9301-0f7935d6b4f4.gif)

An implementation of a simple Raytracer in C#, loosely following the guidance of http://raytracing.github.io.

# Want to try it out ?

There is a simple demo that can be found in the releases section:
https://github.com/MuellerMarvin/RayTracer-in-CSharp/releases/tag/0.0.1

# What can it currently do ?

It can render simple spheres with a diffuse shader.

# How to compile ?

The repo consists of a VS2019 Solution that runs best with .NET 5 installed.
Just open it and given you have selected .NET 5 in the installer it should compile by itself.

# Which materials are there ?

### Lambertian Reflective ###
A standard diffuse material.


![spheres](https://user-images.githubusercontent.com/8641639/119091756-d53fc700-ba0d-11eb-9301-0f7935d6b4f4.gif)
(All spheres have this material with the color gray as albedo)

### Normal Reflector ###
An unrealistic that reflects every ray towards the surface-normal.
This results in a material where every point has the color of what it would 'see' from that perspective.

![ezgif-1-81741423ce70](https://user-images.githubusercontent.com/8641639/120057676-46523080-c045-11eb-96b2-696f461b3bc4.gif)
(Only the left sphere has the material, the others are Lambertian Diffusive)

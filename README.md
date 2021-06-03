# RayTracer-in-CSharp

https://user-images.githubusercontent.com/8641639/120634937-24551580-c46c-11eb-8a31-f1b866e2ff6b.mp4

An implementation of a simple Raytracer in C#, loosely following the guidance of http://raytracing.github.io.

# Want to try it out ?

There is a simple demo that can be found in the releases section:
https://github.com/MuellerMarvin/RayTracer-in-CSharp/releases/tag/0.0.1

# What can it currently do ?

It can render simple spheres with a number of different materials.
New materials can also be created in-code using the IMaterial interface.

# How to compile ?

The repo consists of a VS2019 Solution that runs best with .NET 5 installed.
Just open it and given you have selected .NET 5 in the installer it should compile by itself.

# Which materials are there ?

### Lambertian Reflective ###
A standard diffuse material.


![spheres](https://user-images.githubusercontent.com/8641639/119091756-d53fc700-ba0d-11eb-9301-0f7935d6b4f4.gif)

(All spheres have this material with the color gray as albedo)

### Metal ###
Metal with an adjustable level of roughness.

https://user-images.githubusercontent.com/8641639/120634937-24551580-c46c-11eb-8a31-f1b866e2ff6b.mp4

(The middle sphere is metal, the others Lambertian Diffusive)

### Normal Reflective ###
An unrealistic that reflects every ray towards the surface-normal.
This results in a material where every point has the color of what it would 'see' from that perspective.

![normalreflective](https://user-images.githubusercontent.com/8641639/120059884-8a98fd00-c054-11eb-8702-4cc7d776ad3f.gif)

(Only the left sphere has the material, the others are Lambertian Diffusive)




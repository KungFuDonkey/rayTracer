using System;
using System.IO;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using System.Collections.Generic;
namespace Template
{
	class MyApplication
	{
        // member variables
        public Surface screen;
        Vector3 viewPoint;
        Vector3 viewDirection;
        float screenDistance;
        ray[] rays;
        List<@object> objects = new List<@object>();
        List<lightsource> lightsources = new List<lightsource>();
        public void Init()
		{
            viewPoint = new Vector3(0f,0f,-1f);
            viewDirection = new Vector3(0f, 0f, 1f);
            screenDistance = 1f;

            objects.Add(new sphere(new Vector3(0, 0, 1), 0.5f, new Vector3(1, 1, 0)));
            objects.Add(new sphere(new Vector3(0.5f, 0.5f, 1), 0.3f, new Vector3(1, 0, 1)));
            //objects.Add(new pyramid(new Vector3(0, 0, 2), new Vector3(1, 1, 1), new Vector3(1, 0, 1), new Quaternion(rad(65), 0, 0)));
            objects.Add(new plane(3f, new Vector3(1, 1, 0), Quaternion.Identity));
            lightsources.Add(new lightsource(new Vector3(0, 2, -2), 400));

            rays = new ray[screen.width * screen.height];
            for(int y = 0; y < screen.height; y++)
            {
                for(int x = 0; x < screen.width; x++)
                {
                    Vector3 screenpoint = new Vector3(RX(x), RY(y), 0);
                    Vector3 direction = screenpoint - viewPoint;
                    direction.Normalize();
                    rays[x + y * screen.width] = new ray(screenpoint, direction);
                }
            }

            for (int y = 0; y < screen.height; y++)
            {
                for (int x = 0; x < screen.width; x++)
                {
                    ray ray = rays[x + y * screen.width];
                    for (int i = 0; i < objects.Count; ++i)
                    {
                        objects[i].rayIntersection(ray);
                    }
                    for (int i = 0; i < lightsources.Count; ++i)
                    {
                        lightsources[i].calcIntersection(ray, objects);
                    }
                    screen.pixels[x + y * screen.width] = uncharted2(ray.color,ray.illumination);
                }
            }

            /*
            int rayTracer = GL.CreateProgram();
            int computeShaderID = 0;
            int compute_shader = LoadShader("../../shaders/ray-tracing.glsl", ShaderType.ComputeShader, rayTracer, computeShaderID);
            GL.LinkProgram(rayTracer);
            */

        }
        // tick: renders one frame
        public void Tick()
        {
            //screen.Clear(0);
            //TODO: integrate screendistance and viewdirection so you can set a point anywhere on the grid
            //TODO: make sure rays cant see anything in front of the screen
            //TODO: move all code into /shaders/ray-tracing.glsl
            //      TODO: create a program that can read from the rayTracer program and fils the screen with quads that draw the image (vertexpoints -> geometryshader -> fractureshader better for memory)

            //TODO: create a light source and cast ray shadows
            //TODO: add more shapes to the scene than only 1 sphere
            //TODO: make sure shapes in the front are rendered first
            //TODO: create bounciness of light

            //TODO: add more different shapes to the scene (square, pyramid)
        }
        public float RX(int x)
        {
            float half = screen.width / 2;
            return (x - half) / half;
        }
        public float RY(int y)
        {
            float verthalf = screen.height / 2;
            float horzhalf = screen.width / 2;
            return (y-verthalf) / -horzhalf;
        }

        public float rad(float degree)
        {
            return (float)(degree * Math.PI / 180);
        }

        public void RenderGL()
        {
        }
        Vector3 luminanceValues = new Vector3(0.2126f, 0.7152f, 0.0722f);
        public int reinhard(Vector3 color, float illumination)
        {
            float factor = (float)Math.Pow(2, illumination);
            color *= factor;
            float l_old = Vector3.Dot(color, luminanceValues);
            float l_new = l_old / (l_old + 1);
            color *= (l_new / l_old);
            return ((int)(color.X * 255) << 16) + ((int)(color.Y * 255) << 8) + (int)(color.Z * 255);
        }
        public int reinhardJodie(Vector3 color, float illumination)
        {
            float factor = (float)Math.Pow(2, illumination);
            color *= factor;
            float l = Vector3.Dot(color, luminanceValues);
            Vector3 tv = colorDivision(color, floatAddition(1, color));
            color = Vector3.Lerp(color / (1.0f + l), tv, 0.69f);
            return ((int)(color.X * 255) << 16) + ((int)(color.Y * 255) << 8) + (int)(color.Z * 255);
        }
        public int uncharted2(Vector3 color, float illumination)
        {
            float factor = (float)Math.Pow(2, illumination);
            color *= factor;
            float exposure_bias = 2.0f;
            Vector3 curr = uncharted2_tonemap_partial(color * exposure_bias);

            Vector3 W = new Vector3(11.2f);
            Vector3 whiteScale = colorDivision(Vector3.One, uncharted2_tonemap_partial(W));
            color = curr * whiteScale;
            return ((int)(color.X * 255) << 16) + ((int)(color.Y * 255) << 8) + (int)(color.Z * 255);
        }
        public Vector3 uncharted2_tonemap_partial(Vector3 x)
        {
            return floatAddition(-(0.02f / 0.3f), colorDivision(floatAddition(0.004f, x * floatAddition(0.05f, 0.15f * x)), floatAddition(0.06f, x * floatAddition(0.5f, 0.15f * x))));
        }
        public Vector3 floatAddition(float f, Vector3 vec)
        {
            return new Vector3(vec.X + f, vec.Y + f, vec.Z + f);
        }
        public Vector3 colorDivision(Vector3 left, Vector3 right)
        {
            float x = left.X / right.X;
            float y = left.Y / right.Y;
            float z = left.Z / right.Z;
            return new Vector3(x, y, z);
        }
        void LoadShader(string name, ShaderType type, int program, out int ID)
        {
            ID = GL.CreateShader(type);
            using (StreamReader sr = new StreamReader(name))
                GL.ShaderSource(ID, sr.ReadToEnd());
            GL.CompileShader(ID);
            GL.AttachShader(program, ID);
            Console.WriteLine(GL.GetShaderInfoLog(ID));
        }
    }
}
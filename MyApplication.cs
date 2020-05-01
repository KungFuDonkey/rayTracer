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

            //objects.Add(new sphere(new Vector3(0, 0, 1), 0.5f, new Vector3(0, 1, 0)));
            //objects.Add(new sphere(new Vector3(0.5f, 0.5f, 1), 0.3f, new Vector3(0, 0, 1)));
            objects.Add(new plane(2f, new Vector3(1, 0, 0), new Vector3(0, 0, 1)));
            lightsources.Add(new lightsource(new Vector3(0, 1, 0), 75));

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
                    for (int i = 0; i < objects.Count; ++i)
                    {
                        objects[i].rayIntersection(rays[x + y * screen.width]);
                    }

                    for (int i = 0; i < lightsources.Count; ++i)
                    {
                        lightsources[i].calcIntersection(rays[x + y * screen.width], objects);
                    }

                    int r = Math.Min((int)(rays[x + y * screen.width].color.X * 255), 255);
                    int g = Math.Min((int)(rays[x + y * screen.width].color.Y * 255), 255);
                    int b = Math.Min((int)(rays[x + y * screen.width].color.Z * 255), 255);
                    screen.pixels[x + y * screen.width] = (r << 16) + (g << 8) + b;
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
        public void RenderGL()
        {
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
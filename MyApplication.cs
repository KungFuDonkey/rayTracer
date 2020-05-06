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
        float maxLight;
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
            //objects.Add(new pyramid(new Vector3(0, 0, 2), new Vector3(1, 1, 1), RGBtoHSL(new Vector3(1, 0, 1)), new Quaternion(rad(65), 0, 0)));
            objects.Add(new plane(2f, RGBtoHSL(new Vector3(1, 1, 0)), Quaternion.Identity));
            lightsources.Add(new lightsource(new Vector3(0, 1, 0), 60));

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
                    if(rays[x + y * screen.width].color.Z > maxLight)
                    {
                        maxLight = rays[x + y * screen.width].color.Z;
                    }
                }
            }
            for (int y = 0; y < screen.height; y++)
            {
                for (int x = 0; x < screen.width; x++)
                {
                    screen.pixels[x + y * screen.width] = HSLtoRGB(rays[x + y * screen.width].color);
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

        public Vector3 RGBtoHSL(Vector3 RGB)
        {
            Vector3 HSL = new Vector3();
            float min = Math.Min(RGB.X, Math.Min(RGB.Y, RGB.Z));
            float max = Math.Max(RGB.X, Math.Max(RGB.Y, RGB.Z));
            HSL.Z = (min + max) / 2;
            if(min == max)
            {
                HSL.Y = 0;
            }
            else if(HSL.Z < 0.5)
            {
                HSL.Y = (max - min) / (max + min);
            }
            else
            {
                HSL.Y = (max - min) / (2.0f - max - min);
            }

            if(max == RGB.X)
            {
                HSL.X = (RGB.Y - RGB.Z) / (max - min);
            }
            else if(max == RGB.Y)
            {
                HSL.X = 2.0f + (RGB.Z - RGB.X) / (max - min);
            }
            else if(max == RGB.Z)
            {
                HSL.X = 4.0f + (RGB.X - RGB.Y) / (max - min);
            }
            HSL.X *= 60;
            HSL.Z = 0;
            return HSL;
        }
        public int HSLtoRGB(Vector3 HSL)
        {
            float[] rgb = new float[3];
            if(HSL.Y == 0)
            {
                rgb[0] = HSL.Z * 255;
                return ((int)rgb[0] << 16) + ((int)rgb[0] << 8) + (int)rgb[0];
            }

            if(maxLight > 1)
            {
                HSL.Z = reinhardExtended(HSL.Z);
            }

            float temp1 = 0;
            if(HSL.Z < 0.5)
            {
                temp1 = HSL.Z * (1.0f + HSL.Y);
            }
            else
            {
                temp1 = HSL.Z + HSL.Y - HSL.Z * HSL.Y;
            }
            float temp2 = 2 * HSL.Z - temp1;
            float hue = (float)Math.Round(HSL.X / 360, 3);
            float[] temp = new float[3]
            {
                hue + 0.333f > 1 ? hue - 0.667f : hue + 0.333f,
                hue,
                hue - 0.333f < 0 ? hue + 0.667f : hue - 0.333f
            };
            for(int i = 0; i<3; ++i)
            {
                if(6 * temp[i] < 1)
                {
                    rgb[i] = temp2 + (temp1 - temp2) * 6 * temp[i];
                }
                else if(2 * temp[i] < 1)
                {
                    rgb[i] = temp1;
                }
                else if(3 * temp[i] < 2)
                {
                    rgb[i] = temp2 + (temp1 - temp2) * (0.666f - temp[i]) * 6;
                }
                else
                {
                    rgb[i] = temp2;
                }
                rgb[i] *= 255;
            }
            return ((int)rgb[0] << 16) + ((int)rgb[1] << 8) + (int)rgb[2];
        }

        public float clamp(float illumination)
        {
            if(illumination > 1)
                return 1f;
            return illumination;
        }

        public float reinhard(float illumination)
        {
            return illumination / (1 + illumination);
        }

        public float reinhardExtended(float illumination)
        {
            return illumination * (1.0f + illumination / (maxLight * maxLight)) / (1.0f + illumination);
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
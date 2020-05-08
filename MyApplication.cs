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

            objects.Add(new sphere(new Vector3(0, 0, 2), 0.5f, RGBToHSL(new Vector3(0, 1, 0)), 20));
            //objects.Add(new sphere(new Vector3(0, 1f, 1), 0.3f, RGBToHSL(new Vector3(0, 0, 1)), 0));
            //objects.Add(new pyramid(new Vector3(0, 0, 2), new Vector3(1, 1, 1), RGBToHSL(new Vector3(1, 0, 1)), new Quaternion(rad(60), 0, 0)));
            //objects.Add(new box(new Vector3(0, 0, 1.5f), new Vector3(1, 1, 1), RGBToHSL(new Vector3(1, 0, 0)), new Quaternion(rad(45), rad(45), 0), 0));
            //objects.Add(new triangle(new Vector3(-1, 1, 1), new Vector3(-1, 0, 1), new Vector3(1, 0, 1), RGBToHSL(new Vector3(0, 0, 1))));
            objects.Add(new plane(1f, RGBToHSL(new Vector3(1, 0, 0)), new Quaternion(0, rad(90), 0)));
            objects.Add(new plane(1f, RGBToHSL(new Vector3(1, 0, 1)), new Quaternion(0, rad(-90), 0)));
            objects.Add(new plane(1f, RGBToHSL(new Vector3(0, 0, 1)), new Quaternion(0, rad(180), 0)));
            lightsources.Add(new lightsource(new Vector3(0, 1, 0), 30));

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

            for(int i = 0; i < rays.Length; ++i)
            {
                rays[i].calculateColor(objects, lightsources);
                maxLight = rays[i].nextColor.Z > maxLight ? rays[i].nextColor.Z : maxLight;
            }

            for(int y = 0; y < screen.height; ++y)
            {
                for(int x = 0; x < screen.width; ++x)
                {
                    screen.pixels[x + y * screen.width] = HSLToRGB(rays[x + y * screen.width].color);
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
        void LoadShader(string name, ShaderType type, int program, out int ID)
        {
            ID = GL.CreateShader(type);
            using (StreamReader sr = new StreamReader(name))
                GL.ShaderSource(ID, sr.ReadToEnd());
            GL.CompileShader(ID);
            GL.AttachShader(program, ID);
            Console.WriteLine(GL.GetShaderInfoLog(ID));
        }

        Vector3 RGBToHSL(Vector3 RGB)
        {
            Vector3 HSL = new Vector3();
            float min = Math.Min(RGB.X, Math.Min(RGB.Y, RGB.Z));
            float max = Math.Max(RGB.X, Math.Max(RGB.Y, RGB.Z));
            HSL.Z = (min + max) / 2;

            if (min == max)
            {
                HSL.Y = 0;
            }
            else if (HSL.Z < 0.5)
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
            else
            {
                HSL.X = 4.0f + (RGB.X - RGB.Y) / (max - min);
            }

            HSL.X *= 60;
            HSL.X = HSL.X < 0 ? HSL.X + 360 : HSL.X;
            HSL.Z = 0;
            return HSL;
        }

        int HSLToRGB(Vector3 HSL)
        {
            float[] RGB = new float[3];

            if(HSL.Y == 0)
            {
                RGB[0] = HSL.Z * 255;
                return ((int)RGB[0] << 16) + ((int)RGB[0] << 8) + (int)RGB[0];
            }

            if(maxLight > 1)
            {
                HSL.Z = HSL.Z * (1.0f + HSL.Z / (maxLight * maxLight)) / (1.0f + HSL.Z);
            }

            float temp1 = HSL.Z < 0.5 ? HSL.Z * (1.0f + HSL.Y) : HSL.Z + HSL.Y - HSL.Z * HSL.Y;
            float temp2 = 2 * HSL.Z - temp1;
            float hue = (float)Math.Round(HSL.X / 360, 3);
            RGB[0] = hue + 0.333f > 1 ? hue - 0.667f : hue + 0.333f;
            RGB[1] = hue;
            RGB[2] = hue - 0.333f < 0 ? hue + 0.667f : hue - 0.333f;

            for(int i = 0; i < 3; ++i)
            {
                if(6 * RGB[i] < 1)
                {
                    RGB[i] = temp2 + (temp1 - temp2) * 6 * RGB[i];
                }
                else if(2 * RGB[i] < 1)
                {
                    RGB[i] = temp1;
                }
                else if(3 * RGB[i] < 2)
                {
                    RGB[i] = temp2 + (temp1 - temp2) * (0.666f - RGB[i]) * 6;
                }
                else
                {
                    RGB[i] = temp2;
                }

                RGB[i] *= 255;
            }

            return ((int)RGB[0] << 16) + ((int)RGB[1] << 8) + (int)RGB[2];
        }
    }
}
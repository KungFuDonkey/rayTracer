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
            obj obj = new obj("../../shapes/monkey.obj");
            viewPoint = new Vector3(0f,0f,-1f);
            viewDirection = new Vector3(0f, 0f, 1f);
            screenDistance = 1f;


            objects.Add(new mesh(obj, new Vector3(0, 0, 2), new Vector3(1,1,1), new Quaternion(0,rad(180),0), 0.5f));
            //objects.Add(new sphere(new Vector3(0, 0, 2), 0.5f, new Vector3(1, 1, 1), 0));
            //objects.Add(new sphere(new Vector3(0, 1f, 1), 0.3f, RGBToHSL(new Vector3(0, 0, 1)), 0));
            //objects.Add(new pyramid(new Vector3(0, 0, 2), new Vector3(1, 1, 1), RGBToHSL(new Vector3(1, 0, 1)), new Quaternion(rad(60), 0, 0)));
            //objects.Add(new box(new Vector3(0, 0, 1.5f), new Vector3(1, 1, 1), new Vector3(1, 0, 0), new Quaternion(rad(45), rad(45), 0), 0));
            //objects.Add(new triangle(new Vector3(-1, 1, 1), new Vector3(-1, 0, 1), new Vector3(1, 0, 1), RGBToHSL(new Vector3(0, 0, 1))));
            objects.Add(new plane(1f, new Vector3(1, 0, 0), new Quaternion(0, rad(90), 0), 20));
            objects.Add(new plane(1f, new Vector3(0, 1, 0), new Quaternion(0, rad(-90), 0), 20));
            objects.Add(new plane(1f, new Vector3(0, 0, 1), new Quaternion(0, rad(180), 0), 20));
            lightsources.Add(new arealight(80, new box(new Vector3(0, 1, 0), new Vector3(0.2f, 0.2f, 0.2f), new Vector3(1, 1, 1), Quaternion.Identity)));

            rays = new ray[screen.width * screen.height];
            for(int y = 0; y < screen.height; y++)
            {
                for(int x = 0; x < screen.width; x++)
                {
                    Vector3 screenpoint = new Vector3(RX(x), RY(y), 0);
                    Vector3 direction = screenpoint - viewPoint;
                    direction.Normalize();
                    ray ray = new ray(screenpoint, direction);
                    ray.calculateColor(objects, lightsources, 0);
                    screen.pixels[x + y * screen.width] = ray.result;
                }
                Console.WriteLine(y);
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
    }
}
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
        int VBO;
        int VCOL;
        float[] pixels;
        float[] pixelPositions;
        int drawProgram;
        int attribute_vcol;
        int attribute_vpos;
        public void Init()
		{
            viewPoint = new Vector3(0f,0f,-1f);
            viewDirection = new Vector3(0f, 0f, 1f);
            screenDistance = 1f;
            objects.Add(new sphere(new Vector3(0, 0, 1), 0.5f, 0x00ff00));
            objects.Add(new sphere(new Vector3(0.5f, 0.5f, 1), 0.3f, 0x0000ff));
            objects.Add(new plane(new Vector3(0, 0f, 2f),new Vector2(0.5f,0.5f), 0xff0000, new Vector3(0, 0, 1f)));
            lightsources.Add(new lightsource(new Vector3(-1, 0, -0.4f)));
            rays = new ray[screen.width * screen.height];
            pixelPositions = new float[screen.width * screen.height * 2];
            pixels = new float[screen.width * screen.height * 3];
            int posindex = 0;
            int colindex = 0;
            for(int y = 0; y < screen.height; y++)
            {
                for(int x = 0; x < screen.width; x++)
                {
                    Vector3 screenpoint = new Vector3(RX(x), RY(y), 0);
                    Vector3 direction = screenpoint - viewPoint;
                    direction.Normalize();
                    rays[x + y * screen.width] = new ray(screenpoint, direction);
                    pixelPositions[posindex] = RX(x);
                    pixelPositions[posindex + 1] = TY(y);
                    posindex += 2;
                    pixels[colindex] = 0;
                    pixels[colindex + 1] = 0;
                    pixels[colindex + 2] = 0;
                }
            }
            int vsID;
            int fsID;
            drawProgram = GL.CreateProgram();
            LoadShader("../../shaders/vs.glsl", ShaderType.VertexShader, drawProgram, out vsID);
            LoadShader("../../shaders/fs.glsl", ShaderType.FragmentShader, drawProgram, out fsID);
            GL.LinkProgram(drawProgram);
            attribute_vpos = GL.GetAttribLocation(drawProgram, "vPosition");
            attribute_vcol = GL.GetAttribLocation(drawProgram, "vColor");
            VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, pixelPositions.Length * 4, pixelPositions, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(attribute_vpos, 2, VertexAttribPointerType.Float, false, 0, 0);
            VCOL = GL.GenBuffer();
            
        }
        // tick: renders one frame
        public void Tick()
        {
            screen.Clear(0);
            int colindex = 0;
            for (int y = 0; y < screen.height; y++)
            {
                for(int x = 0; x < screen.width; x++)
                {
                    for(int i = 0; i < objects.Count; ++i)
                    {
                        objects[i].rayIntersection(rays[x + y * screen.width]);
                    }
                    for(int i = 0; i < lightsources.Count; ++i)
                    {
                        lightsources[i].calcIntersection(rays[x + y * screen.width], objects);
                    }
                    screen.pixels[x + y * screen.width] = rays[x + y * screen.width].color;
                    pixels[colindex] = (rays[x + y * screen.width].color >> 16) / 255;
                    pixels[colindex + 1] = (rays[x + y * screen.width].color << 8 >> 16) / 255;
                    pixels[colindex + 2] = (rays[x + y * screen.width].color << 16 >> 16) / 255;
                    colindex += 3;
                }
            }
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
        public float TY(int y)
        {
            float verthalf = screen.height / 2;
            return (y - verthalf) / -verthalf;
        }
        public void RenderGL()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, VCOL);
            GL.BufferData(BufferTarget.ArrayBuffer, pixels.Length * 4, pixels, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(attribute_vcol, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.UseProgram(drawProgram);
            GL.EnableVertexAttribArray(attribute_vpos);
            GL.EnableVertexAttribArray(attribute_vcol);
            GL.DrawArrays(PrimitiveType.Points, 0, screen.width * screen.height);
            GL.UseProgram(0);
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
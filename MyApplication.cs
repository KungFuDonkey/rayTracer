using System;
using System.IO;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using System.Collections.Generic;
namespace Template
{
	class MyApplication
	{
        public Surface screen;
        int ray_program;
        int tex_w;
        int tex_h;
        uint tex_output;

        float[] spheres;
        float[] planes;
        float[] triangles;
        float[] areaLightsources;
        int[] count;
        float[] vertices;
        int attributeSpheres;
        int attributeAreaLightsources;
        int attributeCount;
        float[] colors;
        int attributeColor;
        int attributePlane;
        int attributeTriangle;
        int attributeVertices;


        // initialize
        public void Init()
        {
            
            //dimensions of the image
            tex_w = screen.width;
            tex_h = screen.height;

            //OpenGL settings for the quad texture
            GL.GenTextures(1, out tex_output);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, tex_output);
            GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, new int[] { (int)All.ClampToEdge });
            GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, new int[] { (int)All.ClampToEdge });
            GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, new int[] { (int)All.Linear });
            GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, new int[] { (int)All.Linear });
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba32f, tex_w, tex_h, 0, PixelFormat.Rgba, PixelType.Float, IntPtr.Zero);

            //set the acces to write only for the compute shader
            GL.BindImageTexture(0, tex_output, 0, false, 0, TextureAccess.WriteOnly, SizedInternalFormat.Rgba32f);

            //create compute shader program
            ray_program = GL.CreateProgram();

            int ray_shader = 0;
            LoadShader("../../shaders/cs.glsl", ShaderType.ComputeShader, ray_program, out ray_shader);
            GL.LinkProgram(ray_program);
            int length;
            GetProgramParameterName par = (GetProgramParameterName)All.ProgramBinaryLength;
            GL.GetProgram(ray_program, par, out length);
            byte[] bytes = new byte[length];
            int binaryShaderLength;
            BinaryFormat binaryFormat;
            GL.GetProgramBinary(ray_program, length, out binaryShaderLength, out binaryFormat, bytes);
            string[] lines = new string[1] { length.ToString() };
            File.WriteAllLines("../../shaders/cs.inf", lines);
            lines = new string[length];
            for (int i = 0; i < length; i++)
            {
                lines[i] = bytes[i].ToString();
            }
            File.WriteAllLines("../../shaders/cs.bin", lines);

            //get arrays of the shader
            attributeCount = GL.GetUniformLocation(ray_program, "count");
            attributeSpheres = GL.GetUniformLocation(ray_program, "spheres");
            attributeAreaLightsources = GL.GetUniformLocation(ray_program, "areaLightsources");
            attributeColor = GL.GetUniformLocation(ray_program, "colors");
            attributePlane = GL.GetUniformLocation(ray_program, "planes");
            attributeTriangle = GL.GetUniformLocation(ray_program, "triangles");
            attributeVertices = GL.GetUniformLocation(ray_program, "vertices");
            //create objects for the scene
            List<arealight> arealights = new List<arealight>();
            arealights.Add(new arealight(10, new sphere(new Vector3(0, 1, 0), 0.2f, 0)));

            List<sphere> sphere = new List<sphere>();
            sphere.Add(new sphere(new Vector3(0, 0, 2), 1f, 3));

            List<plane> plane = new List<plane>();
            //plane.Add(new plane(1f, 3, new Quaternion(0, rad(90), 0)));

            List<shapes> shape = new List<shapes>();
            //shape.Add(new box(new Vector3(0, 0, 2), new Vector3(0.5f, 0.2f, 1), 4, Quaternion.Identity));

            //add the objects to float arrays
            List<float> floats = new List<float>();
            foreach(sphere s in sphere)
            {
                s.AddToArray(ref floats);
            }
            spheres = floats.ToArray();

            floats = new List<float>();
            foreach(arealight l in arealights)
            {
                l.AddToArray(ref floats);
            }
            areaLightsources = floats.ToArray();

            floats = new List<float>();
            foreach(plane p in plane)
            {
                p.AddToArray(ref floats);
            }
            planes = floats.ToArray();

            floats = new List<float>();
            List<float> tr = new List<float>();
            foreach(shapes s in shape)
            {
                s.AddToArray(ref floats);
                foreach(triangle t in s.shape)
                {
                    t.AddToArray(ref tr);
                }
            }
            
            triangles = tr.ToArray();
            vertices = floats.ToArray();
            vertices = new float[9] { -1, -1, 1, 1, -1, 1, 1, 1, 1 };
            triangles = new float[5] { 0, 2, 1, 1, 1 };
            //count for the shader loops
            count = new int[4] { sphere.Count, plane.Count, triangles.Length / 5, arealights.Count };

            //colorpalette for scene
            colors = new float[60]
            {
                1, 1, 1, //white
                1, 0, 0, //red
                0, 1, 0, //green
                0, 0, 1, //blue
                1, 1, 0, //yellow
                0, 1, 1, //brown
                1, 0, 1, //purple
                0, 0, 0, 
                0, 0, 0,
                0, 0, 0,
                0, 0, 0,
                0, 0, 0,
                0, 0, 0,
                0, 0, 0,
                0, 0, 0,
                0, 0, 0,
                0, 0, 0,
                0, 0, 0,
                0, 0, 0,
                0, 0, 0 //black
            };
        }
        // tick: renders one frame
        public void Tick()
        {
            //bind arrays to compute shader
            GL.Uniform1(attributeCount, count.Length, count);
            GL.Uniform1(attributeSpheres, spheres.Length, spheres);
            GL.Uniform1(attributeAreaLightsources, areaLightsources.Length, areaLightsources);
            GL.Uniform1(attributeColor, colors.Length, colors);
            GL.Uniform1(attributePlane, planes.Length, planes);
            GL.Uniform1(attributeTriangle, triangles.Length, triangles);
            GL.Uniform1(attributeVertices, vertices.Length, vertices);

            //run compute shader
            GL.UseProgram(ray_program);
            GL.DispatchCompute(tex_w, tex_h, 1);

            //wait for all pixels to render
            GL.MemoryBarrier(MemoryBarrierFlags.ShaderImageAccessBarrierBit);

            //bind new image to screen filling quad
            GL.Enable(EnableCap.Texture2D);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.BindTexture(TextureTarget.Texture2D, tex_output);
            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(0, 1); GL.Vertex2(-1, 1);
            GL.TexCoord2(1, 1); GL.Vertex2(1, 1);
            GL.TexCoord2(1, 0); GL.Vertex2(1, -1);
            GL.TexCoord2(0, 0); GL.Vertex2(-1, -1);
            GL.End();
            GL.Disable(EnableCap.Texture2D);
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
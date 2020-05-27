using System;
using System.IO;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using OpenTK.Input;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
namespace Template
{
	class MyApplication
	{
        public Surface screen;
        int ray_program;
        int tex_w;
        int tex_h;
        uint tex_output;

        List<sphere> sphere;
        List<plane> plane;
        List<shapes> shape;
        List<arealight> arealights;
        List<directionalLight> directionalLights;

        float[] spheres;
        float[] planes;
        float[] areaLightsources;
        float[] directionalLightsources;
        float[] vertices;
        float[] colors;

        int sphereLength;
        int planeLength;
        int areaLightsourcesLength;
        int directionalLightsourcesLength;
        int verticeLength;
        int attributeSpheres;
        int attributeAreaLightsources;
        int attributeDirectionalLightsources;
        int attributeColor;
        int attributePlane;
        int attributeVertices;

        Vector3 moveDirection;
        Quaternion rotation;
        Stopwatch gameTime;

        //Initialize
        public void Init()
        {
            //obj monkey = new obj("../../shapes/monkey.obj");
            //obj cube = new obj("../../shapes/block.obj");

            //Dimensions of the image
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

            //Set the acces to write only for the compute shader
            GL.BindImageTexture(0, tex_output, 0, false, 0, TextureAccess.WriteOnly, SizedInternalFormat.Rgba32f);

            //Set the colors used in the scene
            colors = new float[60]
            {
                1, 1, 1, //white
                1, 0, 0, //red
                0, 1, 0, //green
                0, 0, 1, //blue
                1, 1, 0, //yellow
                0, 1, 1, //turqoise
                1, 0, 1, //purple
                0.5f, 0.5f, 0.5f, //gray 
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

            //Create objects for the scene
            arealights = new List<arealight>();
            //arealights.Add(new arealight(1, new sphere(new Vector3(0, 1, 0), 0.2f, 0)));

            directionalLights = new List<directionalLight>();
            directionalLights.Add(new directionalLight(new Vector3(0,1,0), 0, 0.2f));

            sphere = new List<sphere>();
            //sphere.Add(new sphere(new Vector3(0, 0, 1), 0.5f, 0, 1f, 1.517f)) ;
            sphere.Add(new sphere(new Vector3(0, 0, 2), 0.5f, 3));

            plane = new List<plane>();
            //plane.Add(new plane(2f, 0, new Quaternion(0, rad(90), 0), 0.7f));
            //plane.Add(new plane(2f, 0, new Quaternion(0, rad(-90), 0), 0.7f));
            //plane.Add(new plane(3f, 0, Quaternion.Identity, 0.7f));

            shape = new List<shapes>();
            //shape.Add(new mesh(monkey, new Vector3(0,0,2), 3, Quaternion.Identity, 1));
            //shape.Add(new mesh(cube, new Vector3(0,0,2), 3, Quaternion.Identity, 1));
            //shape.Add(new box(new Vector3(0, 0, 2), new Vector3(1, 1, 1), 3, Quaternion.Identity));

            //Read base shader
            List<string> lines = new List<string>() {
                File.ReadAllText("../../shaders/ray-tracing-base.glsl")
            };

            //Add the objects to float arrays and build GLSL functions for the objects
            StringBuilder normal = new StringBuilder("void calcObjects(vec3 ray_origin, vec3 ray_direction, inout float t, inout vec3 col, inout float absorption, inout float refraction, inout vec3 normal){\n");
            StringBuilder faster = new StringBuilder("bool calcObjects(vec3 ray_origin, vec3 ray_direction, float tmax){\n");
            normal.AppendLine("    float d;");
            normal.AppendLine("    float discriminant;");
            normal.AppendLine("    float s;");
            faster.AppendLine("    float d;");
            faster.AppendLine("    float discriminant;");
            faster.AppendLine("    float s;");

            List<float> floats = new List<float>();
            foreach(sphere s in sphere)
            {
                s.AddToArray(floats, colors, normal, faster);
            }
            spheres = floats.ToArray();
            sphereLength = spheres.Length / 3;

            floats = new List<float>();
            foreach(plane p in plane)
            {
                p.AddToArray(floats, colors, normal);
            }
            planes = floats.ToArray();
            planeLength = planes.Length / 4;

            normal.AppendLine("    vec3 object_position;");
            faster.AppendLine("    vec3 object_position;");

            floats = new List<float>();
            List<float> tr = new List<float>();
            foreach(shapes s in shape)
            {
                s.AddToArray(floats);
                foreach(triangle t in s.shape)
                {
                    t.AddToArray(colors, normal, faster);
                }
            }
            vertices = floats.ToArray();
            verticeLength = vertices.Length / 3;

            faster.AppendLine("    return false;");
            faster.AppendLine("}");
            lines.Add(faster.ToString());

            faster = new StringBuilder("void calcAreaLightSources(vec3 ray_origin, float absorption, vec3 normal){\n");
            faster.AppendLine("    float lightsource_emittance;");
            faster.AppendLine("    vec3 light_direction;");
            faster.AppendLine("    float tmax;");
            faster.AppendLine("    float angle;");
            faster.AppendLine("    bool collision;");
            faster.AppendLine("    vec3 object_color;");
            faster.AppendLine("    vec3 point_of_intersection;");

            floats = new List<float>();
            foreach (arealight l in arealights)
            {
                l.AddToArray(floats,colors, normal, faster);
            }
            areaLightsources = floats.ToArray();
            areaLightsourcesLength = areaLightsources.Length / 3;

            foreach (directionalLight d in directionalLights)
            {
                d.AddToArray(floats, colors, normal, faster);
            }
            directionalLightsources = floats.ToArray();
            directionalLightsourcesLength = directionalLightsources.Length / 3;

            skydome skye = new skydome();
            skye.AddToArray(normal);
            normal.AppendLine("}");
            faster.AppendLine("}");

            lines.Add(normal.ToString());
            lines.Add(faster.ToString());

            //Produce new shader
            File.WriteAllLines("../../shaders/ray-tracing-full.glsl", lines);

            //Create compute shader program
            ray_program = GL.CreateProgram();

            int ray_shader = 0;
            LoadShader("../../shaders/ray-tracing-full.glsl", ShaderType.ComputeShader, ray_program, out ray_shader);
            GL.LinkProgram(ray_program);

            //Get arrays of the shader
            attributeSpheres = GL.GetUniformLocation(ray_program, "spheres");
            attributeAreaLightsources = GL.GetUniformLocation(ray_program, "areaLightsources");
            attributeDirectionalLightsources = GL.GetUniformLocation(ray_program, "directionalLightsources");
            attributeColor = GL.GetUniformLocation(ray_program, "colors");
            attributePlane = GL.GetUniformLocation(ray_program, "planes");
            attributeVertices = GL.GetUniformLocation(ray_program, "vertices");

            moveDirection = new Vector3(0);
            gameTime = new Stopwatch();
            gameTime.Start();
        }

        //Render one frame
        public void Tick()
        {
            //Bind arrays to compute shader
            GL.Uniform3(attributeSpheres, sphereLength, spheres);
            GL.Uniform3(attributeAreaLightsources, areaLightsourcesLength, areaLightsources);
            GL.Uniform3(attributeDirectionalLightsources, directionalLightsourcesLength, directionalLightsources);
            GL.Uniform4(attributePlane, planeLength, planes);
            GL.Uniform3(attributeVertices, verticeLength, vertices);
            GL.Uniform3(attributeColor, 20, colors);

            //Run compute shader
            GL.UseProgram(ray_program);
            GL.DispatchCompute(tex_w, tex_h, 1);

            //Wait for all pixels to render
            GL.MemoryBarrier(MemoryBarrierFlags.ShaderImageAccessBarrierBit);

            //Bind new image to screen filling quad
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

            //Movement for the player
            KeyboardState keys = Keyboard.GetState();
            moveDirection = Vector3.Zero;

            if (keys[Key.Up])
                moveDirection.Z = 1;
            else if (keys[Key.Down])
                moveDirection.Z = -1;
            if (keys[Key.Left])
                moveDirection.X = -1;
            else if (keys[Key.Right])
                moveDirection.X = 1;

            rotation = Quaternion.Identity;

            if (keys[Key.A])
                rotation *= new Quaternion(0, rad(1), 0);
            if (keys[Key.D])
                rotation *= new Quaternion(0, rad(-1), 0);
            if (keys[Key.W])
                rotation *= new Quaternion(rad(1), 0, 0);
            if (keys[Key.S])
                rotation *= new Quaternion(rad(-1), 0, 0);

            foreach (sphere s in sphere)
            {
                s.move((float)gameTime.Elapsed.TotalSeconds * moveDirection, spheres);
                s.rotate(rotation, spheres);
            }
            foreach (plane p in plane)
            {
                p.move((float)gameTime.Elapsed.TotalSeconds * moveDirection, planes);
                p.rotate(rotation, planes);
            }
            foreach (shapes s in shape)
            {
                s.move((float)gameTime.Elapsed.TotalSeconds * moveDirection, vertices);
                s.rotate(rotation, vertices);
            }
            foreach (arealight a in arealights)
            {
                a.move((float)gameTime.Elapsed.TotalSeconds * moveDirection, areaLightsources);
                a.rotate(rotation, areaLightsources);
            }
            foreach(directionalLight d in directionalLights)
            {
                d.rotate(rotation, directionalLightsources);
            }

            gameTime.Restart();
        }

        //Change degrees to radians
        public float rad(float degree)
        {
            return (float)(degree * Math.PI / 180);
        }

        //Load shader
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
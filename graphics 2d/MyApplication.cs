using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Template
{
	class MyApplication
	{
		//Member variables
		public Surface screen;
        int ray_program;
        int tex_w;
        int tex_h;
        uint tex_output;

        float[] circles;
        float[] lightsources;
        float[] colors;
        float[] boxes;

        int attributeCircles;
        int attributeLightsources;
        int attributeColors;
        int attributeBoxes;
        int boxesLength;
        int circlesLength;
        int colorsLength;
        int lightsourcesLength;

        //Initialize
        public void Init()
		{
            tex_w = screen.width;
            tex_h = screen.height;

            //Dimensions of the image
            GL.GenTextures(1, out tex_output);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, tex_output);
            GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, new int[] { (int)All.ClampToEdge });
            GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, new int[] { (int)All.ClampToEdge });
            GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, new int[] { (int)All.Linear });
            GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, new int[] { (int)All.Linear });
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba32f, tex_w, tex_h, 0, PixelFormat.Rgba, PixelType.Float, IntPtr.Zero);
            GL.BindImageTexture(0, tex_output, 0, false, 0, TextureAccess.WriteOnly, SizedInternalFormat.Rgba32f);

            //Objects
            circles = new float[6] { 0.5f, -0.2f, 0.05f, -0.4f, 0.5f, 0.05f};
            boxes = new float[8] { -0.4f, -0.2f, -0.4f, -0.4f, -0.2f, -0.4f, -0.2f, -0.2f };
            lightsources = new float[4] { -1, 0, 1, 0};
            colors = new float[6] { 1, 1, 1, 1, 0, 0};
            circlesLength = circles.Length / 3;
            boxesLength = boxes.Length / 2;
            colorsLength = colors.Length / 3;
            lightsourcesLength = lightsources.Length / 2;

            //Read basic shader
            List<string> lines = new List<string>()
            {
                File.ReadAllText("../../shaders/cs.glsl")
            };

            //Create shader functions
            StringBuilder normal = new StringBuilder("bool hitObjects(vec2 ray_o, vec2 ray_d, float tmax){\n");
            normal.AppendLine("    float b;");
            normal.AppendLine("    float s;");
            normal.AppendLine("    float discriminant;");

            for (int i = 0; i < circles.Length / 3; ++i)
            {
                normal.AppendLine("    b = 2.0 * dot(ray_o - circles[" + i + "].xy, ray_d);");
                normal.AppendLine("    discriminant = b * b - 4 * (dot(ray_o - circles[" + i + "].xy, ray_o - circles[" + i + "].xy) - circles[" + i + "].z * circles[" + i + "].z);");
                normal.AppendLine("    if(discriminant >= 0){");
                normal.AppendLine("       s = (-b - sqrt(discriminant)) / 2;");
                normal.AppendLine("       s = s < 0 ? (-b + sqrt(discriminant)) / 2 : s;");
                normal.AppendLine("       if(s >= 0 && s < tmax) return true;");
                normal.AppendLine("    }");
            }

            normal.AppendLine("    vec2 line;");

            for(int i = 0; i < boxes.Length / 8; ++i)
            {
                for (int j = 0; j < 4; ++j)
                {
                    normal.AppendLine("    line = normalize(boxes[" + (i * 8 + (j + 1 > 3 ? 0 : j + 1)) + "] - boxes[" + (i * 8 + j) + "]);");
                    normal.AppendLine("    b = distance(boxes[" + (i * 8 + j) + "], boxes[" + (i * 8 + (j + 1 > 3 ? 0 : j + 1)) + "]);");
                    normal.AppendLine("    s = (ray_o.y * ray_d.x + ray_d.y * boxes[" + (i * 8 + j) + "].x - ray_d.y * ray_o.x - boxes[" + (i * 8 + j) + "].y * ray_d.x)/(ray_d.x * line.y - ray_d.y * line.x);");
                    normal.AppendLine("    if(s < b && s > 0){");
                    normal.AppendLine("       s = (boxes[" + (i * 8 + j) + "].x - ray_o.x + line.x * s) / ray_d.x;");
                    normal.AppendLine("       if(s >= 0 && s < tmax) return true;");
                    normal.AppendLine("    }");
                }
            }

            normal.AppendLine("    return false;");
            normal.AppendLine("}");
            normal.AppendLine("void lightsource(vec2 ray_o, inout vec4 pixel){");
            normal.AppendLine("    vec2 lightray;");
            normal.AppendLine("    float luminance;");
            normal.AppendLine("    float tmax;");

            for (int i = 0; i < lightsources.Length / 2; ++i)
            {
                normal.AppendLine("    lightray = lightsources[" + i + "] - ray_o ;");
                normal.AppendLine("    luminance = 1 / (length(lightray) * length(lightray));");
                normal.AppendLine("    tmax = length(lightray) - 0.0001;");
                normal.AppendLine("    lightray = normalize(lightray);");
                normal.AppendLine("    if(!hitObjects(ray_o, lightray, tmax))");
                normal.AppendLine("       pixel += vec4(colors[" + i + "], 0) * luminance;");
            }

            normal.AppendLine("}");
            lines.Add(normal.ToString());
            File.WriteAllLines("../../shaders/cs-full.glsl", lines);
            ray_program = GL.CreateProgram();

            //Create a new program
            int ray_shader = 0;
            LoadShader("../../shaders/cs-full.glsl", ShaderType.ComputeShader, ray_program, out ray_shader);
            GL.LinkProgram(ray_program);

            attributeBoxes = GL.GetUniformLocation(ray_program, "boxes");
            attributeColors = GL.GetUniformLocation(ray_program, "colors");
            attributeCircles = GL.GetUniformLocation(ray_program, "circles");
            attributeLightsources = GL.GetUniformLocation(ray_program, "lightsources");
        }

		//Render one frame
		public void Tick()
		{
            GL.Uniform2(attributeBoxes, boxesLength, boxes);
            GL.Uniform2(attributeLightsources, lightsourcesLength, lightsources);
            GL.Uniform3(attributeCircles, circlesLength, circles);
            GL.Uniform3(attributeColors, colorsLength, colors);

            {
                GL.UseProgram(ray_program);
                GL.DispatchCompute(tex_w, tex_h, 1);
            }

            GL.Enable(EnableCap.Texture2D);
            GL.MemoryBarrier(MemoryBarrierFlags.ShaderImageAccessBarrierBit);

            {
                GL.Clear(ClearBufferMask.ColorBufferBit);
                GL.BindTexture(TextureTarget.Texture2D, tex_output);
                GL.Begin(PrimitiveType.Quads);
                GL.TexCoord2(0, 1); GL.Vertex2(-1, 1);
                GL.TexCoord2(1, 1); GL.Vertex2(1, 1);
                GL.TexCoord2(1, 0); GL.Vertex2(1, -1);
                GL.TexCoord2(0, 0); GL.Vertex2(-1, -1);
                GL.End();
            }

            GL.Disable(EnableCap.Texture2D);
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
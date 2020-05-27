using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using OpenTK;
namespace Template
{
    class sphere : @object
    {
        public float radius;
        public sphere(Vector3 _position, float _radius, int _color, float _absorption = 1, float _refraction = 0)
        {
            position = _position;
            radius = _radius;
            color = _color;
            absorption = _absorption;
            refraction = _refraction;
        }

        //Build GLSL functions
        public void AddToArray(List<float> array, float[] colors, StringBuilder normal, StringBuilder faster)
        {
            index = array.Count / 3;

            array.Add(position.X);
            array.Add(position.Y);
            array.Add(position.Z);

            normal.AppendLine("    d = 2.0 * dot(ray_origin - spheres[" + index + "], ray_direction);");
            normal.AppendLine("    discriminant = d * d - 4 * (dot(ray_origin - spheres[" + index + "], ray_origin - spheres[" + index + "]) - " + (radius * radius) + ");");
            normal.AppendLine("    if(discriminant >= 0) {");
            normal.AppendLine("        s = (-d - sqrt(discriminant)) / 2;");
            normal.AppendLine("        s = s < 0 ? (-d + sqrt(discriminant)) / 2 : s;");
            normal.AppendLine("        if(s > 0 && s < t) {");
            normal.AppendLine("            t = s;");
            normal.AppendLine("            col = vec3(" + colors[color*3] + ", " + colors[color * 3 + 1] + ", " + colors[color * 3 + 2]+"); ");
            normal.AppendLine("            normal = normalize(ray_origin + s * ray_direction - spheres[" + index + "]);");
            normal.AppendLine("            refraction = " + refraction + ";");
            normal.AppendLine("            absorption = " + absorption + ";");
            normal.AppendLine("        }");
            normal.AppendLine("    }");

            if (refraction == 0)
            {
                faster.AppendLine("    d = 2.0 * dot(ray_origin - spheres[" + index + "], ray_direction);");
                faster.AppendLine("    discriminant = d * d - 4 * (dot(ray_origin - spheres[" + index + "], ray_origin - spheres[" + index + "]) - " + (radius * radius) + ");");
                faster.AppendLine("    if(discriminant >= 0) {");
                faster.AppendLine("        s = (-d - sqrt(discriminant)) / 2;");
                faster.AppendLine("        s = s < 0 ? (-d + sqrt(discriminant)) / 2 : s;");
                faster.AppendLine("        if(s > 0 && s < tmax) return true;");
                faster.AppendLine("    }");
            }
        }

        //Move sphere based on user input
        public override void move(Vector3 direction, float[] array)
        {
            position -= direction;
            array[index * 3] = position.X;
            array[index * 3 + 1] = position.Y;
            array[index * 3 + 2] = position.Z;
        }

        //Rotate sphere based on user input
        public override void rotate(Quaternion rotate, float[] array)
        {
            position = rotate * position;
            array[index * 3] = position.X;
            array[index * 3 + 1] = position.Y;
            array[index * 3 + 2] = position.Z;
        }
    }
}

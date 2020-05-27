using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;
namespace Template
{
    class skydome
    {
        float radius = 100;
        public Vector3 lookDir;
        public skydome()
        {
            lookDir = new Vector3(1, 1, 0);

        }
        public void AddToArray(StringBuilder normal)
        {
            normal.AppendLine("    d = 2.0 * dot(ray_origin, ray_direction);");
            normal.AppendLine("    discriminant = d * d - 4 * (dot(ray_origin, ray_origin) - " + (radius * radius) + ");");
            normal.AppendLine("    if(discriminant >= 0) {");
            normal.AppendLine("        s = (-d + sqrt(discriminant)) / 2;");
            normal.AppendLine("        if(s > 0 && s < t) {");
            normal.AppendLine("            t = s;");
            normal.AppendLine("            normal = vec3(0,0,0);");
            normal.AppendLine("            refraction = 0;");
            normal.AppendLine("            absorption = 1;");
            normal.AppendLine("            col = texture(skydome, vec2((ray_direction.x + 1)/2, (ray_direction.y + 1)/2)).xyz * 0.1;");
            normal.AppendLine("        }");
            normal.AppendLine("    }");
        }
        public void rotate(Quaternion rotation)
        {
            lookDir = rotation * lookDir;
        }
    }
}

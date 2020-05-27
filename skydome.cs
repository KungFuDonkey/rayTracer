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
        public skydome()
        {


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
            normal.AppendLine("            col = ray_direction;");
            normal.AppendLine("        }");
            normal.AppendLine("    }");
        }
    }
}

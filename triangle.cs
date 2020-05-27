using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Template
{
	class triangle : @object
	{
		public int[] vertices;

		public triangle(int corner1, int corner2, int corner3, int _color, float _absorption = 1, float _refraction = 0)
		{
			vertices = new int[3];
			vertices[0] = corner1;
			vertices[1] = corner2;
			vertices[2] = corner3;
            color = _color;
            absorption = _absorption;
            refraction = _refraction;
        }

        //Build GLSL functions
        public void AddToArray(float[] colors, StringBuilder normal, StringBuilder faster)
        {
            normal.AppendLine("    object_position = normalize(cross(vertices[" + vertices[1] + "] - vertices[" + vertices[0] + "], vertices[" + vertices[2] + "] - vertices[" + vertices[0] + "]));");
            normal.AppendLine("    d = -dot(object_position, vertices[" + vertices[0] + "]);");
            normal.AppendLine("    if(dot(ray_direction, object_position) > 0){");
            normal.AppendLine("        s = -(dot(ray_origin, object_position) + d) / dot(ray_direction, object_position);");
            normal.AppendLine("        if(s > 0 && s < t){");
            normal.AppendLine("            if(dot(object_position,cross(vertices[" + vertices[1] + "] - vertices[" + vertices[0] + "], ray_origin + s * ray_direction - vertices[" + vertices[0] + "])) >= 0){");
            normal.AppendLine("                if(dot(object_position,cross(vertices[" + vertices[2] + "] - vertices[" + vertices[1] + "], ray_origin + s * ray_direction - vertices[" + vertices[1] + "])) >= 0){");
            normal.AppendLine("                    if(dot(object_position,cross(vertices[" + vertices[0] + "] - vertices[" + vertices[2] + "], ray_origin + s * ray_direction - vertices[" + vertices[2] + "])) >= 0){");
            normal.AppendLine("                        t = s;");
            normal.AppendLine("                        col = vec3(" + colors[color * 3] + ", " + colors[color * 3 + 1] + ", " + colors[color * 3 + 2] + "); ");
            normal.AppendLine("                        normal = -object_position;");
            normal.AppendLine("                        refraction = " + refraction + ";");
            normal.AppendLine("                        absorption = " + absorption + ";");
            normal.AppendLine("                    }");
            normal.AppendLine("                }");
            normal.AppendLine("            }");
            normal.AppendLine("        }");
            normal.AppendLine("    }");

            if(refraction == 0)
            {
                faster.AppendLine("    object_position = normalize(cross(vertices[" + vertices[1] + "] - vertices[" + vertices[0] + "], vertices[" + vertices[2] + "] - vertices[" + vertices[0] + "]));");
                faster.AppendLine("    d = -dot(object_position, vertices[" + vertices[0] + "]);");
                faster.AppendLine("    if(dot(ray_direction, object_position) > 0){");
                faster.AppendLine("        s = -(dot(ray_origin, object_position) + d) / dot(ray_direction, object_position);");
                faster.AppendLine("        if(s > 0 && s < tmax){");
                faster.AppendLine("            if(dot(object_position,cross(vertices[" + vertices[1] + "] - vertices[" + vertices[0] + "], ray_origin + s * ray_direction - vertices[" + vertices[0] + "])) >= 0){");
                faster.AppendLine("                if(dot(object_position,cross(vertices[" + vertices[2] + "] - vertices[" + vertices[1] + "], ray_origin + s * ray_direction - vertices[" + vertices[1] + "])) >= 0){");
                faster.AppendLine("                    if(dot(object_position,cross(vertices[" + vertices[0] + "] - vertices[" + vertices[2] + "], ray_origin + s * ray_direction - vertices[" + vertices[2] + "])) >= 0){");
                faster.AppendLine("                        return true;");
                faster.AppendLine("                    }");
                faster.AppendLine("                }");
                faster.AppendLine("            }");
                faster.AppendLine("        }");
                faster.AppendLine("    }");

            }
        }

        //Get start index of shape in vertices list
        public void changeIndex(int index)
        {
            for(int i = 0; i < 3; ++i)
            {
                vertices[i] += index;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Template
{
	class plane : @object
	{
		float d;
		Vector3 plane_normal;

		public plane(float _d, int _color, Quaternion _rotation, float _absorption = 1, float _refraction = 0)
		{
			d = -_d;
			color = _color;
			plane_normal = _rotation * Vector3.UnitZ;
            plane_normal.Normalize();
			absorption = _absorption;
            refraction = _refraction;
		}

        public void AddToArray(ref List<float> array, StringBuilder normal)
        {
            index = array.Count / 4;
            array.Add(plane_normal.X);
            array.Add(plane_normal.Y);
            array.Add(plane_normal.Z);
            array.Add(d);
            normal.AppendLine("    if(dot(ray_direction, planes[" + index + "].xyz) > 0){");
            normal.AppendLine("        s = -(dot(ray_origin, planes[" + index + "].xyz) + planes[" + index + "].w) / dot(ray_direction, planes[" + index + "].xyz);");
            normal.AppendLine("        if(s > 0 && s < t){");
            normal.AppendLine("            t = s;");
            normal.AppendLine("            col = " + color + ";");
            normal.AppendLine("            normal = -planes[" + index + "].xyz;");
            normal.AppendLine("            absorption = " + absorption + ";");
            normal.AppendLine("        }");
            normal.AppendLine("    }");
        }

        public override void move(Vector3 direction, float[] array)
        {
            d += Vector3.Dot(plane_normal, direction);
            array[index * 4 + 3] = d;
        }
        public override void rotate(Quaternion rotate, float[] array)
        {
            plane_normal = rotate * plane_normal;
            plane_normal.Normalize();
            array[index * 4] = plane_normal.X;
            array[index * 4 + 1] = plane_normal.Y;
            array[index * 4 + 2] = plane_normal.Z;
        }
    }
}

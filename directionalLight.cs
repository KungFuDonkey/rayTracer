using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template
{
	class directionalLight : lightsource
	{
		Vector3 direction;
		int color;
		float strength;
        int index;

		public directionalLight(Vector3 _direction, int _color, float _strength)
		{
			direction = _direction;
            direction.Normalize();
			color = _color;
			strength = _strength;
		}

		public void AddToArray(ref List<float> array, StringBuilder normal, StringBuilder light)
		{
            index = array.Count / 3;
            array.Add(direction.X);
            array.Add(direction.Y);
            array.Add(direction.Z);
            light.AppendLine("    tmax = 10000;");
            light.AppendLine("    angle = dot(normal, directionalLightsources[" + index + "]);");
            light.AppendLine("    point_of_intersection = ray_origin + directionalLightsources[" + index + "] * 0.0001;");
            light.AppendLine("    collision = false;");
            light.AppendLine("    if(normal == vec3(0,0,0))");
            light.AppendLine("        angle = 99;");
            light.AppendLine("    else");
            light.AppendLine("        angle = dot(normal, light_direction);");
            light.AppendLine("    if(!collision){");
            light.AppendLine("        collision = calcObjects(point_of_intersection, directionalLightsources[" + index + "], tmax);");
            light.AppendLine("        if(!collision){");
            light.AppendLine("            getColor(int(" + color + "), lightsource_color);");
            light.AppendLine("            color += lightsource_color * energy * " + strength + " * angle * absorption * 0.25;");
            light.AppendLine("        }");
            light.AppendLine("    }");
            normal.AppendLine("   if(dot(ray_direction, directionalLightsources[" + index + "]) > 0.9999){");
            normal.AppendLine("       if(9000 < t){");
            normal.AppendLine("          t = 9000;");
            normal.AppendLine("          col = " + color + ";");
            normal.AppendLine("          normal = vec3(0,0,0);");
            normal.AppendLine("          absorption = 1;");
            normal.AppendLine("       }");
            normal.AppendLine("   }");
        }

        public void rotate(Quaternion rotation, float[] array)
        {
            direction = rotation * direction;
            array[index * 3] = direction.X;
            array[index * 3 + 1] = direction.Y;
            array[index * 3 + 2] = direction.Z;
        }
    }
}

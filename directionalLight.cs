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
			color = _color;
			strength = _strength;
		}

		public void AddToArray(ref List<float> array, StringBuilder light)
		{
            array.Add(direction.X);
            array.Add(direction.Y);
            array.Add(direction.Z);
            light.AppendLine("    tmax = 10000;");
            light.AppendLine("    angle = dot(normal, directional_lightsources[" + index + "]);");
            light.AppendLine("    point_of_intersection = ray_origin + directional_lightsources[" + index + "] * 0.0001;");
            light.AppendLine("    collision = false;");
            light.AppendLine("    if(angle < 0) collision = true;");
            light.AppendLine("    if(!collision){");
            light.AppendLine("        collision = calcObjects(point_of_intersection, directional_lightsources[" + index + "], tmax);");
            light.AppendLine("        if(!collision){");
            light.AppendLine("            getColor(int(" + color + "), lightsource_color);");
            light.AppendLine("            color += lightsource_color * energy * " + strength + " * angle * absorption;");
            light.AppendLine("        }");
            light.AppendLine("    }");
        }
    }
}

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

        //Build GLSL functions
		public void AddToArray(List<float> array, float[] colors, StringBuilder normal, StringBuilder light)
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
            light.AppendLine("        angle = 2;");
            light.AppendLine("    else");
            light.AppendLine("        angle = dot(normal, directionalLightsources[" + index + "]);");
            light.AppendLine("    if(!collision){");
            light.AppendLine("        collision = calcObjects(point_of_intersection, directionalLightsources[" + index + "], tmax);");
            light.AppendLine("        if(!collision){");
            light.AppendLine("            color += vec3(" + colors[color * 3] + ", " + colors[color * 3 + 1] + ", " + colors[color * 3 + 2] + ") * energy * " + strength + " * angle * absorption * 0.25;");
            light.AppendLine("        }");
            light.AppendLine("    }");

            normal.AppendLine("   if(dot(ray_direction, directionalLightsources[" + index + "]) > 0.9999){");
            normal.AppendLine("       if(9000 < t){");
            normal.AppendLine("          t = 9000;");
            normal.AppendLine("          col = vec3(" + colors[color * 3] + ", " + colors[color * 3 + 1] + ", " + colors[color * 3 + 2] + "); ");
            normal.AppendLine("          normal = vec3(0,0,0);");
            normal.AppendLine("          absorption = 1;");
            normal.AppendLine("       }");
            normal.AppendLine("   }");
        }

        //Rotate light based on user input
        public void rotate(Quaternion rotation, float[] array)
        {
            direction = rotation * direction;

            array[index * 3] = direction.X;
            array[index * 3 + 1] = direction.Y;
            array[index * 3 + 2] = direction.Z;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Template
{
	class arealight : lightsource
	{
		sphere shape;
        int index;
		public arealight(float _emittance, sphere _shape)
		{
			emittance = _emittance;
			shape = _shape;
		}

        public void AddToArray(ref List<float> array, StringBuilder normal, StringBuilder light)
        {
            index = array.Count / 3;
            array.Add(shape.position.X);
            array.Add(shape.position.Y);
            array.Add(shape.position.Z);
            light.AppendLine("    light_direction = areaLightsources[" + index + "] - ray_origin;");
            light.AppendLine("    tmax = length(light_direction) - 0.0002;");
            light.AppendLine("    lightsource_emittance = " + emittance + " / (12.456 * length(light_direction) * length(light_direction));");
            light.AppendLine("    light_direction = normalize(light_direction);");
            light.AppendLine("    if(normal == vec3(0,0,0))");
            light.AppendLine("        angle = 1;");
            light.AppendLine("    else");
            light.AppendLine("        angle = dot(normal, light_direction);");
            light.AppendLine("    point_of_intersection = ray_origin + light_direction * 0.0001;");
            light.AppendLine("    collision = false;");
            light.AppendLine("    if(angle < 0) collision = true;");
            light.AppendLine("    if(!collision){");
            light.AppendLine("        collision = calcObjects(point_of_intersection, light_direction, tmax);");
            light.AppendLine("        if(!collision){");
            light.AppendLine("            getColor(int(" + shape.color + "), lightsource_color);");
            light.AppendLine("            color += lightsource_color * lightsource_emittance * energy * angle * absorption;");
            light.AppendLine("        }");
            light.AppendLine("    }");
            normal.AppendLine("    d = 2.0 * dot(ray_origin - areaLightsources[" + index + "], ray_direction);");
            normal.AppendLine("    discriminant = d * d - 4 * (dot(ray_origin - areaLightsources[" + index + "], ray_origin - areaLightsources[" + index + "]) - " + (shape.radius * shape.radius) + ");");
            normal.AppendLine("    if(discriminant >= 0) {");
            normal.AppendLine("        s = (-d - sqrt(discriminant)) / 2;");
            normal.AppendLine("        s = s < 0 ? (-d + sqrt(discriminant)) / 2 : s;");
            normal.AppendLine("        if(s > 0 && s < t) {");
            normal.AppendLine("            t = s;");
            normal.AppendLine("            col = " + shape.color + ";");
            normal.AppendLine("            normal = vec3(0,0,0);");
            normal.AppendLine("            absorption = 1;");
            normal.AppendLine("        }");
            normal.AppendLine("    }");
        }

        public void move(Vector3 direction, float[] array)
        {
            shape.position -= direction;
            array[index * 3] = shape.position.X;
            array[index * 3 + 1] = shape.position.Y;
            array[index * 3 + 2] = shape.position.Z;

        }
        public void rotate(Quaternion rotate, float[] array)
        {
            shape.position = rotate * shape.position;
            array[index * 3] = shape.position.X;
            array[index * 3 + 1] = shape.position.Y;
            array[index * 3 + 2] = shape.position.Z;
        }
    }
}

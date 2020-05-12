using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Template
{
	class lightsource
	{
		public @object shape;
		public float emittance;
		public float epsilon = 0.00001f;

		public lightsource(float _emittance, @object _shape)
		{
			emittance = _emittance;
			shape = _shape;
		}

		public void calcIntersection(ray ray, List<@object> objects)
		{
			if (ray.t == float.MaxValue)
				return;
			Vector3 lightDirection = (shape.position - ray.pointOfIntersection);
			float illumination = (float)(emittance / (4 * Math.PI * lightDirection.Length * lightDirection.Length));
			float tmax = lightDirection.Length - 2 * epsilon;
			lightDirection.Normalize();
			ray.pointOfIntersection += epsilon * lightDirection;

			for(int i = 0; i < objects.Count; ++i)
			{
				float t = 0;
				if (objects[i].calcIntersection(ray.pointOfIntersection, lightDirection, ref t))
				{
					if(t > tmax)
					{
						continue;
					}

					return;
				}
			}

			ray.nextColor.Z += illumination * Vector3.Dot(ray.normal, lightDirection);
		}

		public void rayIntersection(ray ray)
		{
			shape.rayIntersection(ray);
		}
	}
}

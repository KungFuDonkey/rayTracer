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
		@object shape;

		public arealight(float _emittance, @object _shape)
		{
			emittance = _emittance;
			shape = _shape;
		}

		public override void calcIntersection(ray ray, List<@object> objects)
		{
			if (ray.t == float.MaxValue)
				return;
			Vector3 lightDirection = (shape.position - ray.pointOfIntersection);
			float illumination = (float)(emittance / (4 * Math.PI * lightDirection.Length * lightDirection.Length));
			float tmax = lightDirection.Length - 2 * epsilon;
			lightDirection.Normalize();
			ray.pointOfIntersection += epsilon * lightDirection;

			for (int i = 0; i < objects.Count; ++i)
			{
				float t = objects[i].calcIntersection(ray.pointOfIntersection, lightDirection);
				if (t > 0 && t < tmax)
				{
					return;
				}
			}

			ray.color += illumination * Vector3.Dot(ray.normal, lightDirection) * shape.color * ray.energy * ray.absorption / 100;
		}

		public override void rayIntersection(ray ray)
		{
			shape.rayIntersection(ray);
		}
	}
}

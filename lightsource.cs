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
		public Vector3 position;
		public float radius;
		public Vector3 color;
		public float emittance;
		public float epsilon = 0.000001f;

		public lightsource(Vector3 _position, float _emittance)
		{
			position = _position;
			emittance = _emittance;
		}

		public void calcIntersection(ray ray, List<@object> objects)
		{
			if (ray.t == float.MaxValue)
				return;
			Vector3 pointOfContact = ray.origin + ray.t * ray.direction;
			Vector3 lightDirection = (position - pointOfContact);
			float illumination = (float)(emittance / (4 * Math.PI * lightDirection.Length * lightDirection.Length));
			float tmax = lightDirection.Length - 2 * epsilon;
			lightDirection.Normalize();
			pointOfContact += epsilon * lightDirection;

			for(int i = 0; i < objects.Count; ++i)
			{
				float t = 0;
				if (objects[i].calcIntersection(pointOfContact, lightDirection, ref t))
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
	}
}

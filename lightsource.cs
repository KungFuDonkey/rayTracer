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
		public int color;
		public float emittance;
		public float epsilon = 0.01f;

		public lightsource(Vector3 _position)
		{
			position = _position;
		}

		public void calcIntersection(ray ray, List<@object> objects)
		{
			if (ray.t <= 0)
				return;
			Vector3 pointOfContact = ray.origin + ray.t * ray.direction;
			Vector3 lightDirection = (position - pointOfContact);
			float tmax = lightDirection.Length - 2 * epsilon;
            pointOfContact += epsilon * lightDirection;
            lightDirection.Normalize();
			for(int i = 0; i < objects.Count; ++i)
			{
				float t = 0;
				if (objects[i].calcIntersection(pointOfContact, lightDirection, ref t))
				{
					if(t > tmax)
					{
						continue;
					}
					ray.color = 0;
					return;
				}
			}
		}
	}
}

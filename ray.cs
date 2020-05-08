using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Template
{
	class ray
	{
		public Vector3 origin;
		public Vector3 direction;
		public float t;
		public Vector3 color;
		public Vector3 normal;
		public float absorption;

		public ray(Vector3 _origin, Vector3 _direction)
		{
			origin = _origin;
			direction = _direction;
			t = float.MaxValue;
			color = Vector3.Zero;
			normal = Vector3.Zero;
			absorption = 0;
		}

		public void calculateColor(List<@object> objects, List<lightsource> lightsources)
		{
			for(int iterations = 0; iterations < 5; ++iterations)
			{
				for(int i = 0; i < objects.Count; ++i)
				{
					objects[i].rayIntersection(this);
				}

				if(absorption == 100 || t == float.MaxValue)
				{
					break;
				}
				else
				{
					origin = origin + t * direction;
					direction = direction - 2 * (Vector3.Dot(normal, direction)) * normal;
					origin += direction * 0.000001f;
					t = float.MaxValue;
				}
			}

			for (int i = 0; i < lightsources.Count; ++i)
			{
				lightsources[i].calcIntersection(this, objects);
			}
		}
	}
}

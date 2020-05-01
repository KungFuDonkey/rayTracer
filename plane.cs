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

		public plane(float _d, Vector3 _color, Vector3 _rotation)
		{
			d = -_d;
			color = _color;
			rotation = _rotation;
		}

		public override bool calcIntersection(Vector3 origin, Vector3 direction, ref float t)
		{
			float dotProduct = Vector3.Dot(direction, rotation);
			if (dotProduct > 1e-6)
			{
				t = -(Vector3.Dot(origin, rotation) + d) / dotProduct;
				return t >= 0;
			}

			return false;
		}

		public override void rayIntersection(ray ray)
		{
			float t = 0;
			if(calcIntersection(ray.origin, ray.direction, ref t))
			{
				if(t < ray.t)
				{
					ray.color = color;
					ray.t = t;
				}
			}
		}
	}
}

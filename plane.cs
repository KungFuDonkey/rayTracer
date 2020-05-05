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
		Vector3 normal;

		public plane(float _d, Vector3 _color, Quaternion _rotation)
		{
			d = -_d;
			color = _color;
			rotation = _rotation;
		}

		public override bool calcIntersection(Vector3 origin, Vector3 direction, ref float t)
		{
			float dotProduct = Vector3.Dot(direction, normal);
			if (dotProduct > 1e-6)
			{
				t = -(Vector3.Dot(origin, normal) + d) / dotProduct;
				return t >= 0;
			}

			return false;
		}
	}
}

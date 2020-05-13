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

		public plane(float _d, Vector3 _color, Quaternion _rotation, float _absorption = 100)
		{
			d = -_d;
			color = _color;
			normal = _rotation * Vector3.UnitZ;
			absorption = _absorption;
		}

		public override float calcIntersection(Vector3 origin, Vector3 direction)
		{
			float dotProduct = Vector3.Dot(direction, normal);
			if (dotProduct > 1e-6)
			{
				float t = -(Vector3.Dot(origin, normal) + d) / dotProduct;
				return t;
			}

			return -1;
		}

		public override Vector3 getNormal(Vector3 pointOfIntersection)
		{
			return -normal;
		}
	}
}

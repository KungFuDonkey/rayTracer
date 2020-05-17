using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Template
{
	class @object
	{
		public Vector3 position;
		public Vector3 color;
		protected Quaternion rotation;
		protected float absorption;

		public virtual float calcIntersection(Vector3 origin, Vector3 direction, bool lightray)
		{
			return -1;
		}

		public virtual void rayIntersection(ray ray)
		{
			float t = calcIntersection(ray.origin, ray.direction, false);

			if (t < ray.t && t > 0)
			{
				ray.nextColor = color;
				ray.t = t;
				ray.normal = getNormal(Vector3.Zero);
				ray.absorption = absorption;
			}
		}

		public virtual Vector3 getNormal(Vector3 pointOfIntersection)
		{
			return Vector3.Zero;
		}
	}
}

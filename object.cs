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
		protected Vector3 position;
		protected Vector3 color;
		protected Quaternion rotation;

		public virtual bool calcIntersection(Vector3 origin, Vector3 direction, ref float t)
		{
			return false;
		}

		public virtual void rayIntersection(ray ray)
		{
			float t = float.MaxValue;
			if (calcIntersection(ray.origin, ray.direction, ref t))
			{
				if (t < ray.t)
				{
					ray.color = color;
					ray.t = t;
				}
			}
		}
	}
}

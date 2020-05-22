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
		public int color;
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
			}
		}

		public virtual Vector3 getNormal(Vector3 pointOfIntersection)
		{
			return Vector3.Zero;
		}

        public virtual void AddToArray(ref List<float> array)
        {

        }

        public virtual void AddToArray(ref List<int> array)
        {

        }

    }
}

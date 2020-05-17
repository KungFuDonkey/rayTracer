using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Template
{
	class shapes : @object
	{
		protected Vector3 dimensions;
		protected triangle[] shape;
		public override void rayIntersection(ray ray)
		{
			for (int i = 0; i < shape.Length; ++i)
			{
				float t = shape[i].calcIntersection(ray.origin, ray.direction, false);

				if(t < ray.t && t > 0)
				{
					ray.t = t;
					ray.nextColor = color;
					ray.normal = shape[i].getNormal(Vector3.Zero);
					ray.absorption = absorption;
				}
			}
		}
	}
}

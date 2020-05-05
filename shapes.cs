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


		public override bool calcIntersection(Vector3 origin, Vector3 direction, ref float t)
		{
			bool intersection = false;

			for(int i = 0; i < shape.Length; ++i)
			{
				float s = float.MaxValue;
				if (shape[i].calcIntersection(origin, direction, ref s))
				{
					t = s < t ? s : t;
					intersection = true;
				}
			}

			return intersection;
		}
	}
}

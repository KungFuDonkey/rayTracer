using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.ES20;

namespace Template
{
	class lightsource
	{
		public float emittance;
		public float epsilon = 0.0001f;

		public virtual void calcIntersection(ray ray, List<@object> objects)
		{

		}

		public virtual void rayIntersection(ray ray)
		{
			
		}
	}
}

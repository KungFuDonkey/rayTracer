using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Template
{
	class rectangle : @object
	{
		Vector3 dimensions;

		public rectangle(Vector3 _position, Vector3 _dimensions, int _color)
		{
			position = _position;
			dimensions = _dimensions;
			color = _color;
		}

		public override bool calcIntersection(Vector3 origin, Vector3 direction, ref float t)
		{
			return false;
		}

		public override void rayIntersection(ray ray)
		{
			
		}
	}
}

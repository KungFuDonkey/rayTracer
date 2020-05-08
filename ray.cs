using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Template
{
	class ray
	{
		public Vector3 origin;
		public Vector3 direction;
		public float t;
		public Vector3 color;
		public Vector3 normal;

		public ray(Vector3 _origin, Vector3 _direction)
		{
			origin = _origin;
			direction = _direction;
			t = float.MaxValue;
			color = Vector3.Zero;
			normal = Vector3.Zero;
		}
	}
}

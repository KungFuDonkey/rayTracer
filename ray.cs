﻿using System;
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
		public int color;

		public ray(Vector3 _origin, Vector3 _direction)
		{
			origin = _origin;
			direction = _direction;
			t = float.MaxValue;
			color = 0;
		}
	}
}
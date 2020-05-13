using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template
{
	class directionalLight
	{
		Vector3 direction;
		Vector3 color;

		public directionalLight(Vector3 _direction, Vector3 _color)
		{
			direction = _direction;
			color = _color;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Template
{
	class pyramid: shapes
	{
		public pyramid(Vector3 _position, Vector3 _dimensions, Vector3 _color, Quaternion _rotation)
		{
			position = _position;
			dimensions = _dimensions * 0.5f;
			color = _color;
			rotation = _rotation;
			rotation.Normalize();

			shape = new triangle[6];

			Vector3 right = rotation * Vector3.UnitX;
			Vector3 up = rotation * Vector3.UnitY;
			Vector3 forward = rotation * Vector3.UnitZ;

			Vector3 u = position + up * dimensions.Y;
			Vector3 fl = position - up * dimensions.Y - right * dimensions.X - forward * dimensions.Z;
			Vector3 fr = position - up * dimensions.Y + right * dimensions.X - forward * dimensions.Z;
			Vector3 bl = position - up * dimensions.Y - right * dimensions.X + forward * dimensions.Z;
			Vector3 br = position - up * dimensions.Y + right * dimensions.X + forward * dimensions.Z;

			shape[0] = new triangle(u, fl, fr);
			shape[1] = new triangle(u, fl, bl);
			shape[2] = new triangle(u, fr, br);
			shape[3] = new triangle(u, bl, br);
			shape[4] = new triangle(fl, bl, br);
			shape[5] = new triangle(fl, fr, br);
		}
	}
}

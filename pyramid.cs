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
		public pyramid(Vector3 _position, Vector3 _dimensions, int _color, Quaternion _rotation, float _absorption = 1, float _refraction = 0)
		{
			position = _position;
			dimensions = _dimensions * 0.5f;
			color = _color;
			rotation = _rotation;
			rotation.Normalize();
			absorption = _absorption;
			refraction = _refraction;

			Vector3 right = rotation * Vector3.UnitX;
			Vector3 up = rotation * Vector3.UnitY;
			Vector3 forward = rotation * Vector3.UnitZ;

			vertices[0] = position + up * dimensions.Y;
			vertices[1] = position - up * dimensions.Y - right * dimensions.X - forward * dimensions.Z;
			vertices[2] = position - up * dimensions.Y + right * dimensions.X - forward * dimensions.Z;
			vertices[3] = position - up * dimensions.Y - right * dimensions.X + forward * dimensions.Z;
			vertices[4] = position - up * dimensions.Y + right * dimensions.X + forward * dimensions.Z;

            shape = new triangle[6];
            shape[0] = new triangle(0, 1, 2, _color, _absorption);
			shape[1] = new triangle(0, 3, 1, _color, _absorption);
			shape[2] = new triangle(0, 2, 4, _color, _absorption);
			shape[3] = new triangle(0, 4, 3, _color, _absorption);
			shape[4] = new triangle(1, 3, 4, _color, _absorption);
			shape[5] = new triangle(1, 4, 2, _color, _absorption);
		}
	}
}

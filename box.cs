using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Template
{
	class box : shapes
	{
		public box(Vector3 _position, Vector3 _dimensions, int _color, Quaternion _rotation, float _absorption = 1)
		{
			position = _position;
			dimensions = _dimensions / 2;
			color = _color;
			rotation = _rotation;
			rotation.Normalize();
			absorption = _absorption;

			Vector3 right = rotation * Vector3.UnitX;
			Vector3 up = rotation * Vector3.UnitY;
			Vector3 forward = rotation * Vector3.UnitZ;
            vertices = new Vector3[8];
			vertices[0] = position - forward * dimensions.Z + up * dimensions.Y - right * dimensions.X;
			vertices[1] = position - forward * dimensions.Z + up * dimensions.Y + right * dimensions.X;
			vertices[2] = position - forward * dimensions.Z - up * dimensions.Y - right * dimensions.X;
			vertices[3] = position - forward * dimensions.Z - up * dimensions.Y + right * dimensions.X;
			vertices[4] = position + forward * dimensions.Z + up * dimensions.Y - right * dimensions.X;
			vertices[5] = position + forward * dimensions.Z + up * dimensions.Y + right * dimensions.X;
			vertices[6] = position + forward * dimensions.Z - up * dimensions.Y - right * dimensions.X;
			vertices[7] = position + forward * dimensions.Z - up * dimensions.Y + right * dimensions.X;

            shape = new triangle[12];
            shape[0] = new triangle(0, 2, 1, _color, _absorption);
			shape[1] = new triangle(1, 2, 3, _color, _absorption);
			shape[2] = new triangle(0, 1, 4, _color, _absorption);
			shape[3] = new triangle(1, 5, 4, _color, _absorption);
			shape[4] = new triangle(1, 7, 5, _color, _absorption);
			shape[5] = new triangle(1, 3, 7, _color, _absorption);
			shape[6] = new triangle(2, 6, 3, _color, _absorption);
			shape[7] = new triangle(6, 7, 3, _color, _absorption);
			shape[8] = new triangle(0, 4, 6, _color, _absorption);
			shape[9] = new triangle(0, 6, 2, _color, _absorption);
			shape[10] = new triangle(4, 5, 6, _color, _absorption);
			shape[11] = new triangle(6, 5, 7, _color, _absorption);
		}

    }
}

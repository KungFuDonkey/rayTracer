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
		public box(Vector3 _position, Vector3 _dimensions, Vector3 _color, Quaternion _rotation, float _absorption = 100)
		{
			position = _position;
			dimensions = _dimensions / 2;
			color = _color;
			rotation = _rotation;
			rotation.Normalize();
			absorption = _absorption;

			shape = new triangle[12];

			Vector3 right = rotation * Vector3.UnitX;
			Vector3 up = rotation * Vector3.UnitY;
			Vector3 forward = rotation * Vector3.UnitZ;

			Vector3 ful = position - forward * dimensions.Z + up * dimensions.Y - right * dimensions.X;
			Vector3 fur = position - forward * dimensions.Z + up * dimensions.Y + right * dimensions.X;
			Vector3 fdl = position - forward * dimensions.Z - up * dimensions.Y - right * dimensions.X;
			Vector3 fdr = position - forward * dimensions.Z - up * dimensions.Y + right * dimensions.X;
			Vector3 bul = position + forward * dimensions.Z + up * dimensions.Y - right * dimensions.X;
			Vector3 bur = position + forward * dimensions.Z + up * dimensions.Y + right * dimensions.X;
			Vector3 bdl = position + forward * dimensions.Z - up * dimensions.Y - right * dimensions.X;
			Vector3 bdr = position + forward * dimensions.Z - up * dimensions.Y + right * dimensions.X;

			shape[0] = new triangle(ful, fdl, fur);
			shape[1] = new triangle(fur, fdl, fdr);
			shape[2] = new triangle(ful, fur, bul);
			shape[3] = new triangle(fur, bur, bul);
			shape[4] = new triangle(fur, bdr, bur);
			shape[5] = new triangle(fur, fdr, bdr);
			shape[6] = new triangle(fdl, bdl, fdr);
			shape[7] = new triangle(bdl, bdr, fdr);
			shape[8] = new triangle(ful, bul, bdl);
			shape[9] = new triangle(ful, bdl, fdl);
			shape[10] = new triangle(bul, bur, bdl);
			shape[11] = new triangle(bdl, bur, bdr);
		}
	}
}

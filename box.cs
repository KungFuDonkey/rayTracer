using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Template
{
	class box : @object
	{
		Vector3 dimensions;
		plane[] sides;

		public box(Vector3 _position, Vector3 _dimensions, Vector3 _color, Vector3 _rotation)
		{
			position = _position;
			dimensions = _dimensions;
			color = _color;
			rotation = _rotation;
			rotation.Normalize();

			sides = new plane[6];
			sides[0] = new plane(position.Length + dimensions.Z / 2, color, rotation);
		}

		public override bool calcIntersection(Vector3 origin, Vector3 direction, ref float t)
		{
			float[] ts = new float[6];
			ts[0] = (position.X - origin.X) / direction.X;
			ts[1] = (position.X + dimensions.X - origin.X) / direction.X;
			ts[2] = (position.Y - origin.Y) / direction.Y;
			ts[3] = (position.Y + dimensions.Y - origin.Y) / direction.Y;
			ts[4] = (position.Z - origin.Z) / direction.Z;
			ts[5] = (position.Z + dimensions.Z - origin.Z) / direction.Z;

			for(int i = 0; i < 6; ++i)
			{
				Vector3 p = origin + direction * ts[i];
			}

			return false;
		}

		public override void rayIntersection(ray ray)
		{
			
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Template
{
	class triangle : @object
	{
		Vector3 normal;
		Vector3[] vertices;
		float d;

		public triangle(Vector3 corner1, Vector3 corner2, Vector3 corner3)
		{
			vertices = new Vector3[3];
			vertices[0] = corner1;
			vertices[1] = corner2;
			vertices[2] = corner3;

			Vector3 v1 = corner2 - corner1;
			Vector3 v2 = corner3 - corner1;
			normal = Vector3.Cross(v1, v2);
			normal.Normalize();
			d = -Vector3.Dot(normal, corner1);
		}

		public triangle(Vector3 corner1, Vector3 corner2, Vector3 corner3, Vector3 _color, float _absorption = 100)
		{
			vertices = new Vector3[3];
			vertices[0] = corner1;
			vertices[1] = corner2;
			vertices[2] = corner3;

			Vector3 v1 = corner2 - corner1;
			Vector3 v2 = corner3 - corner1;
			normal = Vector3.Cross(v1, v2);
			d = -Vector3.Dot(normal, corner1);
			color = _color;
			absorption = _absorption;
		}

		public override bool calcIntersection(Vector3 origin, Vector3 direction, ref float t)
		{
			float dotProduct = Vector3.Dot(direction, normal);
			if (dotProduct > 0.000001)
			{
				t = -(Vector3.Dot(origin, normal) + d) / dotProduct;
			}
			else if (dotProduct < -0.000001)
			{
				t = -(Vector3.Dot(origin, -normal) - d) / Vector3.Dot(direction, -normal);
			}
			else
			{
				return false;
			}

			if(t < 0)
			{
				return false;
			}

			Vector3 point = origin + t * direction;

			for(int i  = 0; i < 3; ++i)
			{
				Vector3 edge = vertices[i + 1 > 2 ? 0 : i + 1] - vertices[i];
				Vector3 vertexPoint = point - vertices[i];
				Vector3 cross = Vector3.Cross(edge, vertexPoint);

				if(Vector3.Dot(normal, cross) < 0)
				{
					return false;
				}
			}

			return true;
		}

		public override Vector3 getNormal(Vector3 pointOfIntersection)
		{
			return -normal;
		}
	}
}

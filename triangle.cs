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
		public Vector3[] vertices;
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

        public triangle(Vector3 corner1, Vector3 corner2, Vector3 corner3, Vector3 _normal)
        {
            vertices = new Vector3[3];
            vertices[0] = corner1;
            vertices[1] = corner2;
            vertices[2] = corner3;
            Vector3 v1 = corner2 - corner1;
            Vector3 v2 = corner3 - corner1;
            normal = Vector3.Cross(v1, v2);
            if (Vector3.Dot(normal, _normal) > 0)
            {
                vertices[1] = corner3;
                vertices[2] = corner2;
                normal *= -1;
            }
            normal.Normalize();
            d = -Vector3.Dot(normal, corner1);

		}

		public override float calcIntersection(Vector3 origin, Vector3 direction, bool lightray)
		{
			float dotProduct = Vector3.Dot(direction, normal);
			float t;
			if (dotProduct > 0.000001)
			{
				t = -(Vector3.Dot(origin, normal) + d) / dotProduct;
			}
			else if (dotProduct < -0.000001 && lightray)
			{
				t = -(Vector3.Dot(origin, -normal) - d) / Vector3.Dot(direction, -normal);
			}
			else
			{
				return -1;
			}

			if(t < 0)
			{
				return -1;
			}

			Vector3 point = origin + t * direction;

			for(int i  = 0; i < 3; ++i)
			{
				Vector3 edge = vertices[i + 1 > 2 ? 0 : i + 1] - vertices[i];
				Vector3 vertexPoint = point - vertices[i];
				Vector3 cross = Vector3.Cross(edge, vertexPoint);

				if(Vector3.Dot(normal, cross) < 0)
				{
					return -1;
				}
			}

			return t;
		}

		public override Vector3 getNormal(Vector3 pointOfIntersection)
		{
			return -normal;
		}

        public void transelate(Vector3 direction)
        {
            for(int i = 0; i < 3; i++)
            {
                vertices[i] += direction;
            }
            d = -Vector3.Dot(normal, vertices[0]);
        }
        public void rotate(Quaternion rotation)
        {
            normal = rotation * normal;
            vertices[0] = rotation * vertices[0];
            vertices[1] = rotation * vertices[1];
            vertices[2] = rotation * vertices[2];
        }
        public void resize(Vector3 meshDimension, Vector3 newDimension)
        {
            for(int i = 0; i < 3; i++)
            {
                vertices[i].X = meshDimension.X == 0 ? 0 : vertices[i].X / meshDimension.X;
                vertices[i].Y = meshDimension.Y == 0 ? 0 : vertices[i].Y / meshDimension.Y;
                vertices[i].Z = meshDimension.Z == 0 ? 0 : vertices[i].Z / meshDimension.Z;
                vertices[i] *= newDimension;
            }
            d = -Vector3.Dot(normal, vertices[0]);
        }
        public void resize(Vector3 meshDimension, float multiplier)
        {
            for(int i = 0; i < 3; i++)
            {
                vertices[i] *= multiplier;
            }
            d = -Vector3.Dot(normal, vertices[0]);
        }
	}
}

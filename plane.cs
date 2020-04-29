﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Template
{
	class plane : @object
	{
		Vector3 dimensions;
		Vector3 rotation;

		public plane(Vector3 _position, Vector3 _dimensions, int _color, Vector3 _rotation)
		{
			position = _position;
			dimensions = _dimensions;
			color = _color;
			rotation = _rotation;
		}

		public override bool calcIntersection(Vector3 origin, Vector3 direction, ref float t)
		{
			float dotProduct = Vector3.Dot(direction, rotation);
			if (dotProduct > 1e-6)
			{
				t = -Vector3.Dot(origin - position - dimensions, rotation) / dotProduct;
				if(t < 0)
				{
					return false;
				}
				Vector3 p = origin + t * direction;
				if(p.X > position.X && p.X < position.X + dimensions.X)
					if(p.Y > position.Y && p.Y < position.Y + dimensions.Y)
						return true;
			}

			return false;
		}

		public override void rayIntersection(ray ray)
		{
			float t = 0;
			if(calcIntersection(ray.origin, ray.direction, ref t))
			{
				if(t < ray.t)
				{
					ray.color = color;
				}
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Template
{
	class shapes : @object
	{
		protected Vector3 dimensions;
		public triangle[] shape;
        public Vector3[] vertices;
        public override void rayIntersection(ray ray)
		{
			for (int i = 0; i < shape.Length; ++i)
			{
				float t = shape[i].calcIntersection(ray.origin, ray.direction, false);

				if(t < ray.t && t > 0)
				{
				}
			}
		}
        public override void AddToArray(ref List<float> array)
        {
            for (int i = 0; i < shape.Length; ++i)
            {
                shape[i].changeIndex(array.Count);
            }
            for (int i = 0; i < vertices.Length; ++i)
            {
                array.Add(vertices[i].X);
                array.Add(vertices[i].Y);
                array.Add(vertices[i].Z);
            }
        }
    }
}

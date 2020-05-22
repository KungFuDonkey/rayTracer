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
		public int[] vertices;

		public triangle(int corner1, int corner2, int corner3, int _color, float _absorption)
		{
			vertices = new int[3];
			vertices[0] = corner1;
			vertices[1] = corner2;
			vertices[2] = corner3;
            //Vector3 v1 = corner2 - corner1;
            //Vector3 v2 = corner3 - corner1;
            //normal = Vector3.Cross(v1, v2);
            //normal.Normalize();
            //d = -Vector3.Dot(normal, corner1);
            color = _color;
            absorption = _absorption;
        }

        public override void AddToArray(ref List<float> array)
        {
            for (int i = 0; i < 3; ++i)
            {
                array.Add(vertices[i]);
            }
            array.Add(color);
            array.Add(absorption);
        }

        public void changeIndex(int index)
        {
            for(int i = 0; i < 3; ++i)
            {
                vertices[i] += index;
            }
        }
    }
}

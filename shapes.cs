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
        public void AddToArray(ref List<float> array)
        {
            index = array.Count;
            for (int i = 0; i < shape.Length; ++i)
            {
                shape[i].changeIndex(array.Count / 3);
            }
            for (int i = 0; i < vertices.Length; ++i)
            {
                array.Add(vertices[i].X);
                array.Add(vertices[i].Y);
                array.Add(vertices[i].Z);
            }
        }
        public override void move(Vector3 direction, float[] array)
        {
            for(int i = 0; i < vertices.Length; ++i)
            {
                vertices[i] -= direction;
                array[index + i * 3] = vertices[i].X;
                array[index + i * 3 + 1] = vertices[i].Y;
                array[index + i * 3 + 2] = vertices[i].Z;
            }
        }
        public override void rotate(Quaternion rotate, float[] array)
        {
            for (int i = 0; i < vertices.Length; ++i)
            {
                vertices[i] = rotate * vertices[i];
                array[index + i * 3] = vertices[i].X;
                array[index + i * 3 + 1] = vertices[i].Y;
                array[index + i * 3 + 2] = vertices[i].Z;
            }
        }
    }
}

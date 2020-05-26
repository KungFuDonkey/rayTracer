using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Template
{
	unsafe class @object
	{
		public Vector3 position;
		public int color;
		protected Quaternion rotation;
		protected float absorption;
        protected int index;
        protected float refraction;
        public virtual void AddToArray(ref List<float> array, StringBuilder normal, StringBuilder faster)
        {

        }

        public virtual void move(Vector3 direction, float[] array)
        {

        }

        public virtual void rotate(Quaternion rotate, float[] array)
        {

        }
    }
}

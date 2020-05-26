using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
namespace Template
{
    class mesh : shapes
    {

        public mesh(obj obj, Vector3 _position, int _color, Quaternion _rotation, float multiplier, float _absorption = 1)
        {
            position = _position;
            color = _color;
            rotation = _rotation;
            rotation.Normalize();
            absorption = _absorption;

            shape = new triangle[obj.shape.Length];
            vertices = new Vector3[obj.vertex.Length];
            for(int i = 0; i < vertices.Length; ++i)
            {
                vertices[i] = obj.vertex[i];
                vertices[i] *= multiplier;
                vertices[i] = rotation * vertices[i];
                vertices[i] += position;
            }
            for (int i = 0; i < shape.Length; i++)
            {
                shape[i] = new triangle(obj.shape[i].vertices[0], obj.shape[i].vertices[1], obj.shape[i].vertices[2], _color, _absorption);
            }
        }
    }

}

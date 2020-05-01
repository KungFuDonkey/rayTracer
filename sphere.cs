using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using OpenTK;
namespace Template
{
    class sphere : @object
    {
        float radius;
        public sphere(Vector3 _position, float _radius, Vector3 _color)
        {
            position = _position;
            radius = _radius;
            color = _color;
        }

        public override void rayIntersection(ray ray)
        {
            float t = 0;
            if(calcIntersection(ray.origin, ray.direction, ref t))
            {
                if(t > 0 && t < ray.t)
                {
                    ray.t = t;
                    ray.color = color;
                }
            } 
        }

        public override bool calcIntersection(Vector3 origin, Vector3 direction, ref float t)
        {
            Vector3 c = position - origin;
            if(c.Length < radius)
            {
                return true;
            }
            t = Vector3.Dot(c, direction);
            Vector3 q = c - t * direction;
            float p2 = Vector3.Dot(q, q);
            if (p2 > (radius * radius))
                return false;
            t -= (float)Math.Sqrt(radius * radius - p2);
            if(t > 0)
                return true;
            return false;
        }
    }
}

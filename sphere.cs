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
        public sphere(Vector3 _position, float _radius, Vector3 _color, float _absorption = 100)
        {
            position = _position;
            radius = _radius;
            color = _color;
            absorption = _absorption;
        }

        public override void rayIntersection(ray ray)
        {
            float t = float.MaxValue;
            if (calcIntersection(ray.origin, ray.direction, ref t))
            {
                if (t < ray.t)
                {
                    ray.nextColor = color;
                    ray.t = t;
                    ray.normal = getNormal(ray.origin + ray.direction * t);
                    ray.absorption = absorption;
                }
            }
        }

        public override bool calcIntersection(Vector3 origin, Vector3 direction, ref float t)
        {
            Vector3 conversion = origin - position;
            float b = 2.0f * Vector3.Dot(conversion, direction);
            float c = Vector3.Dot(conversion, conversion) - radius * radius;
            float discriminant = b * b - 4 * c;

            if(discriminant < 0)
            {
                return false;
            }
            else
            {
                t = (float)((-b - Math.Sqrt(discriminant)) / 2);

                if(t < 0)
                {
                    t = (float)((-b + Math.Sqrt(discriminant)) / 2);

                    if(t < 0)
                    {
                        return false;
                    }
                }

                if(t < 0)
                {
                    return false;
                }
                return true;
            }
        }

        public override Vector3 getNormal(Vector3 pointOfIntersection)
        {
            Vector3 normal = pointOfIntersection - position;
            normal.Normalize();
            return normal;
        }
    }
}

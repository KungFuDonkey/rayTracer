﻿using System;
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
        public float radius;
        public sphere(Vector3 _position, float _radius, int _color, float _absorption = 1)
        {
            position = _position;
            radius = _radius;
            color = _color;
            absorption = _absorption;
        }

        public override void rayIntersection(ray ray)
        {
            float t = calcIntersection(ray.origin, ray.direction, false);
            if (t < ray.t && t > 0)
            {
            }
        }

        public override float calcIntersection(Vector3 origin, Vector3 direction, bool lightray)
        {
            Vector3 conversion = origin - position;
            float b = 2.0f * Vector3.Dot(conversion, direction);
            float c = Vector3.Dot(conversion, conversion) - radius * radius;
            float discriminant = b * b - 4 * c;

            if(discriminant < 0)
            {
                return -1;
            }
            else
            {
                float t = (float)((-b - Math.Sqrt(discriminant)) / 2);

                if(t < 0)
                {
                    t = (float)((-b + Math.Sqrt(discriminant)) / 2);
                }

                return t;
            }
        }

        public override Vector3 getNormal(Vector3 pointOfIntersection)
        {
            Vector3 normal = pointOfIntersection - position;
            normal.Normalize();
            return normal;
        }

        public override void AddToArray(ref List<float> array)
        {
            array.Add(position.X);
            array.Add(position.Y);
            array.Add(position.Z);
            array.Add(radius);
            array.Add(color);
            array.Add(absorption);
        }
    }
}

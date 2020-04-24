using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using OpenTK;
namespace Template
{
    class sphere
    {
        Vector3 position;
        float radius;
        int color;
        public sphere(Vector3 _position, float _radius, int _color)
        {
            position = _position;
            radius = _radius;
            color = _color;
        }

        public int calcIntersection(Vector3 rayDirection, Vector3 rayOrigin)
        {
            Vector3 origin = rayOrigin - position;
            float a = Vector3.Dot(rayDirection, rayDirection);
            float b = 2 * Vector3.Dot(origin,rayDirection);
            float c = Vector3.Dot(origin, origin) - (float)Math.Pow(radius,2);
            //Console.WriteLine((b*b) + " - 4(" + a + ")(" + c + ") = " + (b*b - 4 * a * c));
            if(b*b - 4*a*c >= 0)
            {
                return color;
            }
            else
            {
                return 0;
            }
        }
        public void Update()
        {
            position.Z -= 0.01f;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Template
{
	class ray
	{
		public Vector3 origin;
		public Vector3 direction;
		public float t;
		public Vector3 nextColor;
		public Vector3 normal;
		public float absorption;
		public Vector3 color;
		public float colorPercentage = 100;
        public int result;
        public Vector3 pointOfIntersection;
        public Vector3 energy;
        public float lighting;

		public ray(Vector3 _origin, Vector3 _direction)
		{
			origin = _origin;
			direction = _direction;
			t = float.MaxValue;
			nextColor = Vector3.Zero;
			normal = Vector3.Zero;
			absorption = 0;
			color = Vector3.Zero;
            energy = Vector3.One;
		}

		public void calculateColor(List<@object> objects, List<lightsource> lightsources, int iteration)
		{
            iteration++;

            for(int i = 0; i < objects.Count; ++i)
            {
                objects[i].rayIntersection(this);
            }

            for (int i = 0; i < lightsources.Count; ++i)
            {
                lightsources[i].rayIntersection(this);
            }
            if (t != float.MaxValue)
            {

                pointOfIntersection = origin + t * direction;
                energy *= nextColor;

                for(int i = 0; i < lightsources.Count; ++i)
                {
                    lightsources[i].calcIntersection(this, objects);
                }

                if(absorption != 100)
                {
                    if (iteration == 5)
                    {
                        return;
                    }

                    origin = pointOfIntersection;
                    direction = direction - 2 * Vector3.Dot(normal, direction) * normal;
                    origin += direction * 0.00001f;
                    t = float.MaxValue;
                    calculateColor(objects, lightsources, iteration);
                }
            }

            if(iteration == 1)
            {
                color.X = (-1 / (1 + color.X) + 1);
                color.Y = (-1 / (1 + color.Y) + 1);
                color.Z = (-1 / (1 + color.Z) + 1);
                result = ((int)(color.X * 255) << 16) + ((int)(color.Y * 255) << 8) + (int)(color.Z * 255);
            }
        }

        Vector3 RGBToHSL(Vector3 RGB)
        {
            Vector3 HSL = new Vector3();
            float min = Math.Min(RGB.X, Math.Min(RGB.Y, RGB.Z));
            float max = Math.Max(RGB.X, Math.Max(RGB.Y, RGB.Z));
            HSL.Z = (min + max) / 2;

            if (min == max)
            {
                HSL.Y = 0;
            }
            else if (HSL.Z < 0.5)
            {
                HSL.Y = (max - min) / (max + min);
            }
            else
            {
                HSL.Y = (max - min) / (2.0f - max - min);
            }

            if (max == RGB.X)
            {
                HSL.X = (RGB.Y - RGB.Z) / (max - min);
            }
            else if (max == RGB.Y)
            {
                HSL.X = 2.0f + (RGB.Z - RGB.X) / (max - min);
            }
            else
            {
                HSL.X = 4.0f + (RGB.X - RGB.Y) / (max - min);
            }

            HSL.X *= 60;
            HSL.X = HSL.X < 0 ? HSL.X + 360 : HSL.X;
            HSL.Z = 0;
            return HSL;
        }

        Vector3 HSLToRGB(Vector3 HSL)
        {
            float[] RGB = new float[3];

            if (HSL.Y == 0)
            {
                return new Vector3(HSL.Z);
            }

            float temp1 = HSL.Z < 0.5 ? HSL.Z * (1.0f + HSL.Y) : HSL.Z + HSL.Y - HSL.Z * HSL.Y;
            float temp2 = 2 * HSL.Z - temp1;
            float hue = (float)Math.Round(HSL.X / 360, 3);
            RGB[0] = hue + 0.333f > 1 ? hue - 0.667f : hue + 0.333f;
            RGB[1] = hue;
            RGB[2] = hue - 0.333f < 0 ? hue + 0.667f : hue - 0.333f;

            for (int i = 0; i < 3; ++i)
            {
                if (6 * RGB[i] < 1)
                {
                    RGB[i] = temp2 + (temp1 - temp2) * 6 * RGB[i];
                }
                else if (2 * RGB[i] < 1)
                {
                    RGB[i] = temp1;
                }
                else if (3 * RGB[i] < 2)
                {
                    RGB[i] = temp2 + (temp1 - temp2) * (0.666f - RGB[i]) * 6;
                }
                else
                {
                    RGB[i] = temp2;
                }
            }

            return new Vector3(RGB[0], RGB[1], RGB[2]);
        }
    }
}

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
        public triangle[] boundingBox;
        public mesh(obj obj, Vector3 _position, Vector3 _dimensions, Vector3 _color, Quaternion _rotation, float _absorption = 100)
        {
            position = _position;
            dimensions = _dimensions / 2;
            color = _color;
            rotation = _rotation;
            rotation.Normalize();
            absorption = _absorption;

            shape = new triangle[obj.shape.Length];
            for(int i = 0; i < shape.Length; i++)
            {
                shape[i] = new triangle(obj.shape[i].vertices[0], obj.shape[i].vertices[1], obj.shape[i].vertices[2]);
                shape[i].resize(obj.dimensions, dimensions);
                shape[i].rotate(rotation);
                shape[i].transelate(position);
                Console.WriteLine(shape[i].vertices[0] + " " + shape[i].vertices[1] + " " + shape[i].vertices[2]);
            }

            Vector3 right = rotation * Vector3.UnitX;
            Vector3 up = rotation * Vector3.UnitY;
            Vector3 forward = rotation * Vector3.UnitZ;

            Vector3 ful = position - forward * dimensions.Z + up * dimensions.Y - right * dimensions.X;
            Vector3 fur = position - forward * dimensions.Z + up * dimensions.Y + right * dimensions.X;
            Vector3 fdl = position - forward * dimensions.Z - up * dimensions.Y - right * dimensions.X;
            Vector3 fdr = position - forward * dimensions.Z - up * dimensions.Y + right * dimensions.X;
            Vector3 bul = position + forward * dimensions.Z + up * dimensions.Y - right * dimensions.X;
            Vector3 bur = position + forward * dimensions.Z + up * dimensions.Y + right * dimensions.X;
            Vector3 bdl = position + forward * dimensions.Z - up * dimensions.Y - right * dimensions.X;
            Vector3 bdr = position + forward * dimensions.Z - up * dimensions.Y + right * dimensions.X;

            boundingBox = new triangle[12];
            boundingBox[0] = new triangle(ful, fdl, fur);
            boundingBox[1] = new triangle(fur, fdl, fdr);
            boundingBox[2] = new triangle(ful, fur, bul);
            boundingBox[3] = new triangle(fur, bur, bul);
            boundingBox[4] = new triangle(fur, bdr, bur);
            boundingBox[5] = new triangle(fur, fdr, bdr);
            boundingBox[6] = new triangle(fdl, bdl, fdr);
            boundingBox[7] = new triangle(bdl, bdr, fdr);
            boundingBox[8] = new triangle(ful, bul, bdl);
            boundingBox[9] = new triangle(ful, bdl, fdl);
            boundingBox[10] = new triangle(bul, bur, bdl);
            boundingBox[11] = new triangle(bdl, bur, bdr);
        }

        public mesh(obj obj, Vector3 _position, Vector3 _color, Quaternion _rotation, float multiplier, float _absorption = 100)
        {
            position = _position;
            color = _color;
            rotation = _rotation;
            rotation.Normalize();
            absorption = _absorption;

            shape = new triangle[obj.shape.Length];
            for (int i = 0; i < shape.Length; i++)
            {
                shape[i] = new triangle(obj.shape[i].vertices[0], obj.shape[i].vertices[1], obj.shape[i].vertices[2]);
                shape[i].resize(obj.dimensions, multiplier);
                shape[i].rotate(rotation);
                shape[i].transelate(position);
            }
            dimensions = obj.dimensions * multiplier;
            Vector3 right = rotation * Vector3.UnitX;
            Vector3 up = rotation * Vector3.UnitY;
            Vector3 forward = rotation * Vector3.UnitZ;

            Vector3 ful = position - forward * dimensions.Z + up * dimensions.Y - right * dimensions.X;
            Vector3 fur = position - forward * dimensions.Z + up * dimensions.Y + right * dimensions.X;
            Vector3 fdl = position - forward * dimensions.Z - up * dimensions.Y - right * dimensions.X;
            Vector3 fdr = position - forward * dimensions.Z - up * dimensions.Y + right * dimensions.X;
            Vector3 bul = position + forward * dimensions.Z + up * dimensions.Y - right * dimensions.X;
            Vector3 bur = position + forward * dimensions.Z + up * dimensions.Y + right * dimensions.X;
            Vector3 bdl = position + forward * dimensions.Z - up * dimensions.Y - right * dimensions.X;
            Vector3 bdr = position + forward * dimensions.Z - up * dimensions.Y + right * dimensions.X;

            boundingBox = new triangle[12];
            boundingBox[0] = new triangle(ful, fdl, fur);
            boundingBox[1] = new triangle(fur, fdl, fdr);
            boundingBox[2] = new triangle(ful, fur, bul);
            boundingBox[3] = new triangle(fur, bur, bul);
            boundingBox[4] = new triangle(fur, bdr, bur);
            boundingBox[5] = new triangle(fur, fdr, bdr);
            boundingBox[6] = new triangle(fdl, bdl, fdr);
            boundingBox[7] = new triangle(bdl, bdr, fdr);
            boundingBox[8] = new triangle(ful, bul, bdl);
            boundingBox[9] = new triangle(ful, bdl, fdl);
            boundingBox[10] = new triangle(bul, bur, bdl);
            boundingBox[11] = new triangle(bdl, bur, bdr);
        }
        public override void rayIntersection(ray ray)
        {
            for (int i = 0; i < boundingBox.Length; ++i)
            {
                float s = boundingBox[i].calcIntersection(ray.origin, ray.direction, false);
                if (s > 0)
                {
                    for (int j = 0; j < shape.Length; ++j)
                    {
                        float t = shape[j].calcIntersection(ray.origin, ray.direction, false);
                        if (t < ray.t && t > 0)
                        {
                            ray.t = t;
                            ray.nextColor = color;
                            ray.normal = shape[j].getNormal(Vector3.Zero);
                            ray.absorption = absorption;
                        }
                    }
                    return;
                }
            }
        }
    }
}

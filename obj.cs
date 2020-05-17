using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using OpenTK;
namespace Template
{
    class obj 
    {
        public triangle[] shape;
        public Vector3 dimensions;
        public obj(string name)
        {
            string[] lines = File.ReadAllLines(name);
            float[] minmax = new float[6];
            List<Vector3> vertices = new List<Vector3>();
            List<Vector3> normals = new List<Vector3>();
            List<triangle> triangles = new List<triangle>();
            int i = 0;
            while (i < lines.Length)
            {
                if (lines[i][0] == 'v' && lines[i][1] == ' ')
                {
                    string[] input = lines[i].Split(' ');
                    float x = float.Parse(input[1]);
                    float y = float.Parse(input[2]);
                    float z = float.Parse(input[3]);
                    minmax[0] = x < minmax[0] ? x : minmax[0];
                    minmax[1] = y < minmax[1] ? y : minmax[1];
                    minmax[2] = z < minmax[2] ? z : minmax[2];
                    minmax[3] = x > minmax[3] ? x : minmax[3];
                    minmax[4] = y > minmax[4] ? y : minmax[4];
                    minmax[5] = z > minmax[5] ? z : minmax[5];
                    vertices.Add(new Vector3(x, y, z));
                }
                else if (lines[i][0] == 'v' && lines[i][1] == 'n')
                {
                    break;
                }
                i++;
            }
            while(i < lines.Length)
            {
                if(lines[i][0] == 'v' && lines[i][1] == 'n')
                {
                    string[] input = lines[i].Split(' ');
                    normals.Add(new Vector3(float.Parse(input[1]), float.Parse(input[2]), float.Parse(input[3])));
                }
                else
                {
                    break;
                }
                i++;

            }
            while (i < lines.Length)
            {
                if (lines[i][0] == 'f')
                {
                    string[] input = lines[i].Split(' ');
                    int[] verts = new int[3];
                    for (int j = 1; j < input.Length; j++)
                    {
                        string[] nums = input[j].Split('/');
                        verts[j - 1] = int.Parse(nums[0]) - 1;
                    }
                    string[] index = input[1].Split('/');

                    triangles.Add(new triangle(vertices[verts[0]], vertices[verts[1]], vertices[verts[2]], normals[int.Parse(index[2]) - 1]));
                }
                i++;
            }
            shape = triangles.ToArray();
            dimensions = new Vector3(minmax[3] - minmax[0], minmax[4] - minmax[1], minmax[5] - minmax[2]) / 2;

            Vector3 centre = new Vector3(minmax[3] + minmax[0], minmax[4] + minmax[1], minmax[5] + minmax[2]) / 2;
            for(int j = 0; j < shape.Length; ++j)
            {
                shape[j].transelate(-centre);
            }
        }
    }
}

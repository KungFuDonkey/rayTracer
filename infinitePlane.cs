using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
namespace Template
{
    class infinitePlane : plane
    {
        public infinitePlane(float _d, Vector3 _color, Quaternion _orientation)
        {
            d = -_d;
            color = _color;
            normalVector = _orientation * Vector3.UnitZ;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Text;
using System.Threading.Tasks;

namespace SUPERDEATH.Scripts
{
    public class LineObject : GameObject
    {

        public LineObject(PrimitiveMesh p) : base(Transform.Zero(), p, RenderType.Primitive, isSolid:false)
        {

            transform = Transform.Zero();
            solid = false;
            mesh = p;

        }

    }
}

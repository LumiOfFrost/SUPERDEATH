using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SUPERDEATH.Scripts
{

    public enum RenderType
    {

        Model,
        Primitive,
        None

    }

    public class GameObject
    {

        public RenderType renderType;

        public virtual void Update(Main main, GameTime gameTime)
        {
            collider = new BoundingBox(transform.position - transform.scale / 2, transform.position + transform.scale / 2);
        }

        public virtual void Init()
        {

        }

        public string tag;

        public Transform transform;

        public Model model;

        public PrimitiveMesh mesh;

        public BoundingBox collider;

        public Vector3 velocity;

        public bool solid;

        public GameObject(Transform tform, Model mdl, RenderType rType, bool usesModel, string tg = "", bool isSolid = true)
        {

            tag = tg;

            model = mdl;

            transform = tform;

            velocity = Vector3.Zero;

            renderType = rType;

            solid = isSolid;

            collider = new BoundingBox(transform.position - transform.scale / 2, transform.position + transform.scale / 2);

        }
        public GameObject(Transform tform, PrimitiveMesh pMesh, RenderType rType, string tg = "", bool isSolid = true)
        {

            tag = tg;

            mesh = pMesh;

            transform = tform;

            velocity = Vector3.Zero;

            renderType = rType;

            solid = isSolid;

            collider = new BoundingBox(transform.position - transform.scale / 2, transform.position + transform.scale / 2);

        }

        public float GetDistance(Vector3 value2)
        {
            return (float)Math.Sqrt((transform.GetCenter().X - value2.X) * ((transform.GetCenter().X - value2.X) +
            ((transform.GetCenter().Y - value2.Y) * ((transform.GetCenter().Y - value2.Y) +
            ((transform.GetCenter().Z - value2.Z) * ((transform.GetCenter().Z - value2.Z)))))));
        }

    }

}

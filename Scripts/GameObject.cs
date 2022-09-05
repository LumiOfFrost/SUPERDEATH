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
        None

    }

    public class GameObject
    {

        public virtual void Update(Main main, GameTime gameTime)
        {
            collider = new BoundingBox(transform.position - transform.scale / 2, transform.position + transform.scale / 2);
        }

        public virtual void Init()
        {
            collider = new BoundingBox(transform.position - transform.scale / 2, transform.position + transform.scale / 2);
        }

        public RenderType renderType;

        public string tag;

        public Transform transform;

        public string model;

        public BoundingBox collider;

        public Vector3 velocity;

        public bool solid;

        public string texture;

        public GameObject(Transform tform, string modelName, string textureName, RenderType rType, string tg = "", bool isSolid = true)
        {

            renderType = rType;

            tag = tg;

            model = modelName;

            this.texture = textureName;

            transform = tform;

            velocity = Vector3.Zero;

            solid = isSolid;

            collider = new BoundingBox(transform.position - transform.scale / 2, transform.position + transform.scale / 2);

        }

        public GameObject()
        {



        }

        public float GetDistance(Vector3 value2)
        {
            return (float)Math.Sqrt((value2.X - transform.GetCenter().X) * ((value2.X - transform.GetCenter().X) +
            (value2.Y - (transform.GetCenter().Y) * ((value2.Y - transform.GetCenter().Y) +
            ((value2.Z - transform.GetCenter().Z) * ((value2.Z - transform.GetCenter().Z)))))));
        }

    }

}

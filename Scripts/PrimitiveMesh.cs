using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SUPERDEATH.Scripts
{
    public class PrimitiveMesh
    {

        public IndexBuffer indexBuffer;
        public VertexBuffer vertexBuffer;
        public Texture2D texture;
        public Color color;
        public PrimitiveType primitiveType;
        public Tri[] tris;

        public PrimitiveMesh(GraphicsDevice graphicsDevice, VertexPositionColorNormalTexture[] vertices, Tri[] tr, Color col, Texture2D tex, PrimitiveType pType)
        {

            color = col;
            texture = tex;
            tris = tr;
            primitiveType = pType;

            vertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionColorNormalTexture), vertices.Length, BufferUsage.WriteOnly);
            vertexBuffer.SetData<VertexPositionColorNormalTexture>(vertices);

            short[] indies;

            if (tris != null)
            {

                indies = new short[tris.Length * 3];

                for (int i = 0; i < tris.Length; i++)
            {

                for (int j = 0; j < 3; j++)
                {

                    indies[i * 3 + j] = tris[i].indices[j];

                }

            }

            } else
            {

                indies = new short[2] { 0, 1 };

            }

            indexBuffer = new IndexBuffer(graphicsDevice, typeof(short), indies.Length, BufferUsage.WriteOnly);
            indexBuffer.SetData(indies);

        }

        public void Draw(GraphicsDevice graphicsDevice, BasicEffect basicEffect)
        {

            graphicsDevice.SetVertexBuffer(vertexBuffer);
            graphicsDevice.Indices = indexBuffer;

            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.CullClockwiseFace;
            graphicsDevice.RasterizerState = rasterizerState;

            basicEffect.EnableDefaultLighting();

            if (texture != null)
            {

                basicEffect.Texture = texture;
                basicEffect.TextureEnabled = true;

            } else
            {

                basicEffect.TextureEnabled = false;

            }

            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                if (primitiveType != PrimitiveType.LineList)
                {
                    graphicsDevice.DrawIndexedPrimitives(primitiveType, 0, 0, tris.Length);
                } else
                {
                    graphicsDevice.DrawIndexedPrimitives(primitiveType, 0, 0, 2);
                }
                
            }

        }

        public static PrimitiveMesh GetLine(GraphicsDevice graphicsDevice, Vector3 position, Vector3 position2, Color col)
        {

            return new PrimitiveMesh
                (

                graphicsDevice,
                new VertexPositionColorNormalTexture[2]
                {
                    new VertexPositionColorNormalTexture(position, col, MathUtils.NormalizeVector3(position - position2), Vector2.Zero),
                    new VertexPositionColorNormalTexture(position2, col, MathUtils.NormalizeVector3(position2 - position), Vector2.Zero)
                },
                null,
                col,
                null,
                PrimitiveType.LineList

                );

        }

        public static PrimitiveMesh GetCuboid(GraphicsDevice graphicsDevice, Vector3 position, Vector3 scale, Color col, Texture2D tex = null)
        {

            return new PrimitiveMesh
                (graphicsDevice,
                new VertexPositionColorNormalTexture[8]
                {
                    new VertexPositionColorNormalTexture(new Vector3(position.X - scale.X / 2, position.Y + scale.Y / 2, position.Z - scale.Z / 2), col, new Vector3(-1, 1, -1), Vector2.Zero),
                    new VertexPositionColorNormalTexture(new Vector3(position.X + scale.X / 2, position.Y + scale.Y / 2, position.Z - scale.Z / 2), col, new Vector3(1, 1, -1), Vector2.Zero),
                    new VertexPositionColorNormalTexture(new Vector3(position.X - scale.X / 2, position.Y + scale.Y / 2, position.Z + scale.Z / 2), col, new Vector3(-1, 1, 1), Vector2.Zero),
                    new VertexPositionColorNormalTexture(new Vector3(position.X + scale.X / 2, position.Y + scale.Y / 2, position.Z + scale.Z / 2), col, new Vector3(1, 1,1), Vector2.Zero),
                    new VertexPositionColorNormalTexture(position - scale / 2, col, new Vector3(-1, -1, -1), Vector2.Zero),
                    new VertexPositionColorNormalTexture(new Vector3(position.X + scale.X / 2, position.Y - scale.Y / 2, position.Z - scale.Z / 2), col, new Vector3(1, -1, -1), Vector2.Zero),
                    new VertexPositionColorNormalTexture(new Vector3(position.X - scale.X / 2, position.Y - scale.Y / 2, position.Z + scale.Z / 2), col, new Vector3(-1, -1, 1), Vector2.Zero),
                    new VertexPositionColorNormalTexture(new Vector3(position.X + scale.X / 2, position.Y - scale.Y / 2, position.Z + scale.Z / 2), col, new Vector3(1, -1, 1), Vector2.Zero)
                },
                new Tri[12]
                {
                    //Up
                    new Tri(1, 0, 2),
                    new Tri(2, 3, 1),
                    //Down
                    new Tri(6, 4, 5),
                    new Tri(5, 7, 6),
                    //Front
                    new Tri(3, 2, 6),
                    new Tri(6, 7, 3),
                    //Back
                    new Tri(5, 0, 1),
                    new Tri(5, 4, 0),
                    //Left
                    new Tri(2, 0, 4),
                    new Tri(4, 6, 2),
                    //Right
                    new Tri(1, 3, 7),
                    new Tri(7, 5, 1)
                },
                col,
                tex,
                PrimitiveType.TriangleList);

        }

        public static Vector3 CalculateSurfaceNormal(Vector3 p1, Vector3 p2, Vector3 p3)
        {

            Vector3 u = p2 - p1;
            Vector3 v = p3 - p1;

            Vector3 normal;

            normal.X = u.Y * v.Z - u.Z * v.Y;
            normal.Y = u.Z * v.X - u.X * v.Z;
            normal.Z = u.X * v.Y - u.Y * u.X;

            return normal;

        }

    }

    public class Tri
    {

        public short[] indices;

        public Tri(short in1, short in2, short in3)
        {

            indices = new short[3];

            indices[0] = in1;
            indices[1] = in2;
            indices[2] = in3;

        }

    }

}

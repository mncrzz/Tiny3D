using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Tiny3DEngine
{
    public class Objects
    {
        public float floorLevel = -2.0f;

        public int CreateDisplayList()
        {
            int index = GL.GenLists(1);
            GL.NewList(index, ListMode.Compile);
            this.CreateCube(0.0f, 0.0f, 0.0f);
            GL.EndList();
            return index;
        }

        public int CreateGround()
        {
            int displayList = GL.GenLists(1);
            GL.NewList(displayList, ListMode.Compile);

            GL.Color4(Color4.DarkGray);
            GL.Begin(PrimitiveType.Quads);

            float size = 7.5f;
            float y = floorLevel;

            GL.Vertex3(-size, y, -size);
            GL.Vertex3(size, y, -size);
            GL.Vertex3(size, y, size);
            GL.Vertex3(-size, y, size);

            GL.End();
            GL.EndList();

            return displayList;
        }

        public void CreateCube(float x, float y, float z)
        {
            Vector3[] vertices = {
                new Vector3(-0.5f + x, -0.5f + y,  0.5f + z),
                new Vector3( 0.5f + x, -0.5f + y,  0.5f + z),
                new Vector3( 0.5f + x,  0.5f + y,  0.5f + z),
                new Vector3(-0.5f + x,  0.5f + y,  0.5f + z),
                new Vector3(-0.5f + x, -0.5f + y, -0.5f + z),
                new Vector3( 0.5f + x, -0.5f + y, -0.5f + z),
                new Vector3( 0.5f + x,  0.5f + y, -0.5f + z),
                new Vector3(-0.5f + x,  0.5f + y, -0.5f + z)
            };

            int[][] faces = {
                new int[] {0, 1, 2, 3},
                new int[] {1, 5, 6, 2},
                new int[] {5, 4, 7, 6},
                new int[] {4, 0, 3, 7},
                new int[] {3, 2, 6, 7},
                new int[] {4, 5, 1, 0}
            };

            Color4[] colors = {
                Color4.Red, Color4.Green, Color4.Blue,
                Color4.Yellow, Color4.Magenta, Color4.Cyan
            };

            for (int i = 0; i < 6; i++)
            {
                GL.Color4(colors[i]);
                GL.Begin(PrimitiveType.Triangles);

                int v0 = faces[i][0], v1 = faces[i][1], v2 = faces[i][2], v3 = faces[i][3];

                GL.Vertex3(vertices[v0]); GL.Vertex3(vertices[v1]); GL.Vertex3(vertices[v2]);
                GL.Vertex3(vertices[v0]); GL.Vertex3(vertices[v2]); GL.Vertex3(vertices[v3]);

                GL.End();
            }
        }

        public Vector3 GetCubeMapTexCoord(int face, int vertex)
        {
            Vector3[,] coords = {
                { new Vector3(1, -1, 1), new Vector3(1, -1, -1), new Vector3(1, 1, -1), new Vector3(1, 1, 1) },
                { new Vector3(-1, -1, -1), new Vector3(-1, -1, 1), new Vector3(-1, 1, 1), new Vector3(-1, 1, -1) },
                { new Vector3(-1, 1, -1), new Vector3(-1, 1, 1), new Vector3(1, 1, 1), new Vector3(1, 1, -1) },
                { new Vector3(-1, -1, 1), new Vector3(1, -1, 1), new Vector3(1, -1, -1), new Vector3(-1, -1, -1) },
                { new Vector3(1, -1, -1), new Vector3(1, -1, 1), new Vector3(1, 1, 1), new Vector3(1, 1, -1) },
                { new Vector3(-1, -1, 1), new Vector3(-1, -1, -1), new Vector3(-1, 1, -1), new Vector3(-1, 1, 1) }
            };
            return coords[face, vertex];
        }
    }
}
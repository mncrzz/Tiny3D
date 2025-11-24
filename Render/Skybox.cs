using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System.Drawing;

namespace Tiny3DEngine
{
    public class SkyBox
    {
        private int skyboxDisplayList = 0;
        public bool showSkybox = true;
        private int skyboxTexture = 0;
        private Objects objects = new Objects();

        public void Start()
        {
            skyboxTexture = LoadSkyboxTextures();
            skyboxDisplayList = CreateSkybox();
        }

        public void Unload()
        {
            GL.DeleteLists(skyboxDisplayList, 1);
            GL.DeleteTexture(skyboxTexture);
        }

        public void Render(Matrix4 viewMatrix)
        {
            if (showSkybox)
            {
                GL.DepthMask(false);
                GL.MatrixMode(MatrixMode.Modelview);
                GL.LoadIdentity();

                Matrix4 skyboxView = viewMatrix;
                skyboxView.Row3 = new Vector4(0, 0, 0, 1);
                GL.LoadMatrix(ref skyboxView);

                GL.CallList(skyboxDisplayList);
                GL.DepthMask(true);
            }
        }

        private int LoadSkyboxTextures()
        {
            string[] faces = {
                "skybox/right.png",
                "skybox/left.png",
                "skybox/top.png",
                "skybox/bottom.png",
                "skybox/front.png",
                "skybox/back.png"
            };

            int textureID = GL.GenTexture();
            GL.BindTexture(TextureTarget.TextureCubeMap, textureID);

            TextureTarget[] targets = {
                TextureTarget.TextureCubeMapPositiveX, TextureTarget.TextureCubeMapNegativeX,
                TextureTarget.TextureCubeMapPositiveY, TextureTarget.TextureCubeMapNegativeY,
                TextureTarget.TextureCubeMapPositiveZ, TextureTarget.TextureCubeMapNegativeZ
            };

            for (int i = 0; i < 6; i++)
            {
                if (!File.Exists(faces[i]))
                {
                    Console.WriteLine($"Skipping missing texture: {faces[i]}");
                    continue;
                }

                try
                {
                    using (var bitmap = new Bitmap(faces[i]))
                    {
                        var data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                            System.Drawing.Imaging.ImageLockMode.ReadOnly,
                            System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                        GL.TexImage2D(targets[i], 0, PixelInternalFormat.Rgba,
                            bitmap.Width, bitmap.Height, 0, PixelFormat.Bgra,
                            PixelType.UnsignedByte, data.Scan0);

                        bitmap.UnlockBits(data);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"ERROR loading texture {faces[i]}: {ex.Message}");
                }
            }

            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);

            return textureID;
        }

        private int CreateSkybox()
        {
            int displayList = GL.GenLists(1);
            GL.NewList(displayList, ListMode.Compile);

            float size = 10.0f;
            Vector3[] vertices = {
                new Vector3(-size, -size, size), new Vector3(size, -size, size), new Vector3(size, size, size), new Vector3(-size, size, size),
                new Vector3(-size, -size, -size), new Vector3(-size, size, -size), new Vector3(size, size, -size), new Vector3(size, -size, -size),
                new Vector3(-size, size, -size), new Vector3(-size, size, size), new Vector3(size, size, size), new Vector3(size, size, -size),
                new Vector3(-size, -size, -size), new Vector3(size, -size, -size), new Vector3(size, -size, size), new Vector3(-size, -size, size),
                new Vector3(size, -size, -size), new Vector3(size, size, -size), new Vector3(size, size, size), new Vector3(size, -size, size),
                new Vector3(-size, -size, -size), new Vector3(-size, -size, size), new Vector3(-size, size, size), new Vector3(-size, size, -size)
            };

            GL.Enable(EnableCap.TextureCubeMap);
            GL.BindTexture(TextureTarget.TextureCubeMap, skyboxTexture);

            for (int i = 0; i < 6; i++)
            {
                GL.Begin(PrimitiveType.Quads);
                for (int j = 0; j < 4; j++)
                {
                    Vector3 texCoord = objects.GetCubeMapTexCoord(i, j);
                    GL.TexCoord3(texCoord);
                    GL.Vertex3(vertices[i * 4 + j]);
                }
                GL.End();
            }

            GL.Disable(EnableCap.TextureCubeMap);
            GL.EndList();
            return displayList;
        }
    }
}
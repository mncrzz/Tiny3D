using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL;

namespace Tiny3DEngine
{
    public class FreeCamera
    {
        public Vector3 Position { get; set; }
        public Vector3 Front { get; private set; }
        public Vector3 Up { get; private set; }
        public Vector3 Right { get; private set; }
        public float Yaw { get; private set; }
        public float Pitch { get; private set; }
        public float MovementSpeed { get; set; }
        public float MouseSensitivity { get; set; }
        public bool firstMove = true;
        private Vector2 lastMousePos;
        public bool cameraActive = false;

        public void Start()
        {
            Position = new Vector3(0.0f, 2.0f, 8.0f);
            Front = -Vector3.UnitZ;
            Up = Vector3.UnitY;
            Right = Vector3.UnitX;
            Yaw = -90.0f;
            Pitch = -10.0f;
            MovementSpeed = 5.0f;
            MouseSensitivity = 0.1f;
            UpdateVectors();
        }

        public Matrix4 GetViewMatrix()
        {
            return Matrix4.LookAt(Position, Position + Front, Up);
        }

        public void SetupProjection(float fov, float x, float y)
        {
            GL.MatrixMode(MatrixMode.Projection);
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(
                MathHelper.DegreesToRadians(fov),
                x / y,
                0.1f,
                100.0f
            );
            GL.LoadMatrix(ref projection);
            GL.MatrixMode(MatrixMode.Modelview);
        }

        public void ProcessKeyboard(Vector3 direction, float deltaTime)
        {
            float velocity = MovementSpeed * deltaTime;
            Vector3 move = Vector3.Zero;

            Vector3 horizontalFront = Vector3.Normalize(new Vector3(Front.X, 0, Front.Z));
            Vector3 horizontalRight = Vector3.Normalize(new Vector3(Right.X, 0, Right.Z));

            move += direction.Z * horizontalFront;
            move += direction.X * horizontalRight;
            move += direction.Y * Vector3.UnitY;

            if (move != Vector3.Zero)
            {
                Position += Vector3.Normalize(move) * velocity;
            }
        }

        public void Update(MouseState mouseState, KeyboardState keyboardState, float deltaTime)
        {
            if (cameraActive)
            {
                Vector3 moveDirection = Vector3.Zero;

                if (keyboardState.IsKeyDown(Keys.W))
                    moveDirection += new Vector3(0, 0, 1);
                if (keyboardState.IsKeyDown(Keys.S))
                    moveDirection += new Vector3(0, 0, -1);
                if (keyboardState.IsKeyDown(Keys.A))
                    moveDirection += new Vector3(-1, 0, 0);
                if (keyboardState.IsKeyDown(Keys.D))
                    moveDirection += new Vector3(1, 0, 0);
                if (keyboardState.IsKeyDown(Keys.Q))
                    moveDirection += new Vector3(0, -1, 0);
                if (keyboardState.IsKeyDown(Keys.E))
                    moveDirection += new Vector3(0, 1, 0);

                float speedMultiplier = keyboardState.IsKeyDown(Keys.LeftShift) ? 2.0f : 1.0f;
                MovementSpeed = 5.0f * speedMultiplier;

                if (moveDirection != Vector3.Zero)
                {
                    ProcessKeyboard(moveDirection, deltaTime);
                }

                if (firstMove)
                {
                    lastMousePos = new Vector2(mouseState.X, mouseState.Y);
                    firstMove = false;
                }
                else
                {
                    float deltaX = mouseState.X - lastMousePos.X;
                    float deltaY = mouseState.Y - lastMousePos.Y;
                    lastMousePos = new Vector2(mouseState.X, mouseState.Y);

                    ProcessMouseMovement(deltaX, deltaY);
                }
            }
        }

        public void ProcessMouseMovement(float xOffset, float yOffset, bool constrainPitch = true)
        {
            xOffset *= MouseSensitivity;
            yOffset *= MouseSensitivity;

            Yaw += xOffset;
            Pitch -= yOffset;

            if (constrainPitch)
                Pitch = MathHelper.Clamp(Pitch, -89.0f, 89.0f);

            UpdateVectors();
        }

        private void UpdateVectors()
        {
            Vector3 front;
            front.X = MathF.Cos(MathHelper.DegreesToRadians(Yaw)) * MathF.Cos(MathHelper.DegreesToRadians(Pitch));
            front.Y = MathF.Sin(MathHelper.DegreesToRadians(Pitch));
            front.Z = MathF.Sin(MathHelper.DegreesToRadians(Yaw)) * MathF.Cos(MathHelper.DegreesToRadians(Pitch));

            Front = Vector3.Normalize(front);
            Right = Vector3.Normalize(Vector3.Cross(Front, Vector3.UnitY));
            Up = Vector3.Normalize(Vector3.Cross(Right, Front));
        }
    }
}
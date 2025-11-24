using Dear_ImGui_Sample.Backends;
using ImGuiNET;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using opentk_learn.Editor;
using System.Runtime.InteropServices;
using Tiny3DEngine;

namespace opentk_learn
{
    public class Game : GameWindow
    {
        private FreeCamera camera;
        private UI ui;
        private SkyBox skybox;
        private Objects objects;
        private CubePhysics cubePhysics;
        private EditorUI editorUI;

        private float frameTime = 0.0f;
        private int fps = 0;
        private int indDisplayList = 0;
        private float rotationAngleX = 0.0f;
        private float rotationAngleY = 0.0f;
        private float rotationAngleZ = 0.0f;
        private int displayedFps = 0;
        public float fov = 45.0f;
        private bool isWireframe = false;
        private bool physicsEnabled = false;
        private int groundDisplayList = 0;
        private bool _isPlaying = false;

        public Game(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
            Console.WriteLine("-----INFO-----");
            Console.WriteLine(GL.GetString(StringName.Version));
            Console.WriteLine(GL.GetString(StringName.Vendor));
            Console.WriteLine(GL.GetString(StringName.Renderer));
            Console.WriteLine(GL.GetString(StringName.ShadingLanguageVersion));
            Console.WriteLine("-----INFO-----");
            Console.WriteLine("\nStarted Tiny3D!");

            VSync = VSyncMode.On;
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            Start();
        }

        private void Start()
        {
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Less);
            GL.DebugMessageCallback(DebugProcCallback, nint.Zero);
            GL.Enable(EnableCap.DebugOutput);
            GL.Enable(EnableCap.DebugOutputSynchronous);

            camera = new FreeCamera();
            cubePhysics = new CubePhysics();
            objects = new Objects();
            skybox = new SkyBox();
            editorUI = new EditorUI();
            ui = new UI();

            camera.Start();
            cubePhysics.Start();
            skybox.Start();
            ui.Start();

            cubePhysics.SetPosition(new Vector3(0, 3, 0));
            groundDisplayList = objects.CreateGround();
            camera.SetupProjection(fov, Size.X, Size.Y);

            ImguiImplOpenTK4.Init(this);
            ImguiImplOpenGL3.Init();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, e.Width, e.Height);
            camera.SetupProjection(fov, Size.X, Size.Y);
            var io = ImGui.GetIO();
            io.DisplaySize = new System.Numerics.Vector2(e.Width, e.Height);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            frameTime += (float)args.Time;
            fps++;
            if (frameTime >= 1.0f)
            {
                displayedFps = fps;
                Title = $"Tiny3D | FPS - {fps}";
                frameTime = 0.0f;
                fps = 0;
            }

            if (physicsEnabled)
            {
                cubePhysics.Update((float)args.Time, objects.floorLevel);
                if (cubePhysics.LinearVelocity.Length < 0.1f && cubePhysics.Position.Y <= -1.3f)
                {
                    cubePhysics.WakeUp();
                }
            }

            camera.Update(MouseState, KeyboardState, (float)args.Time);

            if (KeyboardState.IsKeyPressed(Keys.Tab))
            {
                camera.cameraActive = !camera.cameraActive;
                if (camera.cameraActive)
                {
                    CursorState = CursorState.Grabbed;
                    camera.firstMove = true;
                }
                else
                {
                    CursorState = CursorState.Normal;
                }
            }

            if (KeyboardState.IsKeyDown(Keys.Escape))
            {
                Close();
            }
            base.OnUpdateFrame(args);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            ImguiImplOpenGL3.NewFrame();
            ImguiImplOpenTK4.NewFrame();
            ImGui.NewFrame();

            GL.ClearColor(Color4.Black);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            skybox.Render(camera.GetViewMatrix());

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            Matrix4 view = camera.GetViewMatrix();
            GL.LoadMatrix(ref view);

            GL.PolygonMode(MaterialFace.FrontAndBack, isWireframe ? PolygonMode.Line : PolygonMode.Fill);
            GL.CallList(groundDisplayList);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.LoadMatrix(ref view);

            if (physicsEnabled)
            {
                Matrix4 modelMatrix = cubePhysics.GetModelMatrix();
                GL.MultMatrix(ref modelMatrix);
            }
            else
            {
                GL.Translate(Vector3.Zero);
                GL.Rotate(rotationAngleX, 1.0f, 0.0f, 0.0f);
                GL.Rotate(rotationAngleY, 0.0f, 1.0f, 0.0f);
                GL.Rotate(rotationAngleZ, 0.0f, 0.0f, 1.0f);
            }

            if (!physicsEnabled && !_isPlaying)
            {
                rotationAngleX = (rotationAngleX + 40.0f * (float)args.Time) % 360f;
                rotationAngleY = (rotationAngleY + 50.0f * (float)args.Time) % 360f;
                rotationAngleZ = (rotationAngleZ + 60.0f * (float)args.Time) % 360f;
            }

            GL.PolygonMode(MaterialFace.FrontAndBack, isWireframe ? PolygonMode.Line : PolygonMode.Fill);
            objects.CreateCube(0.0f, 0.0f, 0.0f);

            if (editorUI != null)
            {
                editorUI.Draw(displayedFps, ref isWireframe, ref fov, this, ref physicsEnabled, cubePhysics,
                             Vector3.Zero,
                             () => camera.SetupProjection(fov, Size.X, Size.Y),
                             ref rotationAngleX, ref rotationAngleY, ref rotationAngleZ);
            }

            ImGui.Render();
            ImguiImplOpenGL3.RenderDrawData(ImGui.GetDrawData());

            if (ImGui.GetIO().ConfigFlags.HasFlag(ImGuiConfigFlags.ViewportsEnable))
            {
                ImGui.UpdatePlatformWindows();
                ImGui.RenderPlatformWindowsDefault();
                Context.MakeCurrent();
            }

            SwapBuffers();
            base.OnRenderFrame(args);
        }

        public readonly static DebugProc DebugProcCallback = Window_DebugProc;

        public void OnClosed()
        {
            ImguiImplOpenGL3.Shutdown();
            ImguiImplOpenTK4.Shutdown();
        }

        protected override void OnUnload()
        {
            cubePhysics?.Dispose();
            GL.DeleteLists(indDisplayList, 1);
            skybox.Unload();
            base.OnUnload();
        }
        private static void Window_DebugProc(DebugSource source, DebugType type, int id, DebugSeverity severity, int length, nint messagePtr, nint userParam)
        {
            string message = Marshal.PtrToStringAnsi(messagePtr, length);
            bool showMessage = source != DebugSource.DebugSourceApplication;

            if (showMessage)
            {
                switch (severity)
                {
                    case DebugSeverity.DebugSeverityHigh: Console.Error.WriteLine($"Error: [{source}] {message}"); break;
                    case DebugSeverity.DebugSeverityMedium: Console.WriteLine($"Warning: [{source}] {message}"); break;
                    case DebugSeverity.DebugSeverityLow: Console.WriteLine($"Info: [{source}] {message}"); break;
                    default: Console.WriteLine($"[{source}] {message}"); break;
                }
            }
        }
    }
}
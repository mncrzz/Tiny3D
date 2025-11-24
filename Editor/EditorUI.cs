using ImGuiNET;
using Tiny3DEngine;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;

namespace opentk_learn.Editor
{
    public class EditorUI
    {
        private bool _showDemoWindow = false;
        private bool _showMenu = true;
        private System.Numerics.Vector3 _impulseForce = new System.Numerics.Vector3(0, 10, 0);
        private System.Numerics.Vector3 _angularImpulse = new System.Numerics.Vector3(2, 2, 2);
        private System.Numerics.Vector3 _cubePosition = new System.Numerics.Vector3(0, 3, 0);

        public void Draw(int fps, ref bool isWireframe, ref float fov, Game game,
               ref bool physicsEnabled, CubePhysics cubePhysics, Vector3 cubePosition, Action setupProjection,
               ref float rotationAngleX, ref float rotationAngleY, ref float rotationAngleZ)
        {
            DrawMainMenuBar();
            if (_showDemoWindow)
                ImGui.ShowDemoWindow(ref _showDemoWindow);
            if (_showMenu)
            {
                DrawMenu(ref physicsEnabled, cubePhysics, ref cubePosition, ref isWireframe, ref fov, setupProjection, game,
                        ref rotationAngleX, ref rotationAngleY, ref rotationAngleZ);
            }
        }

        private void DrawMainMenuBar()
        {
            if (ImGui.BeginMainMenuBar())
            {
                if (ImGui.BeginMenu("File"))
                {
                    if (ImGui.MenuItem("New "))
                    {

                    }
                    if (ImGui.MenuItem("Save"))
                    {

                    }
                    if (ImGui.MenuItem("Load"))
                    {

                    }
                    ImGui.Separator();
                    if (ImGui.MenuItem("Exit"))
                    {

                    }
                    ImGui.EndMenu();
                }

                if (ImGui.BeginMenu("View"))
                {
                    ImGui.MenuItem("Menu", null, ref _showMenu);
                    ImGui.MenuItem("ImGui Demo", null, ref _showDemoWindow);
                    ImGui.EndMenu();
                }

                ImGui.EndMainMenuBar();
            }
        }

        private void DrawMenu(ref bool physicsEnabled, CubePhysics cubePhysics, ref Vector3 cubePosition,
                            ref bool isWireframe, ref float fov, Action setupProjection, Game game,
                            ref float rotationAngleX, ref float rotationAngleY, ref float rotationAngleZ)
        {
            ImGui.Begin("Editor Menu");

            ImGui.Checkbox("Enable Physics", ref physicsEnabled);
            ImGui.Separator();

            if (physicsEnabled)
            {
                ImGui.Text("Cube State:");
                ImGui.Text($"Position: ({cubePhysics.Position.X:F2}, {cubePhysics.Position.Y:F2}, {cubePhysics.Position.Z:F2})");
                ImGui.Text($"Velocity: ({cubePhysics.LinearVelocity.X:F2}, {cubePhysics.LinearVelocity.Y:F2}, {cubePhysics.LinearVelocity.Z:F2})");

                ImGui.Separator();

                if (ImGui.Button("Add Impulse"))
                {
                    cubePhysics.AddRandomImpulse();
                }
                ImGui.SameLine();
                if (ImGui.Button("Reset Physics"))
                {
                    cubePhysics.ResetPhysics();
                }
            }

            ImGui.Separator();

            ImGui.Text("Graphics Settings");

            ImGui.Checkbox("Wireframe Mode", ref isWireframe);
            if (ImGui.SliderFloat("FOV", ref fov, 30.0f, 120.0f))
            {
                setupProjection();
            }

            ImGui.Separator();
            ImGui.Text($"Window Size: {game.Size.X} x {game.Size.Y}");

            ImGui.End();
        }
    }
}
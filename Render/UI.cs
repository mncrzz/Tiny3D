using Dear_ImGui_Sample.Backends;
using ImGuiNET;

namespace Tiny3DEngine
{
    public class UI
    {
        public void Start()
        {
            ImGui.CreateContext();
            ImGuiIOPtr io = ImGui.GetIO();
            io.ConfigFlags |= ImGuiConfigFlags.NavEnableKeyboard;
            io.ConfigFlags &= ~ImGuiConfigFlags.DockingEnable;
            io.ConfigFlags &= ~ImGuiConfigFlags.ViewportsEnable;
            SetupImGuiStyle();
        }

        public void SetupImGuiStyle()
        {
            var style = ImGui.GetStyle();
            var colors = style.Colors;

            style.WindowRounding = 8.0f;
            style.ChildRounding = 6.0f;
            style.FrameRounding = 6.0f;
            style.PopupRounding = 6.0f;
            style.ScrollbarRounding = 8.0f;
            style.GrabRounding = 6.0f;

            style.WindowPadding = new System.Numerics.Vector2(12, 12);
            style.FramePadding = new System.Numerics.Vector2(8, 6);
            style.ItemSpacing = new System.Numerics.Vector2(8, 6);
            style.ItemInnerSpacing = new System.Numerics.Vector2(6, 6);
            style.ScrollbarSize = 14.0f;
            style.GrabMinSize = 12.0f;

            style.WindowBorderSize = 1.0f;
            style.ChildBorderSize = 1.0f;
            style.PopupBorderSize = 1.0f;
            style.FrameBorderSize = 1.0f;

            colors[(int)ImGuiCol.Text] = new System.Numerics.Vector4(0.95f, 0.95f, 0.95f, 1.0f);
            colors[(int)ImGuiCol.TextDisabled] = new System.Numerics.Vector4(0.5f, 0.5f, 0.5f, 1.0f);
            colors[(int)ImGuiCol.WindowBg] = new System.Numerics.Vector4(0.12f, 0.12f, 0.12f, 0.95f);
            colors[(int)ImGuiCol.ChildBg] = new System.Numerics.Vector4(0.15f, 0.15f, 0.15f, 1.0f);
            colors[(int)ImGuiCol.PopupBg] = new System.Numerics.Vector4(0.1f, 0.1f, 0.1f, 0.95f);
            colors[(int)ImGuiCol.Border] = new System.Numerics.Vector4(0.3f, 0.3f, 0.3f, 0.5f);
            colors[(int)ImGuiCol.BorderShadow] = new System.Numerics.Vector4(0.0f, 0.0f, 0.0f, 0.0f);
            colors[(int)ImGuiCol.FrameBg] = new System.Numerics.Vector4(0.2f, 0.2f, 0.2f, 1.0f);
            colors[(int)ImGuiCol.FrameBgHovered] = new System.Numerics.Vector4(0.3f, 0.3f, 0.3f, 1.0f);
            colors[(int)ImGuiCol.FrameBgActive] = new System.Numerics.Vector4(0.35f, 0.35f, 0.35f, 1.0f);
            colors[(int)ImGuiCol.TitleBg] = new System.Numerics.Vector4(0.08f, 0.08f, 0.08f, 1.0f);
            colors[(int)ImGuiCol.TitleBgActive] = new System.Numerics.Vector4(0.15f, 0.15f, 0.15f, 1.0f);
            colors[(int)ImGuiCol.TitleBgCollapsed] = new System.Numerics.Vector4(0.08f, 0.08f, 0.08f, 0.75f);
            colors[(int)ImGuiCol.MenuBarBg] = new System.Numerics.Vector4(0.14f, 0.14f, 0.14f, 1.0f);
            colors[(int)ImGuiCol.ScrollbarBg] = new System.Numerics.Vector4(0.1f, 0.1f, 0.1f, 1.0f);
            colors[(int)ImGuiCol.ScrollbarGrab] = new System.Numerics.Vector4(0.3f, 0.3f, 0.3f, 1.0f);
            colors[(int)ImGuiCol.ScrollbarGrabHovered] = new System.Numerics.Vector4(0.4f, 0.4f, 0.4f, 1.0f);
            colors[(int)ImGuiCol.ScrollbarGrabActive] = new System.Numerics.Vector4(0.5f, 0.5f, 0.5f, 1.0f);
            colors[(int)ImGuiCol.CheckMark] = new System.Numerics.Vector4(0.9f, 0.9f, 0.9f, 1.0f);
            colors[(int)ImGuiCol.SliderGrab] = new System.Numerics.Vector4(0.6f, 0.6f, 0.6f, 1.0f);
            colors[(int)ImGuiCol.SliderGrabActive] = new System.Numerics.Vector4(0.8f, 0.8f, 0.8f, 1.0f);
            colors[(int)ImGuiCol.Button] = new System.Numerics.Vector4(0.25f, 0.25f, 0.25f, 1.0f);
            colors[(int)ImGuiCol.ButtonHovered] = new System.Numerics.Vector4(0.35f, 0.35f, 0.35f, 1.0f);
            colors[(int)ImGuiCol.ButtonActive] = new System.Numerics.Vector4(0.45f, 0.45f, 0.45f, 1.0f);
            colors[(int)ImGuiCol.Header] = new System.Numerics.Vector4(0.25f, 0.25f, 0.25f, 1.0f);
            colors[(int)ImGuiCol.HeaderHovered] = new System.Numerics.Vector4(0.35f, 0.35f, 0.35f, 1.0f);
            colors[(int)ImGuiCol.HeaderActive] = new System.Numerics.Vector4(0.4f, 0.4f, 0.4f, 1.0f);
            colors[(int)ImGuiCol.Separator] = new System.Numerics.Vector4(0.3f, 0.3f, 0.3f, 0.5f);
            colors[(int)ImGuiCol.SeparatorHovered] = new System.Numerics.Vector4(0.4f, 0.4f, 0.4f, 0.7f);
            colors[(int)ImGuiCol.SeparatorActive] = new System.Numerics.Vector4(0.5f, 0.5f, 0.5f, 1.0f);
            colors[(int)ImGuiCol.ResizeGrip] = new System.Numerics.Vector4(0.3f, 0.3f, 0.3f, 0.5f);
            colors[(int)ImGuiCol.ResizeGripHovered] = new System.Numerics.Vector4(0.4f, 0.4f, 0.4f, 0.7f);
            colors[(int)ImGuiCol.ResizeGripActive] = new System.Numerics.Vector4(0.5f, 0.5f, 0.5f, 1.0f);
            colors[(int)ImGuiCol.PlotLines] = new System.Numerics.Vector4(0.7f, 0.7f, 0.7f, 1.0f);
            colors[(int)ImGuiCol.PlotLinesHovered] = new System.Numerics.Vector4(0.9f, 0.9f, 0.9f, 1.0f);
            colors[(int)ImGuiCol.PlotHistogram] = new System.Numerics.Vector4(0.6f, 0.6f, 0.6f, 1.0f);
            colors[(int)ImGuiCol.PlotHistogramHovered] = new System.Numerics.Vector4(0.8f, 0.8f, 0.8f, 1.0f);
            colors[(int)ImGuiCol.TextSelectedBg] = new System.Numerics.Vector4(0.4f, 0.4f, 0.4f, 0.5f);
            colors[(int)ImGuiCol.DragDropTarget] = new System.Numerics.Vector4(0.7f, 0.7f, 0.7f, 0.9f);
        }
    }
}
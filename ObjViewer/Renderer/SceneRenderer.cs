using System;
using System.Collections.Generic;
using System.Numerics;
using GraphicsPipeline;
using GraphicsPipeline.Components.Clipping;
using GraphicsPipeline.Components.Interpolation;
using GraphicsPipeline.Components.Rasterization;
using GraphicsPipeline.Components.Shaders.Debug;
using Utils.Components;
using Utils.Utils;
namespace ObjViewer.Renderer;

public abstract class SceneRenderer<TFIn, TV, TF, TR, TC> : ISceneRenderer
    where TFIn : unmanaged
    where TV : IVertexShader<Vertex, TFIn>, new()
    where TF : IFragmentShader<TFIn>, new()
    where TR : IRasterizer<TFIn>, new()
    where TC : IClipper<TFIn>, new()
{
    protected class GizmosBuilder(SceneRenderer<TFIn, TV, TF, TR, TC> renderer)
    {
        public void SetViewProjection(in Matrix4x4 viewProjection) => renderer._gizmoVertexShader.ViewProjection = viewProjection;

        public void DrawTriangle(Vector3 a, Vector3 b, Vector3 c)
        {
            renderer._gizmoVertices.Add(a);
            renderer._gizmoVertices.Add(b);
            renderer._gizmoVertices.Add(c);
            renderer._gizmoIndices.Add(renderer._gizmoVertices.Count - 3);
            renderer._gizmoIndices.Add(renderer._gizmoVertices.Count - 2);
            renderer._gizmoIndices.Add(renderer._gizmoVertices.Count - 1);
        }

        public void DrawQuad(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
        {
            DrawTriangle(a, b, c);
            DrawTriangle(a, c, d);
        }

        public void DrawSphere(Vector3 center, float radius, int segments)
        {
            for (var i = 0; i <= segments; i++)
            {
                var phi = MathF.PI * i / segments;
                for (var j = 0; j <= segments; j++)
                {
                    var theta = MathF.PI * 2 * j / segments;
                    var x = MathF.Cos(theta) * MathF.Sin(phi);
                    var y = MathF.Cos(phi);
                    var z = MathF.Sin(theta) * MathF.Sin(phi);
                    renderer._gizmoVertices.Add(center + new Vector3(x, y, z) * radius);
                }
            }
        
            var offset = renderer._gizmoVertices.Count - (segments + 1) * (segments + 1);
            for (var i = 0; i < segments; i++)
            {
                for (var j = 0; j < segments; j++)
                {
                    var a = offset + i * (segments + 1) + j;
                    var b = a + 1;
                    var c = a + segments + 1;
                    var d = c + 1;
                    DrawQuad(renderer._gizmoVertices[a], renderer._gizmoVertices[b], renderer._gizmoVertices[d], renderer._gizmoVertices[c]);
                }
            }
        }

        public void DrawCircle(Vector3 center, float radius, int segments)
        {
            var angleStep = MathF.PI * 2 / segments;
            for (var i = 0; i < segments; i++)
            {
                Vector3 a = center + new Vector3(MathF.Cos(i * angleStep) * radius, 0, MathF.Sin(i * angleStep) * radius);
                Vector3 b = center + new Vector3(MathF.Cos((i + 1) * angleStep) * radius, 0, MathF.Sin((i + 1) * angleStep) * radius);
                DrawTriangle(center, a, b);
            }
        }

        public void DrawCube(Vector3 center, float size)
        {
            var halfSize = size / 2;
            Vector3 a = center + new Vector3(-halfSize, -halfSize, -halfSize);
            Vector3 b = center + new Vector3(halfSize, -halfSize, -halfSize);
            Vector3 c = center + new Vector3(halfSize, -halfSize, halfSize);
            Vector3 d = center + new Vector3(-halfSize, -halfSize, halfSize);
            Vector3 e = center + new Vector3(-halfSize, halfSize, -halfSize);
            Vector3 f = center + new Vector3(halfSize, halfSize, -halfSize);
            Vector3 g = center + new Vector3(halfSize, halfSize, halfSize);
            Vector3 h = center + new Vector3(-halfSize, halfSize, halfSize);

            DrawQuad(a, b, c, d);
            DrawQuad(e, f, g, h);
            DrawQuad(a, b, f, e);
            DrawQuad(d, c, g, h);
            DrawQuad(b, c, g, f);
            DrawQuad(a, d, h, e);
        }
    }

    private readonly GizmosBuilder _gizmosBuilder;
    
    private readonly GraphicsPipeline<Vector3, Vector3> _gizmoPipeline;

    private readonly GizmoVertexShader _gizmoVertexShader = new();
    private readonly GizmoFragmentShader _gizmoFragmentShader = new();
    private readonly ScanlineTriangleRasterizer<Vector3, VectorInterpolator> _gizmoRasterizer = new();

    private readonly List<Vector3> _gizmoVertices = [];
    private readonly List<int> _gizmoIndices = [];

    private readonly GraphicsPipeline<Vertex, TFIn> _graphicsPipeline;

    protected SceneRenderer(IPostProcessor? postProcessor = null)
    {
        _graphicsPipeline = new GraphicsPipeline<Vertex, TFIn>(VertexShader, FragmentShader, Rasterizer, Clipper, postProcessor);
        _gizmoPipeline = new GraphicsPipeline<Vector3, Vector3>(_gizmoVertexShader, _gizmoFragmentShader, _gizmoRasterizer, new SimpleClipper<Vector3>())
        {
            ScissorTestEnabled = true,
            DepthTestEnabled = false,
        };
        _gizmosBuilder = new GizmosBuilder(this);
    }

    protected TV VertexShader { get; } = new();
    protected TF FragmentShader { get; } = new();
    protected TR Rasterizer { get; } = new();
    protected TC Clipper { get; } = new();

    protected bool BackfaceCulling
    {
        get => _graphicsPipeline.BackfaceCullingEnabled;
        set => _graphicsPipeline.BackfaceCullingEnabled = value;
    }

    public Vector4 ClearColor { get; set; } = ColorUtils.SlateGray;
    
    public bool GizmosEnabled { get; set; }

    public void RenderScene(in Scene scene, IRenderTarget renderTarget)
    {
        OnRenderScene(in scene, renderTarget);
        renderTarget.Clear(ClearColor);
        _graphicsPipeline.Render(scene.Model.Mesh.Vertices, scene.Model.Mesh.Indices, renderTarget);

        if (!GizmosEnabled)
            return;
        
        OnRenderGizmos(in scene, _gizmosBuilder);
        RenderGizmos(renderTarget);
    }

    protected abstract void OnRenderScene(in Scene scene, IRenderTarget renderTarget);
    protected virtual void OnRenderGizmos(in Scene scene, GizmosBuilder gizmos) { }

    private void RenderGizmos(IRenderTarget renderTarget)
    {
        _gizmoPipeline.Render(_gizmoVertices, _gizmoIndices, renderTarget);
        _gizmoVertices.Clear();
        _gizmoIndices.Clear();
    }
}

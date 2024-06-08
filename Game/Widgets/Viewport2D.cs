using System;
using Lime;

namespace Game.Widgets;

[TangerineRegisterNode]
public class Viewport2D : Widget {
    public Camera2D Camera { get; set; }

    public Viewport2D() { }

    public override void AddToRenderChain(RenderChain chain) {
        AddSelfToRenderChain(chain, Layer);
    }

    private RenderChain renderChain = new RenderChain();

    public Vector2 ViewportToWorldPoint(Vector2 worlPoint) {
        var pNdc = LocalToWorldTransform.CalcInversed().TransformVector(worlPoint);
        pNdc.X = (pNdc.X / Size.X) * 2.0f - 1.0f;
        pNdc.Y = 1.0f - (pNdc.Y / Size.Y) * 2.0f;

        var viewProj = (Matrix44)ComputeViewMatrix() * ComputeProjectionMatrix();
        var p = viewProj.CalcInverted().ProjectVector(pNdc);
        return p;
    }

    protected override bool PartialHitTest(ref HitTestArgs args) {
        if (!BoundingRectHitTest(args.Point))
            return false;


        var prevPoint = args.Point;

        try {
            foreach (var n in Nodes)
                n.RenderChainBuilder?.AddToRenderChain(renderChain);


            var pNdc = LocalToWorldTransform.CalcInversed().TransformVector(args.Point);
            pNdc.X = (pNdc.X / Size.X) * 2.0f - 1.0f;
            pNdc.Y = 1.0f - (pNdc.Y / Size.Y) * 2.0f;

            var viewProj = (Matrix44)ComputeViewMatrix() * ComputeProjectionMatrix();
            var p = viewProj.CalcInverted().ProjectVector(pNdc);

            args.Point = p;

            if (renderChain.HitTest(ref args)) {
                return true;
            }
        } finally {
            renderChain.Clear();

            args.Point = prevPoint;
        }

        return base.PartialHitTest(ref args);
    }

    protected override Lime.RenderObject GetRenderObject() {
        var prevClipRegion = renderChain.ClipRegion;

        try {
            var ro = RenderObjectPool<RenderObject>.Acquire();

            ro.ViewportSize = Size;
            ro.ViewportWorldTransform = LocalToWorldTransform;

            ro.ViewMatrix = ComputeViewMatrix();
            ro.ProjectionMatrix = ComputeProjectionMatrix();

            renderChain.ClipRegion = ComputeViewRect();

            foreach (var n in Nodes) {
                n.RenderChainBuilder?.AddToRenderChain(renderChain);
            }

            renderChain.GetRenderObjects(ro.Objects);

            return ro;
        } finally {
            renderChain.Clear();
            renderChain.ClipRegion = prevClipRegion;
        }
    }

    private Rectangle ComputeViewRect() {
        var invViewProj = ((Matrix44)ComputeViewMatrix() * ComputeProjectionMatrix()).CalcInverted();

        var lb = invViewProj.ProjectVector(new Vector2(-1.0f, -1.0f));
        var rb = invViewProj.ProjectVector(new Vector2(1.0f, -1.0f));
        var rt = invViewProj.ProjectVector(new Vector2(1.0f, 1.0f));
        var lt = invViewProj.ProjectVector(new Vector2(-1.0f, 1.0f));

        var aabb = new Rectangle(
                float.MaxValue, float.MaxValue,
                float.MinValue, float.MinValue
            ).IncludingPoint(lb)
            .IncludingPoint(rb)
            .IncludingPoint(rt)
            .IncludingPoint(lt);

        return aabb;
    }

    private Matrix32 ComputeViewMatrix() {
        // TODO: Don't use scale
        return Camera.LocalToWorldTransform.CalcInversed();
    }

    private Matrix44 ComputeProjectionMatrix() {
        var aspectRatio = Camera.UseCustomAspectRatio ? Camera.CustomAspectRatio : Width / Height;
        var w = aspectRatio * Camera.OrthographicSize;
        var h = Camera.OrthographicSize;

        return Matrix44.CreateOrthographicOffCenter(
            -w * 0.5f, w * 0.5f, h * 0.5f, -h * 0.5f, -50.0f, 50.0f
        );
    }

    private class RenderObject : Lime.RenderObject {
        public RenderObjectList Objects = new RenderObjectList();

        public Matrix32 ViewMatrix;
        public Matrix44 ProjectionMatrix;

        public Vector2 ViewportSize;
        public Matrix32 ViewportWorldTransform;

        public override void Render() {
            System.Diagnostics.Debug.Assert(Renderer.Transform2.IsIdentity());

            Renderer.PushState(RenderState.Transform2 | RenderState.Viewport | RenderState.Projection);

            try {
                Renderer.Viewport = new Viewport(ComputeViewportBounds());
                Renderer.Transform2 = ViewMatrix;
                Renderer.Projection = ProjectionMatrix;

                Objects.Render();
            } finally {
                Renderer.PopState();
            }
        }

        private WindowRect ComputeViewportBounds() {
            var proj = (Matrix44)ViewportWorldTransform * Renderer.FixupWVP(Renderer.Projection);

            var lt = proj.ProjectVector(Vector2.Zero);
            var rb = proj.ProjectVector(ViewportSize);

            var minNdc = Vector2.Min(lt, rb);
            var maxNdc = Vector2.Max(lt, rb);

            var bounds = Renderer.Viewport.Bounds;

            var minScreen = (Vector2)bounds.Origin + (minNdc + Vector2.One) * (Vector2)bounds.Size * 0.5f;
            var maxScreen = (Vector2)bounds.Origin + (maxNdc + Vector2.One) * (Vector2)bounds.Size * 0.5f;

            return new WindowRect { Origin = (IntVector2)minScreen, Size = (IntVector2)(maxScreen - minScreen) };
        }

        protected override void OnRelease() {
            base.OnRelease();

            Objects.Clear();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Extensions;
using Stride.Graphics.GeometricPrimitives;
using Stride.Physics;
using Stride.Rendering;

namespace Stride.Assets.Presentation.AssetEditors.Gizmos
{
    [GizmoComponent(typeof(ConstraintComponent), false)]
    public class PhysicsConstraintGizmo : EntityGizmo<ConstraintComponent>
    {
        private static readonly Color OrangeUniformColor = new Color(0xFF, 0x98, 0x2B);
        private static readonly Color PurpleUniformColor = new Color(0xB1, 0x24, 0xF2);

        private PivotMarker PivotA;
        private PivotMarker PivotB;

        public PhysicsConstraintGizmo(ConstraintComponent component) : base(component)
        {
            RenderGroup = PhysicsShapesGroup;
        }

        protected override Entity Create()
        {
            // We want to scale pivots with the zoom. There's two of them, so we'll let
            // base.Update to set scale on this empty entity and later copy it over.
            GizmoScalingEntity = new Entity();
            
            return new Entity($"Physics Constraint Gizmo Root Entity (id={ContentEntity.Id})");
        }

        protected override void Destroy()
        {
            if (PivotA.Entity != null)
                PivotA.Entity.Scene = null;
            if (PivotB.Entity != null)
                PivotB.Entity.Scene = null;
        }

        public override void Update()
        {
            base.Update();

            if (Component.Description == null || !IsEnabled || !IsSelected)
            {
                if (PivotA.Model != null)
                    PivotA.Model.Enabled = false;
                if (PivotB.Model != null)
                    PivotB.Model.Enabled = false;

                return;
            }

            UpdatePivot(ref PivotA, Component.BodyA, Component.Description.PivotInA, OrangeUniformColor, "PivotInA");
            UpdatePivot(ref PivotB, Component.BodyB, Component.Description.PivotInB, PurpleUniformColor, "PivotInB");
        }

        private void UpdatePivot(
            ref PivotMarker pivotMarker,
            RigidbodyComponent rigidbody,
            Vector3 pivot,
            Color markerColor,
            string entityName)
        {
            if (rigidbody == null)
            {
                if (pivotMarker.Model != null)
                    pivotMarker.Model.Enabled = false;

                return;
            }

            // First we create a debug entity which will be reused by any rigidbody referenced
            if (pivotMarker.Entity == null)
            {
                var sphere = GeometricPrimitive.Sphere.New(GraphicsDevice, 0.05f, 16).ToMeshDraw();
                var material = GizmoUniformColorMaterial.Create(GraphicsDevice, markerColor, false);

                pivotMarker.Entity = new Entity(entityName);
                pivotMarker.Model = new ModelComponent
                {
                    Model = new Model { material, new Mesh { Draw = sphere } },
                    RenderGroup = RenderGroup,
                };


                pivotMarker.Entity.Add(pivotMarker.Model);

                //var physicsComponent = new StaticColliderComponent
                //{
                //    Enabled = false,
                //    ColliderShape = new SphereColliderShape(false, 0.05f),
                //};
                //pivotMarker.Entity.Add(physicsComponent);

                // adding entity into the scene will cause the physics component to get picked up by the processor,
                // which we need for the AddDebugEntity call
                EditorScene.Entities.Add(pivotMarker.Entity);

                //physicsComponent.AddDebugEntity(EditorScene, RenderGroup, true);
                //pivotMarker.Model = physicsComponent.DebugEntity.GetChild(0).Get<ModelComponent>();
            }

            // on each frame we'll update the transform of the entity
            // we're setting the pivot entity as a child of the rigidbody to have the correct local rotation
            //pivotMarker.Entity.SetParent(rigidbody.Entity);
            // we're dividing the pivot by the scale, because the constraint ignores it
            rigidbody.Entity.Transform.UpdateWorldMatrix();
            rigidbody.Entity.Transform.WorldMatrix.Decompose(out _, out Quaternion rotation, out var position);
            rotation.Rotate(ref pivot);
            pivotMarker.Entity.Transform.Position = position + pivot;

            // we want the pivot marker to keep the same size irrespective of the scale of the rigidbody
            //rigidbody.Entity.Transform.WorldMatrix.Decompose(out var parentWorldScale, out _);
            var targetScale = GizmoScalingEntity.Transform.Scale;
            pivotMarker.Entity.Transform.Scale = targetScale;// / parentWorldScale;

            // and ensure the model is enabled
            pivotMarker.Model.Enabled = true;
        }

        private struct PivotMarker
        {
            public Entity Entity;
            public ModelComponent Model;
        }
    }
}

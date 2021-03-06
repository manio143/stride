// Copyright (c) Stride contributors (https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using Stride.Core;
using Stride.Core.Mathematics;

namespace Stride.Physics.Constraints
{
    /// <summary>
    /// Description of a <see cref="HingeConstraint"/>.
    /// </summary>
    [Display("Hinge")]
    [DataContract(nameof(HingeConstraintDesc))]
    public class HingeConstraintDesc : IConstraintDesc
    {
        /// <summary>
        /// Position local to rigidbody A.
        /// </summary>
        /// <userdoc>
        /// Position local to rigidbody A.
        /// </userdoc>
        [Display(0)]
        public Vector3 PivotInA { get; set; }

        /// <summary>
        /// Position local to rigidbody B.
        /// </summary>
        /// <remarks>
        /// Ignored when creating a body-world constraint.
        /// </remarks>
        /// <userdoc>
        /// Position local to rigidbody B. Ignored when creating body-world constraint.
        /// </userdoc>
        [Display(1)]
        public Vector3 PivotInB { get; set; }

        /// <summary>
        /// Axis on which the hinge will rotate relative to body A.
        /// </summary>
        /// <userdoc>
        /// Axis on which the hinge will rotate relative to body A.
        /// </userdoc>
        [Display(2)]
        public Quaternion AxisInA { get; set; }

        /// <summary>
        /// Axis on which the hinge will rotate relative to body B.
        /// </summary>
        /// <userdoc>
        /// Axis on which the hinge will rotate relative to body B.
        /// </userdoc>
        [Display(3)]
        public Quaternion AxisInB { get; set; }

        /// <summary>
        /// Use Axis in relation to body A. TODO: this isn't completely accurate
        /// </summary>
        [Display(8)]
        public bool UseReferenceFrameA { get; set; }

        // TODO: document these
        [Display(5)]
        public bool SetLimit { get; set; }
        [Display(6)]
        public float LowerLimit { get; set; } = 1;
        [Display(7)]
        public float UpperLimit { get; set; } = -1;


        public Constraint Build(RigidbodyComponent rigidbodyA, RigidbodyComponent rigidbodyB)
        {
            if (rigidbodyA == null)
                throw new ArgumentNullException(nameof(rigidbodyA));
            if (rigidbodyB != null && rigidbodyB.Simulation != rigidbodyA.Simulation)
                throw new Exception("Both RigidBodies must be on the same simulation");

            var rbA = rigidbodyA.InternalRigidBody;
            var rbB = rigidbodyB?.InternalRigidBody;
            var axisA = Vector3.UnitX;
            AxisInA.Rotate(ref axisA);
            var axisB = Vector3.UnitX;
            AxisInA.Rotate(ref axisB);

            HingeConstraint hinge = new HingeConstraint
            {
                InternalHingeConstraint =
                            rigidbodyB == null ?
                                new BulletSharp.HingeConstraint(rbA, PivotInA, axisA) :
                                new BulletSharp.HingeConstraint(rbA, rbB, PivotInA, PivotInB, axisA, axisB, UseReferenceFrameA),
            };
            hinge.InternalConstraint = hinge.InternalHingeConstraint;

            if (SetLimit)
            {
                hinge.SetLimit(LowerLimit, UpperLimit);
            }

            return hinge;
        }
    }
}

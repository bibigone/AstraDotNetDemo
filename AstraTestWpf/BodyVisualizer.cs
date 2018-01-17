using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace AstraTestWpf
{
    /// <summary>
    /// Helper class for visualization of body tracking as skeletons, which can be shown over depth map.
    /// Skeleton visualized as circle joints connected by lines (bones).
    /// </summary>
    internal sealed class BodyVisualizer
    {
        /// <summary>
        /// Creates visualizer. Call from UI thread because during construction <c>ImageSource</c> is being created.
        /// </summary>
        /// <param name="dispatcher">Dispatcher of owner thread. As a rule, UI thread.</param>
        /// <param name="depthWidth">Width of depth map.</param>
        /// <param name="depthHeight">Height of depth map.</param>
        public BodyVisualizer(Dispatcher dispatcher, int depthWidth, int depthHeight)
        {
            if (dispatcher.Thread != Thread.CurrentThread)
            {
                throw new InvalidOperationException(
                    "Call this constructor from UI thread please, because it creates ImageSource object for UI");
            }

            this.dispatcher = dispatcher;
            this.depthWidth = depthWidth;
            this.depthHeight = depthHeight;

            // Astra SDK supports up to 6 bodies
            bodies = new Astra.Body[6];

            // WPF stuff to draw skeleton
            drawingGroup = new DrawingGroup
            {
                ClipGeometry = new RectangleGeometry(new Rect(0, 0, depthWidth, depthHeight)),
            };
            ImageSource = new DrawingImage(drawingGroup);
        }

        /// <summary>
        /// Image with visualized skeletons (<c>Astra.BodyFrame</c>). You can use this property in WPF controls/windows.
        /// </summary>
        public ImageSource ImageSource { get; }

        /// <summary>Visualization setting: pen for border around joint circles.</summary>
        public Pen JointBorder { get; set; } = new Pen(Brushes.Black, 2);

        /// <summary>Visualization setting: brush to fill circle of well-tracked joint.</summary>
        public Brush TrackedJointFill { get; set; } = Brushes.LightGray;

        /// <summary>Visualization setting: brush to fill circle of joint with low confidence.</summary>
        public Brush LowConfidenceJointFill { get; set; } = Brushes.Yellow;

        /// <summary>Visualization setting: radius of circle for well-tracked joint.</summary>
        public double TrackedJointCircleRadius { get; set; } = 7;

        /// <summary>Visualization setting:  radius of circle for joint with low confidence.</summary>
        public double LowConfidenceJointCircleRadius { get; set; } = 4;

        /// <summary>Visualization setting: pen to draw well-tracked bone.</summary>
        public Pen TrackedBonePen { get; set; } = new Pen(Brushes.White, 5);

        /// <summary>Visualization setting: pen to draw bone with low confidence.</summary>
        public Pen LowConfidenceBonePen { get; set; } = new Pen(Brushes.LightYellow, 3);

        /// <summary>
        /// Updates <c>ImageSource</c> based on <c>Astra.BodyFrame</c>, received from Astra sensor.
        /// </summary>
        /// <param name="bodyFrame">Body frame, received from Astra SDK. Can be <c>null</c>.</param>
        /// <remarks>Can be called from background thread.</remarks>
        public void Update(Astra.BodyFrame bodyFrame)
        {
            // Is compatible?
            if (bodyFrame == null || bodyFrame.Width != depthWidth || bodyFrame.Height != depthHeight)
                return;

            // 1st step: get information about bodies
            lock (bodies)
            {
                bodyFrame.CopyBodyData(ref bodies);
            }

            // 2nd step: we can update ImageSource only from its owner thread (as a rule, UI thread)
            dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(DrawBodies));
        }

        private void DrawBodies()
        {
            lock (bodies)
            {
                using (var dc = drawingGroup.Open())
                {
                    // Our image must fit depth map, this why we have to draw transparent rectangle to set size of our image
                    // There is no other way to set size of DrawingImage
                    dc.DrawRectangle(Brushes.Transparent, null, new Rect(0, 0, depthWidth, depthHeight));

                    // Draw skeleton for each tracked body
                    foreach (var body in bodies)
                    {
                        if (body.Status == Astra.BodyStatus.Tracking)
                        {
                            DrawBones(dc, body);
                            DrawJoints(dc, body);
                        }
                    }
                }
            }
        }

        private void DrawBones(DrawingContext dc, Astra.Body body)
        {
            foreach (var bone in bones)
            {
                var parentJoint = FindJoint(body, bone.ParentJointType);
                if (IsJointOk(parentJoint))
                {
                    var endJoint = FindJoint(body, bone.EndJointType);
                    if (IsJointOk(endJoint))
                    {
                        var isTracked =
                            parentJoint.Status == Astra.JointStatus.Tracked
                            && endJoint.Status == Astra.JointStatus.Tracked;

                        var pen = isTracked
                            ? TrackedBonePen
                            : LowConfidenceBonePen;

                        dc.DrawLine(pen, ToImagePoint(parentJoint), ToImagePoint(endJoint));
                    }
                }
            }
        }

        private static bool IsJointOk(Astra.Joint joint)
            => joint != null && joint.Status != Astra.JointStatus.NotTracked;

        private static Astra.Joint FindJoint(Astra.Body body, Astra.JointType jointType)
            => body.Joints.FirstOrDefault(j => j.Type == jointType);

        private static Point ToImagePoint(Astra.Joint joint)
            => new Point(joint.DepthPosition.X, joint.DepthPosition.Y);

        private void DrawJoints(DrawingContext dc, Astra.Body body)
        {
            foreach (var joint in body.Joints)
            {
                if (IsJointOk(joint))
                {
                    var isTracked = joint.Status == Astra.JointStatus.Tracked;

                    var brush = isTracked
                        ? TrackedJointFill
                        : LowConfidenceJointFill;

                    var radius = isTracked
                        ? TrackedJointCircleRadius
                        : LowConfidenceJointCircleRadius;

                    dc.DrawEllipse(brush, JointBorder, ToImagePoint(joint), radius, radius);
                }
            }
        }

        private readonly Dispatcher dispatcher;
        private readonly int depthWidth;
        private readonly int depthHeight;
        private readonly DrawingGroup drawingGroup;
        private Astra.Body[] bodies;

        #region Bones structure

        /// <summary>
        /// Bone is connector of two joints
        /// </summary>
        private struct Bone
        {
            public Astra.JointType ParentJointType;
            public Astra.JointType EndJointType;

            public Bone(Astra.JointType parentJointType, Astra.JointType endJointType)
            {
                ParentJointType = parentJointType;
                EndJointType = endJointType;
            }
        }

        /// <summary>
        /// Skeleton structure = list of bones = list of joint connectors
        /// </summary>
        private static readonly IReadOnlyList<Bone> bones = new Bone[]
        {
            // spine, neck, and head
            new Bone(Astra.JointType.BaseSpine, Astra.JointType.MidSpine),
            new Bone(Astra.JointType.MidSpine, Astra.JointType.ShoulderSpine),
            new Bone(Astra.JointType.ShoulderSpine, Astra.JointType.Neck),
            new Bone(Astra.JointType.Neck, Astra.JointType.Head),
            // left arm
            new Bone(Astra.JointType.ShoulderSpine, Astra.JointType.LeftShoulder),
            new Bone(Astra.JointType.LeftShoulder, Astra.JointType.LeftElbow),
            new Bone(Astra.JointType.LeftElbow, Astra.JointType.LeftWrist),
            new Bone(Astra.JointType.LeftWrist, Astra.JointType.LeftHand),
            // right arm
            new Bone(Astra.JointType.ShoulderSpine, Astra.JointType.RightShoulder),
            new Bone(Astra.JointType.RightShoulder, Astra.JointType.RightElbow),
            new Bone(Astra.JointType.RightElbow, Astra.JointType.RightWrist),
            new Bone(Astra.JointType.RightWrist, Astra.JointType.RightHand),
            // left leg
            new Bone(Astra.JointType.BaseSpine, Astra.JointType.LeftHip),
            new Bone(Astra.JointType.LeftHip, Astra.JointType.LeftKnee),
            new Bone(Astra.JointType.LeftKnee, Astra.JointType.LeftFoot),
            // right leg
            new Bone(Astra.JointType.BaseSpine, Astra.JointType.RightHip),
            new Bone(Astra.JointType.RightHip, Astra.JointType.RightKnee),
            new Bone(Astra.JointType.RightKnee, Astra.JointType.RightFoot),
        };

        #endregion
    }
}

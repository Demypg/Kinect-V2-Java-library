using Microsoft.Kinect;
using System;

namespace KinectGestureLibrary
{
    public class PseudoBody
    {
        private Body body;

        public PseudoBody(Body body)
        {
            this.body = body;
        }

        public PseudoJoint getJoint(JointType type)
        {
            return new PseudoJoint(body.Joints[type], body.JointOrientations[type]);
        }

        public PseudoJoint getJoint(int type)
        {
            return getJoint((JointType) type);
        }

        public static int parseJointType(string type)
        {
            return (int) Enum.Parse(typeof(JointType), type, true);
        }

        public PseudoJoint getJoint(string type)
        {
            return getJoint(parseJointType(type));
        }

        public long getTrackingID()
        {
            return (long)body.TrackingId;
        }
    }
}

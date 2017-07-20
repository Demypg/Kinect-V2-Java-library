using Microsoft.Kinect;

namespace KinectGestureLibrary
{
    public class PseudoJoint
    {
        private Joint joint;
        private JointOrientation orientation;

        public PseudoJoint(Joint joint, JointOrientation aOrientation)
        {
            this.joint = joint;
            this.orientation = aOrientation;
        }

        public double[] getPosition()
        {
            double[] pos = { joint.Position.X, joint.Position.Y, joint.Position.Z };
            return pos;
        }

        public double[] getRotations()
        {
            double[] rot = { orientation.Orientation.X, orientation.Orientation.Y, orientation.Orientation.Z, orientation.Orientation.W };
            return rot;
        }
    }
}

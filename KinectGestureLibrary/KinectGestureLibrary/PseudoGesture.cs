using Microsoft.Kinect.VisualGestureBuilder;

namespace KinectGestureLibrary
{
    /// <summary>
    /// Pseudogesture represents a normal kinect gesture, and stores a name and gesture type
    /// </summary>
    class PseudoGesture
    {
        /// <summary>The Gesture type</summary>
        public GestureType gestureType { get; }
        /// <summary>The name this gesture should have</summary>
        public string gestureName { get; }

        /// <summary>
        /// Constructor for a PseudoGesture
        /// </summary>
        /// <param name="aName">This gesture's name</param>
        /// <param name="aType">The type this gesture should be</param>
        public PseudoGesture(string aName, GestureType aType)
        {
            this.gestureName = aName;
            this.gestureType = aType;
        }
    }
}

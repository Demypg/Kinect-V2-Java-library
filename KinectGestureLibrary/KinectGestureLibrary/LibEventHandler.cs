using System;
using System.Collections.Generic;
using Microsoft.Kinect;

namespace KinectGestureLibrary
{
    /// <summary>
    /// The main Event system for the library. Any c# events that should be triggered will be implemented here.
    /// InterfaceEvents will also be triggered at the same time as normal c# delegate events.
    /// </summary>
    public class LibEventHandler
    {
        /// <summary>A reference to the interfaceEventsHandler</summary>
        private LibInterfaceEventHandler interfaceEventHandler;

        /// <summary>
        /// Constructor for the event handler, accepts an external reference to a interfaceEventhandler
        /// </summary>
        /// <param name="aIhdlr">An reference to a interface EventHandler</param>
        public LibEventHandler(LibInterfaceEventHandler aIhdlr)
        {
            interfaceEventHandler = aIhdlr;
        }
//----------------------------------------------------------------------------------------------------------------------------------------------------
        // Event for updating the body frame
        public event EventHandler<UpdateBodyEventArgs> UpdateBody;
        protected void OnUpdateBody(UpdateBodyEventArgs aBodies)
        {
            EventHandler<UpdateBodyEventArgs> handler = UpdateBody;
            if (handler != null)
               handler(this, aBodies);
        }
        public void raiseUpdateBodyEvent(Body[] aBodies)
        {
            OnUpdateBody(new UpdateBodyEventArgs() { Bodies = aBodies });
            interfaceEventHandler.updateBodyEvent.FireEvent(new UpdateBodyEventArgs() { Bodies = aBodies });
        }
//----------------------------------------------------------------------------------------------------------------------------------------------------
        // Event for notifying when the trackingIds have been changed (body added or removed from the kinect view)
        public event EventHandler<TrackingIDsChangedEventArgs> TrackingIDsChanged;
        protected void onTrackingIDsChanged(TrackingIDsChangedEventArgs aTrackingIDs)
        {
            EventHandler<TrackingIDsChangedEventArgs> handler = TrackingIDsChanged;
            if (handler != null)
                handler(this, aTrackingIDs);
        }
        public void raiseTrackingIDsChangedEvent(long[] aTrackingIDs)
        {
            onTrackingIDsChanged(new TrackingIDsChangedEventArgs() { TrackingIDs = aTrackingIDs });
            interfaceEventHandler.trackingIdsChangedEvent.FireEvent(new TrackingIDsChangedEventArgs() { TrackingIDs = aTrackingIDs });
        }
//----------------------------------------------------------------------------------------------------------------------------------------------------
        // Event for updating the gesture result
        public event EventHandler<GestureResultEventArgs> GestureResult;
        protected void onGestureResult(GestureResultEventArgs aResults)
        {
            EventHandler<GestureResultEventArgs> handler = GestureResult;
            if (handler != null)
                handler(this, aResults);
        }
        public void raiseGestureResultEvent(long aTrackingID, string aGestureName, float aConfidence)
        {
            onGestureResult(new GestureResultEventArgs() { TrackingID = aTrackingID, GestureName = aGestureName, ConfidenceOrProgress = aConfidence });
            interfaceEventHandler.gestureResultEvent.FireEvent(new GestureResultEventArgs() { TrackingID = aTrackingID, GestureName = aGestureName, ConfidenceOrProgress = aConfidence });

        }
//----------------------------------------------------------------------------------------------------------------------------------------------------
        // Event for updating the gesture result
        public event EventHandler<GestureResultEventArgs> GestureContinuousResult;
        protected void onGestureContinuousResult(GestureResultEventArgs aResults)
        {
            EventHandler<GestureResultEventArgs> handler = GestureContinuousResult;
            if (handler != null)
                handler(this, aResults);
        }
        public void raiseGestureContinuousResultEvent(long aTrackingID, string aGestureName, float aProgress)
        {
            onGestureContinuousResult(new GestureResultEventArgs() { TrackingID = aTrackingID, GestureName = aGestureName, ConfidenceOrProgress = aProgress });
            interfaceEventHandler.gestureContinuousResultEvent.FireEvent(new GestureResultEventArgs() { TrackingID = aTrackingID, GestureName = aGestureName, ConfidenceOrProgress = aProgress });

        }
        //----------------------------------------------------------------------------------------------------------------------------------------------------        
        // Event for sending information to the library user
        public delegate void MessageEventHandler(object source, MessageEventArgs e);
        public event MessageEventHandler Message;
        protected virtual void OnMessage(MessageEventArgs message)
        {
            MessageEventHandler handler = Message;
            if (handler != null)
                handler(this, message);
        }
        public void raiseMessageEvent(int aType, string aMessage)
        {
            OnMessage(new MessageEventArgs() { errorType = aType, message = aMessage });
            interfaceEventHandler.messageEvent.FireEvent(new MessageEventArgs() { errorType = aType, message = aMessage });
        }
//----------------------------------------------------------------------------------------------------------------------------------------------------
        // Event for sending 2 lists; This event gets triggered whenever the kinect has detected a gesture;
        // - ConfidenceList lists all gestures whose confidence where the highest at the point the kinect detected a gesture
        // - ProgressList lists all gestures whose progress where the highest  at the point the kinect detected a gesture
        public delegate void GestureListEventHandler(object source, GestureListEventArgs e);
        public event GestureListEventHandler GestureList;
        protected virtual void OnGestureList(GestureListEventArgs gestureList)
        {
            GestureListEventHandler handler = GestureList;
            if (handler != null)
                handler(this, gestureList);
        }
        public void raiseGestureListEvent(Dictionary<string,float> aConfList, Dictionary<string,float> aProgList, long aTrackingID)
        {
            OnGestureList(new GestureListEventArgs() { confidenceList = aConfList, progressList = aProgList, TrackingID= aTrackingID });

            interfaceEventHandler.gestureListEvent.FireEvent(new GestureListEventArgs() { confidenceList = aConfList, progressList = aProgList, TrackingID = aTrackingID });
        }

    }

    /// <summary>
    /// A class extending EventArgs. Specifies the type Body[] as the event argument for the UpdateBody event
    /// </summary>
    public class UpdateBodyEventArgs : EventArgs
    {
        public Body[] Bodies { get; set; }
    }

    /// <summary>
    /// A class extending EventArgs. Specifies the type ulong[] as the event argument for the trackingIDsChanged event
    /// </summary>
    public class TrackingIDsChangedEventArgs : EventArgs
    {
        public long[] TrackingIDs { get; set; }
    }

    /// <summary>
    /// A class extending EventArgs. 
    /// </summary>
    public class GestureResultEventArgs : EventArgs
    {
        public long TrackingID { get; set; }
        public string GestureName { get; set; }
        public float ConfidenceOrProgress { get; set; }
    }

    /// <summary>
    /// A class extending EventArgs. Holds a string and errortype for the MessageEvent 
    /// </summary>
    public class MessageEventArgs : EventArgs
    {
        public string message { get; set; }
        //errortype 0 = info message, errortype 1 = error message
        public int errorType { get; set; }
    }

    /// <summary>
    /// A class extending EventArgs. Holds 2 lists of gestures
    /// </summary>
    public class GestureListEventArgs : EventArgs
    {
        public Dictionary<string, float> confidenceList { get; set; }
        public Dictionary<string, float> progressList { get; set; }
        public long TrackingID { get; set; }
    }

}

using System;
using System.Collections.Generic;
using System.Linq;

namespace KinectGestureLibrary
{
    /// <summary>
    /// An Interface Event system for the library. Any c# events that should be triggered will be implemented here.
    /// The interface event handler will only store references to interface object which act as events.
    /// When an interface is implemented and the abstract methods overriden, those methods get called the same way
    /// normal events would.
    /// </summary>
    public class LibInterfaceEventHandler
    {
        // References to all events
        public InterfaceEvent messageEvent { get; }
        public InterfaceEvent updateBodyEvent { get; }
        public InterfaceEvent trackingIdsChangedEvent { get; }
        public InterfaceEvent gestureResultEvent { get; }
        public InterfaceEvent gestureListEvent { get; }
        public InterfaceEvent gestureContinuousResultEvent { get; }

        /// <summary>
        /// Constructor for an interface event handler
        /// </summary>
        public LibInterfaceEventHandler()
        {
            this.messageEvent = new MessageEvent();
            this.updateBodyEvent = new UpdateBodyEvent();
            this.trackingIdsChangedEvent = new TrackingIDsChangedEvent();
            this.gestureResultEvent = new GestureResultEvent();
            this.gestureListEvent = new GestureListEvent();
            this.gestureContinuousResultEvent = new GestureContinuousResultEvent();
        }
    }

    //simple interface for the UpdateBodyEvent
    public interface UpdateBodyEventListener : Listener
    {
        void UpdateBody(PseudoBody[] bodies);
    }

    public class UpdateBodyEvent : InterfaceEvent
    {
        public override void callListeners(Listener listener, EventArgs aEvAr)
        {
            UpdateBodyEventArgs uArgs = (UpdateBodyEventArgs)aEvAr;
            UpdateBodyEventListener uListener = (UpdateBodyEventListener)listener;

            //Convert a array of bodies to a array of KDOCBodies
            PseudoBody[] KDOCBodies = new PseudoBody[uArgs.Bodies.Count()];
            for (int i = 0; i < uArgs.Bodies.Count(); i++)
            {
                KDOCBodies[i] = new PseudoBody(uArgs.Bodies[i]);
            }
            //call the common method defined in the interface:
            uListener.UpdateBody(KDOCBodies);
        }
    }

    //simple interface for the TrackingIDsChangedEvent
    public interface TrackingIDsChangedEventListener : Listener
    {
        void TrackingIDsChanged(long[] list);
    }

    public class TrackingIDsChangedEvent : InterfaceEvent
    {
        public override void callListeners(Listener listener, EventArgs aEvAr)
        {
            TrackingIDsChangedEventArgs tArgs = (TrackingIDsChangedEventArgs)aEvAr;
            TrackingIDsChangedEventListener tListener = (TrackingIDsChangedEventListener)listener;
            //call the common method defined in the interface:
            tListener.TrackingIDsChanged(tArgs.TrackingIDs);
        }
    }

    //simple interface for the GestureResultEvent
    public interface GestureResultEventListener : Listener
    {
        void GestureResult(long TrackingID, string GestureName, float Confidence);
    }

    public class GestureResultEvent : InterfaceEvent
    {
        public override void callListeners(Listener listener, EventArgs aEvAr)
        {
            GestureResultEventArgs gArgs = (GestureResultEventArgs)aEvAr;
            GestureResultEventListener gListener = (GestureResultEventListener)listener;
            //call the common method defined in the interface:
            gListener.GestureResult(gArgs.TrackingID,gArgs.GestureName,gArgs.ConfidenceOrProgress);
        }
    }

    //simple interface for the GestureResultEvent
    public interface GestureContinuousResultEventListener : Listener
    {
        void GestureContinuousResult(long TrackingID, string GestureName, float Confidence);
    }

    public class GestureContinuousResultEvent : InterfaceEvent
    {
        public override void callListeners(Listener listener, EventArgs aEvAr)
        {
            GestureResultEventArgs gArgs = (GestureResultEventArgs)aEvAr;
            GestureContinuousResultEventListener gListener = (GestureContinuousResultEventListener)listener;
            //call the common method defined in the interface:
            gListener.GestureContinuousResult(gArgs.TrackingID, gArgs.GestureName, gArgs.ConfidenceOrProgress);
        }
    }

    public interface MessageEventListener : Listener
    {
        void MessageRecieved(int type, String message);
        //types:
        //0: information
        //1: error
    }

    public class MessageEvent : InterfaceEvent
    {
        public override void callListeners(Listener listener, EventArgs aEvAr)
        {
            MessageEventArgs mArgs = (MessageEventArgs)aEvAr;
            MessageEventListener mListener = (MessageEventListener)listener;
            //call the common method defined in the interface:
            mListener.MessageRecieved(mArgs.errorType, mArgs.message);
        }
    }

    //simple interface for the GestureResultEvent
    public interface GestureListEventListener : Listener
    {
        void GestureList(StringLookupTable confidenceList, StringLookupTable progressList, long TrackinID);
    }

    public class GestureListEvent : InterfaceEvent
    {
        public override void callListeners(Listener listener, EventArgs aEvAr)
        {
            GestureListEventArgs gArgs = (GestureListEventArgs)aEvAr;
            GestureListEventListener gListener = (GestureListEventListener)listener;
            //call the common method defined in the interface:
            gListener.GestureList(new StringLookupTable(gArgs.confidenceList),new StringLookupTable(gArgs.progressList),gArgs.TrackingID);
        }
    }
   

    /// <summary>
    /// simple interface so that every Listener type can be used in the interface event abstract class
    /// </summary>
    public interface Listener { }

    /// <summary>
    /// The basic structure on how a interface event should look like, the only thing missing is the 
    /// calling of every eventListener. This is the only part that is event dependant.
    /// </summary>
    public abstract class InterfaceEvent{
        // A list of subscribers to iterate through
        protected List<Listener> ListenersEvent;
        /// <summary>
        /// Adds listeners for the Event to the iterable list
        /// </summary>
        /// <param name="aListener">The object that wants to subscribe</param>
        public void addEventListener(Listener aListener)
        {
            //Check if the list isn't empty
            if (ListenersEvent == null)
                ListenersEvent = new List<Listener>();
            //Add the subscriber to the list
            ListenersEvent.Add(aListener);
        }
        /// <summary>
        /// A trigger for the Event
        /// </summary>
        /// <param name="aEventArgs">The data that gets send to the subscribers</param>
        public void FireEvent(EventArgs aEventArgs)
        {
            //check that the list isn't empty
            if (ListenersEvent != null && ListenersEvent.Any())
            {
                //iterate through the list and call the subscribers method
                foreach (Listener listener in ListenersEvent)
                {
                    callListeners(listener,aEventArgs);
                }
            }
        }
        /// <summary>
        /// calling of all listeners
        /// </summary>
        /// <param name="listener">the listener to call</param>
        /// <param name="aEvAr">the arguments to send with the event</param>
        public abstract void callListeners(Listener listener, EventArgs aEvAr);
    }
}

package gesturelibraryinjava;

import kinectgesturelibrary.*;

/**
 *
 * @author Demy
 */
public class LibraryThread implements Runnable{
    
    private GestureLibMain lib;
    private LibInterfaceEventHandler evHandler;
    
    public LibraryThread(){
        //create a instance of the library's main class and get the object that has all the events
        lib = new GestureLibMain();
        
        //configure the library to use interfaces for events
        lib.setinterfaceEventsMode(true);
        
        //get the Java eventHandler class from the library
        evHandler = lib.getinterfaceEventHandler();
        
        
        //subscribing to events:
        
        
        //anonymous inner class for Message events
        //always include a messageEventListener because any type of error or info gets send over this event
        evHandler.getmessageEvent().addEventListener(new MessageEventListener(){
            @Override
            public void MessageRecieved(int i, String string) {
                switch(i){
                    case 0:
                        //normal messages have 0 as value
                        System.out.println(string);
                        break;
                    case 1:
                        //error messages have 1 as value
                        System.err.println(string);
                        break;
                }
            }
        });
        
        //anonymous inner class for GestureResult events, gets triggered whenever a discrete gesture has been seen by the kinect
        evHandler.getgestureResultEvent().addEventListener(new GestureResultEventListener(){
            @Override
            public void GestureResult(long l, String string, float f) {
                //System.out.println("Person " + l + " performed " + string + " with " + f + " as confidence");
                //quick note: the trackingID l was originally an ulong(uInt64) variable. So trackingId's can have a negative value.
            }
        });
        
        //anonymous inner class for UpdateBody events, 
        //this event gives an array of bodies that have the full joint positions of a person in view
        evHandler.getupdateBodyEvent().addEventListener(new UpdateBodyEventListener(){
            @Override
            public void UpdateBody(PseudoBody[] pbs) {
                for(PseudoBody b : pbs){
                    System.out.println("body " + b.getTrackingID() + " Spine Mid point position: " + b.getJoint(1).getPosition()[1]);
                }
            }
        });
        
        //anonymous inner class for TrackingIdsChanged events, 
        //this event gets called whenever a person enters or steps out of view of the kinect
        //every person in view gest assigned a randomly generated ID, TrackingIDs is a full list of all possible trackable bodies
        evHandler.gettrackingIdsChangedEvent().addEventListener(new TrackingIDsChangedEventListener(){
            @Override
            public void TrackingIDsChanged(long[] longs) {
                //System.out.print("Someone entered the/went out of view, new ids:");
                //for(long l : longs){
                //    System.out.print(" "+ l);
                //}
                //System.out.println("");
            }
        });
        
        //anonymous inner class for TrackingIdsChanged events,
        //this event returns a total of 3 gestures each time a person in view performs a gesture.
        //This list of gestures are in decreasing order with the gesture with highest confidence at the top.
        //This is handy when you want to see which gestures get triggered at the same time, and what the kinect realy thinks.
        evHandler.getgestureListEvent().addEventListener(new GestureListEventListener(){
            @Override
            public void GestureList(StringLookupTable slt, StringLookupTable slt1, long l) {
                //System.out.print("The top 3 gestures performed by " + l + " are: ");
                //String[] nameList = slt.getNameListInDecreasingOrder();//note: Decreasing order sorted by confidence, not alphabetical
                //for(String s : nameList){
                //    System.out.print(s+", ");
                //}
                //System.out.println("");
            }
        });
        
    }

    @Override
    public void run() {
        //Create example configuration file
        //lib.createExampleConfig();
        
        //Start of the kinect camera, be sure to call this ONLY after you've subscribed to the message events
        //startKinect requires a string that is the address to the gesture library
        lib.startKinect("..\\..\\Databases\\ABGesture.gbd","..\\..\\Configurations\\configExample.xml");
        //this will also hault the Thread here, resulting in that anything run after this will not be run
        
        //In the config file you can link/bundle gestures together to only trigger a single event.
        //This is handy when you have a database that supports lefthanded and righthanded gestures.
        //It is possible to omit the configuration file from the method call.
        
        //nothing runs here
    }
}

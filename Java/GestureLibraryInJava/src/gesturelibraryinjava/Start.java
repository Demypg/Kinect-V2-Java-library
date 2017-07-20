package gesturelibraryinjava;

import java.io.File;
import java.io.IOException;
import net.sf.jni4net.Bridge;

/**
 *
 * @author Demy
 */
public class Start {

    /**
     * @param args the command line arguments
     */
    public static void main(String[] args) {
        //initialise jni4net
        try {

            Bridge.setVerbose(true);
            Bridge.init();
            //CHANGE TO CORRECT FILE SPACE BEFORE USE
            Bridge.LoadAndRegisterAssemblyFrom(new File("..\\..\\JNI4NET\\Build_library\\final_result\\KinectGestureLibrary.j4n.dll"));
            
        } catch (IOException e) {
            e.printStackTrace();
        }
        //start the event handling thread
        LibraryThread start = new LibraryThread();
        new Thread(start).start();
        
    }
    
}

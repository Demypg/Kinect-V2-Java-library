using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace KinectGestureLibrary
{
    /// <summary>
    /// ConfigSerializer either serializes or de-serializes a configuration file for the library
    /// </summary>
    class ConfigSerializer
    {
        /// <summary>Eventhandler to send notifications</summary>
        private LibEventHandler evHandler { get; }

        /// <summary>
        /// A constructor for the ConfigSerializer class
        /// </summary>
        /// <param name="aEvhandler">eventhandler</param>
        public ConfigSerializer(LibEventHandler aEvhandler)
        {
            this.evHandler = aEvhandler;
        }

        /// <summary>
        /// Serializes a configuration object to a file
        /// </summary>
        /// <param name="aConfig">The object to serialize</param>
        /// <param name="aConfLocation">The location where the configuration will be stored, including the name</param>
        public void serializeConfig(GestureLibraryConfiguration aConfig, string aConfLocation)
        {
            XmlSerializer mySerializer = new XmlSerializer(typeof(GestureLibraryConfiguration));
            // To write to a file, create a StreamWriter object.  
            StreamWriter myWriter = new StreamWriter(aConfLocation);
            mySerializer.Serialize(myWriter, aConfig);
            myWriter.Close();
        }

        /// <summary>
        /// De-serializes a configuration file to an object
        /// </summary>
        /// <param name="aConfLocation">The location + name of the configuration file to read</param>
        /// <returns>A de-serialized configuration object</returns>
        public GestureLibraryConfiguration deSerializeConfig(string aConfLocation)
        {
            // Construct an instance of the XmlSerializer with the type  
            // of object that is being deserialized.  
            XmlSerializer mySerializer = new XmlSerializer(typeof(GestureLibraryConfiguration));
            // To read the file, create a FileStream.  
            FileStream myFileStream = new FileStream(aConfLocation, FileMode.Open);
            // Call the Deserialize method and cast to the object type.
            try{
                return (GestureLibraryConfiguration)mySerializer.Deserialize(myFileStream);
            }
            catch(Exception e)
            {
                evHandler.raiseMessageEvent(1, "Couldn't locate configuration file.");
                Console.WriteLine("Couldn't locate configuration file.");
            }
            return null;
        }
    }
}

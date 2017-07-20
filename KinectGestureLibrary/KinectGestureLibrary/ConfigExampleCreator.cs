namespace KinectGestureLibrary
{
    /// <summary>
    /// ConfigExampleCreator does exactly as the name implies, when an instance of this class has been made,
    /// it produces a configuration example file at the given location
    /// </summary>
    class ConfigExampleCreator
    {
        /// <summary>
        /// Create an instance of this class and generate an example configuration file
        /// </summary>
        /// <param name="aWhere">Destination folder + name of example config</param>
        /// <param name="aSerializer">A serializer object</param>
        public ConfigExampleCreator(string aWhere, ConfigSerializer aSerializer)
        {
            GestureLibraryConfiguration conf = new GestureLibraryConfiguration();
            conf.addLink(new string[2] { "gesture1", "gesture2" }, "gesture3", "Discrete");
            conf.addLink(new string[3] { "gesture4", "gesture5", "gesture6" }, "gesture7", "Continuous");
            conf.addIgnore("testGesture");
            conf.AverageFilterBufferSize = 10;
            aSerializer.serializeConfig(conf, aWhere);
        }

    }
}

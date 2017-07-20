using System.Collections.Generic;
using System.Linq;

namespace KinectGestureLibrary
{
    /// <summary>
    /// StringLookupTable acts as a wrapper for Dictionairy. 
    /// This class mimics the functionality of a Java HashMap with string as keys.
    /// </summary>
    public class StringLookupTable
    {
        /// <summary>Reference to the dictionairy to wrap</summary>
        private Dictionary<string, float> dict;

        /// <summary>
        /// Constructor for StringLookupTable
        /// </summary>
        /// <param name="aDict">The dictionairy to wrap</param>
        public StringLookupTable(Dictionary<string,float> aDict)
        {
            this.dict = aDict;
        }

        /// <summary>
        /// Returns the float value by name
        /// </summary>
        /// <param name="aName">The name to look for</param>
        /// <returns>float</returns>
        public float getValueByName(string aName)
        {
            float temp = 0;
            dict.TryGetValue(aName, out temp);
            return temp;
        }

        /// <summary>
        /// Get all names stored in the dictionairy. (Decreasing order is dictated by interfaceEventsHandler that uses this class)
        /// </summary>
        /// <returns>A list of all names stored in the library</returns>
        public string[] getNameListInDecreasingOrder()
        {
            return dict.Keys.ToArray<string>();
        }

        /// <summary>
        /// Get all floats stored in the dictionairy. (Decreasing order is dictated by interfaceEventsHandler that uses this class)
        /// </summary>
        /// <returns>all float values in the library</returns>
        public float[] getConfidenceListInDecreasingOrder()
        {
            return dict.Values.ToArray<float>();
        }



    }
}

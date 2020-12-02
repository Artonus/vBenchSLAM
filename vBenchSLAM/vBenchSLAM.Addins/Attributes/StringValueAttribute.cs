using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vBenchSLAM.Addins.Attributes
{
    /// <summary>
    /// Used to assign the text value to the enum
    /// </summary>
    public class StringValueAttribute : Attribute
    {
        /// <summary>
        /// Holds the string value for the enum
        /// </summary>
        public string StringValue { get; protected set; }

        public StringValueAttribute(string stringValue)
        {
            StringValue = stringValue;
        }
    }
}

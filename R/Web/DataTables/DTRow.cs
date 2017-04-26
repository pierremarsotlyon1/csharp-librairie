using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R
{
    public abstract class DTRow
    {
        /// <summary>
        /// Set the ID property of the dt-tag tr node to this value
        /// </summary>
        public virtual string DtRowId
        {
            get { return null; }
        }

        /// <summary>
        /// Add this class to the dt-tag tr node
        /// </summary>
        public virtual string DtRowClass
        {
            get { return null; }
        }

        /// <summary>
        /// Add this data property to the row's dt-tag tr node allowing abstract data to be added to the node, using the HTML5 data-* attributes.
        /// This uses the jQuery data() method to set the data, which can also then be used for later retrieval (for example on a click event).
        /// </summary>
        public virtual object DtRowData
        {
            get { return null; }
        }
    }
}

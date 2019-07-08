using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.ootii.Input
{
    /// <summary>
    /// Delegate allows us to specify a function that is used to
    /// process determine the state of the input.
    /// </summary>
    /// <returns></returns>
    public delegate float CustomTestFunction();

    /// <summary>
    /// Provides a container for aliases. We need this
    /// so the alias can use a support input. A support
    /// input is a second input that is also required
    /// in order for the alias to be activated. Think 'shift'
    /// or 'control', but it could be anything.
    /// </summary>
    public struct InputAlias
    {
        /// <summary>
        /// Name associated with the alias
        /// </summary>
        public string Name;

        /// <summary>
        /// Primary input ID to check first. It must be
        /// active in order for the whole alias to be active.
        /// </summary>
        public int PrimaryID;

        /// <summary>
        /// Support input that must me active in order
        /// for the whole alias to be active.
        /// </summary>
        public int SupportID;

        /// <summary>
        /// Function that can be assigned in order to test whether
        /// the alias is active or not.
        /// </summary>
        public CustomTestFunction CustomTest;

        /// <summary>
        /// Consturctor
        /// </summary>
        /// <param name="rName">Name of the alias</param>
        /// <param name="rPrimaryID">Primary ID of the alias</param>
        /// <param name="rSupportID">Support ID of the alias</param>
        public InputAlias(string rName, int rPrimaryID, int rSupportID)
        {
            Name = rName;
            PrimaryID = rPrimaryID;
            SupportID = rSupportID;
            CustomTest = null;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="rName">Name of the alias</param>
        /// <param name="rCustomTest">Custom function used to test for success</param>
        public InputAlias(string rName, CustomTestFunction rCustomTest)
        {
            Name = rName;
            PrimaryID = 0;
            SupportID = 0;
            CustomTest = rCustomTest;
        }
    }
}

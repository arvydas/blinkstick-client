/* Copyright (c) Peter Palotas 2007
 *  
 *  All rights reserved.
 *  
 *  Redistribution and use in source and binary forms, with or without
 *  modification, are permitted provided that the following conditions are
 *  met:
 *  
 *      * Redistributions of source code must retain the above copyright 
 *        notice, this list of conditions and the following disclaimer.    
 *      * Redistributions in binary form must reproduce the above copyright 
 *        notice, this list of conditions and the following disclaimer in 
 *        the documentation and/or other materials provided with the distribution.
 *      * Neither the name of the copyright holder nor the names of its 
 *        contributors may be used to endorse or promote products derived 
 *        from this software without specific prior written permission.
 *  
 *  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 *  "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 *  LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
 *  A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
 *  CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
 *  EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
 *  PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
 *  PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
 *  LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
 *  NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 *  SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 *  
 *  $Id: CommandLineOptionGroupAttribute.cs 3 2007-07-29 13:32:10Z palotas $
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace Plossum.CommandLine
{
    /// <summary>
    /// Attribute used to specify an option group in a command line option manager object.
    /// </summary>
    /// <remarks>Option groups are used for logical grouping of options. This is in part useful for 
    /// grouping related options in the usage documentation generated, but also to place certain 
    /// restrictions on a group of options.</remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=true)]
    public sealed class CommandLineOptionGroupAttribute : System.Attribute
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLineOptionGroupAttribute"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <remarks>The id must be unique within the option manager object.</remarks>
        public CommandLineOptionGroupAttribute(string id)
        {
            mId = id;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        /// <remarks>This is the name that will be displayed as a headline for the options contained in the
        /// group in any generated documentation. If not explicitly set it will be the same as <see cref="Id"/>.</remarks>
        public string Name
        {
            get { return mName; }
            set { mName = value; }
        }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description
        {
            get { return mDescription; }
            set { mDescription = value; }
        }

        /// <summary>
        /// Gets the id.
        /// </summary>
        /// <value>The id.</value>
        public string Id
        {
            get { return mId; }
        }

        /// <summary>
        /// Gets or sets the requirements placed on the options in this group.
        /// </summary>
        /// <value>requirements placed on the options in this group.</value>
         public OptionGroupRequirement Require
        {
            get { return mRequired; }
            set { mRequired = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether explicit assignment is required for the options
        /// of this group.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if explicit assignment is required by the options of this group; otherwise, <c>false</c>.        
        /// </value>
        /// <remarks>This defaults all options in this group to the specified value, but setting another value 
        /// explicitly on an option overrides this setting.</remarks>
        public bool RequireExplicitAssignment
        {
            get { return mRequireExplicitAssignment.Value; }
            set { mRequireExplicitAssignment = value; }
        }
        
        #endregion

        #region Internal properties

        /// <summary>
        /// Gets a value indicating whether this instance has 
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has specified a require explicit assignment value; otherwise, <c>false</c>.
        /// </value>
        internal bool HasRequireExplicitAssignment
        {
            get { return mRequireExplicitAssignment.HasValue;  }
        }

        #endregion

        #region Private fields

        private string mId;
        private string mName;
        private string mDescription;
        private OptionGroupRequirement mRequired;
        private bool? mRequireExplicitAssignment;

        #endregion
    }
}

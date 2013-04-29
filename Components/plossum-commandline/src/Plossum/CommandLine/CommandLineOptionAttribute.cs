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
 *  $Id: CommandLineOptionAttribute.cs 12 2007-08-05 10:26:08Z palotas $
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace Plossum.CommandLine
{
    /// <summary>
    /// Attribute indicating that the field to which is applied should receive the value of a command line option, 
    /// and also specifies the properties of that option.
    /// </summary>
    /// <remarks>This attribute may be applied to properties, fields and methods. If applied to a method, the method
    /// must accept exactly one argument which may not be an array or collection type. The method will then be called 
    /// once every time the option is specified with the value of the option.  If specified on a field or property
    /// the type of that member must be one of the built-in types, a one-dimensional array of one of the 
    /// built in types, a <see cref="System.Collections.ICollection"/> or a 
    /// <see cref="T:System.Collections.Generic.ICollection`1"/>. If an array type or a collection type, the enumerable 
    /// will contain all values that the option is specified with (if specified multiple times).</remarks>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Method, AllowMultiple=true)]
    public sealed class CommandLineOptionAttribute : System.Attribute
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLineOptionAttribute"/> class.
        /// </summary>
        public CommandLineOptionAttribute()
        {
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the group to which this option belongs.
        /// </summary>
        /// <value>The id of the group to which this option belongs, or null if this option does not belong to any group.</value>
        public string GroupId
        {
            get { return mGroupId; }
            set { mGroupId = value; }
        }

        /// <summary>
        /// Gets or sets the names of other options that must not be specified on the command line if this option is specified.
        /// </summary>
        /// <value>The names of other options that must not be specified on the command line if this option is specified.</value>
        /// <remarks>Option names can be comma- or space-separated.  If an option prohibits another option, this 
        /// automatically means that the other option also prohibits the first option, and it need not be 
        /// explicitly specified.</remarks>
        /// <seealso cref="CommandLineOptionGroupAttribute.Require"/>
        public string Prohibits
        {
            get { return mProhibits; }
            set { mProhibits = value; }
        }

        /// <summary>
        /// Gets or sets the name of this option.
        /// </summary>
        /// <value>The name of this option.</value>
        /// <remarks>This is the text that the user will specify on the command line. If not explicitly set, this 
        /// will take on the name of the member to which the attribute was applied.</remarks>
        public string Name
        {
            get { return mName; }
            set { mName = value; }
        }

        /// <summary>
        /// Gets or sets how option values will be assigned if this option is applied to a field of type bool.
        /// </summary>
        /// <value>The bool function of this option.</value>
        /// <remarks>This value is ignored for any other member types than Boolean. See <see cref="BoolFunction"/> for 
        /// more information.</remarks>
        public BoolFunction BoolFunction
        {
            get { return mBoolFunction; }
            set { mBoolFunction = value; }
        }

        /// <summary>
        /// Gets or sets the maximum number of times this option may be specified on the command line.
        /// </summary>
        /// <value>The the maximum number of times this option may be specified on the command line.</value>
        /// <remarks><para>The default value for this option is 1 for any non-collection or array type, and 0 otherwise.</para>
        /// <para>If this value is set to zero that means that there is no upper bound on the number of times
        /// this option may be specified.</para>
        /// <note>Note that this value must be either 0 or 1 if the member is not a method, or if the field or 
        /// property does not represent an array or collection type.</note></remarks>
        public int MaxOccurs
        {
            get { return mMaxOccurs ?? -1; }
            set { mMaxOccurs = value; }
        }

        /// <summary>
        /// Gets or sets the minimum number of times this option may be specified on the command line.
        /// </summary>
        /// <value>The the minimum number of times this option may be specified on the command line.</value>
        /// <remarks>
        ///     <para>The default value for this option is 0.</para>
        ///     <note>This value must be less than or equal to <see cref="MaxOccurs"/>, unless <see cref="MaxOccurs"/> is 
        ///         equal to 0.</note>
        /// </remarks>
        public int MinOccurs
        {
            get { return mMinOccurs; }
            set { mMinOccurs = value; }
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
        /// Gets or sets the default assignment value for this option.
        /// </summary>
        /// <value>The default assignment value for this option, or null if this option does not have a default assignment value.</value>
        /// <remarks>This value must be null unless <see cref="RequireExplicitAssignment"/> is set to <b>true</b>. If
        /// set, this means that if the option is specified without an explicit assignment, this is the value the option
        /// will be take. However if the option is never specified on the command line, this value will <i>not</i> be 
        /// assigned to the associated member.</remarks>
        public object DefaultAssignmentValue
        {
            get { return mDefaultValue; }
            set { mDefaultValue = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this option requires an explicit assignment character or not.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this option requires an explicit assignment character; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>Requiring an explicit assignment character means that a valid assignment character must follow
        /// the option name on the command line to assign a value to this option. If this value is not set to true
        /// the assignment character between an option name and its intended value may be omitted.</remarks>
        public bool RequireExplicitAssignment
        {
            get { return mRequireExplicitAssignment.Value; }
            set { mRequireExplicitAssignment = value; }
        }

        /// <summary>
        /// Gets or sets the aliases of this option.
        /// </summary>
        /// <value>A comma- or space-separated list of the aliases for this option.</value>
        /// <remarks>An option may have one or more aliases, which are other names with which to refer to the 
        /// same option. This is common in the UN*X world, where many programs provide both a long and a short name
        /// for most options.</remarks>
        public string Aliases
        {
            get { return mAliases; }
            set { mAliases = value; }
        }

        /// <summary>
        /// Gets or sets the maximum value for a numeric option.
        /// </summary>
        /// <value>The maximum value for a numeric option.</value>
        /// <remarks>This option should be null for any non-numeric option. This option also defaults to null for non-numeric
        /// options, or the maximum value for the type of this option for numerical options.</remarks>
        public object MaxValue
        {
            get { return mMaxValue; }
            set { mMaxValue = value; }
        }

        /// <summary>
        /// Gets or sets the minimum value for a numeric option.
        /// </summary>
        /// <value>The minimum value for a numeric option.</value>
        /// <remarks>This option should be null for any non-numeric option. This option also defaults to null for non-numeric
        /// options, or the minimum value for the type of this option for numerical options.</remarks>
        public object MinValue
        {
            get { return mMinValue; }
            set { mMinValue = value; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has its <see cref="MaxOccurs"/> property explicitly set.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has its <see cref="MaxOccurs"/> property explicitly set.; otherwise, <c>false</c>.
        /// </value>
        internal bool IsMaxOccursSet
        {
            get { return mMaxOccurs != null; }
        }

        /// <summary>
        /// Gets a value indicating whether this option requires explicit assignment.
        /// </summary>
        /// <value>A value indicating whether this option requires explicit assignment. If this value is 
        /// null that means that it should inherit this value from its group or ultimately the manager.</value>
        internal bool? DoesRequireExplicitAssignment
        {
            get { return mRequireExplicitAssignment; }
        }

        #endregion

        #region Private fields

        private BoolFunction mBoolFunction = BoolFunction.TrueIfPresent;
        private int? mMaxOccurs;
        private int mMinOccurs; // defaults to 0
        private string mGroupId;
        private string mProhibits;
        private string mName;
        private string mDescription;
        private object mDefaultValue;
        private bool? mRequireExplicitAssignment;
        private string mAliases;
        private object mMinValue;
        private object mMaxValue;

        #endregion 
    }
    
}

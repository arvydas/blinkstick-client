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
 *  $Id: OptionInfo.cs 7 2007-08-04 12:02:15Z palotas $
 */
using System;
using SCG = System.Collections.Generic;
using System.Text;
using C5;
using System.Globalization;

namespace Plossum.CommandLine
{
    /// <summary>
    /// Represents the descriptive properties of a command line option.
    /// </summary>
    public sealed class OptionInfo : IDisposable
    {
        #region Constructors 

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionInfo"/> class.
        /// </summary>
        /// <param name="usageInfo">The <see cref="UsageInfo" /> creating this OptionInfo</param>
        /// <param name="option">The option.</param>
        /// <param name="optionStyle">The option style.</param>
        internal OptionInfo(UsageInfo usageInfo, Option option, OptionStyles optionStyle)
        {
            mOption = option;
            mOptionStyles = optionStyle;
            mUsageInfo = usageInfo;

            foreach (string alias in mOption.Aliases)
            {
                mAliases.Add(OptionStyleManager.PrefixOptionForDescription(mOptionStyles, alias));
            }
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets an enumeration containing strings representing the prefixed names of the aliases of this option.
        /// </summary>
        /// <value>an enumeration containing strings representing the prefixed names of the aliases of this option.</value>
        public SCG.IEnumerable<string> Aliases
        {
            get 
            { 
                return mAliases; 
            }
        }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description
        {
            get { return mOption.Description; }
            set { mOption.Description = value; }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return OptionStyleManager.PrefixOptionForDescription(mOptionStyles, mOption.Name); }
        }

        /// <summary>
        /// Gets the id. 
        /// </summary>
        /// <value>The id.</value>
        /// <remarks>The id of an option is the same as its <see cref="Name"/>.</remarks>
        public string Id
        {
            get { return mOption.Name; }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override string ToString()
        {
            return ToString(0, 25, 50);
        }

        /// <summary>
        /// Returns a formatted string describing this option and its aliases.
        /// </summary>
        /// <param name="indent">The indentation to use.</param>
        /// <param name="nameColumnWidth">Width of the name column.</param>
        /// <param name="descriptionColumnWidth">Width of the description column.</param>
        /// <returns>a formatted string describing this option and its aliases that is suitable for displaying 
        /// as a help message.</returns>
        public string ToString(int indent, int nameColumnWidth, int descriptionColumnWidth)
        {
            if (nameColumnWidth < 1)
                throw new ArgumentException(String.Format(CultureInfo.CurrentUICulture, Plossum.Resources.CommandLineStrings.ArgMustBeGreaterThanZero, "nameColumnWidth"), "nameColumnWidth");

            if (descriptionColumnWidth < 1)
                throw new ArgumentException(String.Format(CultureInfo.CurrentUICulture, Plossum.Resources.CommandLineStrings.ArgMustBeGreaterThanZero, "descriptionColumnWidth"), "descriptionColumnWidth");

            if (indent < 0)
                throw new ArgumentException(String.Format(CultureInfo.CurrentUICulture, Plossum.Resources.CommandLineStrings.ArgMustBeNonNegative, "indent"), "indent");

            StringBuilder names = new StringBuilder();

            names.Append(Name);
            foreach (string alias in mOption.Aliases)
            {
                names.Append(", ");
                names.Append(OptionStyleManager.PrefixOptionForDescription(mOptionStyles, alias));
            }

            ColumnInfo nameColumn = new ColumnInfo(nameColumnWidth, names.ToString(), Alignment.Left);
            ColumnInfo descColumn = new ColumnInfo(descriptionColumnWidth, Description ?? "e", Alignment.Left, VerticalAlignment.Bottom);
            return StringFormatter.FormatInColumns(indent, mUsageInfo.ColumnSpacing, nameColumn, descColumn);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            mAliases.Dispose();
        }

        #endregion

        #region Private fields

        private ArrayList<string> mAliases = new ArrayList<string>();
        private OptionStyles mOptionStyles;
        private Option mOption;
        private UsageInfo mUsageInfo;
        #endregion
    }
}

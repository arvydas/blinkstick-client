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
 *  $Id: UsageInfo.cs 7 2007-08-04 12:02:15Z palotas $
 */
using System;
using SCG = System.Collections.Generic;
using System.Text;
using C5;
using System.Diagnostics;
using Plossum.Resources;
using System.Globalization;

namespace Plossum.CommandLine
{
    /// <summary>
    /// Represents the properties of a <see cref="CommandLineManagerAttribute"/> (or rather the object to which its 
    /// applied) that describe the command line syntax.
    /// </summary>
    /// <remarks>This class is the only way to programatically set usage descriptions, group names and similar, which 
    /// is required if globalization of the usage description is desired.  Users can not instantiate objects of this 
    /// class, but they are retrieved by the <see cref="CommandLineParser.UsageInfo"/> property.</remarks>
    public sealed class UsageInfo
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UsageInfo"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="optionStyles">The option styles.</param>
        /// <param name="parser">The parser.</param>
        internal UsageInfo(SCG.IEnumerable<KeyValuePair<string, IOption>> options, OptionStyles optionStyles, CommandLineParser parser)
        {
            mParser = parser;
            foreach (KeyValuePair<string, IOption> entry in options)
            {
                Option option = entry.Value as Option;
                if (option != null)
                {
                    if (option.Group != null)
                    {
                        if (!mGroups.Contains(option.Group.Id))
                        {
                            mGroups.Add(option.Group.Id, new OptionGroupInfo(this, option.Group, optionStyles));
                        }
                    }
                    else
                    {
                        Debug.Assert(!mOptions.Contains(option.Name));
                        mOptions.Add(option.Name, new OptionInfo(this, option, optionStyles));
                    }
                }
            }
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the name of the application.
        /// </summary>
        /// <value>The name of the application.</value>
        public string ApplicationName
        {
            get { return mParser.ApplicationName; }
            set { mParser.ApplicationName = value; }
        }

        /// <summary>
        /// Gets or sets the application version.
        /// </summary>
        /// <value>The application version.</value>
        public string ApplicationVersion
        {
            get { return mParser.ApplicationVersion; }
            set { mParser.ApplicationVersion = value; }
        }

        /// <summary>
        /// Gets or sets the application copyright.
        /// </summary>
        /// <value>The application copyright.</value>
        public string ApplicationCopyright
        {
            get { return mParser.ApplicationCopyright; }
            set { mParser.ApplicationCopyright = value; }
        }

        /// <summary>
        /// Gets or sets the application description.
        /// </summary>
        /// <value>The application description.</value>
        public string ApplicationDescription
        {
            get { return mParser.ApplicationDescription; }
            set { mParser.ApplicationDescription = value; }
        }

        /// <summary>
        /// Gets an enumeration of <see cref="OptionInfo"/> objects describing the options of this 
        /// command line manager that are <i>not</i> part of any option group.
        /// </summary>
        /// <value>an enumeration of <see cref="OptionInfo"/> objects describing the options of this 
        /// command line manager that are <i>not</i> part of any option group.</value>
        public SCG.IEnumerable<OptionInfo> Options
        {
            get { return mOptions.Values; }
        }

        /// <summary>
        /// Gets an enumeration of the <see cref="OptionGroupInfo"/> objects describin the option groups
        /// of this command line manager.
        /// </summary>
        /// <value>an enumeration of the <see cref="OptionGroupInfo"/> objects describin the option groups
        /// of this command line manager.</value>
        public SCG.IEnumerable<OptionGroupInfo> Groups
        {
            get { return mGroups.Values; }
        }

        /// <summary>
        /// Gets or sets the column spacing to use for any string formatting involving multiple columns.
        /// </summary>
        /// <value>The column spacing used for any string formatting involving multiple columns.</value>
        public int ColumnSpacing
        {
            get { return mColumnSpacing; }
            set
            {
                if (value < 0)
                    throw new ArgumentException(String.Format(CultureInfo.CurrentUICulture, CommandLineStrings.ArgMustBeNonNegative, "value"), "value");
                mColumnSpacing = value;
            }
        }

        /// <summary>
        /// Gets or sets the width of the indent to use for any string formatting by this <see cref="UsageInfo"/>.
        /// </summary>
        /// <value>the width of the indent to use for any string formatting by this <see cref="UsageInfo"/>.</value>
        public int IndentWidth
        {
            get { return mIndentWidth; }
            set
            {
                if (value < 0)
                    throw new ArgumentException(String.Format(CultureInfo.CurrentUICulture, CommandLineStrings.ArgMustBeNonNegative, "value"), "value");
                mIndentWidth = value;
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Gets the option with the specified name.
        /// </summary>
        /// <param name="name">The name of the option to retrieve.</param>
        /// <returns>The option in the option manager described by this object with the specified name, or
        /// a null reference if no such option exists.</returns>        
        public OptionInfo GetOption(string name)
        {
            OptionInfo description;
            if (!mOptions.Find(name, out description))
            {
                // Search through all groups for this option
                foreach (OptionGroupInfo gdesc in mGroups.Values)
                {
                    if ((description = gdesc.GetOption(name)) != null)
                        return description;
                }
                return null;
            }
            return description;
        }

        /// <summary>
        /// Gets the option group with the specified id.
        /// </summary>
        /// <param name="id">The id of the option group to retrieve.</param>
        /// <returns>The option group with the specified id of the option manager described 
        /// by this object, or a null reference if no such option group exists.</returns>
        public OptionGroupInfo GetGroup(string id)
        {
            OptionGroupInfo desc;
            if (!mGroups.Find(id, out desc))
                return null;
            return desc;
        }

        /// <summary>
        /// Gets a string consisting of the program name, version and copyright notice. 
        /// </summary>
        /// <param name="width">The total width in characters in which the string should be fitted</param>
        /// <returns>a string consisting of the program name, version and copyright notice.</returns>
        /// <remarks>This string is suitable for printing as the first output of a console application.</remarks>
        public string GetHeaderAsString(int width)
        {
            StringBuilder result = new StringBuilder();
            result.Append(ApplicationName ?? "Unnamed application");
            if (ApplicationVersion != null)
            {
                result.Append("  ");
                result.Append(CommandLineStrings.Version);
                result.Append(' ');
                result.Append(ApplicationVersion);
            }
            result.Append(Environment.NewLine);

            if (ApplicationCopyright != null)
            {
                result.Append(ApplicationCopyright);
                result.Append(Environment.NewLine);
            }
            return StringFormatter.WordWrap(result.ToString(), width);
        }

        /// <summary>
        /// Gets a formatted string describing the options and groups available.
        /// </summary>
        /// <param name="width">The maximum width of each line in the returned string.</param>
        /// <returns>A formatted string describing the options available in this parser</returns>
        /// <exception cref="ArgumentException">The specified width was too small to generate the requested list.</exception>
        public string GetOptionsAsString(int width)
        {
            // Remove spacing between columns
            width -= ColumnSpacing;

            if (width < 2)
                throw new ArgumentException(String.Format(CultureInfo.CurrentUICulture, CommandLineStrings.WidthMustNotBeLessThan0, ColumnSpacing + 2), "width");

            // Default minimum width
            int maxNameWidth = 5;

            // Get the maximum option name length from options not in groups
            foreach (OptionInfo option in mOptions.Values)
            {
                maxNameWidth = Math.Max(option.Name.Length, maxNameWidth);
                foreach (string alias in option.Aliases)
                {
                    maxNameWidth = Math.Max(alias.Length, maxNameWidth);
                }
            }

            // Get the maximum option name length from option inside groups
            foreach (OptionGroupInfo group in mGroups.Values)
            {
                foreach (OptionInfo option in group.Options)
                {
                    maxNameWidth = Math.Max(option.Name.Length, maxNameWidth);
                    foreach (string alias in option.Aliases)
                    {
                        maxNameWidth = Math.Max(alias.Length, maxNameWidth);
                    }
                }
            }

            // Add room for '--' and comma after the option name.
            maxNameWidth += 3; 

            // Make sure the name column isn't more than half the specified width
            maxNameWidth = Math.Min(width / 2, maxNameWidth);

            return GetOptionsAsString(maxNameWidth, width - maxNameWidth);
        }

        /// <summary>
        /// Gets a string describing all the options of this option manager. Usable for displaying as a help 
        /// message to the user, provided that descriptions for all options and groups are provided.
        /// </summary>
        /// <param name="nameColumnWidth">The width in characters of the column holding the names of the options.</param>
        /// <param name="descriptionColumnWidth">The width in characters of the column holding the descriptions of the options.</param>
        /// <returns>A string describing all the options of this option manager.</returns>
        public string GetOptionsAsString(int nameColumnWidth, int descriptionColumnWidth)
        {
            StringBuilder result = new StringBuilder();
            
            if (!mOptions.IsEmpty)
            {
                result.Append(StringFormatter.WordWrap(CommandLineStrings.Options, nameColumnWidth + descriptionColumnWidth + ColumnSpacing, WordWrappingMethod.Optimal, Alignment.Left, ' '));
                result.Append(Environment.NewLine);
                foreach (OptionInfo option in mOptions.Values)
                {
                    result.Append(option.ToString(IndentWidth, nameColumnWidth, descriptionColumnWidth - IndentWidth));
                    result.Append(Environment.NewLine);
                }
                result.Append(Environment.NewLine);
            }

            foreach (OptionGroupInfo group in mGroups.Values)
            {
                result.Append(group.ToString(0, nameColumnWidth, descriptionColumnWidth));
                result.Append(Environment.NewLine);
            }
            return result.ToString();
        }

        /// <summary>
        /// Gets the list of errors as a formatted string.
        /// </summary>
        /// <param name="width">The width of the field in which to format the error list.</param>
        /// <returns>The list of errors formatted inside a field of the specified <paramref name="width"/></returns>
        public string GetErrorsAsString(int width)
        {
            if (width < IndentWidth + 7)
                throw new ArgumentException(String.Format(CultureInfo.CurrentUICulture, CommandLineStrings.Arg0MustNotBeLessThan1, "width", IndentWidth + 7), "width");

            StringBuilder result = new StringBuilder();
            result.Append(StringFormatter.WordWrap("Errors:", width));
            result.Append(Environment.NewLine);

            StringBuilder errors = new StringBuilder();
            foreach (ErrorInfo error in mParser.Errors)
            {
                errors.Append(error.Message);
                if (error.FileName != null)
                {
                    errors.Append(String.Format(CultureInfo.CurrentUICulture, CommandLineStrings.OnLine0InFile1, error.Line, error.FileName));
                }
                result.Append(StringFormatter.FormatInColumns(IndentWidth, 1, new ColumnInfo(1, "*"), new ColumnInfo(width - 1 - IndentWidth - 1, errors.ToString())));
                result.Append('\n');
                errors.Length = 0;
            }

            return result.ToString();
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        /// <remarks>This is equivalent to calling <see cref="ToString(int)">ToString(78)</see></remarks>
        public override string ToString()
        {
            return ToString(78);
        }

        /// <summary>
        /// Converts this <see cref="UsageInfo"/> instance to a string.
        /// </summary>
        /// <param name="width">The width of the field (in characters) in which to format the usage description.</param>
        /// <returns>A string including the header, and a complete list of the options and their descriptions
        /// available in this <see cref="UsageInfo"/> object.</returns>
        public string ToString(int width)
        {
            return ToString(width, false);
        }

        /// <summary>
        /// Converts this <see cref="UsageInfo"/> instance to a string.
        /// </summary>
        /// <param name="width">The width of the field (in characters) in which to format the usage description.</param>
        /// <param name="includeErrors">if set to <c>true</c> any errors that occured during parsing the command line will be included
        /// in the output.</param>
        /// <returns>A string including the header, optionally errors, and a complete list of the options and their descriptions
        /// available in this <see cref="UsageInfo"/> object.</returns>
        public string ToString(int width, bool includeErrors)
        {
            StringBuilder result = new StringBuilder();
            result.Append(GetHeaderAsString(width));
            result.Append(Environment.NewLine);

            if (mParser.HasErrors && includeErrors)
            {
                result.Append(GetErrorsAsString(width));
                result.Append(Environment.NewLine);
            }

            result.Append(GetOptionsAsString(width));
            result.Append(Environment.NewLine);
            return result.ToString();
        }


        #endregion

        #region Private fields

        private CommandLineParser mParser;
        private TreeDictionary<string, OptionGroupInfo> mGroups = new TreeDictionary<string, OptionGroupInfo>();
        private TreeDictionary<string, OptionInfo> mOptions = new TreeDictionary<string, OptionInfo>();
        private int mColumnSpacing = 3;
        private int mIndentWidth = 3;

        #endregion

    }
}

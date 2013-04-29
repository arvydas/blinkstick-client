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
 *  $Id: OptionStyleManager.cs 3 2007-07-29 13:32:10Z palotas $
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Diagnostics;

namespace Plossum.CommandLine
{
    /// <summary>
    /// Utility class for manipulating <see cref="OptionStyles"/>.
    /// </summary>
    public static class OptionStyleManager
    {
        /// <summary>
        /// Determines whether all of the specified <paramref name="flags"/> are enabled in the specified <paramref name="optionStyle"/>.
        /// </summary>
        /// <param name="optionStyle">The option style.</param>
        /// <param name="flags">The flags.</param>
        /// <returns>
        /// 	<c>true</c> if all of the specified <paramref name="flags"/> are enabled in the specified <paramref name="optionStyle"/>; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsAllEnabled(OptionStyles optionStyle, OptionStyles flags)
        {
            return (optionStyle & flags) == flags;
        }

        /// <summary>
        /// Determines whether any of the specified <paramref name="flags"/> are enabled in the specified <paramref name="optionStyle"/>.
        /// </summary>
        /// <param name="optionStyle">The option style.</param>
        /// <param name="flags">The flags.</param>
        /// <returns>
        /// 	<c>true</c> if any of the specified <paramref name="flags"/> are enabled in the specified <paramref name="optionStyle"/>; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsAnyEnabled(OptionStyles optionStyle, OptionStyles flags)
        {
            return (optionStyle & flags) != OptionStyles.None;
        }

        /// <summary>
        /// Determines whether the specified option style is valid.
        /// </summary>
        /// <param name="optionStyle">The option style.</param>
        /// <returns>
        /// 	<c>true</c> if the specified option style is valid; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>An option style is invalid if the <see cref="OptionStyles.ShortUnix"/> flag is not enabled, but the 
        /// <see cref="OptionStyles.Group"/> or <see cref="OptionStyles.Plus"/> is. This normally doesn't occur
        /// if you only use the binary or to combine flags however, since the values of the group and plus 
        /// options also include the short unix style.</remarks>
        public static bool IsValid(OptionStyles optionStyle)
        {
            return !((optionStyle & OptionStyles.ShortUnix) == OptionStyles.None &&
                (optionStyle & (OptionStyles.Plus | OptionStyles.Group)) != OptionStyles.None);
        }

        /// <summary>
        /// Gets the switch character used to specify an option of the specified style on the command line.
        /// </summary>
        /// <param name="optionStyle">The option style.</param>
        /// <param name="optionName">The name of the option.</param>
        /// <returns>the switch character used to specify this option.</returns>
        /// <remarks><para>If <paramref name="optionStyle"/> includes several option styles, the switch for the most 
        /// specific one representing a single option style prefix is returned. The switches for the corresponding
        /// styles are as follows (in order from most specific to least):</para>
        /// <para>
        /// <list type="table">
        /// <listheader>
        /// <item>
        /// <term>OptionStyle</term>
        /// <term>Prefix</term>
        /// </item>
        /// </listheader>
        /// <item>
        /// <term><i>All</i></term>
        /// <term><c>+</c></term>
        /// </item>
        /// <item>
        /// <term><b>Plus</b></term>
        /// <term><c>+</c></term>
        /// </item>
        /// <item>
        /// <term><i>Group</i></term>
        /// <term><c>-</c></term>
        /// </item>
        /// <item>
        /// <term><b>ShortUnix</b></term>
        /// <term><c>-</c></term>
        /// </item>
        /// <item>
        /// <term><b>File</b></term>
        /// <term><c>@</c></term>
        /// </item>
        /// <item>
        /// <term><b>LongUnix</b></term>
        /// <term><c>--</c></term>
        /// </item>
        /// <item>
        /// <term><b>Windows</b></term>
        /// <term><c>/</c></term>
        /// </item>
        /// </list>
        /// (Items in <i>italics</i> does not represent a single unique prefix)
        /// </para>
        /// </remarks>
        public static string GetPrefix(OptionStyles optionStyle, string optionName)
        {
            if (optionName == null)
                throw new ArgumentNullException("optionName");

            // The ordering here is important
            if (IsAllEnabled(optionStyle, OptionStyles.LongUnix) && optionName.Length > 1)
                return "--";
            if (IsAllEnabled(optionStyle, OptionStyles.Plus))
                return "+";
            else if (IsAllEnabled(optionStyle, OptionStyles.ShortUnix))
                return "-";
            else if (IsAllEnabled(optionStyle, OptionStyles.File))
                return "@";
            else if (IsAnyEnabled(optionStyle, OptionStyles.LongUnix))
                return "--";
            else if (IsAllEnabled(optionStyle, OptionStyles.Windows))
                return "/";
            else
                throw new NotImplementedException(String.Format(CultureInfo.CurrentUICulture, "Internal error: An OptionNameToken was created with an unsupported set of option style flags ({0})", optionStyle.ToString()));
        }

        /// <summary>
        /// Prefixes the option name with the prefix(es) with which it should be used.
        /// </summary>
        /// <param name="optionStyle">The option style.</param>
        /// <param name="optionName">Name of the option.</param>
        /// <returns>A string representing the name of the option prefixed according to the enabled option styles specified.</returns>
        /// <remarks>This method always prefers prefixing options with unix style to windows style prefixes if both are 
        /// enabled.  If <see cref="OptionStyles.Plus"/> is enabled, the option will be prefixed with "[+|-]" to indicate
        /// that either prefix may be used.</remarks>
        public static string PrefixOptionForDescription(OptionStyles optionStyle, string optionName)
        {
            if (optionName == null)
                throw new ArgumentNullException("optionName");

            if (IsAllEnabled(optionStyle, OptionStyles.Plus))
                return "[+|-]" + optionName;
            else if (optionName.Length == 1)
            {
                if (IsAllEnabled(optionStyle, OptionStyles.ShortUnix))
                    return "-" + optionName;
                else if (IsAllEnabled(optionStyle, OptionStyles.LongUnix))
                    return "--" + optionName;
                else
                {
                    Debug.Assert(IsAllEnabled(optionStyle, OptionStyles.Windows));
                    return "/" + optionName;
                }
            }
            else if (!IsAllEnabled(optionStyle, OptionStyles.Group) && IsAllEnabled(optionStyle, OptionStyles.ShortUnix))
                return "-" + optionName;
            else if (IsAllEnabled(optionStyle, OptionStyles.LongUnix))
                return "--" + optionName;
            else
            {
                Debug.Assert(IsAllEnabled(optionStyle, OptionStyles.Windows));
                return "/" + optionName;
            }
        }
    }
}

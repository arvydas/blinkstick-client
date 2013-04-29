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
 *  $Id: OptionStyles.cs 3 2007-07-29 13:32:10Z palotas $
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace Plossum.CommandLine
{
    
    /// <summary>
    /// Flags indicating the option styles recognized by the <see cref="CommandLineParser"/>. 
    /// </summary>
    /// <remarks>Option styles indicates how options may be specified on the command line, for an example the 
    /// <see cref="Windows"/> style dictate that options are prefixed by a slash '/' while the <see cref="LongUnix"/>
    /// style is prefixed by a double dash ('--'). Options may be freely combined using the binary or (|) operator.
    /// <note>Note that both <see cref="Plus"/> and <see cref="Group"/> implies the <see cref="ShortUnix"/> option 
    /// style. You should never use binary operators to prevent this implication.</note></remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2217:DoNotMarkEnumsWithFlags"), Flags()]
    // This is deliberate, they are still flags.
    public enum OptionStyles : int
    {
        /// <summary>
        /// No options are enabled nor will be parsed. Everything on the command line is treated 
        /// as remaining arguments (see <see cref="CommandLineParser.RemainingArguments"/>).
        /// </summary>
        None = 0x00,
        /// <summary>
        /// The windows style for options is recognized. This means that options are prefixed 
        /// with the slash '/' character. An option specified in this style will never be 
        /// grouped (see <see cref="Group"/>).
        /// </summary>
        Windows = 0x01,
        /// <summary>
        /// The long unix style for options is recognized. This means that options with long 
        /// names (more than one character) are prefixed with two dashes '--'. (Also options
        /// of only one character in length can be specified in this style). Options specified
        /// in this style will never be grouped (see <see cref="Group"/>).
        /// </summary>
        LongUnix = 0x02,
        /// <summary>
        /// Option files are recognized and parsed automatically by the <see cref="CommandLineParser"/>.
        /// This allows specifying a file name prefixed by the '@' character on the command line. Any 
        /// such file will then be opened and parsed for additional command line options by the 
        /// command line parser.  The syntax for the options (or additional arguments) specified in the
        /// file is the very same as that for the command line itself).
        /// </summary>
        File = 0x04,
        /// <summary>
        /// The short unix style for options is recognized. This means that options are prefixed
        /// by a single dash ('-').  If <see cref="Group"/> is also specified, any options 
        /// specified in this style will be grouped.
        /// </summary>
        ShortUnix = 0x08,
        /// <summary>
        /// The plus style for options is recognized. Specifying this style implies the <see cref="ShortUnix"/>
        /// style. This means that options can be prefixed with either the dash ('-') character or the 
        /// ('+') character. This can be useful for boolean options when used with the <see cref="BoolFunction.UsePrefix"/>
        /// option, in which case the prefix of the option will indicate what value to set the option to.
        /// Options specified in this style will be grouped if the <see cref="Group"/> option is also specified.
        /// </summary>
        Plus = 0x10 | ShortUnix,
        /// <summary>
        /// Grouping of options is enabled for the <see cref="ShortUnix"/> and <see cref="Plus"/> style. Specifying
        /// this style also implies the <see cref="ShortUnix"/> style.  Grouping of options means that several options
        /// with names only one character long can be concatenated on the command line. For an example the command line
        /// "tar -xvzf file.tar.gz" would be interpreted as "tar -x -v -z -f file.tar.gz". If this option is specified
        /// and option styles with names longer than one character should also be recognized the <see cref="LongUnix"/>
        /// option style should also be enabled.
        /// </summary>
        Group = 0x20 | ShortUnix,
        /// <summary>
        /// This option style indicates a combination of the <see cref="ShortUnix"/> and <see cref="LongUnix"/> flags.
        /// </summary>
        Unix = ShortUnix | LongUnix,
        /// <summary>
        /// This means all option styles above are enabled.
        /// </summary>
        All = 0x3F
    }
}

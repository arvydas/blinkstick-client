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
 *  $Id: ParseErrorCodes.cs 5 2007-07-31 08:46:02Z palotas $
 */

namespace Plossum.CommandLine
{
    /// <summary>
    /// Indicates the type of the error of a <see cref="ErrorInfo"/>.
    /// </summary>
    /// <seealso cref="CommandLineParser.Errors"/>
    public enum ParseErrorCodes
    {
        /// <summary>
        /// A value was quoted but no closing quote was found on the command line.
        /// </summary>
            MissingClosingQuote,
            /// <summary>
            /// An option switch character was specified but without an option name following it.
            /// </summary>
            EmptyOptionName,
            /// <summary>
            /// The option specified does not exist.
            /// </summary>
            UnknownOption,
            /// <summary>
            /// A required option was not specified.
            /// </summary>
            MissingRequiredOption,
            /// <summary>
            /// An assignment character was found where it should not have been on the command line.
            /// </summary>
            UnexpectedAssignment,
            /// <summary>
            /// An option is specified either too many times or too few times on the command line.
            /// </summary>
            IllegalCardinality,
            /// <summary>
            /// An option requiring a value was specified without a value.
            /// </summary>
            MissingValue,
            /// <summary>
            /// An option not accepting a value was explicitly assigned a value.
            /// </summary>
            AssignmentToNonValueOption,
            /// <summary>
            /// An option specified was prohibited by another option that was also specified.
            /// </summary>
            OptionProhibited,
            /// <summary>
            /// A value assigned to a numerical option was outside the range specified by 
            /// <see cref="CommandLineOptionAttribute.MinValue"/> and <see cref="CommandLineOptionAttribute.MaxValue"/>.
            /// </summary>
            Overflow,
            /// <summary>
            /// The value specified for an option was in an invalid format, or an illegal value for the specified type.
            /// </summary>
            InvalidFormat,
            /// <summary>
            /// The file specified was not found, or could not be opened for reading.
            /// </summary>
            FileNotFound,
            /// <summary>
            /// The value specified for the option failed user validation.
            /// </summary>
            InvalidValue,
            /// <summary>
            /// An unknown error occured.
            /// </summary>
            UnknownError
    }
}

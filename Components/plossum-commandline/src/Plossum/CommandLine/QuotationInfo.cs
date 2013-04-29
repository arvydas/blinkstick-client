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
 *  $Id: QuotationInfo.cs 3 2007-07-29 13:32:10Z palotas $
 */
using System;
using SCG = System.Collections.Generic;
using C5;

namespace Plossum.CommandLine
{
    /// <summary>
    /// Represents information about the escapable characters within a quoted value.
    /// </summary>
    public class QuotationInfo
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="QuotationInfo"/> class.
        /// </summary>
        /// <param name="quotationMark">The quotation mark used for this quotation.</param>
        public QuotationInfo(char quotationMark)
        {
            mQuotationMark = quotationMark;
        }

        #endregion 

        #region Public properties

        /// <summary>
        /// Gets the quotation mark.
        /// </summary>
        /// <value>The quotation mark.</value>
        public char QuotationMark
        {
            get { return mQuotationMark; }
        }

        #endregion 

        #region Public methods

        /// <summary>
        /// Adds the escape code to this quotation.
        /// </summary>
        /// <param name="code">The code, i.e. the character that will be used after the escape 
        /// character to denote this escape sequence.</param>
        /// <param name="replacement">The character with which the escape sequence will be replaced.</param>
        public void AddEscapeCode(char code, char replacement)
        {
            mEscapeCodes.Add(code, replacement);
        }

        /// <summary>
        /// Removes an escape code from this quotation.
        /// </summary>
        /// <param name="code">The code to remove.</param>
        public void RemoveEscapeCode(char code)
        {
            mEscapeCodes.Remove(code);
        }

        /// <summary>
        /// Determines whether the specified character is an escape code within this quotation.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns>
        /// 	<c>true</c> if the specified character is an escape code within this quotation; otherwise, <c>false</c>.
        /// </returns>
        public bool IsEscapeCode(char code)
        {
            return mEscapeCodes.Contains(code);
        }

        /// <summary>
        /// Escapes the escape code character specified.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns>The replacement character for the specified escape code if one is available in this 
        /// quotation, otherwise returns the character unchanged.</returns>
        public char EscapeCharacter(char code)
        {
            char replacement;
            if (!mEscapeCodes.Find(code, out replacement))
                return code;
            else
                return replacement;
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"></see> to compare with the current <see cref="T:System.Object"></see>.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            QuotationInfo qi = obj as QuotationInfo;
            if (qi == null)
                return false;

            return mQuotationMark.Equals(qi.mQuotationMark) && mEscapeCodes.Equals(qi.mEscapeCodes);
        }

        /// <summary>
        /// Serves as a hash function for a particular type. <see cref="M:System.Object.GetHashCode"></see> is suitable for use in hashing algorithms and data structures like a hash table.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override int GetHashCode()
        {
            return mQuotationMark.GetHashCode() ^ mEscapeCodes.GetHashCode();
        }

        #endregion 

        #region Private fields

        private IDictionary<char, char> mEscapeCodes = new HashDictionary<char, char>();
        private char mQuotationMark;

        #endregion 
    }
}

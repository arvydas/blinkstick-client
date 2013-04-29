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
 *  $Id: Token.cs 3 2007-07-29 13:32:10Z palotas $
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace Plossum.CommandLine
{
    /// <summary>
    /// Represents a command line token.
    /// </summary>
    internal class Token
    {
        #region Public types

        /// <summary>
        /// Represents the various token types
        /// </summary>
        public enum TokenTypes
        {
            /// <summary>
            /// A token of type <see cref="ValueToken"/>
            /// </summary>
            ValueToken,
            /// <summary>
            /// A token of type <see cref="AssignmentToken"/>
            /// </summary>
            AssignmentToken,
            /// <summary>
            /// A token of type <see cref="OptionNameToken"/>
            /// </summary>
            OptionNameToken,
            /// <summary>
            /// A token of type <see cref="EndToken"/>
            /// </summary>
            EndToken,
            /// <summary>
            /// A token of type <see cref="OptionFileToken"/>
            /// </summary>
            OptionFileToken
        }
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Token"/> class.
        /// </summary>
        /// <param name="tokenType">Type of the token.</param>
        /// <param name="text">The text representing this token. (Mainly for use in error messages etc)</param>
        public Token(TokenTypes tokenType, string text)
        {
            Debug.Assert(text != null);
            mTokenType = tokenType;
            mText = text;
        }

        #endregion

        /// <summary>
        /// Gets the type of the token.
        /// </summary>
        /// <value>The type of the token.</value>
        public TokenTypes TokenType
        {
            get { return mTokenType; }
        }

        /// <summary>
        /// Gets the text.
        /// </summary>
        /// <value>The text.</value>
        public string Text
        {
            get { return mText; }
        }

        private TokenTypes mTokenType;
        private string mText;
    }
}

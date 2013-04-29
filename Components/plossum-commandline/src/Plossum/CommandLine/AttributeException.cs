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
 *  $Id: AttributeException.cs 3 2007-07-29 13:32:10Z palotas $
 */
using System;
using System.Reflection;
using System.Runtime.Serialization;
using System.Globalization;

namespace Plossum.CommandLine
{
    // Indicates programming error
    /// <summary>
    /// The exception is thrown when the values of the attributes <see cref="CommandLineManagerAttribute"/>, <see cref="CommandLineOptionAttribute"/>
    /// and <see cref="CommandLineOptionGroupAttribute"/> are set incorrectly or the attributes are used in an
    /// erroneous way.
    /// </summary>
    /// <remarks>This exception indicates a programming error rather than a user error, and should never be thrown in 
    /// a finished program.</remarks>
    [Serializable]
    public class AttributeException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeException"/> class.
        /// </summary>
        public AttributeException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeException"/> class.
        /// </summary>
        /// <param name="attributeType">Type of the attribute on which the error is present.</param>
        /// <param name="objectType">Type of the object implementing the attribute on which the error occured.</param>
        /// <param name="message">The error message.</param>
        public AttributeException(Type attributeType, Type objectType, string message)
            : this(String.Format(CultureInfo.CurrentUICulture, "In attribute {0} defined on {1}; {2}",
            attributeType.Name, objectType.FullName, message))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeException"/> class.
        /// </summary>
        /// <param name="attributeType">Type of the attribute on which the error is present.</param>
        /// <param name="member">The member assigned the attribute with the error.</param>
        /// <param name="message">The error message.</param>
        public AttributeException(Type attributeType, MemberInfo member, string message)
            : this(String.Format(CultureInfo.CurrentUICulture, "In attribute {0} defined on member \"{1}\" of {2}; {3}",
            attributeType.Name, member.Name, member.DeclaringType.FullName, message))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public AttributeException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The inner exception.</param>
        public AttributeException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"></see> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"></see> is zero (0). </exception>
        /// <exception cref="T:System.ArgumentNullException">The info parameter is null. </exception>
        protected AttributeException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

    }
}

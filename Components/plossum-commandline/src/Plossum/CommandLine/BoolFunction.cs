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
 *  $Id: BoolFunction.cs 3 2007-07-29 13:32:10Z palotas $
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace Plossum.CommandLine
{
    /// <summary>
    /// Specifies how a boolean command line option should be evaluated.
    /// </summary>
    /// <remarks>This enumeration applies to the <see cref="CommandLineOptionAttribute.BoolFunction"/> property, and 
    /// is ignored unless the attribute is applied to an attribute with the base type of <b>bool</b>. 
    /// Options that are boolean values can be treated in different ways according to the values of this enumeration.</remarks>
    public enum BoolFunction
    {
        /// <summary>
        /// The option is set to the value specified, which means that the option requires a value (unless a default value 
        /// is assigned, see <see cref="CommandLineOptionAttribute.DefaultAssignmentValue"/>). The value must be specified as 
        /// either <see cref="Boolean.TrueString"/> or <see cref="Boolean.FalseString"/>.
        /// </summary>
        Value,
        /// <summary>
        /// The option does not accept a value, and if present on the command line the member to which the attribute is
        /// applied will be set to <b>true</b>.
        /// </summary>
        TrueIfPresent,
        /// <summary>
        /// The option does not accept a value, and if present on the command line the member to which the attribute is
        /// applied will be set to <b>false</b>.
        /// </summary>
        FalseIfPresent,
        /// <summary>
        /// The option does not accept a value. Instead if present on the command line the member to which the attribute
        /// is applied will be set to <b>true</b> if the prefix of the option was '+', otherwise; <b>false</b>. It is an
        /// error to use this enumeration if <see cref="OptionStyles.Plus"/> was not specified.
        /// </summary>
        UsePrefix
    }
}

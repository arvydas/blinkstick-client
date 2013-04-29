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
 *  $Id: OptionGroupRequirement.cs 3 2007-07-29 13:32:10Z palotas $
 */
 using System;
using System.Collections.Generic;
using System.Text;

namespace Plossum.CommandLine
{
    /// <summary>
    /// Specifies the requirements on an option group.
    /// </summary>
    public enum OptionGroupRequirement
    {
        /// <summary>
        /// Indicates that no requirement is placed on the options of this group.
        /// </summary>
        None,
        /// <summary>
        /// Indicates that at most one of the options in this group may be specified on the command line.
        /// </summary>
        AtMostOne,
        /// <summary>
        /// Indicates that at least one of the options in this group may be specified on the command line.
        /// </summary>
        AtLeastOne,
        /// <summary>
        /// Indicates that exactly one of the options in this group must be specified on the command line.
        /// </summary>
        ExactlyOne,
        /// <summary>
        /// Indicates that all options of this group must be specified.
        /// </summary>
        All        
    }
}

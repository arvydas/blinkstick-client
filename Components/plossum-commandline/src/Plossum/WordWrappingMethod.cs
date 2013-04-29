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
 *  $Id: WordWrappingMethod.cs 3 2007-07-29 13:32:10Z palotas $
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace Plossum
{
    /// <summary>
    /// Represents the word wrapping method to use for various operations performed by <see cref="StringFormatter"/>.
    /// </summary>
    public enum WordWrappingMethod
    {
        /// <summary>
        /// Uses a greedy algorithm for performing the word wrapping, 
        /// that puts as many words on a line as possible, then moving on to the next line to do the 
        /// same until there are no more words left to place.
        /// </summary>
        /// <remarks>This is the fastest method, but will often create a less esthetically pleasing result than the
        /// <see cref="Optimal"/> method.</remarks>
        Greedy,
        /// <summary>
        /// Uses an algorithm attempting to create an optimal solution of breaking the lines, where the optimal solution is the one
        /// where the remaining space on the end of each line is as small as possible.
        /// </summary>
        /// <remarks>This method creates esthetically more pleasing results than those created by the <see cref="Greedy"/> method, 
        /// but it does so at a significantly slower speed.  This method will work fine for wrapping shorter strings for console 
        /// output, but should probably not be used for a real time WYSIWYG word processor or something similar.</remarks>
        Optimal
    }
}

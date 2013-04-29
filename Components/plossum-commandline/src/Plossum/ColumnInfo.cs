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
 *  $Id: ColumnInfo.cs 7 2007-08-04 12:02:15Z palotas $
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace Plossum
{
    /// <summary>
    /// Represents a column to be used with <see cref="StringFormatter.FormatInColumns"/>.
    /// </summary>
    public struct ColumnInfo
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnInfo"/> struct.
        /// </summary>
        /// <param name="width">The width of the column in (fixed-width) characters.</param>
        /// <param name="content">The content of this column.</param>
        /// <param name="alignment">The alignment to use for this column.</param>
        /// <param name="verticalAlignment">The vertical alignment to use for this column</param>        
        /// <param name="method">The word wrapping method to use for this column</param>
        public ColumnInfo(int width, string content, Alignment alignment, VerticalAlignment verticalAlignment, WordWrappingMethod method)
        {
            mWidth = width;
            mContent = content;
            mAlignment = alignment;
            mVerticalAlignment = verticalAlignment;
            mWordWrappingMethod = method;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnInfo"/> struct.
        /// </summary>
        /// <param name="width">The width of the column in (fixed-width) characters.</param>
        /// <param name="content">The content of this column.</param>
        /// <param name="alignment">The alignment to use for this column.</param>
        /// <param name="verticalAlignment">The vertical alignment to use for this column</param>
        public ColumnInfo(int width, string content, Alignment alignment, VerticalAlignment verticalAlignment)
            : this(width, content, alignment, verticalAlignment, WordWrappingMethod.Optimal)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnInfo"/> struct.
        /// </summary>
        /// <param name="width">The width of the column in (fixed-width) characters.</param>
        /// <param name="content">The content of this column.</param>
        /// <param name="alignment">The alignment to use for this column.</param>
        /// <remarks>The word wrapping method used will be the one described by <see cref="Plossum.WordWrappingMethod.Optimal"/>.</remarks>
        public ColumnInfo(int width, string content, Alignment alignment)
            : this(width, content, alignment, VerticalAlignment.Top)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnInfo"/> struct.
        /// </summary>
        /// <param name="width">The width of the column in (fixed-width) characters.</param>
        /// <param name="content">The content of this column.</param>
        /// <remarks>The word wrapping method used will be the one described by <see cref="Plossum.WordWrappingMethod.Optimal"/>, and 
        /// each line in this column will be left aligned.</remarks>
        public ColumnInfo(int width, string content)
            : this(width, content, Alignment.Left)
        {
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(ColumnInfo lhs, ColumnInfo rhs)
        {
            return lhs.Equals(rhs);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(ColumnInfo lhs, ColumnInfo rhs)
        {
            return !lhs.Equals(rhs);
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">Another object to compare to.</param>
        /// <returns>
        /// true if obj and this instance are the same type and represent the same value; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (!(obj is ColumnInfo))
                return false;

            ColumnInfo ci = (ColumnInfo)obj;

            return Width.Equals(ci.Width) && Content.Equals(ci.Content) && Alignment.Equals(ci.Alignment) && WordWrappingMethod.Equals(ci.WordWrappingMethod);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        public override int GetHashCode()
        {
            return Width.GetHashCode() ^ Content.GetHashCode() ^ Alignment.GetHashCode() ^ WordWrappingMethod.GetHashCode();
        }

        #endregion

        #region Public properties 

        /// <summary>
        /// Gets the width of this column in fixed width characters.
        /// </summary>
        /// <value>the width of this column in fixed width characters.</value>
        public int Width
        {
            get { return mWidth; }
        }

        /// <summary>
        /// Gets the content of this column.
        /// </summary>
        /// <value>The content of this column.</value>
        public string Content
        {
            get { return mContent; }
        }

        /// <summary>
        /// Gets the alignment of this column.
        /// </summary>
        /// <value>The alignment of this column.</value>
        public Alignment Alignment
        {
            get { return mAlignment; }
        }

        /// <summary>
        /// Gets the word wrapping method to use for this column.
        /// </summary>
        /// <value>The the word wrapping method to use for this column.</value>
        public WordWrappingMethod WordWrappingMethod
        {
            get { return mWordWrappingMethod; }
        }

        /// <summary>
        /// Gets or sets the vertical alignment of the contents of this column.
        /// </summary>
        /// <value>The vertical alignment of this column.</value>
        public VerticalAlignment VerticalAlignment
        {
            get { return mVerticalAlignment; }
            set { mVerticalAlignment = value; }
        }
	
        #endregion

        #region Private fields

        private WordWrappingMethod mWordWrappingMethod;
        private Alignment mAlignment;
        private int mWidth;
        private string mContent;
        private VerticalAlignment mVerticalAlignment;

        #endregion
    }

}

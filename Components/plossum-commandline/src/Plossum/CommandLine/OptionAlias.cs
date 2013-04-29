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
 *  $Id: OptionAlias.cs 3 2007-07-29 13:32:10Z palotas $
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace Plossum.CommandLine
{
    internal class OptionAlias : IOption
    {
        public OptionAlias(string aliasName, Option definingOption)
	    {
            mAliasName = aliasName;
            mDefiningOption = definingOption;
	    }

        public object Value
        {
            set { mDefiningOption.Value = value; }
        }

        public bool RequiresValue
        {
            get { return mDefiningOption.RequiresValue; }
        }

        public bool RequireExplicitAssignment
        {
            get
            {
                return mDefiningOption.RequireExplicitAssignment;
            }
            set
            {
                mDefiningOption.RequireExplicitAssignment = value;
            }
        }

        public C5.ICollection<Option> ProhibitedBy
        {
            get { return mDefiningOption.ProhibitedBy; }
        }

        public OptionGroup Group
        {
            get { return mDefiningOption.Group; }
        }

        public string Name
        {
            get { return mAliasName; }
        }

        public BoolFunction BoolFunction
        {
            get { return mDefiningOption.BoolFunction; }
        }

        public int MaxOccurs
        {
            get { return mDefiningOption.MaxOccurs; }
        }

        public int MinOccurs
        {
            get { return mDefiningOption.MinOccurs; }
        }

        public string Description
        {
            get { return mDefiningOption.Description; }
        }

        public int SetCount
        {
            get
            {
                return mDefiningOption.SetCount;
            }
            set
            {
                mDefiningOption.SetCount = value;
            }
        }

        public bool AcceptsValue
        {
            get { return mDefiningOption.AcceptsValue; }
        }

        public bool HasDefaultValue
        {
            get { return mDefiningOption.HasDefaultValue; }
        }

        public bool IsBooleanType
        {
            get { return mDefiningOption.IsBooleanType; }
        }

        public bool IsAlias
        {
            get { return true; }
        }

        public Option DefiningOption
        {
            get { return mDefiningOption; }
        }

        public void SetDefaultValue()
        {
            mDefiningOption.SetDefaultValue();
        }

        public object MinValue
        {
            get { return mDefiningOption.MinValue; }
        }

        public object MaxValue
        {
            get { return mDefiningOption.MaxValue; }
        }

        private Option mDefiningOption;
        private string mAliasName;

        #region IOption Members


        public bool IsIntegralType
        {
            get { return mDefiningOption.IsIntegralType; }
        }

        public bool IsFloatingPointType
        {
            get { return mDefiningOption.IsFloatingPointType; }
        }

        public bool IsDecimalType
        {
            get { return mDefiningOption.IsDecimalType; }
        }

        public bool IsNumericalType
        {
            get { return mDefiningOption.IsNumericalType; }
        }

        #endregion
    }
}

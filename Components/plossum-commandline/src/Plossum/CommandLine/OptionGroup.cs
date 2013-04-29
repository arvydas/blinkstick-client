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
 *  $Id: OptionGroup.cs 3 2007-07-29 13:32:10Z palotas $
 */
using System;
using SCG = System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using C5;

namespace Plossum.CommandLine
{
    internal class OptionGroup
    {
        public OptionGroup(string id, string name, string description, OptionGroupRequirement require, bool requireExplicitAssignment, SCG.IComparer<string> keyComparer)
        {
            Debug.Assert(!String.IsNullOrEmpty(id));
            Debug.Assert(keyComparer != null);

            mId = id;
            mName = name;
            mDescription = description;
            mRequire = require;
            mOptions = new TreeDictionary<string, Option>(keyComparer);
            mRequireExplicitAssignment = requireExplicitAssignment;
        }

        #region Public properties

        public string Id
        {
            get { return mId; }
        }

        public OptionGroupRequirement Require
        {
            get { return mRequire; }
        }

        public string Description
        {
            get { return mDescription; }
            set { mDescription = value; }
        }
	
        public string Name
        {
            get { return mName; }
            set { mName = value; }
        }

        public IDictionary<string, Option> Options
        {
            get { return mOptions; }
        }

        public bool RequireExplicitAssignment
        {
            get { return mRequireExplicitAssignment; }
        }

        public string GetOptionNamesAsString()
        {
            StringBuilder names = new StringBuilder();
            bool isFirst = true;
            foreach (KeyValuePair<string, Option> entry in mOptions)
            {
                if (!isFirst)
                    names.Append(", ");
                else
                    isFirst = false;


                names.Append('\"');
                names.Append(entry.Key);
                names.Append('\"');
            }
            return names.ToString();
        }
        #endregion

        #region Private fields

        private string mId;
        private string mName;
        private OptionGroupRequirement mRequire;
        private string mDescription;
        private IDictionary<string, Option> mOptions;
        private bool mRequireExplicitAssignment;

        #endregion
    }
}

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
 *  $Id: Option.cs 12 2007-08-05 10:26:08Z palotas $
 */
using System;
using SCG = System.Collections.Generic;
using System.Text;
using System.Reflection;
using Plossum.CommandLine;
using C5;
using System.Globalization;
using System.Diagnostics;
using Plossum.Resources;

namespace Plossum.CommandLine
{
    /// <summary>
    /// Representation of a command line option. 
    /// </summary>
    /// <remarks>Objects of this class is created by the constructor of <see cref="CommandLineParser"/>. This object is then used
    /// for setting and getting values of the option manager object used by the parser.</remarks>
    internal class Option : IOption
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Option"/> class.
        /// </summary>
        /// <param name="attribute">The attribute describing this option.</param>
        /// <param name="memberInfo">The <see cref="MemberInfo"/> object pointing to the member to which the attribute was applied.</param>
        /// <param name="cmdLineObject">The command line manager object.</param>
        /// <param name="optionGroups">A complete list of all available option groups.</param>
        /// <param name="numberFormatInfo">The number format info to use for parsing numerical arguments.</param>
        public Option(CommandLineOptionAttribute attribute, MemberInfo memberInfo, object cmdLineObject, 
            ICollection<OptionGroup> optionGroups, NumberFormatInfo numberFormatInfo)
        {            
            mObject = cmdLineObject;
            mMember = memberInfo;
            mUsage = attribute.BoolFunction;
            mDescription = attribute.Description;
            mNumberFormatInfo = numberFormatInfo ?? CultureInfo.CurrentCulture.NumberFormat;
            mDefaultValue = attribute.DefaultAssignmentValue;
            mMinValue = attribute.MinValue;
            mMaxValue = attribute.MaxValue;

            // Check the validity of the member for which this attribute was defined
            switch (memberInfo.MemberType)
            {
                case MemberTypes.Field:
                    FieldInfo fieldInfo = (FieldInfo)memberInfo;
                    if (fieldInfo.IsInitOnly || fieldInfo.IsLiteral)
                        throw new AttributeException(typeof(CommandLineOptionAttribute), memberInfo,
                            "Illegal field for this attribute; field must be writeable");

                    mOptionType = fieldInfo.FieldType;
                    break;
                case MemberTypes.Method:
                    MethodInfo method = (MethodInfo)memberInfo;
                    ParameterInfo[] parameters = method.GetParameters();

                    if (parameters.Length != 1)
                        throw new AttributeException(typeof(CommandLineOptionAttribute), memberInfo,
                            "Illegal method for this attribute; the method must accept exactly one parameter");

                    if (parameters[0].IsOut)
                        throw new AttributeException(typeof(CommandLineOptionAttribute), memberInfo,
                            "Illegal method for this attribute; the parameter of the method must not be an out parameter");

                    if (IsArray(parameters[0].ParameterType) || IsCollectionType(parameters[0].ParameterType))
                        throw new AttributeException(typeof(CommandLineOptionAttribute), memberInfo,
                            "Illegal method for this attribute; the parameter of the method must be a non-array and non-collection type");

                    mOptionType = parameters[0].ParameterType;
                    break;
                case MemberTypes.Property:
                    PropertyInfo propInfo = (PropertyInfo)memberInfo;

                    if (!propInfo.CanWrite && !IsCollectionType(propInfo.PropertyType))
                        throw new AttributeException(typeof(CommandLineOptionAttribute), memberInfo,
                            "Illegal property for this attribute; property for non-collection type must be writable");

                    if (!propInfo.CanRead && IsCollectionType(propInfo.PropertyType))
                        throw new AttributeException(typeof(CommandLineOptionAttribute), memberInfo,
                            "Illegal property for this attribute; property for collection type must be readable");

                    if (!(propInfo.CanRead && propInfo.CanWrite) && IsArray(propInfo.PropertyType))
                        throw new AttributeException(typeof(CommandLineOptionAttribute), memberInfo,
                            "Illegal property for this attribute; property representing array type must be both readable and writeable");

                    mOptionType = propInfo.PropertyType;
                    break;
                default:
                    throw new AttributeException(typeof(CommandLineOptionAttribute), memberInfo,
                        "Illegal member for this attribute; member must be a property, method (accepting one parameter) or a field");
            }

            mMinOccurs = attribute.MinOccurs;

            // MaxOccurs does not have a default value (since this is different for various types), so we set it here.
            if (!attribute.IsMaxOccursSet)
            {
                // Use default setting for MaxOccurs
                if (IsArray(mOptionType) || IsCollectionType(mOptionType))
                    mMaxOccurs = 0; // Unlimited 
                else
                    mMaxOccurs = 1;
            }
            else
            {
                mMaxOccurs = attribute.MaxOccurs;
            }

            if (mMinOccurs > mMaxOccurs && mMaxOccurs > 0)
                throw new AttributeException(typeof(CommandLineOptionAttribute), memberInfo, 
                    String.Format(CultureInfo.CurrentUICulture, "MinOccurs ({0}) must not be larger than MaxOccurs ({1})", mMinOccurs, mMaxOccurs));

            if (mMaxOccurs != 1 && !(IsArray(mOptionType) || IsCollectionType(mOptionType)) && mMember.MemberType != MemberTypes.Method)
                throw new AttributeException(typeof(CommandLineOptionAttribute), memberInfo,
                    "Invalid cardinality for member; MaxOccurs must be equal to one (1) for any non-array or non-collection type");

            CommandLineManagerAttribute objectAttr = (CommandLineManagerAttribute)Attribute.GetCustomAttribute(mObject.GetType(), typeof(CommandLineManagerAttribute));
            if (objectAttr == null)
                throw new AttributeException(String.Format(CultureInfo.CurrentUICulture, 
                    "Class {0} contains a CommandLineOptionAttribute, but does not have the attribute CommandLineObjectAttribute", mObject.GetType().FullName));


            // Assign the name of this option from the member itself if no name is explicitly provided 
            if (attribute.Name == null)
            {
                mName = memberInfo.Name;
            }
            else
            {
                mName = attribute.Name;
            }

            // Find the group (if any) that this option belongs to in the list of available option groups
            if (attribute.GroupId != null)
            {
                if (!optionGroups.Find(new Fun<OptionGroup, bool>(
                    delegate(OptionGroup searchGroup)
                    {
                        return attribute.GroupId.Equals(searchGroup.Id);
                    }), out mGroup)) 
                {
                    throw new LogicException(String.Format(CultureInfo.CurrentUICulture, "Undefined group {0} referenced from  member {1} in {2}", attribute.GroupId, memberInfo.Name, cmdLineObject.GetType().FullName));
                }
                mGroup.Options.Add(mName, this);
            }

            // Recursively find out if this option requires explicit assignment
            if (attribute.DoesRequireExplicitAssignment.HasValue)
            {
                mRequireExplicitAssignment = attribute.DoesRequireExplicitAssignment.Value;
            }
            else if (mGroup != null)
            {
                mRequireExplicitAssignment = mGroup.RequireExplicitAssignment;
            }
            else
            {
                mRequireExplicitAssignment = objectAttr.RequireExplicitAssignment;
            }

            // Make sure the type of the field, property or method is supported
            if (!IsTypeSupported(mOptionType))
                throw new AttributeException(typeof(CommandLineOptionAttribute), mMember,
                    "Unsupported type for command line option.");


            // Make sure MinValue and MaxValue is not specified for any non-numerical type.
            if (mMinValue != null || mMaxValue != null)
            {
                if (!IsNumericalType)
                {
                    throw new AttributeException(typeof(CommandLineOptionAttribute), mMember,
                        "MinValue and MaxValue must not be specified for a non-numerical type");
                }
                else if (!mMinValue.GetType().IsAssignableFrom(GetBaseType(mOptionType)))
                {
                    throw new AttributeException(typeof(CommandLineOptionAttribute), mMember,
                        "Illegal value for MinValue or MaxValue, not the same type as option");
                }
            }

            // Some special checks for numerical types
            if (IsNumericalType)
            {
                // Assign the default MinValue if it was not set and this is a numerical type
                if (IsNumericalType && mMinValue == null)
                {
                    mMinValue = GetBaseType(mOptionType).GetField("MinValue", BindingFlags.Static | BindingFlags.Public).GetValue(null);
                }

                // Assign the defaul MaxValue if it was not set and this is a numerical type
                if (IsNumericalType && mMaxValue == null)
                {
                    mMaxValue = GetBaseType(mOptionType).GetField("MaxValue", BindingFlags.Static | BindingFlags.Public).GetValue(null);
                }

                // Check that MinValue <= MaxValue
                if (IsNumericalType && ((IComparable)MinValue).CompareTo(MaxValue) > 0)
                {
                    throw new AttributeException(typeof(CommandLineOptionAttribute), mMember,
                        "MinValue must not be greater than MaxValue");
                }
            }

            // Check that the DefaultValue is not set if the option does not require explicit assignment. 
            // If it were allowed, it would be ambiguos for an option separated from a value by a white space character
            // since we wouldn't know whether that would set the default value or assign it to the following value.
            if (mDefaultValue != null && !mRequireExplicitAssignment)
            {
                throw new AttributeException(typeof(CommandLineOptionAttribute), mMember,
                    "DefaultValue must not be specified when RequireExplicitAssignment is set to false");
            }

            // Check that the type of any set default value matches that of this option, or is string, and
            // convert it to the type of this option.
            if (mDefaultValue != null)
            {
                if (mDefaultValue.GetType() == typeof(string))
                {
                    try
                    {
                        mDefaultValue = GetCheckedValueForSetOperation(mDefaultValue);
                    }
                    catch (OverflowException)
                    {
                        throw new AttributeException(typeof(CommandLineOptionAttribute), mMember,
                            "DefaultValue was less than MinValue or greater than MaxValue for this option");
                    }
                    catch (FormatException)
                    {
                        throw new AttributeException(typeof(CommandLineOptionAttribute), mMember,
                            "DefaultValue was not specified in the correct format for the type of this option");
                    }
                }
                else if (GetBaseType(mOptionType) != mDefaultValue.GetType())
                {
                    try
                    {
                        mDefaultValue = Convert.ChangeType(mDefaultValue, GetBaseType(mOptionType), mNumberFormatInfo);
                    }
                    catch (InvalidCastException)
                    {
                        throw new AttributeException(typeof(CommandLineOptionAttribute), mMember,
                            "The type of the DefaultValue specified is not compatible with the type of the member to which this attribute applies");
                    }
                }
            }            

            // If this is an enum, check that it doesn't have members only distinguishable by case, and
            // add the members to the mEnumerationValues set for speedy access when checking values assigned 
            // to this member.
            Type type = GetBaseType(mOptionType);
            if (type.IsEnum)
            {
                mEnumerationValues = new TreeSet<string>(StringComparer.OrdinalIgnoreCase);
                foreach (FieldInfo field in type.GetFields())
                {
                    if (field.IsLiteral)
                    {
                        if (mEnumerationValues.Contains(field.Name))
                        {
                            throw new AttributeException(typeof(CommandLineOptionAttribute), mMember,
                                "This enumeration is not allowed as a command line option since it contains fields that differ only by case");
                        }
                        mEnumerationValues.Add(field.Name);
                    }
                }                
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Sets the value of the member of the option manager representing this option.
        /// </summary>
        /// <value>the value of the member of the option manager representing this option.</value>
        public object Value
        {
            set
            {
                if (++mSetCount > MaxOccurs && MaxOccurs != 0)
                    throw new InvalidOperationException(CommandLineStrings.InternalErrorOptionValueWasSetMoreThanMaxOccursNumberOfTimes);

                switch (mMember.MemberType)
                {
                    case MemberTypes.Field:
                        SetFieldValue(value);
                        break;
                    case MemberTypes.Property:
                        SetPropertyValue(value);
                        break;
                    case MemberTypes.Method:
                        SetMethodValue(value);
                        break;
                    default:
                        throw new LogicException(
                            String.Format(CultureInfo.CurrentUICulture,
                            "Internal error; unimplemented member type {0} found in Option.Value", mMember.MemberType.ToString()));
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this option requires a value to be assigned to it.
        /// </summary>
        /// <value><c>true</c> if this option requires a value to be assigned to it; otherwise, <c>false</c>.</value>
        public bool RequiresValue
        {
            get
            {
                return AcceptsValue && !HasDefaultValue;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this option requires explicit assignment.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this option requires explicit assignment; otherwise, <c>false</c>.
        /// </value>
	    public bool RequireExplicitAssignment
	    {
		    get { return mRequireExplicitAssignment;}
		    set { mRequireExplicitAssignment = value;}
	    }

        /// <summary>
        /// Gets the collection containing the options prohibiting this option from being specified.
        /// </summary>
        /// <value>The collection containing the options prohibiting this option from being specified.</value>
        public ICollection<Option> ProhibitedBy
        {
            get { return mProhibitedBy; }
        }

        /// <summary>
        /// Gets the group to which this option belongs, or null if this option does not belong to any group.
        /// </summary>
        /// <value>The group to which this option belongs, or null if this option does not belong to any group.</value>
        public OptionGroup Group
        {
            get { return mGroup; }
        }

        /// <summary>
        /// Gets the name of this option
        /// </summary>
        /// <value>The name of this option</value>
        public string Name
        {
            get { return mName; }
        }

        /// <summary>
        /// Gets the bool function used by this option
        /// </summary>
        /// <value>The bool function used by this option</value>
        public BoolFunction BoolFunction
        {
            get { return mUsage; }
        }

        /// <summary>
        /// Gets the max occurs.
        /// </summary>
        /// <value>The max occurs.</value>
        public int MaxOccurs
        {
            get { return mMaxOccurs; }
        }

        /// <summary>
        /// Gets the min occurs.
        /// </summary>
        /// <value>The min occurs.</value>
        public int MinOccurs
        {
            get { return mMinOccurs; }
        }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description
        {
            get { return mDescription; }
            set { mDescription = value; }
        }

        /// <summary>
        /// Gets or sets the number of times the value of this option has been set.
        /// </summary>
        /// <value>The number of times the value of this option has been set.</value>
        public int SetCount
        {
            get { return mSetCount; }
            set { mSetCount = value; }
        }

        /// <summary>
        /// Gets a value indicating whether a value may be assigned to this option.
        /// </summary>
        /// <value><c>true</c> if a value may be assigned to this option; otherwise, <c>false</c>.</value>
        public bool AcceptsValue
        {
            get 
            {
                return GetBaseType(mOptionType) != typeof(bool) ||
                    BoolFunction == BoolFunction.Value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has default value.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has default value; otherwise, <c>false</c>.
        /// </value>
        public bool HasDefaultValue
        {
            get
            {
                return mDefaultValue != null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is boolean type.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is boolean type; otherwise, <c>false</c>.
        /// </value>
        public bool IsBooleanType
        {
            get
            {
                return GetBaseType(mOptionType).Equals(typeof(bool));
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is an alias.
        /// </summary>
        /// <value><c>true</c> if this instance is alias; otherwise, <c>false</c>.</value>
        public bool IsAlias
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the defining option.
        /// </summary>
        /// <value>The defining option.</value>
        /// <remarks>For an <see cref="Option"/> the value returned will be equal to the Option itself, for an <see cref="OptionAlias"/> it
        /// will be the <see cref="Option"/> to which the alias refers.</remarks>
        public Option DefiningOption
        {
            get { return this; }
        }

        /// <summary>
        /// Gets the min value.
        /// </summary>
        /// <value>The min value, or null if no minimum value was specified.</value>
        public object MinValue
        {
            get { return mMinValue; }
        }

        /// <summary>
        /// Gets the max value.
        /// </summary>
        /// <value>The max value or null if no maximum value was specified.</value>
        public object MaxValue
        {
            get { return mMaxValue; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is integral type.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is integral type; otherwise, <c>false</c>.
        /// </value>
        public bool IsIntegralType
        {
            get
            {
                Type baseType = GetBaseType(mOptionType);
                return baseType == typeof(byte) ||
                    baseType == typeof(short) ||
                    baseType == typeof(int) ||
                    baseType == typeof(long) ||
                    baseType == typeof(ushort) ||
                    baseType == typeof(uint) ||
                    baseType == typeof(ulong);

            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is floating point type.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is floating point type; otherwise, <c>false</c>.
        /// </value>
        public bool IsFloatingPointType
        {
            get
            {
                Type baseType = GetBaseType(mOptionType);
                return baseType == typeof(float) ||
                    baseType == typeof(double);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is decimal type.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is decimal type; otherwise, <c>false</c>.
        /// </value>
        public bool IsDecimalType
        {
            get
            {
                return GetBaseType(mOptionType) == typeof(decimal);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is numerical type.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is numerical type; otherwise, <c>false</c>.
        /// </value>
        public bool IsNumericalType
        {
            get
            {
                return IsIntegralType || IsDecimalType || IsFloatingPointType;
            }
        }

        /// <summary>
        /// Gets the valid enumeration values.
        /// </summary>
        /// <value>The valid enumeration values of this option if the base type for this option is an enum, or a null reference otherwise.</value>
        public ICollection<string> ValidEnumerationValues
        {
            get { return new GuardedCollection<string>(mEnumerationValues); }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Sets this option to its default value.
        /// </summary>
        public void SetDefaultValue()
        {
            Debug.Assert(HasDefaultValue);
            Value = mDefaultValue;
        }

        /// <summary>
        /// Adds the alias.
        /// </summary>
        /// <param name="alias">The alias.</param>
        public void AddAlias(string alias)
        {
            mAliases.Add(alias);
        }

        /// <summary>
        /// Gets the names of the aliases referring to this option.
        /// </summary>
        /// <value>The names of the aliases referring to this option.</value>
        public SCG.IEnumerable<string> Aliases
        {
            get
            {
                return mAliases;
            }
        }

        #endregion

        #region Private methods

        private static void AppendToCollection(object collection, object value)
        {
            Debug.Assert(IsCollectionType(collection.GetType()));
            collection.GetType().InvokeMember("Add", BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.Instance, null, collection, new object[] { value }, CultureInfo.CurrentUICulture);            
        }

        private Array AppendToArray(Array array, object value)
        {
            // Get the length of the current array
            int arrayLength = array.GetLength(0);

            // Allocate a new array of the same type but one item longer
            Array newArray = Array.CreateInstance(mOptionType.GetElementType(), arrayLength + 1);

            // Copy the contents of the old array to the new array
            Array.Copy((Array)array, newArray, arrayLength);

            // Set the value of the new (last) element to the specified value
            newArray.SetValue(GetCheckedValueForSetOperation(value), arrayLength);

            return newArray;
        }
        
        private void SetFieldValue(object value)
        {
            FieldInfo field = (FieldInfo)mMember;
            if (field.FieldType.IsArray)
            {
                field.SetValue(mObject, AppendToArray((Array)field.GetValue(mObject), value));
            }
            else if (IsCollectionType(mOptionType))
            {
                AppendToCollection(field.GetValue(mObject), value);
            }
            else
            {
                // Otherwise we just assign the value
                field.SetValue(mObject, GetCheckedValueForSetOperation(value));
            }
        }

        private void SetPropertyValue(object value)
        {
            PropertyInfo property = (PropertyInfo)mMember;
            if (property.PropertyType.IsArray)
            {
                property.SetValue(mObject, AppendToArray((Array)property.GetValue(mObject, null), value), null);                
            }
            else if (IsCollectionType(mOptionType))
            {
                AppendToCollection(property.GetValue(mObject, null), value);
            }
            else
            {
                property.SetValue(mObject, GetCheckedValueForSetOperation(value), null);
            }
        }

        private void SetMethodValue(object value)
        {
            MethodInfo method = (MethodInfo)mMember;
            method.Invoke(mObject, new object[] { GetCheckedValueForSetOperation(value) });
        }

        private static bool IsArray(Type type)
        {
            return type.IsArray;
        }

        private static bool IsNonGenericCollectionType(Type type)
        {
            return type.GetInterface("System.Collections.ICollection") != null;
        }

        private static bool IsGenericCollectionType(Type type)
        {
            return type.GetInterface("System.Collections.Generic.ICollection`1") != null ||
                type.GetInterface("C5.IExtensible`1") != null;
        }

        private static bool IsCollectionType(Type type)
        {
            return IsNonGenericCollectionType(type) || IsGenericCollectionType(type);
        }

        private static Type GetBaseType(Type type)
        {
            if (IsArray(type))
            {
                return type.GetElementType();
            }
            else if (IsNonGenericCollectionType(type))
            {
                return typeof(object);
            }
            else if (IsGenericCollectionType(type))
            {
                Debug.Assert(type.GetGenericArguments().Length == 1);
                return type.GetGenericArguments()[0];
            }
            else
            {
                return type;
            }
        }

        private static bool IsTypeSupported(Type type)
        {
            if (IsArray(type) && type.GetArrayRank() != 1)
                return false;
            else if (IsNonGenericCollectionType(type))
                return true;

            type = GetBaseType(type);

            return
                type.Equals(typeof(bool)) ||
                type.Equals(typeof(byte)) ||
                type.Equals(typeof(sbyte)) ||
                type.Equals(typeof(char)) ||
                type.Equals(typeof(decimal)) ||
                type.Equals(typeof(double)) ||
                type.Equals(typeof(float)) ||
                type.Equals(typeof(int)) ||
                type.Equals(typeof(uint)) ||
                type.Equals(typeof(long)) ||
                type.Equals(typeof(ulong)) ||
                type.Equals(typeof(short)) ||
                type.Equals(typeof(ushort)) ||
                type.Equals(typeof(string)) ||
                type.IsEnum;
        }

        /// <summary>
        /// Gets the checked value for set operation.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        /// <exception cref="FormatException">The string supplied for the value was not in the correct format</exception>
        /// <exception cref="OverflowException">The value supplied represented a number less than <see cref="MinValue"/> or greater than <see cref="MaxValue"/></exception>
        private object GetCheckedValueForSetOperation(object value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            try
            {
                value = ConvertValueTypeForSetOperation(value);
            }
            catch (TargetInvocationException tie)
            {
                // We need to transform this exception to an OverflowException or FormatException
                // if that is the type of the inner exception.
                if (tie.InnerException.GetType() == typeof(OverflowException) || tie.InnerException.GetType() == typeof(FormatException)
                    || tie.InnerException.GetType() == typeof(InvalidOptionValueException))
                    throw tie.InnerException;
                else
                    throw;
            }
            
            if (IsNumericalType)
            {
                Debug.Assert(MinValue != null && MaxValue != null);
                // All numerical types are comparable and MinValue and MaxValue are both automatically set
                // in the constructor for any numerical type supported by this class.
                IComparable comparable = (IComparable)value;
                if (comparable.CompareTo(MinValue) < 0 || comparable.CompareTo(MaxValue) > 0)
                    throw new OverflowException(CommandLineStrings.ValueWasEitherTooLargeOrTooSmallForThisOptionType);
            }

            return value;
        }

        private object ConvertValueTypeForSetOperation(object value)
        {
            Debug.Assert(value.GetType() == typeof(string) || value.GetType() == GetBaseType(mOptionType));

            if (value == null)
                throw new ArgumentNullException("value");

            Type type = GetBaseType(mOptionType);
            if (value.GetType() == typeof(string))
            {
                string stringValue = (string)value;
                if (type.Equals(typeof(string)))
                {
                    return value;
                }
                else if (type.IsEnum)
                {

                    if (!mEnumerationValues.Contains(stringValue))
                    {
                        throw new InvalidEnumerationValueException(String.Format(CultureInfo.CurrentUICulture, CommandLineStrings.InvalidEnumerationValue0, stringValue));
                    }
                    return Enum.Parse(type, stringValue, true);
                }
                else if (type.Equals(typeof(bool)))
                {
                    return bool.Parse(stringValue);
                }
                else // We have a numerical type
                {
                    return type.InvokeMember("Parse", BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod, null, null, new object[] { value, mNumberFormatInfo }, CultureInfo.CurrentUICulture);
                }
            }
            else 
            {
                return value;
            }
        }

        #endregion

        #region Private fields

        /// <summary>
        /// Counts the number of times the value was set for this option
        /// </summary>
        private int mSetCount;
        private Type mOptionType;
        private MemberInfo mMember;
        private string mDescription;
        private string mName;
        private object mObject;
        private OptionGroup mGroup;
        private int mMaxOccurs;
        private int mMinOccurs;
        private BoolFunction mUsage;
        private ICollection<Option> mProhibitedBy = new ArrayList<Option>();
        private NumberFormatInfo mNumberFormatInfo;
        private object mDefaultValue;
        private bool mRequireExplicitAssignment;
        private object mMinValue;
        private object mMaxValue;
        private ICollection<string> mAliases = new ArrayList<string>();
        private ICollection<string> mEnumerationValues;
        #endregion
    }
}

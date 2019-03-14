#region MigraDoc - Creating Documents on the Fly
//
// Authors:
//   Stefan Lange
//   Klaus Potzesny
//   David Stephensen
//
// Copyright (c) 2001-2019 empira Software GmbH, Cologne Area (Germany)
//
// http://www.pdfsharp.com
// http://www.migradoc.com
// http://sourceforge.net/projects/pdfsharp
//
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included
// in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
#endregion

namespace MigraDoc.DocumentObjectModel.Internals
{
    /// <summary>
    /// Represents a nullable boolean value.
    /// </summary>
    internal struct NBool : INullableValue
    {
        public NBool(bool value)
        {
            _value = value ? (sbyte)1 : (sbyte)0;
        }

        NBool(sbyte value)
        {
            _value = value;
        }

        /// <summary>
        /// Gets or sets the value of the instance.
        /// </summary>
        public bool Value
        {
            get { return _value == 1; }
            set { _value = value ? (sbyte)1 : (sbyte)0; }
        }

        /// <summary>
        /// Gets the value of the instance.
        /// </summary>
        object INullableValue.GetValue()
        {
            return Value;
        }

        /// <summary>
        /// Sets the value of the instance.
        /// </summary>
        void INullableValue.SetValue(object value)
        {
            _value = (bool)value ? (sbyte)1 : (sbyte)0;
        }

        /// <summary>
        /// Resets this instance,
        /// i.e. IsNull() will return true afterwards.
        /// </summary>
        public void SetNull()
        {
            _value = -1;
        }

        /// <summary>
        /// Determines whether this instance is null (not set).
        /// </summary>
        public bool IsNull
        {
            get { return _value == -1; }
        }

        /// <summary>
        /// Returns a value indicating whether this instance is equal to the specified object.
        /// </summary>
        public override bool Equals(object value)
        {
            if (value is NBool)
                return this == (NBool)value;
            return false;
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public static bool operator ==(NBool l, NBool r)
        {
            if (l.IsNull)
                return r.IsNull;
            if (r.IsNull)
                return false;
            return l.Value == r.Value;
        }

        public static bool operator !=(NBool l, NBool r)
        {
            return !(l == r);
        }

        public static readonly NBool NullValue = new NBool(-1);

        /// <summary>
        /// -1 (undefined), 0 (false), or 1 (true).
        /// </summary>
        sbyte _value;
    }
}

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
    /// Represents a nullable integer value.
    /// </summary>
    internal struct NInt : INullableValue
    {
        public NInt(int val)
        {
            _value = val;
        }

        /// <summary>
        /// Gets or sets the value of the instance.
        /// </summary>
        public int Value
        {
            get { return _value != int.MinValue ? _value : 0; }
            set { _value = value; }
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
            _value = (int)value;
        }

        /// <summary>
        /// Resets this instance,
        /// i.e. IsNull() will return true afterwards.
        /// </summary>
        public void SetNull()
        {
            _value = int.MinValue;
        }

        /// <summary>
        /// Determines whether this instance is null (not set).
        /// </summary>
        public bool IsNull
        {
            get { return _value == int.MinValue; }
        }

        public static implicit operator NInt(int val)
        {
            return new NInt(val);
        }

        public static implicit operator int(NInt val)
        {
            return val.Value;
        }

        /// <summary>
        /// Returns a value indicating whether this instance is equal to the specified object.
        /// </summary>
        public override bool Equals(object value)
        {
            if (value is NInt)
                return this == (NInt)value;
            return false;
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public static bool operator ==(NInt l, NInt r)
        {
            if (l.IsNull)
                return r.IsNull;
            if (r.IsNull)
                return false;
            return l.Value == r.Value;
        }

        public static bool operator !=(NInt l, NInt r)
        {
            return !(l == r);
        }

        public static readonly NInt NullValue = new NInt(int.MinValue);

        int _value;
    }
}

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

using System;

namespace MigraDoc.DocumentObjectModel.Internals
{
    /// <summary>
    /// Represents a nullable Enum value.
    /// </summary>
    internal struct NEnum : INullableValue
    {
        public NEnum(int val, Type type)
        {
            _type = type;
            _value = val;
        }

        NEnum(int value)
        {
            _type = null;
            _value = value;
        }

        internal Type Type
        {
            get { return _type; }
            set { _type = value; }
        }
        Type _type;

        public int Value
        {
            get { return _value != int.MinValue ? _value : 0; }
            set
            {
                // TODO Remove German remarks!
                //TODO: Klasse Character so ändern, dass symbolName und char in unterschiedlichen Feldern gespeichert werden.
                //Diese Spezialbehandlung entfällt dann.
                if (_type == typeof(SymbolName))
                {
                    //          if (Enum.IsDefined(this .type, (uint)value))
                    _value = value;
                    //          else
                    //            throw new ArgumentOutOfRangeException("value");
                }
                else
                {
                    if (Enum.IsDefined(_type, value))
                        _value = value;
                    else
                        throw new /*InvalidEnum*/ArgumentException(DomSR.InvalidEnumValue(value), "value");

                }
            }
        }

        object INullableValue.GetValue()
        {
            return ToObject();
        }

        void INullableValue.SetValue(object value)
        {
            _value = (int)value;
        }

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

        public object ToObject()
        {
            if (_value != int.MinValue)
                return Enum.ToObject(_type, _value);
            // BUG Have all enum 0 as valid value?
            return Enum.ToObject(_type, 0);
        }

        //public static readonly NEnum NullValue = new NEnum(int.MinValue);

        /// <summary>
        /// Returns a value indicating whether this instance is equal to the specified object.
        /// </summary>
        public override bool Equals(object value)
        {
            if (value is NEnum)
                return this == (NEnum)value;
            return false;
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public static bool operator ==(NEnum l, NEnum r)
        {
            if (l.IsNull)
                return r.IsNull;
            if (r.IsNull)
                return false;
            if (l._type == r._type)
                return l.Value == r.Value;
            return false;
        }

        public static bool operator !=(NEnum l, NEnum r)
        {
            return !(l == r);
        }

        public static NEnum NullValue(Type fieldType)
        {
            return new NEnum(int.MinValue, fieldType);
        }

        int _value;
    }
}

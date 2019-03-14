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
using System.Collections;
using System.Collections.Generic;

namespace MigraDoc.DocumentObjectModel.Internals
{
    /// <summary>
    /// A collection that manages ValueDescriptors.
    /// </summary>
    public class ValueDescriptorCollection : IEnumerable
    {
        /// <summary>
        /// Gets the count of ValueDescriptors.
        /// </summary>
        public int Count
        {
            get { return _list.Count; }
        }

        /// <summary>
        /// Adds the specified ValueDescriptor.
        /// </summary>
        public void Add(ValueDescriptor vd)
        {
            _dictionary.Add(vd.ValueName, vd);
            _list.Add(vd);
        }

        /// <summary>
        /// Gets the <see cref="MigraDoc.DocumentObjectModel.Internals.ValueDescriptor"/> at the specified index.
        /// </summary>
        public ValueDescriptor this[int index]
        {
            get { return _list[index]; }
        }

        /// <summary>
        /// Gets the <see cref="MigraDoc.DocumentObjectModel.Internals.ValueDescriptor"/> with the specified name.
        /// </summary>
        public ValueDescriptor this[string name]
        {
            get { return _dictionary[name]; }
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        readonly List<ValueDescriptor> _list = new List<ValueDescriptor>();
        readonly Dictionary<string, ValueDescriptor> _dictionary = new Dictionary<string, ValueDescriptor>(StringComparer.OrdinalIgnoreCase);
    }
}

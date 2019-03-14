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

using System.Collections.Generic;
using System.Collections;

namespace MigraDoc.DocumentObjectModel.IO
{
    /// <summary>
    /// Used to collect errors reported by the DDL parser.
    /// </summary>
    public class DdlReaderErrors : IEnumerable
    {
        /// <summary>
        /// Adds the specified DdlReaderError at the end of the error list.
        /// </summary>
        public void AddError(DdlReaderError error)
        {
            _errors.Add(error);
        }

        /// <summary>
        /// Gets the DdlReaderError at the specified position.
        /// </summary>
        public DdlReaderError this[int index]
        {
            get { return _errors[index]; }
        }

        /// <summary>
        /// Gets the number of messages that are errors.
        /// </summary>
        public int ErrorCount
        {
            get
            {
                int count = 0;
                for (int idx = 0; idx < _errors.Count; idx++)
                    if (_errors[idx].ErrorLevel == DdlErrorLevel.Error)
                        count++;
                return count;
            }
        }

        private readonly List<DdlReaderError> _errors = new List<DdlReaderError>();

        /// <summary>
        /// Returns an enumerator that iterates through the error collection.
        /// </summary>
        public IEnumerator GetEnumerator()
        {
            return _errors.GetEnumerator();
        }
    }
}

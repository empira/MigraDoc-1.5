#region MigraDoc - Creating Documents on the Fly
//
// Authors:
//   Klaus Potzesny
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
using MigraDoc.DocumentObjectModel;

namespace MigraDoc.RtfRendering
{
    /// <summary>
    /// Renders a ListInfo in the \listoverridetable control.
    /// </summary>
    internal class ListInfoOverrideRenderer : RendererBase
    {
        public ListInfoOverrideRenderer(DocumentObject domObj, RtfDocumentRenderer docRenderer)
            : base(domObj, docRenderer)
        {
            _listInfo = domObj as ListInfo;
        }

        public static void Clear()
        {
            _numberList.Clear();
            _listNumber = 1;
        }

        /// <summary>
        /// Renders a ListInfo to RTF for the \listoverridetable.
        /// </summary>
        internal override void Render()
        {
            int id = ListInfoRenderer.GetListID(_listInfo);
            if (id > -1)
            {
                _rtfWriter.StartContent();
                _rtfWriter.WriteControl("listoverride");
                _rtfWriter.WriteControl("listid", id);
                _rtfWriter.WriteControl("listoverridecount", 0);
                _rtfWriter.WriteControl("ls", _listNumber);
                _rtfWriter.EndContent();
                _numberList.Add(_listInfo, _listNumber);
                ++_listNumber;
            }
        }
        private readonly ListInfo _listInfo;
        private static int _listNumber = 1;
        private static readonly Dictionary<ListInfo, int> _numberList = new Dictionary<ListInfo, int>();
        internal static int GetListNumber(ListInfo li)
        {
            if (_numberList.ContainsKey(li))
                return _numberList[li];

            return -1;
        }
    }
}

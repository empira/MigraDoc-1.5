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

using System;
using System.Collections.Generic;

namespace MigraDoc.Rendering
{
    /// <summary>
    /// Field information used to fill fields when rendering or formatting.
    /// </summary>
    internal class FieldInfos
    {
        internal FieldInfos(Dictionary<string, BookmarkInfo> bookmarks)
        {
            _bookmarks = bookmarks;
        }

        internal void AddBookmark(string name)
        {
            if (PhysicalPageNr <= 0)
                return;

            if (_bookmarks.ContainsKey(name))
                _bookmarks.Remove(name);

            if (PhysicalPageNr > 0)
                _bookmarks.Add(name, new BookmarkInfo(PhysicalPageNr, DisplayPageNr));
        }

        internal int GetShownPageNumber(string bookmarkName)
        {
            if (_bookmarks.ContainsKey(bookmarkName))
            {
                BookmarkInfo bi = _bookmarks[bookmarkName];
                return bi.ShownPageNumber;
            }
            return -1;
        }

        internal int GetPhysicalPageNumber(string bookmarkName)
        {
            if (_bookmarks.ContainsKey(bookmarkName))
            {
                BookmarkInfo bi = _bookmarks[bookmarkName];
                return bi.DisplayPageNumber;
            }
            return -1;
        }

        internal struct BookmarkInfo
        {
            internal BookmarkInfo(int physicalPageNumber, int displayPageNumber)
            {
                DisplayPageNumber = physicalPageNumber;
                ShownPageNumber = displayPageNumber;
            }

            internal readonly int DisplayPageNumber;
            internal readonly int ShownPageNumber;
        }

        readonly Dictionary<string, BookmarkInfo> _bookmarks;
        internal int DisplayPageNr;
        internal int PhysicalPageNr;
        internal int Section;
        internal int SectionPages;
        internal int NumPages;
        internal DateTime Date;
    }
}

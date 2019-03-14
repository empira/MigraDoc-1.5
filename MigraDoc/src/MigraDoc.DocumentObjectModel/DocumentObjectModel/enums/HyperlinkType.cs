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

namespace MigraDoc.DocumentObjectModel
{
    /// <summary>
    /// Specifies the target of the hyperlink.
    /// </summary>
    public enum HyperlinkType
    {
        /// <summary>
        /// Targets a position in the document. Same as 'Bookmark'.
        /// </summary>
        Local = 0,

        /// <summary>
        /// Targets a position in the document. Same as 'Local'.
        /// </summary>
        Bookmark = Local,

        /// <summary>
        /// Targets a position in another PDF document.
        /// This is only supported in PDF. In RTF the other document is opened, but the target position is not moved to.
        /// </summary>
        ExternalBookmark,

        /// <summary>
        /// Targets a position in an embedded document in this or another root PDF document.
        /// This is only supported in PDF.
        /// </summary>
        EmbeddedDocument,

        /// <summary>
        /// Targets a resource on the Internet or network. Same as 'Url'.
        /// </summary>
        Web,

        /// <summary>
        /// Targets a resource on the Internet or network. Same as 'Web'.
        /// </summary>
        Url = Web,

        /// <summary>
        /// Targets a physical file.
        /// </summary>
        File
    }
}

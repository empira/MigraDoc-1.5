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

using System.IO;
using MigraDoc.DocumentObjectModel;

namespace MigraDoc.RtfRendering
{
    /// <summary>
    ///   Renders a Hyperlink to RTF.
    /// </summary>
    internal class HyperlinkRenderer : RendererBase
    {
        internal HyperlinkRenderer(DocumentObject domObj, RtfDocumentRenderer docRenderer)
            : base(domObj, docRenderer)
        {
            _hyperlink = domObj as Hyperlink;
        }

        /// <summary>
        ///   Renders a hyperlink to RTF.
        /// </summary>
        internal override void Render()
        {
            _useEffectiveValue = true;
            _rtfWriter.StartContent();
            _rtfWriter.WriteControl("field");
            _rtfWriter.StartContent();
            _rtfWriter.WriteControl("fldinst", true);
            _rtfWriter.WriteText("HYPERLINK ");
            string name = _hyperlink.Filename;
            if (_hyperlink.IsNull("Type") || _hyperlink.Type == HyperlinkType.Local)
            {
                name = BookmarkFieldRenderer.MakeValidBookmarkName(_hyperlink.BookmarkName);
                _rtfWriter.WriteText(@"\l ");
            }
            else if (_hyperlink.Type == HyperlinkType.File || _hyperlink.Type == HyperlinkType.ExternalBookmark) // Open at least the document for external bookmarks (in PDF: Links to external named destinations).
            {
                string workingDirectory = _docRenderer.WorkingDirectory;
                if (workingDirectory != null)
                    name = Path.Combine(_docRenderer.WorkingDirectory, name);

                name = name.Replace(@"\", @"\\");
            }

            _rtfWriter.WriteText("\"" + name + "\"");
            _rtfWriter.EndContent();
            _rtfWriter.StartContent();
            _rtfWriter.WriteControl("fldrslt");
            _rtfWriter.StartContent();
            _rtfWriter.WriteControl("cs", _docRenderer.GetStyleIndex("Hyperlink"));

            FontRenderer fontRenderer = new FontRenderer(_hyperlink.Font, _docRenderer);
            fontRenderer.Render();

            if (!_hyperlink.IsNull("Elements"))
            {
                foreach (DocumentObject domObj in _hyperlink.Elements)
                    RendererFactory.CreateRenderer(domObj, _docRenderer).Render();
            }
            _rtfWriter.EndContent();
            _rtfWriter.EndContent();
            _rtfWriter.EndContent();
        }

        private readonly Hyperlink _hyperlink;
    }
}
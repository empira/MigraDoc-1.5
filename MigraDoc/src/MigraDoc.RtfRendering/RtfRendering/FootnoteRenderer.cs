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

using MigraDoc.DocumentObjectModel;

namespace MigraDoc.RtfRendering
{
    /// <summary>
    /// Renders a footnote to RTF.
    /// </summary>
    internal class FootnoteRenderer : RendererBase
    {
        public FootnoteRenderer(DocumentObject domObj, RtfDocumentRenderer docRenderer)
            : base(domObj, docRenderer)
        {
            _footnote = domObj as Footnote;
        }

        /// <summary>
        /// Renders a footnote to RTF.
        /// </summary>
        internal override void Render()
        {
            RenderReference();
            RenderContent();
        }

        /// <summary>
        /// Renders the footnote's reference symbol.
        /// </summary>
        internal void RenderReference()
        {
            _rtfWriter.StartContent();
            _rtfWriter.WriteControl("super");
            if (_footnote.IsNull("Reference"))
                _rtfWriter.WriteControl("chftn");
            else
                _rtfWriter.WriteText(_footnote.Reference);
            _rtfWriter.EndContent();
        }


        /// <summary>
        /// Renders the footnote's content.
        /// </summary>
        private void RenderContent()
        {
            _rtfWriter.StartContent();
            _rtfWriter.WriteControl("footnote");
            foreach (DocumentObject obj in _footnote.Elements)
            {
                RendererBase rndrr = RendererFactory.CreateRenderer(obj, _docRenderer);
                if (rndrr != null)
                    rndrr.Render();
            }
            _rtfWriter.EndContent();
        }
        private readonly Footnote _footnote;
    }
}

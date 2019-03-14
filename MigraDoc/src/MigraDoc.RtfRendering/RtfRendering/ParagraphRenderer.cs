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
using MigraDoc.DocumentObjectModel.Tables;

namespace MigraDoc.RtfRendering
{
    /// <summary>
    ///   Class to render a Paragraph to RTF.
    /// </summary>
    internal class ParagraphRenderer : StyleAndFormatRenderer
    {
        public ParagraphRenderer(DocumentObject domObj, RtfDocumentRenderer docRenderer)
            : base(domObj, docRenderer)
        {
            _paragraph = domObj as Paragraph;
        }

        /// <summary>
        ///   Renders the paragraph to RTF.
        /// </summary>
        internal override void Render()
        {
            _useEffectiveValue = true;
            DocumentElements elements = DocumentRelations.GetParent(_paragraph) as DocumentElements;

            _rtfWriter.WriteControl("pard");
            bool isCellParagraph = DocumentRelations.GetParent(elements) is Cell;
            bool isFootnoteParagraph = isCellParagraph ? false : DocumentRelations.GetParent(elements) is Footnote;

            if (isCellParagraph)
                _rtfWriter.WriteControl("intbl");

            RenderStyleAndFormat();
            if (!_paragraph.IsNull("Elements"))
                RenderContent();
            EndStyleAndFormatAfterContent();

            if ((!isCellParagraph && !isFootnoteParagraph) || _paragraph != elements.LastObject)
                _rtfWriter.WriteControl("par");
        }

        /// <summary>
        ///   Renders the paragraph content to RTF.
        /// </summary>
        private void RenderContent()
        {
            DocumentElements elements = DocumentRelations.GetParent(_paragraph) as DocumentElements;
            //First paragraph of a footnote writes the reference symbol:
            if (DocumentRelations.GetParent(elements) is Footnote && _paragraph == elements.First)
            {
                FootnoteRenderer ftntRenderer = new FootnoteRenderer(DocumentRelations.GetParent(elements) as Footnote,
                                                                     _docRenderer);
                ftntRenderer.RenderReference();
            }
            foreach (DocumentObject docObj in _paragraph.Elements)
            {
                if (docObj == _paragraph.Elements.LastObject)
                {
                    if (docObj is Character)
                    {
                        if (((Character)docObj).SymbolName == SymbolName.LineBreak)
                            continue; //Ignore last linebreak.
                    }
                }
                RendererBase rndrr = RendererFactory.CreateRenderer(docObj, _docRenderer);
                if (rndrr != null)
                    rndrr.Render();
            }
        }

        private readonly Paragraph _paragraph;
    }
}
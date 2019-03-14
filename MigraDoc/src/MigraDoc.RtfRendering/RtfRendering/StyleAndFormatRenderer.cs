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
    /// Base class to render objects that have a style and a format attribute (currently cells, paragraphs).
    /// </summary>
    internal abstract class StyleAndFormatRenderer : RendererBase
    {
        internal StyleAndFormatRenderer(DocumentObject domObj, RtfDocumentRenderer docRenderer)
            : base(domObj, docRenderer)
        {
        }

        /// <summary>
        /// Renders format and style. Always call EndFormatAndStyleAfterContent() after the content was written.
        /// </summary>
        protected void RenderStyleAndFormat()
        {
            object styleName = GetValueAsIntended("Style");
            object parStyleName = styleName;
            Style style = _docRenderer.Document.Styles[(string)styleName];
            _hasCharacterStyle = false;
            if (style != null)
            {
                if (style.Type == StyleType.Character)
                {
                    _hasCharacterStyle = true;
                    parStyleName = "Normal";
                }
            }
            else
                parStyleName = null;

            if (parStyleName != null)
                _rtfWriter.WriteControl("s", _docRenderer.GetStyleIndex((string)parStyleName));

            ParagraphFormat frmt = GetValueAsIntended("Format") as ParagraphFormat;
            RendererFactory.CreateRenderer(frmt, _docRenderer).Render();
            _rtfWriter.WriteControl("brdrbtw");// Should separate border, but does not work.
            if (_hasCharacterStyle)
            {
                _rtfWriter.StartContent();
                _rtfWriter.WriteControlWithStar("cs", _docRenderer.GetStyleIndex((string)styleName));
                object font = GetValueAsIntended("Format.Font");
                if (font != null)
                    new FontRenderer(((Font)font), _docRenderer).Render();
            }
        }


        /// <summary>
        /// Ends the format and style rendering. Always paired with RenderStyleAndFormat().
        /// </summary>
        protected void EndStyleAndFormatAfterContent()
        {
            if (_hasCharacterStyle)
            {
                _rtfWriter.EndContent();
            }
        }
        private bool _hasCharacterStyle;
    }
}

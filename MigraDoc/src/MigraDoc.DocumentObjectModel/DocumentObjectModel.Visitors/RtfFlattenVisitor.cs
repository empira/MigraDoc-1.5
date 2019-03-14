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

namespace MigraDoc.DocumentObjectModel.Visitors
{
    /// <summary>
    /// Represents the visitor for flattening the DocumentObject to be used in the RtfRenderer.
    /// </summary>
    internal class RtfFlattenVisitor : VisitorBase
    {
        internal override void VisitFormattedText(FormattedText formattedText)
        {
            Document document = formattedText.Document;
            ParagraphFormat format = null;

            Style style = document._styles[formattedText._style.Value];
            if (style != null)
                format = style._paragraphFormat;
            else if (formattedText._style.Value != "")
                format = document._styles[StyleNames.InvalidStyleName]._paragraphFormat;

            if (format != null)
            {
                if (formattedText._font == null)
                    formattedText.Font = format._font.Clone();
                else if (format._font != null)
                    FlattenFont(formattedText._font, format._font);
            }
        }

        internal override void VisitHyperlink(Hyperlink hyperlink)
        {
            Font styleFont = hyperlink.Document.Styles[StyleNames.Hyperlink].Font;
            if (hyperlink._font == null)
                hyperlink.Font = styleFont.Clone();
            else
                FlattenFont(hyperlink._font, styleFont);
        }
    }
}

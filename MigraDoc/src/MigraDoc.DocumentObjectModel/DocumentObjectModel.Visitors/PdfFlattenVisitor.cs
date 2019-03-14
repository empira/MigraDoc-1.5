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

namespace MigraDoc.DocumentObjectModel.Visitors
{
    /// <summary>
    /// Flattens a document for PDF rendering.
    /// </summary>
    internal class PdfFlattenVisitor : VisitorBase
    {
        /// <summary>
        /// Initializes a new instance of the PdfFlattenVisitor class.
        /// </summary>
        public PdfFlattenVisitor()
        {
            //this .docObject = documentObject;
        }

        internal override void VisitDocumentElements(DocumentElements elements)
        {
#if true
            // New version without sorted list
            int count = elements.Count;
            for (int idx = 0; idx < count; ++idx)
            {
                Paragraph paragraph = elements[idx] as Paragraph;
                if (paragraph != null)
                {
                    Paragraph[] paragraphs = paragraph.SplitOnParaBreak();
                    if (paragraphs != null)
                    {
                        foreach (Paragraph para in paragraphs)
                        {
                            elements.InsertObject(idx++, para);
                            ++count;
                        }
                        elements.RemoveObjectAt(idx--);
                        --count;
                    }
                }
            }
#else
      SortedList splitParaList = new SortedList();

      for (int idx = 0; idx < elements.Count; ++idx)
      {
        Paragraph paragraph = elements[idx] as Paragraph;
        if (paragraph != null)
        {
          Paragraph[] paragraphs = paragraph.SplitOnParaBreak();
          if (paragraphs != null)
            splitParaList.Add(idx, paragraphs);
        }
      }

      int insertedObjects = 0;
      for (int idx = 0; idx < splitParaList.Count; ++idx)
      {
        int insertPosition = (int)splitParaList.GetKey(idx);
        Paragraph[] paragraphs = (Paragraph[])splitParaList.GetByIndex(idx);
        foreach (Paragraph paragraph in paragraphs)
        {
          elements.InsertObject(insertPosition + insertedObjects, paragraph);
          ++insertedObjects;
        }
        elements.RemoveObjectAt(insertPosition + insertedObjects);
        --insertedObjects;
      }
#endif
        }

        internal override void VisitDocumentObjectCollection(DocumentObjectCollection elements)
        {
            List<int> textIndices = new List<int>();
            if (elements is ParagraphElements)
            {
                for (int idx = 0; idx < elements.Count; ++idx)
                {
                    if (elements[idx] is Text)
                        textIndices.Add(idx);
                }
            }

            int[] indices = (int[])textIndices.ToArray();
            if (indices != null)
            {
                int insertedObjects = 0;
                foreach (int idx in indices)
                {
                    Text text = (Text)elements[idx + insertedObjects];
                    string currentString = "";
                    foreach (char ch in text.Content)
                    {
                        // TODO Add support for other breaking spaces (en space, em space, &c.).
                        switch (ch)
                        {
                            case ' ':
                            case '\r':
                            case '\n':
                            case '\t':
                                if (currentString != "")
                                {
                                    elements.InsertObject(idx + insertedObjects, new Text(currentString));
                                    ++insertedObjects;
                                    currentString = "";
                                }
                                elements.InsertObject(idx + insertedObjects, new Text(" "));
                                ++insertedObjects;
                                break;

                            case '-': // minus.
                                elements.InsertObject(idx + insertedObjects, new Text(currentString + ch));
                                ++insertedObjects;
                                currentString = "";
                                break;

                            // Characters that allow line breaks without indication.
                            case '\u200B': // zero width space.
                            case '\u200C': // zero width non-joiner.
                                if (currentString != "")
                                {
                                    elements.InsertObject(idx + insertedObjects, new Text(currentString));
                                    ++insertedObjects;
                                    currentString = "";
                                }
                                break;

                            case '­': // soft hyphen.
                                if (currentString != "")
                                {
                                    elements.InsertObject(idx + insertedObjects, new Text(currentString));
                                    ++insertedObjects;
                                    currentString = "";
                                }
                                elements.InsertObject(idx + insertedObjects, new Text("­"));
                                ++insertedObjects;
                                //currentString = "";
                                break;

                            default:
                                currentString += ch;
                                break;
                        }
                    }
                    if (currentString != "")
                    {
                        elements.InsertObject(idx + insertedObjects, new Text(currentString));
                        ++insertedObjects;
                    }
                    elements.RemoveObjectAt(idx + insertedObjects);
                    --insertedObjects;
                }
            }
        }

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

            Font parentFont = GetParentFont(formattedText);

            if (formattedText._font == null)
                formattedText.Font = parentFont.Clone();
            else if (parentFont != null)
                FlattenFont(formattedText._font, parentFont);
        }

        internal override void VisitHyperlink(Hyperlink hyperlink)
        {
            Font styleFont = hyperlink.Document.Styles[StyleNames.Hyperlink].Font;
            if (hyperlink._font == null)
                hyperlink.Font = styleFont.Clone();
            else
                FlattenFont(hyperlink._font, styleFont);

            FlattenFont(hyperlink._font, GetParentFont(hyperlink));
        }

        protected Font GetParentFont(DocumentObject obj)
        {
            DocumentObject parentElements = DocumentRelations.GetParent(obj);
            DocumentObject parentObject = DocumentRelations.GetParent(parentElements);
            Font parentFont;
            Paragraph paragraph = parentObject as Paragraph;
            if (paragraph != null)
            {
                ParagraphFormat format = paragraph.Format;
                parentFont = format._font;
            }
            else // Hyperlink or FormattedText
            {
                parentFont = parentObject.GetValue("Font") as Font;
            }
            return parentFont;
        }
    }
}

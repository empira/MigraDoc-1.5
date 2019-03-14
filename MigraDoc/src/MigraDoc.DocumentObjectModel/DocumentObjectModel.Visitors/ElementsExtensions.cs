#region PDFsharp - A .NET library for processing PDF
//
// Authors:
//   Stefan Lange
//
// Copyright (c) 2005-2019 empira Software GmbH, Cologne Area (Germany)
//
// http://www.pdfsharp.com
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
using System.Linq;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;

namespace MigraDoc.DocumentObjectModel.Visitors
{
    public static class ElementsExtensions
    {
        /// <summary>
        /// Gets the elements/sections/rows/cells of a DocumentObject.
        /// </summary>
        /// <param name="documentObject">The DocumentObject.</param>
        /// <param name="includeHeaderFooter">Shall headers and footers also be returned. No effect for DocumentObjects that are no and contain no Sections.</param>
        public static IEnumerable<DocumentObject> GetElements(this DocumentObject documentObject, bool includeHeaderFooter = false)
        {
            var document = documentObject as Document;
            if (document != null)
                return document.Sections.Cast<DocumentObject>();

            var section = documentObject as Section;
            if (section != null)
            {
                var elements = section.Elements.Cast<DocumentObject>();

                if (includeHeaderFooter)
                {
                    // Access Header/Footer Primary/FirstPage/EvenPage creates a new HeaderFooter element if not existing. To avoid this, we check if it exists.
                    if (section.Headers.HasHeaderFooter(HeaderFooterIndex.Primary))
                        elements = elements.Concat(section.Headers.Primary.Elements.Cast<DocumentObject>());
                    if (section.Headers.HasHeaderFooter(HeaderFooterIndex.FirstPage))
                        elements = elements.Concat(section.Headers.FirstPage.Elements.Cast<DocumentObject>());
                    if (section.Headers.HasHeaderFooter(HeaderFooterIndex.EvenPage))
                        elements = elements.Concat(section.Headers.EvenPage.Elements.Cast<DocumentObject>());

                    if (section.Footers.HasHeaderFooter(HeaderFooterIndex.Primary))
                        elements = elements.Concat(section.Footers.Primary.Elements.Cast<DocumentObject>());
                    if (section.Footers.HasHeaderFooter(HeaderFooterIndex.FirstPage))
                        elements = elements.Concat(section.Footers.FirstPage.Elements.Cast<DocumentObject>());
                    if (section.Footers.HasHeaderFooter(HeaderFooterIndex.EvenPage))
                        elements = elements.Concat(section.Footers.EvenPage.Elements.Cast<DocumentObject>());
                }
                return elements;
            }

            var headerFooter = documentObject as HeaderFooter;
            if (headerFooter != null)
                return headerFooter.Elements.Cast<DocumentObject>();

            var paragraph = documentObject as Paragraph;
            if (paragraph != null)
                return paragraph.Elements.Cast<DocumentObject>();

            var formattedText = documentObject as FormattedText;
            if (formattedText != null)
                return formattedText.Elements.Cast<DocumentObject>();

            var textFrame = documentObject as TextFrame;
            if (textFrame != null)
                return textFrame.Elements.Cast<DocumentObject>();

            var hyperlink = documentObject as Hyperlink;
            if (hyperlink != null)
                return hyperlink.Elements.Cast<DocumentObject>();

            var table = documentObject as Table;
            if (table != null)
                return table.Rows.Cast<DocumentObject>();

            var row = documentObject as Row;
            if (row != null)
                return row.Cells.Cast<DocumentObject>();

            var cell = documentObject as Cell;
            if (cell != null)
                return cell.Elements.Cast<DocumentObject>();

            return new DocumentObject[] { };
        }

        /// <summary>
        /// Gets the elements/sections/rows/cells of Type T of a DocumentObject.
        /// </summary>
        /// <param name="documentObject">The DocumentObject.</param>
        /// <param name="includeHeaderFooter">Shall headers and footers also be returned. No effect for DocumentObjects that are no and contain no Sections.</param>
        public static IEnumerable<T> GetElements<T>(this DocumentObject documentObject, bool includeHeaderFooter = false) where T : DocumentObject
        {
            var elements = documentObject.GetElements(includeHeaderFooter);
            return elements.Filter<T>();
        }

        /// <summary>
        /// Gets the elements/sections/rows/cells of a DocumentObject recursively.
        /// </summary>
        /// <param name="documentObject">The DocumentObject.</param>
        /// <param name="includeHeaderFooter">Shall headers and footers also be returned. No effect for DocumentObjects that are no and contain no Sections.</param>
        public static IEnumerable<DocumentObject> GetElementsRecursively(this DocumentObject documentObject, bool includeHeaderFooter = false)
        {
            var elements = documentObject.GetElements(includeHeaderFooter);
            foreach (var element in elements)
            {
                yield return element;

                var children = element.GetElementsRecursively(includeHeaderFooter);
                foreach (var child in children)
                    yield return child;
            }
        }

        /// <summary>
        /// Gets the elements/sections/rows/cells of Type T of a DocumentObject recursively.
        /// </summary>
        /// <param name="documentObject">The DocumentObject.</param>
        /// <param name="includeHeaderFooter">Shall headers and footers also be returned. No effect for DocumentObjects that are no and contain no Sections.</param>
        public static IEnumerable<T> GetElementsRecursively<T>(this DocumentObject documentObject, bool includeHeaderFooter = false) where T : DocumentObject
        {
            var elements = documentObject.GetElementsRecursively(includeHeaderFooter);
            return elements.Filter<T>();
        }

        public static IEnumerable<T> Filter<T>(this IEnumerable<DocumentObject> elements) where T : DocumentObject
        {
            foreach (var element in elements)
            {
                var t = element as T;
                if (t != null)
                    yield return t;
            }
        }

        /// <summary>
        /// Gets the font of a DocumentObject stored in Font or Format.Font.
        /// </summary>
        public static Font GetFont(this DocumentObject documentObject)
        {
            var headerFooter = documentObject as HeaderFooter;
            if (headerFooter != null)
                return headerFooter.Format.Font;

            var paragraph = documentObject as Paragraph;
            if (paragraph != null)
                return paragraph.Format.Font;

            var formattedText = documentObject as FormattedText;
            if (formattedText != null)
                return formattedText.Font;

            var hyperlink = documentObject as Hyperlink;
            if (hyperlink != null)
                return hyperlink.Font;

            return null;
        }
    }
}

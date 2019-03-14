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

using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Shapes.Charts;

namespace MigraDoc.DocumentObjectModel.Visitors
{
    /// <summary>
    /// Summary description for VisitorBase.
    /// </summary>
    internal abstract class VisitorBase : DocumentObjectVisitor
    {
        public override void Visit(DocumentObject documentObject)
        {
            IVisitable visitable = documentObject as IVisitable;
            if (visitable != null)
                visitable.AcceptVisitor(this, true);
        }

        protected void FlattenParagraphFormat(ParagraphFormat format, ParagraphFormat refFormat)
        {
            if (format._alignment.IsNull)
                format._alignment = refFormat._alignment;

            if (format._firstLineIndent.IsNull)
                format._firstLineIndent = refFormat._firstLineIndent;

            if (format._leftIndent.IsNull)
                format._leftIndent = refFormat._leftIndent;

            if (format._rightIndent.IsNull)
                format._rightIndent = refFormat._rightIndent;

            if (format._spaceBefore.IsNull)
                format._spaceBefore = refFormat._spaceBefore;

            if (format._spaceAfter.IsNull)
                format._spaceAfter = refFormat._spaceAfter;

            if (format._lineSpacingRule.IsNull)
                format._lineSpacingRule = refFormat._lineSpacingRule;
            if (format._lineSpacing.IsNull)
                format._lineSpacing = refFormat._lineSpacing;

            if (format._widowControl.IsNull)
                format._widowControl = refFormat._widowControl;

            if (format._keepTogether.IsNull)
                format._keepTogether = refFormat._keepTogether;

            if (format._keepWithNext.IsNull)
                format._keepWithNext = refFormat._keepWithNext;

            if (format._pageBreakBefore.IsNull)
                format._pageBreakBefore = refFormat._pageBreakBefore;

            if (format._outlineLevel.IsNull)
                format._outlineLevel = refFormat._outlineLevel;

            if (format._font == null)
            {
                if (refFormat._font != null)
                {
                    //The font is cloned here to avoid parent problems
                    format._font = refFormat._font.Clone();
                    format._font._parent = format;
                }
            }
            else if (refFormat._font != null)
                FlattenFont(format._font, refFormat._font);

            if (format._shading == null)
            {
                if (refFormat._shading != null)
                {
                    format._shading = refFormat._shading.Clone();
                    format._shading._parent = format;
                }
                //format.shading = refFormat.shading;
            }
            else if (refFormat._shading != null)
                FlattenShading(format._shading, refFormat._shading);

            if (format._borders == null)
                format._borders = refFormat._borders;
            else if (refFormat._borders != null)
                FlattenBorders(format._borders, refFormat._borders);

            //if (format.tabStops == null)
            //    format.tabStops = refFormat.tabStops;
            if (refFormat._tabStops != null)
                FlattenTabStops(format.TabStops, refFormat._tabStops);

            if (refFormat._listInfo != null)
                FlattenListInfo(format.ListInfo, refFormat._listInfo);
        }

        protected void FlattenListInfo(ListInfo listInfo, ListInfo refListInfo)
        {
            if (listInfo._continuePreviousList.IsNull)
                listInfo._continuePreviousList = refListInfo._continuePreviousList;
            if (listInfo._listType.IsNull)
                listInfo._listType = refListInfo._listType;
            if (listInfo._numberPosition.IsNull)
                listInfo._numberPosition = refListInfo._numberPosition;
        }

        protected void FlattenFont(Font font, Font refFont)
        {
            if (font._name.IsNull)
                font._name = refFont._name;
            if (font._size.IsNull)
                font._size = refFont._size;
            if (font._color.IsNull)
                font._color = refFont._color;
            if (font._underline.IsNull)
                font._underline = refFont._underline;
            if (font._bold.IsNull)
                font._bold = refFont._bold;
            if (font._italic.IsNull)
                font._italic = refFont._italic;
            if (font._superscript.IsNull)
                font._superscript = refFont._superscript;
            if (font._subscript.IsNull)
                font._subscript = refFont._subscript;
        }

        protected void FlattenShading(Shading shading, Shading refShading)
        {
            //fClear?
            if (shading._visible.IsNull)
                shading._visible = refShading._visible;
            if (shading._color.IsNull)
                shading._color = refShading._color;
        }

        protected Border FlattenedBorderFromBorders(Border border, Borders parentBorders)
        {
            if (border == null)
                border = new Border(parentBorders);

            if (border._visible.IsNull)
                border._visible = parentBorders._visible;

            if (border._style.IsNull)
                border._style = parentBorders._style;

            if (border._width.IsNull)
                border._width = parentBorders._width;

            if (border._color.IsNull)
                border._color = parentBorders._color;

            return border;
        }

        protected void FlattenBorders(Borders borders, Borders refBorders)
        {
            if (borders._visible.IsNull)
                borders._visible = refBorders._visible;
            if (borders._width.IsNull)
                borders._width = refBorders._width;
            if (borders._style.IsNull)
                borders._style = refBorders._style;
            if (borders._color.IsNull)
                borders._color = refBorders._color;

            if (borders._distanceFromBottom.IsNull)
                borders._distanceFromBottom = refBorders._distanceFromBottom;
            if (borders._distanceFromRight.IsNull)
                borders._distanceFromRight = refBorders._distanceFromRight;
            if (borders._distanceFromLeft.IsNull)
                borders._distanceFromLeft = refBorders._distanceFromLeft;
            if (borders._distanceFromTop.IsNull)
                borders._distanceFromTop = refBorders._distanceFromTop;

            if (refBorders._left != null)
            {
                FlattenBorder(borders.Left, refBorders._left);
                FlattenedBorderFromBorders(borders._left, borders);
            }
            if (refBorders._right != null)
            {
                FlattenBorder(borders.Right, refBorders._right);
                FlattenedBorderFromBorders(borders._right, borders);
            }
            if (refBorders._top != null)
            {
                FlattenBorder(borders.Top, refBorders._top);
                FlattenedBorderFromBorders(borders._top, borders);
            }
            if (refBorders._bottom != null)
            {
                FlattenBorder(borders.Bottom, refBorders._bottom);
                FlattenedBorderFromBorders(borders._bottom, borders);
            }
        }

        protected void FlattenBorder(Border border, Border refBorder)
        {
            if (border._visible.IsNull)
                border._visible = refBorder._visible;
            if (border._width.IsNull)
                border._width = refBorder._width;
            if (border._style.IsNull)
                border._style = refBorder._style;
            if (border._color.IsNull)
                border._color = refBorder._color;
        }

        protected void FlattenTabStops(TabStops tabStops, TabStops refTabStops)
        {
            if (!tabStops._fClearAll)
            {
                foreach (TabStop refTabStop in refTabStops)
                {
                    if (tabStops.GetTabStopAt(refTabStop.Position) == null && refTabStop.AddTab)
                        tabStops.AddTabStop(refTabStop.Position, refTabStop.Alignment, refTabStop.Leader);
                }
            }

            for (int i = 0; i < tabStops.Count; i++)
            {
                TabStop tabStop = tabStops[i];
                if (!tabStop.AddTab)
                    tabStops.RemoveObjectAt(i);
            }
            // The TabStopCollection is complete now.
            // Prevent inheritance of tab stops.
            tabStops._fClearAll = true;
        }

        protected void FlattenPageSetup(PageSetup pageSetup, PageSetup refPageSetup)
        {
            if (pageSetup._pageWidth.IsNull && pageSetup._pageHeight.IsNull)
            {
                if (pageSetup._pageFormat.IsNull)
                {
                    pageSetup._pageWidth = refPageSetup._pageWidth;
                    pageSetup._pageHeight = refPageSetup._pageHeight;
                    pageSetup._pageFormat = refPageSetup._pageFormat;
                }
                else
                    PageSetup.GetPageSize(pageSetup.PageFormat, out pageSetup._pageWidth, out pageSetup._pageHeight);
            }
            else
            {
                Unit dummyUnit;
                if (pageSetup._pageWidth.IsNull)
                {
                    if (pageSetup._pageFormat.IsNull)
                        pageSetup._pageHeight = refPageSetup._pageHeight;
                    else
                        PageSetup.GetPageSize(pageSetup.PageFormat, out dummyUnit, out pageSetup._pageHeight);
                }
                else if (pageSetup._pageHeight.IsNull)
                {
                    if (pageSetup._pageFormat.IsNull)
                        pageSetup._pageWidth = refPageSetup._pageWidth;
                    else
                        PageSetup.GetPageSize(pageSetup.PageFormat, out pageSetup._pageWidth, out dummyUnit);
                }
            }
            //      if (pageSetup.pageWidth.IsNull)
            //        pageSetup.pageWidth = refPageSetup.pageWidth;
            //      if (pageSetup.pageHeight.IsNull)
            //        pageSetup.pageHeight = refPageSetup.pageHeight;
            //      if (pageSetup.pageFormat.IsNull)
            //        pageSetup.pageFormat = refPageSetup.pageFormat;
            if (pageSetup._sectionStart.IsNull)
                pageSetup._sectionStart = refPageSetup._sectionStart;
            if (pageSetup._orientation.IsNull)
                pageSetup._orientation = refPageSetup._orientation;
            if (pageSetup._topMargin.IsNull)
                pageSetup._topMargin = refPageSetup._topMargin;
            if (pageSetup._bottomMargin.IsNull)
                pageSetup._bottomMargin = refPageSetup._bottomMargin;
            if (pageSetup._leftMargin.IsNull)
                pageSetup._leftMargin = refPageSetup._leftMargin;
            if (pageSetup._rightMargin.IsNull)
                pageSetup._rightMargin = refPageSetup._rightMargin;
            if (pageSetup._headerDistance.IsNull)
                pageSetup._headerDistance = refPageSetup._headerDistance;
            if (pageSetup._footerDistance.IsNull)
                pageSetup._footerDistance = refPageSetup._footerDistance;
            if (pageSetup._oddAndEvenPagesHeaderFooter.IsNull)
                pageSetup._oddAndEvenPagesHeaderFooter = refPageSetup._oddAndEvenPagesHeaderFooter;
            if (pageSetup._differentFirstPageHeaderFooter.IsNull)
                pageSetup._differentFirstPageHeaderFooter = refPageSetup._differentFirstPageHeaderFooter;
            if (pageSetup._mirrorMargins.IsNull)
                pageSetup._mirrorMargins = refPageSetup._mirrorMargins;
            if (pageSetup._horizontalPageBreak.IsNull)
                pageSetup._horizontalPageBreak = refPageSetup._horizontalPageBreak;
        }

        protected void FlattenHeaderFooter(HeaderFooter headerFooter, bool isHeader)
        { }

        protected void FlattenFillFormat(FillFormat fillFormat)
        { }

        protected void FlattenLineFormat(LineFormat lineFormat, LineFormat refLineFormat)
        {
            if (refLineFormat != null)
            {
                if (lineFormat._width.IsNull)
                    lineFormat._width = refLineFormat._width;
            }
        }

        protected void FlattenAxis(Axis axis)
        {
            if (axis == null)
                return;

            LineFormat refLineFormat = new LineFormat();
            refLineFormat._width = 0.15;
            if (axis._hasMajorGridlines.Value && axis._majorGridlines != null)
                FlattenLineFormat(axis._majorGridlines._lineFormat, refLineFormat);
            if (axis._hasMinorGridlines.Value && axis._minorGridlines != null)
                FlattenLineFormat(axis._minorGridlines._lineFormat, refLineFormat);

            refLineFormat._width = 0.4;
            if (axis._lineFormat != null)
                FlattenLineFormat(axis._lineFormat, refLineFormat);

            //      axis.majorTick;
            //      axis.majorTickMark;
            //      axis.minorTick;
            //      axis.minorTickMark;

            //      axis.maximumScale;
            //      axis.minimumScale;

            //      axis.tickLabels;
            //      axis.title;
        }

        protected void FlattenPlotArea(PlotArea plotArea)
        { }

        protected void FlattenDataLabel(DataLabel dataLabel)
        { }


        #region Chart
        internal override void VisitChart(Chart chart)
        {
            Document document = chart.Document;
            if (chart._style.IsNull)
                chart._style.Value = Style.DefaultParagraphName;
            Style style = document.Styles[chart._style.Value];
            if (chart._format == null)
            {
                chart._format = style._paragraphFormat.Clone();
                chart._format._parent = chart;
            }
            else
                FlattenParagraphFormat(chart._format, style._paragraphFormat);


            FlattenLineFormat(chart._lineFormat, null);
            FlattenFillFormat(chart._fillFormat);

            FlattenAxis(chart._xAxis);
            FlattenAxis(chart._yAxis);
            FlattenAxis(chart._zAxis);

            FlattenPlotArea(chart._plotArea);

            //      if (this .hasDataLabel.Value)
            FlattenDataLabel(chart._dataLabel);

        }
        #endregion

        #region Document
        internal override void VisitDocument(Document document)
        {
        }

        internal override void VisitDocumentElements(DocumentElements elements)
        {
        }
        #endregion

        #region Format
        internal override void VisitStyle(Style style)
        {
            Style baseStyle = style.GetBaseStyle();
            if (baseStyle != null && baseStyle._paragraphFormat != null)
            {
                if (style._paragraphFormat == null)
                    style._paragraphFormat = baseStyle._paragraphFormat;
                else
                    FlattenParagraphFormat(style._paragraphFormat, baseStyle._paragraphFormat);
            }
        }

        internal override void VisitStyles(Styles styles)
        {
        }
        #endregion

        #region Paragraph
        internal override void VisitFootnote(Footnote footnote)
        {
            Document document = footnote.Document;

            ParagraphFormat format = null;

            Style style = document._styles[footnote._style.Value];
            if (style != null)
                format = ParagraphFormatFromStyle(style);
            else
            {
                footnote.Style = StyleNames.Footnote;
                format = document._styles[StyleNames.Footnote]._paragraphFormat;
            }

            if (footnote._format == null)
            {
                footnote._format = format.Clone();
                footnote._format._parent = footnote;
            }
            else
                FlattenParagraphFormat(footnote._format, format);

        }

        internal override void VisitParagraph(Paragraph paragraph)
        {
            Document document = paragraph.Document;

            ParagraphFormat format;

            DocumentObject currentElementHolder = GetDocumentElementHolder(paragraph);
            Style style = document._styles[paragraph._style.Value];
            if (style != null)
                format = ParagraphFormatFromStyle(style);

            else if (currentElementHolder is Cell)
            {
                paragraph._style = ((Cell)currentElementHolder)._style;
                format = ((Cell)currentElementHolder)._format;
            }
            else if (currentElementHolder is HeaderFooter)
            {
                HeaderFooter currHeaderFooter = ((HeaderFooter)currentElementHolder);
                if (currHeaderFooter.IsHeader)
                {
                    paragraph.Style = StyleNames.Header;
                    format = document._styles[StyleNames.Header]._paragraphFormat;
                }
                else
                {
                    paragraph.Style = StyleNames.Footer;
                    format = document._styles[StyleNames.Footer]._paragraphFormat;
                }

                if (currHeaderFooter._format != null)
                    FlattenParagraphFormat(paragraph.Format, currHeaderFooter._format);
            }
            else if (currentElementHolder is Footnote)
            {
                paragraph.Style = StyleNames.Footnote;
                format = document._styles[StyleNames.Footnote]._paragraphFormat;
            }
            else if (currentElementHolder is TextArea)
            {
                paragraph._style = ((TextArea)currentElementHolder)._style;
                format = ((TextArea)currentElementHolder)._format;
            }
            else
            {
                if (paragraph._style.Value != "")
                    paragraph.Style = StyleNames.InvalidStyleName;
                else
                    paragraph.Style = StyleNames.Normal;
                format = document._styles[paragraph.Style]._paragraphFormat;
            }

            if (paragraph._format == null)
            {
                paragraph._format = format.Clone();
                paragraph._format._parent = paragraph;
            }
            else
                FlattenParagraphFormat(paragraph._format, format);
        }
        #endregion

        #region Section
        internal override void VisitHeaderFooter(HeaderFooter headerFooter)
        {
            Document document = headerFooter.Document;
            string styleString;
            if (headerFooter.IsHeader)
                styleString = StyleNames.Header;
            else
                styleString = StyleNames.Footer;

            ParagraphFormat format;
            Style style = document._styles[headerFooter._style.Value];
            if (style != null)
                format = ParagraphFormatFromStyle(style);
            else
            {
                format = document._styles[styleString]._paragraphFormat;
                headerFooter.Style = styleString;
            }

            if (headerFooter._format == null)
            {
                headerFooter._format = format.Clone();
                headerFooter._format._parent = headerFooter;
            }
            else
                FlattenParagraphFormat(headerFooter._format, format);
        }

        internal override void VisitHeadersFooters(HeadersFooters headersFooters)
        {
        }

        internal override void VisitSection(Section section)
        {
            Section prevSec = section.PreviousSection();
            PageSetup prevPageSetup = PageSetup.DefaultPageSetup;
            if (prevSec != null)
            {
                prevPageSetup = prevSec._pageSetup;

                if (!section.Headers.HasHeaderFooter(HeaderFooterIndex.Primary))
                    section.Headers._primary = prevSec.Headers._primary;
                if (!section.Headers.HasHeaderFooter(HeaderFooterIndex.EvenPage))
                    section.Headers._evenPage = prevSec.Headers._evenPage;
                if (!section.Headers.HasHeaderFooter(HeaderFooterIndex.FirstPage))
                    section.Headers._firstPage = prevSec.Headers._firstPage;

                if (!section.Footers.HasHeaderFooter(HeaderFooterIndex.Primary))
                    section.Footers._primary = prevSec.Footers._primary;
                if (!section.Footers.HasHeaderFooter(HeaderFooterIndex.EvenPage))
                    section.Footers._evenPage = prevSec.Footers._evenPage;
                if (!section.Footers.HasHeaderFooter(HeaderFooterIndex.FirstPage))
                    section.Footers._firstPage = prevSec.Footers._firstPage;
            }

            if (section._pageSetup == null)
                section._pageSetup = prevPageSetup;
            else
                FlattenPageSetup(section._pageSetup, prevPageSetup);
        }

        internal override void VisitSections(Sections sections)
        {
        }
        #endregion

        #region Shape
        internal override void VisitTextFrame(TextFrame textFrame)
        {
            if (textFrame._height.IsNull)
                textFrame._height = Unit.FromInch(1);
            if (textFrame._width.IsNull)
                textFrame._width = Unit.FromInch(1);
        }
        #endregion

        #region Table
        internal override void VisitCell(Cell cell)
        {
            // format, shading and borders are already processed.
        }

        internal override void VisitColumns(Columns columns)
        {
            foreach (Column col in columns)
            {
                if (col._width.IsNull)
                    col._width = columns._width;

                if (col._width.IsNull)
                    col._width = "2.5cm";
            }
        }

        internal override void VisitRow(Row row)
        {
            foreach (Cell cell in row.Cells)
            {
                if (cell._verticalAlignment.IsNull)
                    cell._verticalAlignment = row._verticalAlignment;
            }
        }

        internal override void VisitRows(Rows rows)
        {
            foreach (Row row in rows)
            {
                if (row._height.IsNull)
                    row._height = rows._height;
                if (row._heightRule.IsNull)
                    row._heightRule = rows._heightRule;
                if (row._verticalAlignment.IsNull)
                    row._verticalAlignment = rows._verticalAlignment;
            }
        }
        /// <summary>
        /// Returns a paragraph format object initialized by the given style.
        /// It differs from style.ParagraphFormat if style is a character style.
        /// </summary>
        ParagraphFormat ParagraphFormatFromStyle(Style style)
        {
            if (style.Type == StyleType.Character)
            {
                Document doc = style.Document;
                ParagraphFormat format = style._paragraphFormat.Clone();
                FlattenParagraphFormat(format, doc.Styles.Normal.ParagraphFormat);
                return format;
            }
            else
                return style._paragraphFormat;
        }

        internal override void VisitTable(Table table)
        {
            Document document = table.Document;

            if (table._leftPadding.IsNull)
                table._leftPadding = Unit.FromMillimeter(1.2);
            if (table._rightPadding.IsNull)
                table._rightPadding = Unit.FromMillimeter(1.2);

            ParagraphFormat format;
            Style style = document._styles[table._style.Value];
            if (style != null)
                format = ParagraphFormatFromStyle(style);
            else
            {
                table.Style = "Normal";
                format = document._styles.Normal._paragraphFormat;
            }

            if (table._format == null)
            {
                table._format = format.Clone();
                table._format._parent = table;
            }
            else
                FlattenParagraphFormat(table._format, format);

            int rows = table.Rows.Count;
            int clms = table.Columns.Count;

            for (int idxclm = 0; idxclm < clms; idxclm++)
            {
                Column column = table.Columns[idxclm];
                ParagraphFormat colFormat;
                style = document._styles[column._style.Value];
                if (style != null)
                    colFormat = ParagraphFormatFromStyle(style);
                else
                {
                    column._style = table._style;
                    colFormat = table.Format;
                }

                if (column._format == null)
                {
                    column._format = colFormat.Clone();
                    column._format._parent = column;
                    if (column._format._shading == null && table._format._shading != null)
                        column._format._shading = table._format._shading;
                }
                else
                    FlattenParagraphFormat(column._format, colFormat);

                if (column._leftPadding.IsNull)
                    column._leftPadding = table._leftPadding;
                if (column._rightPadding.IsNull)
                    column._rightPadding = table._rightPadding;

                if (column._shading == null)
                    column._shading = table._shading;

                else if (table._shading != null)
                    FlattenShading(column._shading, table._shading);

                if (column._borders == null)
                    column._borders = table._borders;
                else if (table._borders != null)
                    FlattenBorders(column._borders, table._borders);
            }

            for (int idxrow = 0; idxrow < rows; idxrow++)
            {
                Row row = table.Rows[idxrow];

                ParagraphFormat rowFormat;
                style = document._styles[row._style.Value];
                if (style != null)
                {
                    rowFormat = ParagraphFormatFromStyle(style);
                }
                else
                {
                    row._style = table._style;
                    rowFormat = table.Format;
                }

                for (int idxclm = 0; idxclm < clms; idxclm++)
                {
                    Column column = table.Columns[idxclm];
                    Cell cell = row[idxclm];

                    ParagraphFormat cellFormat;
                    Style cellStyle = document._styles[cell._style.Value];
                    if (cellStyle != null)
                    {
                        cellFormat = ParagraphFormatFromStyle(cellStyle);

                        if (cell._format == null)
                            cell._format = cellFormat;
                        else
                            FlattenParagraphFormat(cell._format, cellFormat);
                    }
                    else
                    {
                        if (row._format != null)
                            FlattenParagraphFormat(cell.Format, row._format);

                        if (style != null)
                        {
                            cell._style = row._style;
                            FlattenParagraphFormat(cell.Format, rowFormat);
                        }
                        else
                        {
                            cell._style = column._style;
                            FlattenParagraphFormat(cell.Format, column._format);
                        }
                    }

                    if (cell._format._shading == null && table._format._shading != null)
                        cell._format._shading = table._format._shading;

                    if (cell._shading == null)
                        cell._shading = row._shading;
                    else if (row._shading != null)
                        FlattenShading(cell._shading, row._shading);
                    if (cell._shading == null)
                        cell._shading = column._shading;
                    else if (column._shading != null)
                        FlattenShading(cell._shading, column._shading);
                    if (cell._borders == null)
                        CloneHelper(ref cell._borders, row._borders);
                    else if (row._borders != null)
                        FlattenBorders(cell._borders, row._borders);
                    if (cell._borders == null)
                        cell._borders = column._borders;
                    else if (column._borders != null)
                        FlattenBorders(cell._borders, column._borders);
                }

                if (row._format == null)
                {
                    row._format = rowFormat.Clone();
                    row._format._parent = row;
                    if (row._format._shading == null && table._format._shading != null)
                        row._format._shading = table._format._shading;
                }
                else
                    FlattenParagraphFormat(row._format, rowFormat);

                if (row._topPadding.IsNull)
                    row._topPadding = table._topPadding;
                if (row._bottomPadding.IsNull)
                    row._bottomPadding = table._bottomPadding;

                if (row._shading == null)
                    row._shading = table._shading;
                else if (table._shading != null)
                    FlattenShading(row._shading, table._shading);

                if (row._borders == null)
                    row._borders = table._borders;
                else if (table._borders != null)
                    FlattenBorders(row._borders, table._borders);
            }
        }

        private void CloneHelper(ref Borders borders, Borders source)
        {
            if (source != null)
            {
                borders = source.Clone();
                borders._parent = source._parent;
            }
        }

        #endregion

        internal override void VisitLegend(Legend legend)
        {
            ParagraphFormat parentFormat;
            if (!legend._style.IsNull)
            {
                Style style = legend.Document.Styles[legend.Style];
                if (style == null)
                    style = legend.Document.Styles[StyleNames.InvalidStyleName];

                parentFormat = style._paragraphFormat;
            }
            else
            {
                TextArea textArea = (TextArea)GetDocumentElementHolder(legend);
                legend._style = textArea._style;
                parentFormat = textArea._format;
            }
            if (legend._format == null)
                legend.Format = parentFormat.Clone();
            else
                FlattenParagraphFormat(legend._format, parentFormat);
        }

        internal override void VisitTextArea(TextArea textArea)
        {
            if (textArea == null || textArea._elements == null)
                return;

            Document document = textArea.Document;

            ParagraphFormat parentFormat;

            if (!textArea._style.IsNull)
            {
                Style style = textArea.Document.Styles[textArea.Style];
                if (style == null)
                    style = textArea.Document.Styles[StyleNames.InvalidStyleName];

                parentFormat = style._paragraphFormat;
            }
            else
            {
                Chart chart = (Chart)textArea._parent;
                parentFormat = chart._format;
                textArea._style = chart._style;
            }

            if (textArea._format == null)
                textArea.Format = parentFormat.Clone();
            else
                FlattenParagraphFormat(textArea._format, parentFormat);

            FlattenFillFormat(textArea._fillFormat);
            FlattenLineFormat(textArea._lineFormat, null);
        }

        private DocumentObject GetDocumentElementHolder(DocumentObject docObj)
        {
            DocumentElements docEls = (DocumentElements)docObj._parent;
            return docEls._parent;
        }
    }
}

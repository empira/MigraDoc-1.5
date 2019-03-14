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
using System.Globalization;
using PdfSharp;
using PdfSharp.Drawing;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Internals;

namespace MigraDoc.Rendering
{
    /// <summary>
    /// Represents a formatted document.
    /// </summary>
    public class FormattedDocument : IAreaProvider
    {
        enum PagePosition
        {
            First,
            Odd,
            Even
        }

        private struct HeaderFooterPosition
        {
            internal HeaderFooterPosition(int sectionNr, PagePosition pagePosition)
            {
                _sectionNr = sectionNr;
                _pagePosition = pagePosition;
            }

            public override bool Equals(object obj)
            {
                if (obj is HeaderFooterPosition)
                {
                    HeaderFooterPosition hfp = (HeaderFooterPosition)obj;
                    return _sectionNr == hfp._sectionNr && _pagePosition == hfp._pagePosition;
                }
                return false;
            }

            public override int GetHashCode()
            {
                return _sectionNr.GetHashCode() ^ _pagePosition.GetHashCode();
            }

            readonly int _sectionNr;
            readonly PagePosition _pagePosition;
        }

        internal FormattedDocument(Document document, DocumentRenderer documentRenderer)
        {
            _document = document;
            _documentRenderer = documentRenderer;
        }

        /// <summary>
        /// Formats the document by performing line breaks and page breaks.
        /// </summary>
        internal void Format(XGraphics gfx)
        {
            _bookmarks = new Dictionary<string, FieldInfos.BookmarkInfo>();
            _pageRenderInfos = new Dictionary<int, List<RenderInfo>>();
            _pageInfos = new Dictionary<int, PageInfo>();
            _pageFieldInfos = new Dictionary<int, FieldInfos>();
            _formattedHeaders = new Dictionary<HeaderFooterPosition, FormattedHeaderFooter>();
            _formattedFooters = new Dictionary<HeaderFooterPosition, FormattedHeaderFooter>();
            _gfx = gfx;
            _currentPage = 0;
            _sectionNumber = 0;
            _pageCount = 0;
            _shownPageNumber = 0;
            _documentRenderer.ProgressCompleted = 0;
            _documentRenderer.ProgressMaximum = 0;
            if (_documentRenderer.HasPrepareDocumentProgress)
            {
                foreach (Section section in _document.Sections)
                    _documentRenderer.ProgressMaximum += section.Elements.Count;
            }
            foreach (Section section in _document.Sections)
            {
                _isNewSection = true;
                _currentSection = section;
                ++_sectionNumber;
                if (NeedsEmptyPage())
                    InsertEmptyPage();

                TopDownFormatter formatter = new TopDownFormatter(this, _documentRenderer, section.Elements);
                formatter.FormatOnAreas(gfx, true);
                FillSectionPagesInfo();
                _documentRenderer.ProgressCompleted += section.Elements.Count;
            }
            _pageCount = _currentPage;
            FillNumPagesInfo();
        }

        PagePosition CurrentPagePosition
        {
            get
            {
                if (_isNewSection)
                    return PagePosition.First;
                // Choose header and footer based on the shown page number, not the physical page number.
                if (_shownPageNumber % 2 == 0)
                    return PagePosition.Even;
                return PagePosition.Odd;
            }
        }

        void FormatHeadersFooters()
        {
            HeadersFooters headers = (HeadersFooters)_currentSection.GetValue("Headers", GV.ReadOnly);
            if (headers != null)
            {
                PagePosition pagePos = CurrentPagePosition;
                HeaderFooterPosition hfp = new HeaderFooterPosition(_sectionNumber, pagePos);
                if (!_formattedHeaders.ContainsKey(hfp))
                    FormatHeader(hfp, ChooseHeaderFooter(headers, pagePos));
            }

            HeadersFooters footers = (HeadersFooters)_currentSection.GetValue("Footers", GV.ReadOnly);
            if (footers != null)
            {
                PagePosition pagePos = CurrentPagePosition;
                HeaderFooterPosition hfp = new HeaderFooterPosition(_sectionNumber, pagePos);
                if (!_formattedFooters.ContainsKey(hfp))
                    FormatFooter(hfp, ChooseHeaderFooter(footers, pagePos));
            }
        }


        void FormatHeader(HeaderFooterPosition hfp, HeaderFooter header)
        {
            if (header != null && !_formattedHeaders.ContainsKey(hfp))
            {
                FormattedHeaderFooter formattedHeaderFooter = new FormattedHeaderFooter(header, _documentRenderer, _currentFieldInfos);
                formattedHeaderFooter.ContentRect = GetHeaderArea(_currentSection, _currentPage);
                formattedHeaderFooter.Format(_gfx);
                _formattedHeaders.Add(hfp, formattedHeaderFooter);
            }
        }


        void FormatFooter(HeaderFooterPosition hfp, HeaderFooter footer)
        {
            if (footer != null && !_formattedFooters.ContainsKey(hfp))
            {
                FormattedHeaderFooter formattedHeaderFooter = new FormattedHeaderFooter(footer, _documentRenderer, _currentFieldInfos);
                formattedHeaderFooter.ContentRect = GetFooterArea(_currentSection, _currentPage);
                formattedHeaderFooter.Format(_gfx);
                _formattedFooters.Add(hfp, formattedHeaderFooter);
            }
        }

        /// <summary>
        /// Fills the number pages information after formatting the document.
        /// </summary>
        void FillNumPagesInfo()
        {
            for (int page = 1; page <= _pageCount; ++page)
            {
                if (IsEmptyPage(page))
                    continue;

                FieldInfos fieldInfos = _pageFieldInfos[page];
                fieldInfos.NumPages = _pageCount;
            }
        }

        /// <summary>
        /// Fills the section pages information after formatting a section.
        /// </summary>
        void FillSectionPagesInfo()
        {
            for (int page = _currentPage; page > 0; --page)
            {
                if (IsEmptyPage(page))
                    continue;

                FieldInfos fieldInfos = _pageFieldInfos[page];
                if (fieldInfos.Section != _sectionNumber)
                    break;

                fieldInfos.SectionPages = _sectionPages;
            }
        }

        Rectangle CalcContentRect(int page)
        {
            PageSetup pageSetup = _currentSection.PageSetup;
            XUnit width = pageSetup.EffectivePageWidth.Point;

            width -= pageSetup.RightMargin.Point;
            width -= pageSetup.LeftMargin.Point;

            XUnit height = pageSetup.EffectivePageHeight.Point;

            height -= pageSetup.TopMargin.Point;
            height -= pageSetup.BottomMargin.Point;
            XUnit x;
            XUnit y = pageSetup.TopMargin.Point;
            if (pageSetup.MirrorMargins)
                x = page % 2 == 0 ? pageSetup.RightMargin.Point : pageSetup.LeftMargin.Point;
            else
                x = pageSetup.LeftMargin.Point;
            return new Rectangle(x, y, width, height);
        }

        /// <summary>
        /// Gets the rendering informations for the page content.
        /// </summary>
        /// <param name="page">The page to render.</param>
        /// <returns>Rendering information for the page content.</returns>
        public RenderInfo[] GetRenderInfos(int page)
        {
            if (_pageRenderInfos.ContainsKey(page))
                return (_pageRenderInfos[page]).ToArray();
            return null;
        }
        private Dictionary<int, List<RenderInfo>> _pageRenderInfos;

        /// <summary>
        /// Gets a formatted headerfooter object for header of the given page.
        /// </summary>
        /// <param name="page">The physical page the header shall appear on.</param>
        /// <returns>The required header, null if none exists to render.</returns>
        internal FormattedHeaderFooter GetFormattedHeader(int page)
        {
            FieldInfos fieldInfos = _pageFieldInfos[page];
            int logicalPage = fieldInfos.DisplayPageNr;

            PagePosition pagePos = logicalPage % 2 == 0 ? PagePosition.Even : PagePosition.Odd;

            if (page == 1)
                pagePos = PagePosition.First;
            else //page > 1
            {
                if (IsEmptyPage(page - 1)) // these empty pages only occur between sections.
                    pagePos = PagePosition.First;
                else
                {
                    FieldInfos prevFieldInfos = _pageFieldInfos[page - 1];
                    if (fieldInfos.Section != prevFieldInfos.Section)
                        pagePos = PagePosition.First;
                }
            }

            HeaderFooterPosition hfp = new HeaderFooterPosition(fieldInfos.Section, pagePos);
            if (_formattedHeaders.ContainsKey(hfp))
                return _formattedHeaders[hfp];
            return null;
        }

        /// <summary>
        /// Gets a formatted headerfooter object for footer of the given page.
        /// </summary>
        /// <param name="page">The physical page the footer shall appear on.</param>
        /// <returns>The required footer, null if none exists to render.</returns>
        internal FormattedHeaderFooter GetFormattedFooter(int page)
        {
            FieldInfos fieldInfos = _pageFieldInfos[page];
            int logicalPage = fieldInfos.DisplayPageNr;

            PagePosition pagePos = logicalPage % 2 == 0 ? PagePosition.Even : PagePosition.Odd;

            if (page == 1)
                pagePos = PagePosition.First;
            else //page > 1
            {
                if (IsEmptyPage(page - 1)) // these empty pages only occur between sections.
                    pagePos = PagePosition.First;
                else
                {
                    FieldInfos prevFieldInfos = _pageFieldInfos[page - 1];
                    if (fieldInfos.Section != prevFieldInfos.Section)
                        pagePos = PagePosition.First;
                }
            }

            HeaderFooterPosition hfp = new HeaderFooterPosition(fieldInfos.Section, pagePos);
            if (_formattedFooters.ContainsKey(hfp))
                return _formattedFooters[hfp];
            return null;
        }

        private Rectangle GetHeaderArea(Section section, int page)
        {
            PageSetup pageSetup = section.PageSetup;
            XUnit xPos;
            if (pageSetup.MirrorMargins && page % 2 == 0)
                xPos = pageSetup.RightMargin.Point;
            else
                xPos = pageSetup.LeftMargin.Point;

            XUnit width = pageSetup.EffectivePageWidth.Point;
            width -= pageSetup.LeftMargin + pageSetup.RightMargin;

            XUnit yPos = pageSetup.HeaderDistance.Point;
            XUnit height = pageSetup.TopMargin - pageSetup.HeaderDistance;
            return new Rectangle(xPos, yPos, width, height);
        }

        internal Rectangle GetHeaderArea(int page)
        {
            FieldInfos fieldInfos = _pageFieldInfos[page];
            Section section = _document.Sections[fieldInfos.Section - 1];
            return GetHeaderArea(section, page);
        }

        internal Rectangle GetFooterArea(int page)
        {
            FieldInfos fieldInfos = _pageFieldInfos[page];
            Section section = _document.Sections[fieldInfos.Section - 1];
            return GetFooterArea(section, page);
        }

        private Rectangle GetFooterArea(Section section, int page)
        {
            PageSetup pageSetup = section.PageSetup;
            XUnit xPos;
            if (pageSetup.MirrorMargins && page % 2 == 0)
                xPos = pageSetup.RightMargin.Point;
            else
                xPos = pageSetup.LeftMargin.Point;

            XUnit width = pageSetup.EffectivePageWidth.Point;
            width -= pageSetup.LeftMargin + pageSetup.RightMargin;
            XUnit yPos = pageSetup.EffectivePageHeight.Point;

            yPos -= pageSetup.BottomMargin.Point;
            XUnit height = pageSetup.BottomMargin - pageSetup.FooterDistance;
            return new Rectangle(xPos, yPos, width, height);
        }

        private HeaderFooter ChooseHeaderFooter(HeadersFooters hfs, PagePosition pagePos)
        {
            if (hfs == null)
                return null;

            PageSetup pageSetup = _currentSection.PageSetup;

            if (pagePos == PagePosition.First)
            {
                if (pageSetup.DifferentFirstPageHeaderFooter)
                    return (HeaderFooter)hfs.GetValue("FirstPage", GV.ReadOnly);
            }
            if (pagePos == PagePosition.Even || _shownPageNumber/*_currentPage*/ % 2 == 0)
            {
                if (pageSetup.OddAndEvenPagesHeaderFooter)
                    return (HeaderFooter)hfs.GetValue("EvenPage", GV.ReadOnly);
            }
            return (HeaderFooter)hfs.GetValue("Primary", GV.ReadOnly);
        }

        /// <summary>
        /// Gets the number of pages of the document.
        /// </summary>
        public int PageCount
        {
            get { return _pageCount; }
        }
        int _pageCount;


        /// <summary>
        /// Gets information about the specified page.
        /// </summary>
        /// <param name="page">The page the information is asked for.</param>
        /// <returns>The page information.</returns>
        public PageInfo GetPageInfo(int page)
        {
            if (page < 1 || page > _pageCount)
#if !SILVERLIGHT
                throw new ArgumentOutOfRangeException("page", page, page.ToString(CultureInfo.InvariantCulture));
#else
                throw new PdfSharp.ArgumentOutOfRangeException("page", page, page.ToString(CultureInfo.InvariantCulture));
#endif

            return _pageInfos[page];
        }

        #region IAreaProvider Members

        Area IAreaProvider.GetNextArea()
        {
            if (_isNewSection)
                _sectionPages = 0;

            ++_currentPage;
            ++_shownPageNumber;
            ++_sectionPages;
            InitFieldInfos();
            FormatHeadersFooters();
            _isNewSection = false;
            return CalcContentRect(_currentPage);
        }
        int _currentPage;

        Area IAreaProvider.ProbeNextArea()
        {
            return CalcContentRect(_currentPage + 1);
        }

        void InitFieldInfos()
        {
            _currentFieldInfos = new FieldInfos(_bookmarks);
            _currentFieldInfos.PhysicalPageNr = _currentPage;
            _currentFieldInfos.Section = _sectionNumber;

            if (_isNewSection && !_currentSection.PageSetup._startingNumber.IsNull)
                _shownPageNumber = _currentSection.PageSetup.StartingNumber;

            _currentFieldInfos.DisplayPageNr = _shownPageNumber;
        }

        void IAreaProvider.StoreRenderInfos(List<RenderInfo> renderInfos)
        {
            _pageRenderInfos.Add(_currentPage, renderInfos);
            XSize pageSize = CalcPageSize(_currentSection.PageSetup);
            PageOrientation pageOrientation = CalcPageOrientation(_currentSection.PageSetup);
            PageInfo pageInfo = new PageInfo(pageSize.Width, pageSize.Height, pageOrientation);
            _pageInfos.Add(_currentPage, pageInfo);
            _pageFieldInfos.Add(_currentPage, _currentFieldInfos);
        }

        PageOrientation CalcPageOrientation(PageSetup pageSetup)
        {
            PageOrientation pageOrientation = PageOrientation.Portrait;
            if (_currentSection.PageSetup.Orientation == Orientation.Landscape)
                pageOrientation = PageOrientation.Landscape;

            return pageOrientation;
        }

        XSize CalcPageSize(PageSetup pageSetup)
        {
            return new XSize(pageSetup.PageWidth.Point, pageSetup.PageHeight.Point);
        }

        bool IAreaProvider.PositionHorizontally(LayoutInfo layoutInfo)
        {
            switch (layoutInfo.HorizontalReference)
            {
                case HorizontalReference.PageMargin:
                case HorizontalReference.AreaBoundary:
                    return PositionHorizontallyToMargin(layoutInfo);

                case HorizontalReference.Page:
                    return PositionHorizontallyToPage(layoutInfo);
            }
            return false;
        }

        /// <summary>
        /// Gets the alignment depending on the currentPage for the alignments "Outside" and "Inside".
        /// </summary>
        /// <param name="alignment">The original alignment</param>
        /// <returns>the alignment depending on the currentPage for the alignments "Outside" and "Inside"</returns>
        private ElementAlignment GetCurrentAlignment(ElementAlignment alignment)
        {
            ElementAlignment align = alignment;

            if (align == ElementAlignment.Inside)
            {
                align = _currentPage % 2 == 0 ? ElementAlignment.Far : ElementAlignment.Near;
            }
            else if (align == ElementAlignment.Outside)
            {
                align = _currentPage % 2 == 0 ? ElementAlignment.Near : ElementAlignment.Far;
            }
            return align;
        }

        bool PositionHorizontallyToMargin(LayoutInfo layoutInfo)
        {
            Rectangle rect = CalcContentRect(_currentPage);
            ElementAlignment align = GetCurrentAlignment(layoutInfo.HorizontalAlignment);


            switch (align)
            {
                case ElementAlignment.Near:
                    if (layoutInfo.Left != 0)
                    {
                        layoutInfo.ContentArea.X += layoutInfo.Left;
                        return true;
                    }
                    if (layoutInfo.MarginLeft != 0)
                    {
                        layoutInfo.ContentArea.X += layoutInfo.MarginLeft;
                        return true;
                    }
                    return false;

                case ElementAlignment.Far:
                    XUnit xPos = rect.X + rect.Width;
                    xPos -= layoutInfo.ContentArea.Width;
                    xPos -= layoutInfo.MarginRight;
                    layoutInfo.ContentArea.X = xPos;
                    return true;

                case ElementAlignment.Center:
                    xPos = rect.Width;
                    xPos -= layoutInfo.ContentArea.Width;
                    xPos = rect.X + xPos / 2;
                    layoutInfo.ContentArea.X = xPos;
                    return true;
            }
            return false;
        }

        bool PositionHorizontallyToPage(LayoutInfo layoutInfo)
        {
            XUnit xPos;
            ElementAlignment align = GetCurrentAlignment(layoutInfo.HorizontalAlignment);
            switch (align)
            {
                case ElementAlignment.Near:
#if true
                    // Attempt to make it compatible with MigraDoc CPP.
                    // Ignore layoutInfo.Left if absolute position is specified in layoutInfo.MarginLeft.
                    // Use layoutInfo.Left if layoutInfo.MarginLeft is 0.
                    // TODO We would need HasValue for XUnit to determine whether a value was assigned.
                    if (layoutInfo.HorizontalReference == HorizontalReference.Page ||
                      layoutInfo.HorizontalReference == HorizontalReference.PageMargin)
                        xPos = layoutInfo.MarginLeft != 0 ? layoutInfo.MarginLeft : layoutInfo.Left;
                    else
                        xPos = Math.Max(layoutInfo.MarginLeft, layoutInfo.Left);
#else
                    if (layoutInfo.HorizontalReference == HorizontalReference.Page ||
                      layoutInfo.HorizontalReference == HorizontalReference.PageMargin)
                        xPos = layoutInfo.MarginLeft; // ignore layoutInfo.Left if absolute position is specified
                    else
                        xPos = Math.Max(layoutInfo.MarginLeft, layoutInfo.Left);
#endif
                    layoutInfo.ContentArea.X = xPos;
                    break;

                case ElementAlignment.Far:
                    xPos = _currentSection.PageSetup.EffectivePageWidth.Point;
                    xPos -= layoutInfo.ContentArea.Width;
                    xPos -= layoutInfo.MarginRight;
                    layoutInfo.ContentArea.X = xPos;
                    break;

                case ElementAlignment.Center:
                    xPos = _currentSection.PageSetup.EffectivePageWidth.Point;
                    xPos -= layoutInfo.ContentArea.Width;
                    xPos /= 2;
                    layoutInfo.ContentArea.X = xPos;
                    break;
            }
            return true;
        }

        bool PositionVerticallyToMargin(LayoutInfo layoutInfo)
        {
            Rectangle rect = CalcContentRect(_currentPage);
            XUnit yPos;
            switch (layoutInfo.VerticalAlignment)
            {
                case ElementAlignment.Near:
                    yPos = rect.Y;
                    if (layoutInfo.Top == 0)
                        yPos += layoutInfo.MarginTop;
                    else
                        yPos += layoutInfo.Top;
                    layoutInfo.ContentArea.Y = yPos;
                    break;

                case ElementAlignment.Far:
                    yPos = rect.Y + rect.Height;
                    yPos -= layoutInfo.ContentArea.Height;
                    yPos -= layoutInfo.MarginBottom;
                    layoutInfo.ContentArea.Y = yPos;
                    break;

                case ElementAlignment.Center:
                    yPos = rect.Height;
                    yPos -= layoutInfo.ContentArea.Height;
                    yPos = rect.Y + yPos / 2;
                    layoutInfo.ContentArea.Y = yPos;
                    break;
            }
            return true;
        }

        bool NeedsEmptyPage()
        {
            int nextPage = _currentPage + 1;
            PageSetup pageSetup = _currentSection.PageSetup;
            bool startOnEvenPage = pageSetup.SectionStart == BreakType.BreakEvenPage;
            bool startOnOddPage = pageSetup.SectionStart == BreakType.BreakOddPage;

            if (startOnOddPage)
                return nextPage % 2 == 0;
            if (startOnEvenPage)
                return nextPage % 2 == 1;

            return false;
        }

        void InsertEmptyPage()
        {
            ++_currentPage;
            ++_shownPageNumber;
            _emptyPages.Add(_currentPage, null);

            XSize pageSize = CalcPageSize(_currentSection.PageSetup);
            PageOrientation pageOrientation = CalcPageOrientation(_currentSection.PageSetup);
            PageInfo pageInfo = new PageInfo(pageSize.Width, pageSize.Height, pageOrientation);
            _pageInfos.Add(_currentPage, pageInfo);
        }

        bool PositionVerticallyToPage(LayoutInfo layoutInfo)
        {
            XUnit yPos;
            switch (layoutInfo.VerticalAlignment)
            {
                case ElementAlignment.Near:
                    yPos = Math.Max(layoutInfo.MarginTop, layoutInfo.Top);
                    layoutInfo.ContentArea.Y = yPos;
                    break;

                case ElementAlignment.Far:
                    yPos = _currentSection.PageSetup.EffectivePageHeight.Point;
                    yPos -= layoutInfo.ContentArea.Height;
                    yPos -= layoutInfo.MarginBottom;
                    layoutInfo.ContentArea.Y = yPos;
                    break;

                case ElementAlignment.Center:
                    yPos = _currentSection.PageSetup.EffectivePageHeight.Point;
                    yPos -= layoutInfo.ContentArea.Height;
                    yPos /= 2;
                    layoutInfo.ContentArea.Y = yPos;
                    break;
            }
            return true;
        }

        bool IAreaProvider.PositionVertically(LayoutInfo layoutInfo)
        {
            switch (layoutInfo.VerticalReference)
            {
                case VerticalReference.PreviousElement:
                    return false;

                case VerticalReference.AreaBoundary:
                case VerticalReference.PageMargin:
                    return PositionVerticallyToMargin(layoutInfo);

                case VerticalReference.Page:
                    return PositionVerticallyToPage(layoutInfo);
            }
            return false;
        }

        internal FieldInfos GetFieldInfos(int page)
        {
            return _pageFieldInfos[page];
        }

        FieldInfos IAreaProvider.AreaFieldInfos
        {
            get { return _currentFieldInfos; }
        }

        bool IAreaProvider.IsAreaBreakBefore(LayoutInfo layoutInfo)
        {
            return layoutInfo.PageBreakBefore;
        }

        internal bool IsEmptyPage(int page)
        {
            return _emptyPages.ContainsKey(page);
        }
        #endregion

        Dictionary<string, FieldInfos.BookmarkInfo> _bookmarks;
        int _sectionPages;
        int _shownPageNumber;
        int _sectionNumber;
        Section _currentSection;
        bool _isNewSection;
        FieldInfos _currentFieldInfos;
        Dictionary<int, FieldInfos> _pageFieldInfos;
        Dictionary<HeaderFooterPosition, FormattedHeaderFooter> _formattedHeaders;
        Dictionary<HeaderFooterPosition, FormattedHeaderFooter> _formattedFooters;
        readonly DocumentRenderer _documentRenderer;
        XGraphics _gfx;
        Dictionary<int, PageInfo> _pageInfos;
        readonly Dictionary<int, object> _emptyPages = new Dictionary<int, object>();
        readonly Document _document;
    }
}
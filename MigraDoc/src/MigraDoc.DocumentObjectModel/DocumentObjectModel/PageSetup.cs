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

using MigraDoc.DocumentObjectModel.Internals;

namespace MigraDoc.DocumentObjectModel
{
    /// <summary>
    /// Represents the page setup of a section.
    /// </summary>
    public class PageSetup : DocumentObject
    {
        /// <summary>
        /// Initializes a new instance of the PageSetup class.
        /// </summary>
        public PageSetup()
        { }

        /// <summary>
        /// Initializes a new instance of the PageSetup class with the specified parent.
        /// </summary>
        internal PageSetup(DocumentObject parent) : base(parent) { }

        #region Methods
        /// <summary>
        /// Creates a deep copy of this object.
        /// </summary>
        public new PageSetup Clone()
        {
            return (PageSetup)DeepCopy();
        }

        /// <summary>
        /// Gets the page's size and height for the given PageFormat.
        /// </summary>
        public static void GetPageSize(PageFormat pageFormat, out Unit pageWidth, out Unit pageHeight)
        {
            //Sizes in mm:
            pageWidth = 0;
            pageHeight = 0;
            const int A0Height = 1189;
            const int A0Width = 841;
            int height = 0;
            int width = 0;
            switch (pageFormat)
            {
                case PageFormat.A0:
                    height = A0Height;
                    width = A0Width;
                    break;
                case PageFormat.A1:
                    height = A0Width;
                    width = A0Height / 2;
                    break;
                case PageFormat.A2:
                    height = A0Height / 2;
                    width = A0Width / 2;
                    break;
                case PageFormat.A3:
                    height = A0Width / 2;
                    width = A0Height / 4;
                    break;
                case PageFormat.A4:
                    height = A0Height / 4;
                    width = A0Width / 4;
                    break;
                case PageFormat.A5:
                    height = A0Width / 4;
                    width = A0Height / 8;
                    break;
                case PageFormat.A6:
                    height = A0Height / 8;
                    width = A0Width / 8;
                    break;
                case PageFormat.B5:
                    height = 257;
                    width = 182;
                    break;
                case PageFormat.Letter:
                    pageWidth = Unit.FromPoint(612);
                    pageHeight = Unit.FromPoint(792);
                    break;
                case PageFormat.Legal:
                    pageWidth = Unit.FromPoint(612);
                    pageHeight = Unit.FromPoint(1008);
                    break;
                case PageFormat.Ledger:
                    pageWidth = Unit.FromPoint(1224);
                    pageHeight = Unit.FromPoint(792);
                    break;
                case PageFormat.P11x17:
                    pageWidth = Unit.FromPoint(792);
                    pageHeight = Unit.FromPoint(1224);
                    break;
            }
            if (height > 0)
                pageHeight = Unit.FromMillimeter(height);
            if (width > 0)
                pageWidth = Unit.FromMillimeter(width);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value which defines whether the section starts on next, odd or even page.
        /// </summary>
        public BreakType SectionStart
        {
            get { return (BreakType)_sectionStart.Value; }
            set { _sectionStart.Value = (int)value; }
        }
        [DV(Type = typeof(BreakType))]
        internal NEnum _sectionStart = NEnum.NullValue(typeof(BreakType));

        /// <summary>
        /// Gets or sets the page orientation of the section.
        /// </summary>
        public Orientation Orientation
        {
            get { return (Orientation)_orientation.Value; }
            set { _orientation.Value = (int)value; }
        }
        [DV(Type = typeof(Orientation))]
        internal NEnum _orientation = NEnum.NullValue(typeof(Orientation));

        private bool IsLandscape
        {
            get { return Orientation == Orientation.Landscape; }
        }

        // TODO To be compatible with Word, PageWidth should always return the actual width (e.g. 21 cm for DIN A 4 portrait and 29.7 cm for DIN A 4 landscape).
        // TODO Pagemargins are also "moving": portrait-left becomes landscape-top
        /// <summary>
        /// Gets or sets the page width. If Orientation is set to Landscape, the PageWidth specifies the height of the page.
        /// </summary>
        public Unit PageWidth
        {
            get { return _pageWidth; }
            set { _pageWidth = value; }
        }
        [DV]
        internal Unit _pageWidth = Unit.NullValue;

        /// <summary>
        /// Gets the effective page width, depending on the Orientation this will either be the height or the width.
        /// </summary>
        internal Unit EffectivePageWidth
        {
            get { return IsLandscape ? PageHeight : PageWidth; }
        }

        /// <summary>
        /// Gets or sets the starting number for the first section page.
        /// </summary>
        public int StartingNumber
        {
            get { return _startingNumber.Value; }
            set { _startingNumber.Value = value; }
        }
        [DV]
        internal NInt _startingNumber = NInt.NullValue;

        /// <summary>
        /// Gets or sets the page height. If Orientation is set to Landscape, the PageHeight specifies the width of the page.
        /// </summary>
        public Unit PageHeight
        {
            get { return _pageHeight; }
            set { _pageHeight = value; }
        }
        [DV]
        internal Unit _pageHeight = Unit.NullValue;

        /// <summary>
        /// Gets the effective page height, depending on the Orientation this will either be the height or the width.
        /// </summary>
        internal Unit EffectivePageHeight
        {
            get { return IsLandscape ? PageWidth : PageHeight; }
        }

        /// <summary>
        /// Gets or sets the top margin of the pages in the section.
        /// </summary>
        public Unit TopMargin
        {
            get { return _topMargin; }
            set { _topMargin = value; }
        }
        [DV]
        internal Unit _topMargin = Unit.NullValue;

        /// <summary>
        /// Gets or sets the bottom margin of the pages in the section.
        /// </summary>
        public Unit BottomMargin
        {
            get { return _bottomMargin; }
            set { _bottomMargin = value; }
        }
        [DV]
        internal Unit _bottomMargin = Unit.NullValue;

        /// <summary>
        /// Gets or sets the left margin of the pages in the section.
        /// </summary>
        public Unit LeftMargin
        {
            get { return _leftMargin; }
            set { _leftMargin = value; }
        }
        [DV]
        internal Unit _leftMargin = Unit.NullValue;

        /// <summary>
        /// Gets or sets the right margin of the pages in the section.
        /// </summary>
        public Unit RightMargin
        {
            get { return _rightMargin; }
            set { _rightMargin = value; }
        }
        [DV]
        internal Unit _rightMargin = Unit.NullValue;

        /// <summary>
        /// Gets or sets a value which defines whether the odd and even pages
        /// of the section have different header and footer.
        /// </summary>
        public bool OddAndEvenPagesHeaderFooter
        {
            get { return _oddAndEvenPagesHeaderFooter.Value; }
            set { _oddAndEvenPagesHeaderFooter.Value = value; }
        }
        [DV]
        internal NBool _oddAndEvenPagesHeaderFooter = NBool.NullValue;

        /// <summary>
        /// Gets or sets a value which define whether the section has a different
        /// first page header and footer.
        /// </summary>
        public bool DifferentFirstPageHeaderFooter
        {
            get { return _differentFirstPageHeaderFooter.Value; }
            set { _differentFirstPageHeaderFooter.Value = value; }
        }
        [DV]
        internal NBool _differentFirstPageHeaderFooter = NBool.NullValue;

        /// <summary>
        /// Gets or sets the distance between the header and the page top
        /// of the pages in the section.
        /// </summary>
        public Unit HeaderDistance
        {
            get { return _headerDistance; }
            set { _headerDistance = value; }
        }
        [DV]
        internal Unit _headerDistance = Unit.NullValue;

        /// <summary>
        /// Gets or sets the distance between the footer and the page bottom
        /// of the pages in the section.
        /// </summary>
        public Unit FooterDistance
        {
            get { return _footerDistance; }
            set { _footerDistance = value; }
        }
        [DV]
        internal Unit _footerDistance = Unit.NullValue;

        /// <summary>
        /// Gets or sets a value which defines whether the odd and even pages
        /// of the section should change left and right margin.
        /// </summary>
        public bool MirrorMargins
        {
            get { return _mirrorMargins.Value; }
            set { _mirrorMargins.Value = value; }
        }
        [DV]
        internal NBool _mirrorMargins = NBool.NullValue;

        /// <summary>
        /// Gets or sets a value which defines whether a page should break horizontally.
        /// Currently only tables are supported.
        /// </summary>
        public bool HorizontalPageBreak
        {
            get { return _horizontalPageBreak.Value; }
            set { _horizontalPageBreak.Value = value; }
        }
        [DV]
        internal NBool _horizontalPageBreak = NBool.NullValue;

        /// <summary>
        /// Gets or sets the page format of the section.
        /// </summary>
        public PageFormat PageFormat
        {
            get { return (PageFormat)_pageFormat.Value; }
            set { _pageFormat.Value = (int)value; }
        }
        [DV(Type = typeof(PageFormat))]
        internal NEnum _pageFormat = NEnum.NullValue(typeof(PageFormat));

        /// <summary>
        /// Gets or sets a comment associated with this object.
        /// </summary>
        public string Comment
        {
            get { return _comment.Value; }
            set { _comment.Value = value; }
        }
        [DV]
        internal NString _comment = NString.NullValue;
        #endregion

        /// <summary>
        /// Gets the PageSetup of the previous section, or null, if the page setup belongs 
        /// to the first section.
        /// </summary>
        public PageSetup PreviousPageSetup()
        {
            Section section = Parent as Section;
            if (section != null)
            {
                section = section.PreviousSection();
                if (section != null)
                    return section.PageSetup;
            }
            return null;
        }

        /// <summary>
        /// Gets a PageSetup object with default values for all properties.
        /// </summary>
        internal static PageSetup DefaultPageSetup
        {
            get
            {
                if (_defaultPageSetup == null)
                {
                    _defaultPageSetup = new PageSetup();
                    _defaultPageSetup.PageFormat = PageFormat.A4;
                    _defaultPageSetup.SectionStart = BreakType.BreakNextPage;
                    _defaultPageSetup.Orientation = Orientation.Portrait;
                    _defaultPageSetup.PageWidth = "21cm";
                    _defaultPageSetup.PageHeight = "29.7cm";
                    _defaultPageSetup.TopMargin = "2.5cm";
                    _defaultPageSetup.BottomMargin = "2cm";
                    _defaultPageSetup.LeftMargin = "2.5cm";
                    _defaultPageSetup.RightMargin = "2.5cm";
                    _defaultPageSetup.HeaderDistance = "1.25cm";
                    _defaultPageSetup.FooterDistance = "1.25cm";
                    _defaultPageSetup.OddAndEvenPagesHeaderFooter = false;
                    _defaultPageSetup.DifferentFirstPageHeaderFooter = false;
                    _defaultPageSetup.MirrorMargins = false;
                    _defaultPageSetup.HorizontalPageBreak = false;
                }
                return _defaultPageSetup;
            }
        }
        static PageSetup _defaultPageSetup;

        #region Internal
        /// <summary>
        /// Converts PageSetup into DDL.
        /// </summary>
        internal override void Serialize(Serializer serializer)
        {
            serializer.WriteComment(_comment.Value);
            int pos = serializer.BeginContent("PageSetup");

            if (!_pageHeight.IsNull)
                serializer.WriteSimpleAttribute("PageHeight", PageHeight);

            if (!_pageWidth.IsNull)
                serializer.WriteSimpleAttribute("PageWidth", PageWidth);

            if (!_orientation.IsNull)
                serializer.WriteSimpleAttribute("Orientation", Orientation);

            if (!_leftMargin.IsNull)
                serializer.WriteSimpleAttribute("LeftMargin", LeftMargin);

            if (!_rightMargin.IsNull)
                serializer.WriteSimpleAttribute("RightMargin", RightMargin);

            if (!_topMargin.IsNull)
                serializer.WriteSimpleAttribute("TopMargin", TopMargin);

            if (!_bottomMargin.IsNull)
                serializer.WriteSimpleAttribute("BottomMargin", BottomMargin);

            if (!_footerDistance.IsNull)
                serializer.WriteSimpleAttribute("FooterDistance", FooterDistance);

            if (!_headerDistance.IsNull)
                serializer.WriteSimpleAttribute("HeaderDistance", HeaderDistance);

            if (!_oddAndEvenPagesHeaderFooter.IsNull)
                serializer.WriteSimpleAttribute("OddAndEvenPagesHeaderFooter", OddAndEvenPagesHeaderFooter);

            if (!_differentFirstPageHeaderFooter.IsNull)
                serializer.WriteSimpleAttribute("DifferentFirstPageHeaderFooter", DifferentFirstPageHeaderFooter);

            if (!_sectionStart.IsNull)
                serializer.WriteSimpleAttribute("SectionStart", SectionStart);

            if (!_pageFormat.IsNull)
                serializer.WriteSimpleAttribute("PageFormat", PageFormat);

            if (!_mirrorMargins.IsNull)
                serializer.WriteSimpleAttribute("MirrorMargins", MirrorMargins);

            if (!_horizontalPageBreak.IsNull)
                serializer.WriteSimpleAttribute("HorizontalPageBreak", HorizontalPageBreak);

            if (!_startingNumber.IsNull)
                serializer.WriteSimpleAttribute("StartingNumber", StartingNumber);

            serializer.EndContent(pos);
        }

        /// <summary>
        /// Returns the meta object of this instance.
        /// </summary>
        internal override Meta Meta
        {
            get { return _meta ?? (_meta = new Meta(typeof(PageSetup))); }
        }
        static Meta _meta;
        #endregion
    }
}

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

using PdfSharp.Drawing;

namespace MigraDoc.Rendering
{
    /// <summary>
    /// Abstract base class to serve as a layoutable unit.
    /// </summary>
    public class LayoutInfo
    {
        internal LayoutInfo()
        { }

        /// <summary>
        /// Gets or sets the height necessary to start the document object.
        /// </summary>
        internal XUnit StartingHeight
        {
            get { return _startingHeight; }
            set { _startingHeight = value; }
        }
        XUnit _startingHeight;

        /// <summary>
        /// Gets or sets the height necessary to end the document object.
        /// </summary>
        internal XUnit TrailingHeight
        {
            get { return _trailingHeight; }
            set { _trailingHeight = value; }
        }
        XUnit _trailingHeight;

        /// <summary>
        /// Indicates whether the document object shall be kept on one page
        /// with its successor.
        /// </summary>
        internal bool KeepWithNext
        {
            get { return _keepWithNext; }
            set { _keepWithNext = value; }
        }
        bool _keepWithNext;

        /// <summary>
        /// Indicates whether the document object shall be kept together on one page.
        /// </summary>
        internal bool KeepTogether
        {
            get { return _keepTogether; }
            set { _keepTogether = value; }
        }
        bool _keepTogether;

        /// <summary>
        /// The space that shall be kept free above the element's content.
        /// </summary>
        internal virtual XUnit MarginTop
        {
            get { return _marginTop; }
            set { _marginTop = value; }
        }
        XUnit _marginTop;

        /// <summary>
        /// The space that shall be kept free right to the element's content.
        /// </summary>
        internal XUnit MarginRight
        {
            get { return _marginRight; }
            set { _marginRight = value; }
        }
        XUnit _marginRight;

        /// <summary>
        /// The space that shall be kept free below the element's content.
        /// </summary>
        internal XUnit MarginBottom
        {
            get { return _marginBottom; }
            set { _marginBottom = value; }
        }
        XUnit _marginBottom;

        /// <summary>
        /// The space that shall be kept free left to the element's content.
        /// </summary>
        internal XUnit MarginLeft
        {
            get { return _marginLeft; }
            set { _marginLeft = value; }
        }
        XUnit _marginLeft;

        /// <summary>
        /// Gets or sets the Area needed by the content (including padding and borders for e.g. paragraphs).
        /// </summary>
        public Area ContentArea
        {
            get { return _contentArea; }
            set { _contentArea = value; }
        }
        Area _contentArea;

        /// <summary>
        /// Gets or sets the a value indicating whether the element shall appear on a new page.
        /// </summary>
        internal bool PageBreakBefore
        {
            get { return _pageBreakBefore; }
            set { _pageBreakBefore = value; }
        }
        bool _pageBreakBefore;

        /// <summary>
        /// Gets or sets the reference point for horizontal positioning.
        /// </summary>
        /// <remarks>Default value is AreaBoundary.</remarks>
        internal HorizontalReference HorizontalReference
        {
            get { return _horizontalReference; }
            set { _horizontalReference = value; }
        }
        HorizontalReference _horizontalReference;

        /// <summary>
        /// Gets or sets the reference point for vertical positioning.
        /// </summary>
        /// <remarks>Default value is PreviousElement.</remarks>
        internal VerticalReference VerticalReference
        {
            get { return _verticalReference; }
            set { _verticalReference = value; }
        }
        VerticalReference _verticalReference;

        /// <summary>
        /// Gets or sets the horizontal alignment of the element.
        /// </summary>
        /// <remarks>Default value is Near.</remarks>
        internal ElementAlignment HorizontalAlignment
        {
            get { return _horizontalAlignment; }
            set { _horizontalAlignment = value; }
        }
        ElementAlignment _horizontalAlignment;

        /// <summary>
        /// Gets or sets the vertical alignment of the element.
        /// </summary>
        /// <remarks>Default value is Near.</remarks>
        internal ElementAlignment VerticalAlignment
        {
            get { return _verticalAlignment; }
            set { _verticalAlignment = value; }
        }
        ElementAlignment _verticalAlignment;

        /// <summary>
        /// Gets or sets the floating behavior of surrounding elements.
        /// </summary>
        /// <remarks>Default value is TopBottom.</remarks>
        internal Floating Floating
        {
            get { return _floating; }
            set { _floating = value; }
        }
        Floating _floating;

        /// <summary>
        /// Gets or sets the top position of the element.
        /// </summary>
        internal XUnit Top
        {
            get { return _top; }
            set { _top = value; }
        }
        XUnit _top;

        /// <summary>
        /// Gets or sets the left position of the element.
        /// </summary>
        internal XUnit Left
        {
            get { return _left; }
            set { _left = value; }
        }
        XUnit _left;

        /// <summary>
        /// Gets or sets the minimum width of the element.
        /// </summary>
        internal XUnit MinWidth
        {
            get { return _minWidth; }
            set { _minWidth = value; }
        }
        XUnit _minWidth;
    }
}

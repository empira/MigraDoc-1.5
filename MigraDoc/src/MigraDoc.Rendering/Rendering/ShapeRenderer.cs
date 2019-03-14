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
using PdfSharp.Drawing;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Internals;

namespace MigraDoc.Rendering
{
    /// <summary>
    /// Renders a shape to an XGraphics object.
    /// </summary>
    internal abstract class ShapeRenderer : Renderer
    {
        internal ShapeRenderer(XGraphics gfx, Shape shape, FieldInfos fieldInfos)
            : base(gfx, shape, fieldInfos)
        {
            _shape = shape;
            LineFormat lf = (LineFormat)_shape.GetValue("LineFormat", GV.ReadOnly);
            _lineFormatRenderer = new LineFormatRenderer(lf, gfx);
        }

        internal ShapeRenderer(XGraphics gfx, RenderInfo renderInfo, FieldInfos fieldInfos)
            : base(gfx, renderInfo, fieldInfos)
        {
            _shape = (Shape)renderInfo.DocumentObject;
            LineFormat lf = (LineFormat)_shape.GetValue("LineFormat", GV.ReadOnly);
            _lineFormatRenderer = new LineFormatRenderer(lf, gfx);
            FillFormat ff = (FillFormat)_shape.GetValue("FillFormat", GV.ReadOnly);
            _fillFormatRenderer = new FillFormatRenderer(ff, gfx);
        }

        internal override LayoutInfo InitialLayoutInfo
        {
            get
            {
                LayoutInfo layoutInfo = new LayoutInfo();

                layoutInfo.MarginTop = _shape.WrapFormat.DistanceTop.Point;
                layoutInfo.MarginLeft = _shape.WrapFormat.DistanceLeft.Point;
                layoutInfo.MarginBottom = _shape.WrapFormat.DistanceBottom.Point;
                layoutInfo.MarginRight = _shape.WrapFormat.DistanceRight.Point;
                layoutInfo.KeepTogether = true;
                layoutInfo.KeepWithNext = false;
                layoutInfo.PageBreakBefore = false;
                layoutInfo.VerticalReference = GetVerticalReference();
                layoutInfo.HorizontalReference = GetHorizontalReference();
                layoutInfo.Floating = GetFloating();
                if (layoutInfo.Floating == Floating.TopBottom && !_shape.Top.Position.IsEmpty)
                    layoutInfo.MarginTop = Math.Max(layoutInfo.MarginTop, _shape.Top.Position);
                return layoutInfo;
            }
        }

        Floating GetFloating()
        {
            if (_shape.RelativeVertical != RelativeVertical.Line && _shape.RelativeVertical != RelativeVertical.Paragraph)
                return Floating.None;

            switch (_shape.WrapFormat.Style)
            {
                case WrapStyle.None:
                case WrapStyle.Through:
                    return Floating.None;
            }
            return Floating.TopBottom;
        }

        /// <summary>
        /// Gets the shape width including line width.
        /// </summary>
        protected virtual XUnit ShapeWidth
        {
            get { return _shape.Width + _lineFormatRenderer.GetWidth(); }
        }

        /// <summary>
        /// Gets the shape height including line width.
        /// </summary>
        protected virtual XUnit ShapeHeight
        {
            get { return _shape.Height + _lineFormatRenderer.GetWidth(); }
        }

        /// <summary>
        /// Formats the shape.
        /// </summary>
        /// <param name="area">The area to fit in the shape.</param>
        /// <param name="previousFormatInfo"></param>
        internal override void Format(Area area, FormatInfo previousFormatInfo)
        {
            Floating floating = GetFloating();
            bool fits = floating == Floating.None || ShapeHeight <= area.Height;
            ((ShapeFormatInfo)_renderInfo.FormatInfo).Fits = fits;
            FinishLayoutInfo(area);
        }


        void FinishLayoutInfo(Area area)
        {
            LayoutInfo layoutInfo = _renderInfo.LayoutInfo;
            Area contentArea = new Rectangle(area.X, area.Y, ShapeWidth, ShapeHeight);
            layoutInfo.ContentArea = contentArea;
            layoutInfo.MarginTop = _shape.WrapFormat.DistanceTop.Point;
            layoutInfo.MarginLeft = _shape.WrapFormat.DistanceLeft.Point;
            layoutInfo.MarginBottom = _shape.WrapFormat.DistanceBottom.Point;
            layoutInfo.MarginRight = _shape.WrapFormat.DistanceRight.Point;
            layoutInfo.KeepTogether = true;
            layoutInfo.KeepWithNext = false;
            layoutInfo.PageBreakBefore = false;
            layoutInfo.MinWidth = ShapeWidth;

            if (_shape.Top.ShapePosition == ShapePosition.Undefined)
                layoutInfo.Top = _shape.Top.Position.Point;

            layoutInfo.VerticalAlignment = GetVerticalAlignment();
            layoutInfo.HorizontalAlignment = GetHorizontalAlignment();

            if (_shape.Left.ShapePosition == ShapePosition.Undefined)
                layoutInfo.Left = _shape.Left.Position.Point;

            layoutInfo.HorizontalReference = GetHorizontalReference();
            layoutInfo.VerticalReference = GetVerticalReference();
            layoutInfo.Floating = GetFloating();
        }

        HorizontalReference GetHorizontalReference()
        {
            switch (_shape.RelativeHorizontal)
            {
                case RelativeHorizontal.Margin:
                    return HorizontalReference.PageMargin;
                case RelativeHorizontal.Page:
                    return HorizontalReference.Page;
            }
            return HorizontalReference.AreaBoundary;
        }

        VerticalReference GetVerticalReference()
        {
            switch (_shape.RelativeVertical)
            {
                case RelativeVertical.Margin:
                    return VerticalReference.PageMargin;

                case RelativeVertical.Page:
                    return VerticalReference.Page;
            }
            return VerticalReference.PreviousElement;
        }

        ElementAlignment GetVerticalAlignment()
        {
            switch (_shape.Top.ShapePosition)
            {
                case ShapePosition.Center:
                    return ElementAlignment.Center;

                case ShapePosition.Bottom:
                    return ElementAlignment.Far;
            }
            return ElementAlignment.Near;
        }

        protected void RenderFilling()
        {
            Area contentArea = _renderInfo.LayoutInfo.ContentArea;
            XUnit lineWidth = _lineFormatRenderer.GetWidth();
            // Half of the line is drawn outside the shape, the other half inside the shape.
            // Therefore we have to reduce the position of the filling by 0.5 lineWidth and width and height by 2 lineWidth.
            _fillFormatRenderer.Render(contentArea.X + lineWidth / 2, contentArea.Y + lineWidth / 2,
                contentArea.Width - 2 * lineWidth, contentArea.Height - 2 * lineWidth);
        }

        protected void RenderLine()
        {
            Area contentArea = _renderInfo.LayoutInfo.ContentArea;
            XUnit lineWidth = _lineFormatRenderer.GetWidth();
            XUnit width = contentArea.Width - lineWidth;
            XUnit height = contentArea.Height - lineWidth;
            _lineFormatRenderer.Render(contentArea.X, contentArea.Y, width, height);
        }

        ElementAlignment GetHorizontalAlignment()
        {
            switch (_shape.Left.ShapePosition)
            {
                case ShapePosition.Center:
                    return ElementAlignment.Center;

                case ShapePosition.Right:
                    return ElementAlignment.Far;

                case ShapePosition.Outside:
                    return ElementAlignment.Outside;

                case ShapePosition.Inside:
                    return ElementAlignment.Inside;
            }
            return ElementAlignment.Near;
        }
        protected LineFormatRenderer _lineFormatRenderer;
        protected FillFormatRenderer _fillFormatRenderer;
        protected Shape _shape;
    }
}

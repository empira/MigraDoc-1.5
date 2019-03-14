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

using MigraDoc.DocumentObjectModel.Internals;
using MigraDoc.DocumentObjectModel.Tables;
using PdfSharp.Drawing;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Shapes.Charts;

namespace MigraDoc.Rendering
{
    /// <summary>
    /// Renders a chart to an XGraphics object.
    /// </summary>
    internal class ChartRenderer : ShapeRenderer
    {
        internal ChartRenderer(XGraphics gfx, Chart chart, FieldInfos fieldInfos)
            : base(gfx, chart, fieldInfos)
        {
            _chart = chart;
            ChartRenderInfo renderInfo = new ChartRenderInfo();
            renderInfo.DocumentObject = _shape;
            _renderInfo = renderInfo;
        }

        internal ChartRenderer(XGraphics gfx, RenderInfo renderInfo, FieldInfos fieldInfos)
            : base(gfx, renderInfo, fieldInfos)
        {
            _chart = (Chart)renderInfo.DocumentObject;
        }

        FormattedTextArea GetFormattedTextArea(TextArea area, XUnit width)
        {
            if (area == null)
                return null;

            FormattedTextArea formattedTextArea = new FormattedTextArea(_documentRenderer, area, _fieldInfos);

            if (!double.IsNaN(width))
                formattedTextArea.InnerWidth = width;

            formattedTextArea.Format(_gfx);
            return formattedTextArea;
        }

        FormattedTextArea GetFormattedTextArea(TextArea area)
        {
            return GetFormattedTextArea(area, double.NaN);
        }

        void GetLeftRightVerticalPosition(out XUnit top, out XUnit bottom)
        {
            //REM: Line width is still ignored while layouting charts.
            Area contentArea = _renderInfo.LayoutInfo.ContentArea;
            ChartFormatInfo formatInfo = (ChartFormatInfo)_renderInfo.FormatInfo;
            top = contentArea.Y;

            if (formatInfo.FormattedHeader != null)
                top += formatInfo.FormattedHeader.InnerHeight;

            bottom = contentArea.Y + contentArea.Height;
            if (formatInfo.FormattedFooter != null)
                bottom -= formatInfo.FormattedFooter.InnerHeight;
        }

        Rectangle GetLeftRect()
        {
            Area contentArea = _renderInfo.LayoutInfo.ContentArea;
            ChartFormatInfo formatInfo = (ChartFormatInfo)_renderInfo.FormatInfo;
            XUnit top;
            XUnit bottom;
            GetLeftRightVerticalPosition(out top, out bottom);

            XUnit left = contentArea.X;
            XUnit width = formatInfo.FormattedLeft.InnerWidth;

            return new Rectangle(left, top, width, bottom - top);
        }

        Rectangle GetRightRect()
        {
            Area contentArea = _renderInfo.LayoutInfo.ContentArea;
            ChartFormatInfo formatInfo = (ChartFormatInfo)_renderInfo.FormatInfo;
            XUnit top;
            XUnit bottom;
            GetLeftRightVerticalPosition(out top, out bottom);

            XUnit left = contentArea.X + contentArea.Width - formatInfo.FormattedRight.InnerWidth;
            XUnit width = formatInfo.FormattedRight.InnerWidth;

            return new Rectangle(left, top, width, bottom - top);
        }

        Rectangle GetHeaderRect()
        {
            Area contentArea = _renderInfo.LayoutInfo.ContentArea;
            ChartFormatInfo formatInfo = (ChartFormatInfo)_renderInfo.FormatInfo;

            XUnit left = contentArea.X;
            XUnit top = contentArea.Y;
            XUnit width = contentArea.Width;
            XUnit height = formatInfo.FormattedHeader.InnerHeight;

            return new Rectangle(left, top, width, height);
        }

        Rectangle GetFooterRect()
        {
            Area contentArea = _renderInfo.LayoutInfo.ContentArea;
            ChartFormatInfo formatInfo = (ChartFormatInfo)_renderInfo.FormatInfo;

            XUnit left = contentArea.X;
            XUnit top = contentArea.Y + contentArea.Height - formatInfo.FormattedFooter.InnerHeight;
            XUnit width = contentArea.Width;
            XUnit height = formatInfo.FormattedFooter.InnerHeight;

            return new Rectangle(left, top, width, height);
        }

        Rectangle GetTopRect()
        {
            Area contentArea = _renderInfo.LayoutInfo.ContentArea;
            ChartFormatInfo formatInfo = (ChartFormatInfo)_renderInfo.FormatInfo;

            XUnit left;
            XUnit right;
            GetTopBottomHorizontalPosition(out left, out right);

            XUnit top = contentArea.Y;
            if (formatInfo.FormattedHeader != null)
                top += formatInfo.FormattedHeader.InnerHeight;

            XUnit height = formatInfo.FormattedTop.InnerHeight;

            return new Rectangle(left, top, right - left, height);
        }

        Rectangle GetBottomRect()
        {
            Area contentArea = _renderInfo.LayoutInfo.ContentArea;
            ChartFormatInfo formatInfo = (ChartFormatInfo)_renderInfo.FormatInfo;

            XUnit left;
            XUnit right;
            GetTopBottomHorizontalPosition(out left, out right);

            XUnit top = contentArea.Y + contentArea.Height - formatInfo.FormattedBottom.InnerHeight;
            if (formatInfo.FormattedFooter != null)
                top -= formatInfo.FormattedFooter.InnerHeight;

            XUnit height = formatInfo.FormattedBottom.InnerHeight;
            return new Rectangle(left, top, right - left, height);
        }

        Rectangle GetPlotRect()
        {
            Area contentArea = _renderInfo.LayoutInfo.ContentArea;
            ChartFormatInfo formatInfo = (ChartFormatInfo)_renderInfo.FormatInfo;
            XUnit top = contentArea.Y;
            if (formatInfo.FormattedHeader != null)
                top += formatInfo.FormattedHeader.InnerHeight;

            if (formatInfo.FormattedTop != null)
                top += formatInfo.FormattedTop.InnerHeight;

            XUnit bottom = contentArea.Y + contentArea.Height;
            if (formatInfo.FormattedFooter != null)
                bottom -= formatInfo.FormattedFooter.InnerHeight;

            if (formatInfo.FormattedBottom != null)
                bottom -= formatInfo.FormattedBottom.InnerHeight;

            XUnit left = contentArea.X;
            if (formatInfo.FormattedLeft != null)
                left += formatInfo.FormattedLeft.InnerWidth;

            XUnit right = contentArea.X + contentArea.Width;
            if (formatInfo.FormattedRight != null)
                right -= formatInfo.FormattedRight.InnerWidth;

            return new Rectangle(left, top, right - left, bottom - top);
        }

        internal override void Format(Area area, FormatInfo previousFormatInfo)
        {
            ChartFormatInfo formatInfo = (ChartFormatInfo)_renderInfo.FormatInfo;

            TextArea textArea = (TextArea)_chart.GetValue("HeaderArea", GV.ReadOnly);
            formatInfo.FormattedHeader = GetFormattedTextArea(textArea, _chart.Width.Point);

            textArea = (TextArea)_chart.GetValue("FooterArea", GV.ReadOnly);
            formatInfo.FormattedFooter = GetFormattedTextArea(textArea, _chart.Width.Point);

            textArea = (TextArea)_chart.GetValue("LeftArea", GV.ReadOnly);
            formatInfo.FormattedLeft = GetFormattedTextArea(textArea);

            textArea = (TextArea)_chart.GetValue("RightArea", GV.ReadOnly);
            formatInfo.FormattedRight = GetFormattedTextArea(textArea);

            textArea = (TextArea)_chart.GetValue("TopArea", GV.ReadOnly);
            formatInfo.FormattedTop = GetFormattedTextArea(textArea, GetTopBottomWidth());

            textArea = (TextArea)_chart.GetValue("BottomArea", GV.ReadOnly);
            formatInfo.FormattedBottom = GetFormattedTextArea(textArea, GetTopBottomWidth());

            base.Format(area, previousFormatInfo);
            formatInfo.ChartFrame = ChartMapper.ChartMapper.Map(_chart);
        }


        XUnit AlignVertically(VerticalAlignment vAlign, XUnit top, XUnit bottom, XUnit height)
        {
            switch (vAlign)
            {
                case VerticalAlignment.Bottom:
                    return bottom - height;

                case VerticalAlignment.Center:
                    return (top + bottom - height) / 2;

                default:
                    return top;
            }
        }

        /// <summary>
        /// Gets the width of the top and bottom area.
        /// Used while formatting.
        /// </summary>
        /// <returns>The width of the top and bottom area</returns>
        private XUnit GetTopBottomWidth()
        {
            ChartFormatInfo formatInfo = (ChartFormatInfo)_renderInfo.FormatInfo;
            XUnit width = _chart.Width.Point;
            if (formatInfo.FormattedRight != null)
                width -= formatInfo.FormattedRight.InnerWidth;
            if (formatInfo.FormattedLeft != null)
                width -= formatInfo.FormattedLeft.InnerWidth;
            return width;
        }

        /// <summary>
        /// Gets the horizontal boundaries of the top and bottom area.
        /// Used while rendering.
        /// </summary>
        /// <param name="left">The left boundary of the top and bottom area</param>
        /// <param name="right">The right boundary of the top and bottom area</param>
        private void GetTopBottomHorizontalPosition(out XUnit left, out XUnit right)
        {
            Area contentArea = _renderInfo.LayoutInfo.ContentArea;
            ChartFormatInfo formatInfo = (ChartFormatInfo)_renderInfo.FormatInfo;
            left = contentArea.X;
            right = contentArea.X + contentArea.Width;

            if (formatInfo.FormattedRight != null)
                right -= formatInfo.FormattedRight.InnerWidth;
            if (formatInfo.FormattedLeft != null)
                left += formatInfo.FormattedLeft.InnerWidth;
        }

        void RenderArea(FormattedTextArea area, Rectangle rect)
        {
            if (area == null)
                return;

            TextArea textArea = area.TextArea;

            FillFormatRenderer fillFormatRenderer = new FillFormatRenderer((FillFormat)textArea.GetValue("FillFormat", GV.ReadOnly), _gfx);
            fillFormatRenderer.Render(rect.X, rect.Y, rect.Width, rect.Height);

            XUnit top = rect.Y;
            top += textArea.TopPadding;
            XUnit bottom = rect.Y + rect.Height;
            bottom -= textArea.BottomPadding;
            top = AlignVertically(textArea.VerticalAlignment, top, bottom, area.ContentHeight);

            XUnit left = rect.X;
            left += textArea.LeftPadding;

            RenderInfo[] renderInfos = area.GetRenderInfos();
            RenderByInfos(left, top, renderInfos);

            LineFormatRenderer lineFormatRenderer = new LineFormatRenderer((LineFormat)textArea.GetValue("LineFormat", GV.ReadOnly), _gfx);
            lineFormatRenderer.Render(rect.X, rect.Y, rect.Width, rect.Height);
        }

        internal override void Render()
        {
            RenderFilling();
            Area contentArea = _renderInfo.LayoutInfo.ContentArea;

            ChartFormatInfo formatInfo = (ChartFormatInfo)_renderInfo.FormatInfo;
            if (formatInfo.FormattedHeader != null)
                RenderArea(formatInfo.FormattedHeader, GetHeaderRect());

            if (formatInfo.FormattedFooter != null)
                RenderArea(formatInfo.FormattedFooter, GetFooterRect());

            if (formatInfo.FormattedTop != null)
                RenderArea(formatInfo.FormattedTop, GetTopRect());

            if (formatInfo.FormattedBottom != null)
                RenderArea(formatInfo.FormattedBottom, GetBottomRect());

            if (formatInfo.FormattedLeft != null)
                RenderArea(formatInfo.FormattedLeft, GetLeftRect());

            if (formatInfo.FormattedRight != null)
                RenderArea(formatInfo.FormattedRight, GetRightRect());

            PlotArea plotArea = (PlotArea)_chart.GetValue("PlotArea", GV.ReadOnly);
            if (plotArea != null)
                RenderPlotArea(plotArea, GetPlotRect());

            RenderLine();
        }

        void RenderPlotArea(PlotArea area, Rectangle rect)
        {
            PdfSharp.Charting.ChartFrame chartFrame = ((ChartFormatInfo)_renderInfo.FormatInfo).ChartFrame;

            XUnit top = rect.Y;
            top += area.TopPadding;

            XUnit bottom = rect.Y + rect.Height;
            bottom -= area.BottomPadding;

            XUnit left = rect.X;
            left += area.LeftPadding;

            XUnit right = rect.X + rect.Width;
            right -= area.RightPadding;

            chartFrame.Location = new XPoint(left, top);
            chartFrame.Size = new XSize(right - left, bottom - top);
            chartFrame.DrawChart(_gfx);
        }

        readonly Chart _chart;
    }
}

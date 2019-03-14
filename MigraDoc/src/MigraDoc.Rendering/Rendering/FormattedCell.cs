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
using MigraDoc.DocumentObjectModel;
using PdfSharp.Drawing;
using MigraDoc.DocumentObjectModel.Tables;

namespace MigraDoc.Rendering
{
    /// <summary>
    /// Represents a formatted cell.
    /// </summary>
    public class FormattedCell : IAreaProvider
    {
        internal FormattedCell(Cell cell, DocumentRenderer documentRenderer, Borders cellBorders, FieldInfos fieldInfos, XUnit xOffset, XUnit yOffset)
        {
            _cell = cell;
            _fieldInfos = fieldInfos;
            _yOffset = yOffset;
            _xOffset = xOffset;
            _bordersRenderer = new BordersRenderer(cellBorders, null);
            _documentRenderer = documentRenderer;
        }

        Area IAreaProvider.GetNextArea()
        {
            if (_isFirstArea)
            {
                Rectangle rect = CalcContentRect();
                _isFirstArea = false;
                return rect;
            }
            return null;
        }
        bool _isFirstArea = true;

        Area IAreaProvider.ProbeNextArea()
        {
            return null;
        }

        internal void Format(XGraphics gfx)
        {
            _gfx = gfx;
            _formatter = new TopDownFormatter(this, _documentRenderer, _cell.Elements);
            _formatter.FormatOnAreas(gfx, false);
            _contentHeight = CalcContentHeight(_documentRenderer);
        }

        private Rectangle CalcContentRect()
        {
            Column column = _cell.Column;
            XUnit width = InnerWidth;
            width -= column.LeftPadding.Point;
            Column rightColumn = _cell.Table.Columns[column.Index + _cell.MergeRight];
            width -= rightColumn.RightPadding.Point;

            XUnit height = double.MaxValue;
            return new Rectangle(_xOffset, _yOffset, width, height);
        }

        internal XUnit ContentHeight
        {
            get { return _contentHeight; }
        }

        internal XUnit InnerHeight
        {
            get
            {
                Row row = _cell.Row;
                XUnit verticalPadding = row.TopPadding.Point;
                verticalPadding += row.BottomPadding.Point;

                switch (row.HeightRule)
                {
                    case RowHeightRule.Exactly:
                        return row.Height.Point;

                    case RowHeightRule.Auto:
                        return verticalPadding + _contentHeight;

                    case RowHeightRule.AtLeast:
                    default:
                        return Math.Max(row.Height, verticalPadding + _contentHeight);
                }
            }
        }

        internal XUnit InnerWidth
        {
            get
            {
                XUnit width = 0;
                int cellColumnIdx = _cell.Column.Index;
                for (int toRight = 0; toRight <= _cell.MergeRight; ++toRight)
                {
                    int columnIdx = cellColumnIdx + toRight;
                    width += _cell.Table.Columns[columnIdx].Width;
                }
                width -= _bordersRenderer.GetWidth(BorderType.Right);

                return width;
            }
        }

        FieldInfos IAreaProvider.AreaFieldInfos
        {
            get { return _fieldInfos; }
        }

        void IAreaProvider.StoreRenderInfos(List<RenderInfo> renderInfos)
        {
            _renderInfos = renderInfos;
        }

        bool IAreaProvider.IsAreaBreakBefore(LayoutInfo layoutInfo)
        {
            return false;
        }

        bool IAreaProvider.PositionVertically(LayoutInfo layoutInfo)
        {
            return false;
        }

        bool IAreaProvider.PositionHorizontally(LayoutInfo layoutInfo)
        {
            return false;
        }

        private XUnit CalcContentHeight(DocumentRenderer documentRenderer)
        {
            XUnit height = RenderInfo.GetTotalHeight(GetRenderInfos());
            if (height == 0)
            {
                height = ParagraphRenderer.GetLineHeight(_cell.Format, _gfx, documentRenderer);
                height += _cell.Format.SpaceBefore;
                height += _cell.Format.SpaceAfter;
            }
            return height;
        }

        XUnit _contentHeight = 0;

        internal RenderInfo[] GetRenderInfos()
        {
            if (_renderInfos != null)
                return _renderInfos.ToArray();

            return null;
        }

        readonly FieldInfos _fieldInfos;
        List<RenderInfo> _renderInfos;
        readonly XUnit _xOffset;
        readonly XUnit _yOffset;

        /// <summary>
        /// Gets the cell the formatting information refers to.
        /// </summary>
        public Cell Cell
        {
            get { return _cell;}
        }
        readonly Cell _cell;
        TopDownFormatter _formatter;
        readonly BordersRenderer _bordersRenderer;
        XGraphics _gfx;
        readonly DocumentRenderer _documentRenderer;
    }
}

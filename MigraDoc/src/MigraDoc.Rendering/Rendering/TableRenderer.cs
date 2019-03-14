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
using MigraDoc.DocumentObjectModel.Internals;
using PdfSharp.Drawing;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Fields;
using MigraDoc.DocumentObjectModel.Visitors;
using MigraDoc.DocumentObjectModel.Tables;

namespace MigraDoc.Rendering
{
    /// <summary>
    ///   Renders a table to an XGraphics object.
    /// </summary>
    internal class TableRenderer : Renderer
    {
        internal TableRenderer(XGraphics gfx, Table documentObject, FieldInfos fieldInfos)
            : base(gfx, documentObject, fieldInfos)
        {
            _table = documentObject;
        }

        internal TableRenderer(XGraphics gfx, RenderInfo renderInfo, FieldInfos fieldInfos)
            : base(gfx, renderInfo, fieldInfos)
        {
            _table = (Table)_renderInfo.DocumentObject;
        }

        internal override LayoutInfo InitialLayoutInfo
        {
            get
            {
                LayoutInfo layoutInfo = new LayoutInfo();
                layoutInfo.KeepTogether = _table.KeepTogether;
                layoutInfo.KeepWithNext = false;
                layoutInfo.MarginBottom = 0;
                layoutInfo.MarginLeft = 0;
                layoutInfo.MarginTop = 0;
                layoutInfo.MarginRight = 0;
                return layoutInfo;
            }
        }

        private void InitRendering()
        {
            TableFormatInfo formatInfo = (TableFormatInfo)_renderInfo.FormatInfo;
            _bottomBorderMap = formatInfo.BottomBorderMap;
            _connectedRowsMap = formatInfo.ConnectedRowsMap;
            _formattedCells = formatInfo.FormattedCells;

            _currRow = formatInfo.StartRow;
            _startRow = formatInfo.StartRow;
            _endRow = formatInfo.EndRow;

            _mergedCells = formatInfo.MergedCells;
            _lastHeaderRow = formatInfo.LastHeaderRow;
            _startX = _renderInfo.LayoutInfo.ContentArea.X;
            _startY = _renderInfo.LayoutInfo.ContentArea.Y;
        }

        private void RenderHeaderRows()
        {
            if (_lastHeaderRow < 0)
                return;

            foreach (Cell cell in _mergedCells)
            {
                if (cell.Row.Index <= _lastHeaderRow)
                    RenderCell(cell);
            }
        }

        private void RenderCell(Cell cell)
        {
            Rectangle innerRect = GetInnerRect(CalcStartingHeight(), cell);
            RenderShading(cell, innerRect);
            RenderContent(cell, innerRect);
            RenderBorders(cell, innerRect);
        }

        private void EqualizeRoundedCornerBorders(Cell cell)
        {
            // If any of a corner relevant border is set, we want to copy its values to the second corner relevant border, 
            // to ensure the innerWidth of the cell is the same, regardless of which border is used.
            // If set, we use the vertical borders as source for the values, otherwise we use the horizontal borders.
            RoundedCorner roundedCorner = cell.RoundedCorner;

            if (roundedCorner == RoundedCorner.None)
                return;

            BorderType primaryBorderType = BorderType.Top, secondaryBorderType = BorderType.Top;

            if (roundedCorner == RoundedCorner.TopLeft || roundedCorner == RoundedCorner.BottomLeft)
                primaryBorderType = BorderType.Left;
            if (roundedCorner == RoundedCorner.TopRight || roundedCorner == RoundedCorner.BottomRight)
                primaryBorderType = BorderType.Right;

            if (roundedCorner == RoundedCorner.TopLeft || roundedCorner == RoundedCorner.TopRight)
                secondaryBorderType = BorderType.Top;
            if (roundedCorner == RoundedCorner.BottomLeft || roundedCorner == RoundedCorner.BottomRight)
                secondaryBorderType = BorderType.Bottom;

            // If both borders don't exist, there's nothing to do and we should not create one by accessing it.
            if (!cell.Borders.HasBorder(primaryBorderType) && !cell.Borders.HasBorder(secondaryBorderType))
                return;

            // Get the borders. By using GV.ReadWrite we create the border, if not existing.
            Border primaryBorder = (Border)cell.Borders.GetValue(primaryBorderType.ToString(), GV.ReadWrite);
            Border secondaryBorder = (Border)cell.Borders.GetValue(secondaryBorderType.ToString(), GV.ReadWrite);

            Border source = primaryBorder.Visible ? primaryBorder
                : secondaryBorder.Visible ? secondaryBorder : null;
            Border target = primaryBorder.Visible ? secondaryBorder
                : secondaryBorder.Visible ? primaryBorder : null;

            if (source == null || target == null)
                return;

            target.Visible = source.Visible;
            target.Width = source.Width;
            target.Style = source.Style;
            target.Color = source.Color;
        }

        private void RenderShading(Cell cell, Rectangle innerRect)
        {
            ShadingRenderer shadeRenderer = new ShadingRenderer(_gfx, cell.Shading);
            shadeRenderer.Render(innerRect.X, innerRect.Y, innerRect.Width, innerRect.Height, cell.RoundedCorner);
        }

        private void RenderBorders(Cell cell, Rectangle innerRect)
        {
            XUnit leftPos = innerRect.X;
            XUnit rightPos = leftPos + innerRect.Width;
            XUnit topPos = innerRect.Y;
            XUnit bottomPos = innerRect.Y + innerRect.Height;
            Borders mergedBorders = _mergedCells.GetEffectiveBorders(cell);

            BordersRenderer bordersRenderer = new BordersRenderer(mergedBorders, _gfx);
            XUnit bottomWidth = bordersRenderer.GetWidth(BorderType.Bottom);
            XUnit leftWidth = bordersRenderer.GetWidth(BorderType.Left);
            XUnit topWidth = bordersRenderer.GetWidth(BorderType.Top);
            XUnit rightWidth = bordersRenderer.GetWidth(BorderType.Right);

            if (cell.RoundedCorner == RoundedCorner.TopLeft)
                bordersRenderer.RenderRounded(cell.RoundedCorner, innerRect.X, innerRect.Y, innerRect.Width + rightWidth, innerRect.Height + bottomWidth);
            else if (cell.RoundedCorner == RoundedCorner.TopRight)
                bordersRenderer.RenderRounded(cell.RoundedCorner, innerRect.X - leftWidth, innerRect.Y, innerRect.Width + leftWidth, innerRect.Height + bottomWidth);
            else if (cell.RoundedCorner == RoundedCorner.BottomLeft)
                bordersRenderer.RenderRounded(cell.RoundedCorner, innerRect.X, innerRect.Y - topWidth, innerRect.Width + rightWidth, innerRect.Height + topWidth);
            else if (cell.RoundedCorner == RoundedCorner.BottomRight)
                bordersRenderer.RenderRounded(cell.RoundedCorner, innerRect.X - leftWidth, innerRect.Y - topWidth, innerRect.Width + leftWidth, innerRect.Height + topWidth);

            // Render horizontal and vertical borders only if touching no rounded corner.
            if (cell.RoundedCorner != RoundedCorner.TopRight && cell.RoundedCorner != RoundedCorner.BottomRight)
                bordersRenderer.RenderVertically(BorderType.Right, rightPos, topPos, bottomPos + bottomWidth - topPos);

            if (cell.RoundedCorner != RoundedCorner.TopLeft && cell.RoundedCorner != RoundedCorner.BottomLeft)
                bordersRenderer.RenderVertically(BorderType.Left, leftPos - leftWidth, topPos, bottomPos + bottomWidth - topPos);

            if (cell.RoundedCorner != RoundedCorner.BottomLeft && cell.RoundedCorner != RoundedCorner.BottomRight)
                bordersRenderer.RenderHorizontally(BorderType.Bottom, leftPos - leftWidth, bottomPos, rightPos + rightWidth + leftWidth - leftPos);

            if (cell.RoundedCorner != RoundedCorner.TopLeft && cell.RoundedCorner != RoundedCorner.TopRight)
                bordersRenderer.RenderHorizontally(BorderType.Top, leftPos - leftWidth, topPos - topWidth, rightPos + rightWidth + leftWidth - leftPos);

            RenderDiagonalBorders(mergedBorders, innerRect);
        }

        private void RenderDiagonalBorders(Borders mergedBorders, Rectangle innerRect)
        {
            BordersRenderer bordersRenderer = new BordersRenderer(mergedBorders, _gfx);
            bordersRenderer.RenderDiagonally(BorderType.DiagonalDown, innerRect.X, innerRect.Y, innerRect.Width, innerRect.Height);
            bordersRenderer.RenderDiagonally(BorderType.DiagonalUp, innerRect.X, innerRect.Y, innerRect.Width, innerRect.Height);
        }

        private void RenderContent(Cell cell, Rectangle innerRect)
        {
            FormattedCell formattedCell = _formattedCells[cell];
            RenderInfo[] renderInfos = formattedCell.GetRenderInfos();

            if (renderInfos == null)
                return;

            VerticalAlignment verticalAlignment = cell.VerticalAlignment;
            XUnit contentHeight = formattedCell.ContentHeight;
            XUnit innerHeight = innerRect.Height;
            XUnit targetX = innerRect.X + cell.Column.LeftPadding;

            XUnit targetY;
            if (verticalAlignment == VerticalAlignment.Bottom)
            {
                targetY = innerRect.Y + innerRect.Height;
                targetY -= cell.Row.BottomPadding;
                targetY -= contentHeight;
            }
            else if (verticalAlignment == VerticalAlignment.Center)
            {
                targetY = innerRect.Y + cell.Row.TopPadding;
                targetY += innerRect.Y + innerRect.Height - cell.Row.BottomPadding;
                targetY -= contentHeight;
                targetY /= 2;
            }
            else
                targetY = innerRect.Y + cell.Row.TopPadding;

            RenderByInfos(targetX, targetY, renderInfos);
        }

        private Rectangle GetInnerRect(XUnit startingHeight, Cell cell)
        {
            BordersRenderer bordersRenderer = new BordersRenderer(_mergedCells.GetEffectiveBorders(cell), _gfx);
            FormattedCell formattedCell = _formattedCells[cell];
            XUnit width = formattedCell.InnerWidth;

            XUnit y = _startY;
            if (cell.Row.Index > _lastHeaderRow)
                y += startingHeight;
            else
                y += CalcMaxTopBorderWidth(0);

#if true
            // !!!new 18-03-09 Attempt to fix an exception. begin
            XUnit upperBorderPos;
            if (!_bottomBorderMap.TryGetValue(cell.Row.Index, out upperBorderPos))
            {
                //GetType();
            }
            // !!!new 18-03-09 Attempt to fix an exception. end
#else
            XUnit upperBorderPos = _bottomBorderMap[cell.Row.Index];
#endif

            y += upperBorderPos;
            if (cell.Row.Index > _lastHeaderRow)
                y -= _bottomBorderMap[_startRow];

#if true
            // !!!new 18-03-09 Attempt to fix an exception. begin
            XUnit lowerBorderPos;
            if (!_bottomBorderMap.TryGetValue(cell.Row.Index + cell.MergeDown + 1, out lowerBorderPos))
            {
                //GetType();
            }
            // !!!new 18-03-09 Attempt to fix an exception. end
#else
            XUnit lowerBorderPos = _bottomBorderMap[cell.Row.Index + cell.MergeDown + 1];
#endif

            XUnit height = lowerBorderPos - upperBorderPos;
            height -= bordersRenderer.GetWidth(BorderType.Bottom);

            XUnit x = _startX;
            for (int clmIdx = 0; clmIdx < cell.Column.Index; ++clmIdx)
            {
                x += _table.Columns[clmIdx].Width;
            }
            x += LeftBorderOffset;

            return new Rectangle(x, y, width, height);
        }

        internal override void Render()
        {
            InitRendering();
            RenderHeaderRows();
            if (_startRow < _table.Rows.Count)
            {
                Cell cell = _table[_startRow, 0];

                int cellIdx = _mergedCells.BinarySearch(_table[_startRow, 0], new CellComparer());
                while (cellIdx < _mergedCells.Count)
                {
                    cell = _mergedCells[cellIdx];
                    if (cell.Row.Index > _endRow)
                        break;

                    RenderCell(cell);
                    ++cellIdx;
                }
            }
        }

        private void InitFormat(Area area, FormatInfo previousFormatInfo)
        {
            TableFormatInfo prevTableFormatInfo = (TableFormatInfo)previousFormatInfo;
            TableRenderInfo tblRenderInfo = new TableRenderInfo();
            tblRenderInfo.DocumentObject = _table;

            // Equalize the two borders, that are used to determine a rounded corner's border.
            // This way the innerWidth of the cell, which is got by the saved _formattedCells, is the same regardless of which corner relevant border is set.
            foreach (Row row in _table.Rows)
                foreach (Cell cell in row.Cells)
                    EqualizeRoundedCornerBorders(cell);

            _renderInfo = tblRenderInfo;

            if (prevTableFormatInfo != null)
            {
                _mergedCells = prevTableFormatInfo.MergedCells;
                _formattedCells = prevTableFormatInfo.FormattedCells;
                _bottomBorderMap = prevTableFormatInfo.BottomBorderMap;
                _lastHeaderRow = prevTableFormatInfo.LastHeaderRow;
                _connectedRowsMap = prevTableFormatInfo.ConnectedRowsMap;
                _startRow = prevTableFormatInfo.EndRow + 1;
            }
            else
            {
                _mergedCells = new MergedCellList(_table);
                FormatCells();
                CalcLastHeaderRow();
                CreateConnectedRows();
                CreateBottomBorderMap();
                if (_doHorizontalBreak)
                {
                    CalcLastHeaderColumn();
                    CreateConnectedColumns();
                }
                _startRow = _lastHeaderRow + 1;
            }
            ((TableFormatInfo)tblRenderInfo.FormatInfo).MergedCells = _mergedCells;
            ((TableFormatInfo)tblRenderInfo.FormatInfo).FormattedCells = _formattedCells;
            ((TableFormatInfo)tblRenderInfo.FormatInfo).BottomBorderMap = _bottomBorderMap;
            ((TableFormatInfo)tblRenderInfo.FormatInfo).ConnectedRowsMap = _connectedRowsMap;
            ((TableFormatInfo)tblRenderInfo.FormatInfo).LastHeaderRow = _lastHeaderRow;
        }

        private void FormatCells()
        {
            _formattedCells = new Dictionary<Cell, FormattedCell>(); //new Sorted_List(new CellComparer());
            foreach (Cell cell in _mergedCells)
            {
                FormattedCell formattedCell = new FormattedCell(cell, _documentRenderer, _mergedCells.GetEffectiveBorders(cell),
                                                                _fieldInfos, 0, 0);
                formattedCell.Format(_gfx);
                _formattedCells.Add(cell, formattedCell);
            }
        }

        /// <summary>
        ///   Formats (measures) the table.
        /// </summary>
        /// <param name="area"> The area on which to fit the table. </param>
        /// <param name="previousFormatInfo"> </param>
        internal override void Format(Area area, FormatInfo previousFormatInfo)
        {
            DocumentElements elements = DocumentRelations.GetParent(_table) as DocumentElements;
            if (elements != null)
            {
                Section section = DocumentRelations.GetParent(elements) as Section;
                if (section != null)
                    _doHorizontalBreak = section.PageSetup.HorizontalPageBreak;
            }

            _renderInfo = new TableRenderInfo();
            InitFormat(area, previousFormatInfo);

            // Don't take any Rows higher then MaxElementHeight
            XUnit topHeight = CalcStartingHeight();
            XUnit probeHeight = topHeight;
            XUnit offset;
            if (_startRow > _lastHeaderRow + 1 &&
                _startRow < _table.Rows.Count)
                offset = _bottomBorderMap[_startRow] - topHeight;
            else
                offset = -CalcMaxTopBorderWidth(0);

            int probeRow = _startRow;
            XUnit currentHeight = 0;
            XUnit startingHeight = 0;
            bool isEmpty = false;

            while (probeRow < _table.Rows.Count)
            {
                bool firstProbe = probeRow == _startRow;
                probeRow = _connectedRowsMap[probeRow];
                // Don't take any Rows higher then MaxElementHeight
                probeHeight = _bottomBorderMap[probeRow + 1] - offset;
                // First test whether MaxElementHeight has been set.
                if (MaxElementHeight > 0 && firstProbe && probeHeight > MaxElementHeight - Tolerance)
                    probeHeight = MaxElementHeight - Tolerance;
                //if (firstProbe && probeHeight > MaxElementHeight - Tolerance)
                //    probeHeight = MaxElementHeight - Tolerance;

                //The height for the first new row(s) + headerrows:
                if (startingHeight == 0)
                {
                    if (probeHeight > area.Height)
                    {
                        isEmpty = true;
                        break;
                    }
                    startingHeight = probeHeight;
                }

                if (probeHeight > area.Height)
                    break;

                else
                {
                    _currRow = probeRow;
                    currentHeight = probeHeight;
                    ++probeRow;
                }
            }
            if (!isEmpty)
            {
                TableFormatInfo formatInfo = (TableFormatInfo)_renderInfo.FormatInfo;
                formatInfo.StartRow = _startRow;
                formatInfo._isEnding = _currRow >= _table.Rows.Count - 1;
                formatInfo.EndRow = _currRow;

                UpdateThisPagesBookmarks(_startRow, _currRow);
            }
            FinishLayoutInfo(area, currentHeight, startingHeight);
        }

        /// <summary>
        /// Updates the bookmarks in the given rows.
        /// Otherwise each BookmarkField will refer to the first page of the table, because initially they are set before the table gets splitted over the pages.
        /// </summary>
        private void UpdateThisPagesBookmarks(int startRow, int endRow)
        {
            if (_table.Rows.Count == 0)
                return;

            for (var r = startRow; r <= endRow; r++)
            {
                var row = _table.Rows[r];

                foreach (var bookmark in row.GetElementsRecursively<BookmarkField>())
                    _fieldInfos.AddBookmark(bookmark.Name);
            }
        }

        private void FinishLayoutInfo(Area area, XUnit currentHeight, XUnit startingHeight)
        {
            LayoutInfo layoutInfo = _renderInfo.LayoutInfo;
            layoutInfo.StartingHeight = startingHeight;
            //REM: Trailing height would have to be calculated in case tables had a keep with next property.
            layoutInfo.TrailingHeight = 0;
            if (_currRow >= 0)
            {
                layoutInfo.ContentArea = new Rectangle(area.X, area.Y, 0, currentHeight);
                XUnit width = LeftBorderOffset;
                foreach (Column clm in _table.Columns)
                {
                    width += clm.Width;
                }
                layoutInfo.ContentArea.Width = width;
            }
            layoutInfo.MinWidth = layoutInfo.ContentArea.Width;

            if (!_table.Rows._leftIndent.IsNull)
                layoutInfo.Left = _table.Rows.LeftIndent.Point;

            else if (_table.Rows.Alignment == RowAlignment.Left)
            {
                XUnit leftOffset = LeftBorderOffset;
                leftOffset += _table.Columns[0].LeftPadding;
                layoutInfo.Left = -leftOffset;
            }

            switch (_table.Rows.Alignment)
            {
                case RowAlignment.Left:
                    layoutInfo.HorizontalAlignment = ElementAlignment.Near;
                    break;

                case RowAlignment.Right:
                    layoutInfo.HorizontalAlignment = ElementAlignment.Far;
                    break;

                case RowAlignment.Center:
                    layoutInfo.HorizontalAlignment = ElementAlignment.Center;
                    break;
            }
        }

        private XUnit LeftBorderOffset
        {
            get
            {
                if (_leftBorderOffset < 0)
                {
                    if (_table.Rows.Count > 0 && _table.Columns.Count > 0)
                    {
                        Borders borders = _mergedCells.GetEffectiveBorders(_table[0, 0]);
                        BordersRenderer bordersRenderer = new BordersRenderer(borders, _gfx);
                        _leftBorderOffset = bordersRenderer.GetWidth(BorderType.Left);
                    }
                    else
                        _leftBorderOffset = 0;
                }
                return _leftBorderOffset;
            }
        }

        private XUnit _leftBorderOffset = -1;

        /// <summary>
        ///   Calcs either the height of the header rows or the height of the uppermost top border.
        /// </summary>
        /// <returns> </returns>
        private XUnit CalcStartingHeight()
        {
            XUnit height = 0;
            if (_lastHeaderRow >= 0)
            {
                height = _bottomBorderMap[_lastHeaderRow + 1];
                height += CalcMaxTopBorderWidth(0);
            }
            else
            {
                if (_table.Rows.Count > _startRow)
                    height = CalcMaxTopBorderWidth(_startRow);
            }

            return height;
        }


        private void CalcLastHeaderColumn()
        {
            _lastHeaderColumn = -1;
            foreach (Column clm in _table.Columns)
            {
                if (clm.HeadingFormat)
                    _lastHeaderColumn = clm.Index;
                else break;
            }
            if (_lastHeaderColumn >= 0)
                _lastHeaderRow = CalcLastConnectedColumn(_lastHeaderColumn);

            // Ignore heading format if all the table is heading:
            if (_lastHeaderRow == _table.Rows.Count - 1)
                _lastHeaderRow = -1;
        }

        private void CalcLastHeaderRow()
        {
            _lastHeaderRow = -1;
            foreach (Row row in _table.Rows)
            {
                if (row.HeadingFormat)
                    _lastHeaderRow = row.Index;
                else break;
            }
            if (_lastHeaderRow >= 0)
                _lastHeaderRow = CalcLastConnectedRow(_lastHeaderRow);

            // Ignore heading format if all the table is heading:
            if (_lastHeaderRow == _table.Rows.Count - 1)
                _lastHeaderRow = -1;
        }

        private void CreateConnectedRows()
        {
            _connectedRowsMap = new Dictionary<int, int>(); //new Sorted_List();
            foreach (Cell cell in _mergedCells)
            {
                if (!_connectedRowsMap.ContainsKey(cell.Row.Index))
                {
                    int lastConnectedRow = CalcLastConnectedRow(cell.Row.Index);
                    _connectedRowsMap[cell.Row.Index] = lastConnectedRow;
                }
            }
        }

        private void CreateConnectedColumns()
        {
            _connectedColumnsMap = new Dictionary<int, int>(); //new SortedList();
            foreach (Cell cell in _mergedCells)
            {
                if (!_connectedColumnsMap.ContainsKey(cell.Column.Index))
                {
                    int lastConnectedColumn = CalcLastConnectedColumn(cell.Column.Index);
                    _connectedColumnsMap[cell.Column.Index] = lastConnectedColumn;
                }
            }
        }

        private void CreateBottomBorderMap()
        {
            _bottomBorderMap = new Dictionary<int, XUnit>(); //new SortedList();
            _bottomBorderMap.Add(0, XUnit.FromPoint(0));
            while (!_bottomBorderMap.ContainsKey(_table.Rows.Count))
            {
                CreateNextBottomBorderPosition();
            }
        }

        /// <summary>
        ///   Calculates the top border width for the first row that is rendered or formatted.
        /// </summary>
        /// <param name="row"> The row index. </param>
        private XUnit CalcMaxTopBorderWidth(int row)
        {
            XUnit maxWidth = 0;
            if (_table.Rows.Count > row)
            {
                int cellIdx = _mergedCells.BinarySearch(_table[row, 0], new CellComparer());
                Cell rowCell = _mergedCells[cellIdx];
                while (cellIdx < _mergedCells.Count)
                {
                    rowCell = _mergedCells[cellIdx];
                    if (rowCell.Row.Index > row)
                        break;

                    if (rowCell._borders != null && !rowCell._borders.IsNull())
                    {
                        BordersRenderer bordersRenderer = new BordersRenderer(rowCell.Borders, _gfx);
                        XUnit width = bordersRenderer.GetWidth(BorderType.Top);
                        if (width > maxWidth)
                            maxWidth = width;
                    }
                    ++cellIdx;
                }
            }
            return maxWidth;
        }

        /// <summary>
        ///   Creates the next bottom border position.
        /// </summary>
        private void CreateNextBottomBorderPosition()
        {
            //int lastIdx = _bottomBorderMap.Count - 1;
            // SortedList version:
            //int lastBorderRow = (int)bottomBorderMap.GetKey(lastIdx);
            //XUnit lastPos = (XUnit)bottomBorderMap.GetByIndex(lastIdx);
            int lastBorderRow = 0;
            foreach (int key in _bottomBorderMap.Keys)
            {
                if (key > lastBorderRow)
                    lastBorderRow = key;
            }
            XUnit lastPos = _bottomBorderMap[lastBorderRow];

            Cell minMergedCell = GetMinMergedCell(lastBorderRow);
            FormattedCell minMergedFormattedCell = _formattedCells[minMergedCell];
            XUnit maxBottomBorderPosition = lastPos + minMergedFormattedCell.InnerHeight;
            maxBottomBorderPosition += CalcBottomBorderWidth(minMergedCell);

            foreach (Cell cell in _mergedCells)
            {
                if (cell.Row.Index > minMergedCell.Row.Index + minMergedCell.MergeDown)
                    break;

                if (cell.Row.Index + cell.MergeDown == minMergedCell.Row.Index + minMergedCell.MergeDown)
                {
                    FormattedCell formattedCell = _formattedCells[cell];
                    // !!!new 18-03-09 Attempt to fix an exception. begin
                    // if (cell.Row.Index < _bottomBorderMap.Count)
                    {
                        // !!!new 18-03-09 Attempt to fix an exception. end
#if true
                        // !!!new 18-03-09 Attempt to fix an exception. begin
                        XUnit topBorderPos = maxBottomBorderPosition;
                        if (!_bottomBorderMap.TryGetValue(cell.Row.Index, out topBorderPos))
                        {
                            //GetType();
                        }
                        // !!!new 18-03-09 Attempt to fix an exception. end
#else
                        XUnit topBorderPos = _bottomBorderMap[cell.Row.Index];
#endif
                        XUnit bottomBorderPos = topBorderPos + formattedCell.InnerHeight;
                        bottomBorderPos += CalcBottomBorderWidth(cell);
                        if (bottomBorderPos > maxBottomBorderPosition)
                            maxBottomBorderPosition = bottomBorderPos;
                        // !!!new 18-03-09 Attempt to fix an exception. begin
                    }
                    // !!!new 18-03-09 Attempt to fix an exception. end
                }
            }
            _bottomBorderMap.Add(minMergedCell.Row.Index + minMergedCell.MergeDown + 1, maxBottomBorderPosition);
        }

        /// <summary>
        ///   Calculates bottom border width of a cell.
        /// </summary>
        /// <param name="cell"> The cell the bottom border of the row that is probed. </param>
        /// <returns> The calculated border width. </returns>
        private XUnit CalcBottomBorderWidth(Cell cell)
        {
            Borders borders = _mergedCells.GetEffectiveBorders(cell);
            if (borders != null)
            {
                BordersRenderer bordersRenderer = new BordersRenderer(borders, _gfx);
                return bordersRenderer.GetWidth(BorderType.Bottom);
            }
            return 0;
        }

        /// <summary>
        /// Gets the first cell that ends in the given row or as close as possible.
        /// </summary>
        /// <param name="row">The row to probe.</param>
        /// <returns>The first cell with minimal vertical merge.</returns>
        private Cell GetMinMergedCell(int row)
        {
#if true
            //!!!new 18-03-10 begin
            // Also look at rows above "row", but only consider cells that end at "row" or as close as possible.
            int minMerge = _table.Rows.Count;
            Cell minCell = null;
            foreach (Cell cell in _mergedCells)
            {
                if (cell.Row.Index <= row && cell.Row.Index + cell.MergeDown >= row)
                {
                    if (cell.Row.Index == row && cell.MergeDown == 0)
                    {
                        // Perfect match: non-merged cell in the desired row.
                        minCell = cell;
                        break;
                    }
                    else if (cell.Row.Index + cell.MergeDown - row < minMerge)
                    {
                        minMerge = cell.Row.Index + cell.MergeDown - row;
                        minCell = cell;
                    }
                }
                else if (cell.Row.Index > row)
                    break;
            }
            //!!!new 18-03-10 end
#else
            int minMerge = _table.Rows.Count;
            Cell minCell = null;
            foreach (Cell cell in _mergedCells)
            {
                if (cell.Row.Index == row)
                {
                    if (cell.MergeDown == 0)
                    {
                        minCell = cell;
                        break;
                    }
                    else if (cell.MergeDown < minMerge)
                    {
                        minMerge = cell.MergeDown;
                        minCell = cell;
                    }
                }
                else if (cell.Row.Index > row)
                    break;
            }
#endif
            return minCell;
        }

        /// <summary>
        ///   Calculates the last row that is connected with the given row.
        /// </summary>
        /// <param name="row"> The row that is probed for downward connection. </param>
        /// <returns> The last row that is connected with the given row. </returns>
        private int CalcLastConnectedRow(int row)
        {
            int lastConnectedRow = row;
            foreach (Cell cell in _mergedCells)
            {
                if (cell.Row.Index <= lastConnectedRow)
                {
                    int downConnection = Math.Max(cell.Row.KeepWith, cell.MergeDown);
                    if (lastConnectedRow < cell.Row.Index + downConnection)
                        lastConnectedRow = cell.Row.Index + downConnection;
                }
            }
            return lastConnectedRow;
        }

        /// <summary>
        ///   Calculates the last column that is connected with the specified column.
        /// </summary>
        /// <param name="column"> The column that is probed for downward connection. </param>
        /// <returns> The last column that is connected with the given column. </returns>
        private int CalcLastConnectedColumn(int column)
        {
            int lastConnectedColumn = column;
            foreach (Cell cell in _mergedCells)
            {
                if (cell.Column.Index <= lastConnectedColumn)
                {
                    int rightConnection = Math.Max(cell.Column.KeepWith, cell.MergeRight);
                    if (lastConnectedColumn < cell.Column.Index + rightConnection)
                        lastConnectedColumn = cell.Column.Index + rightConnection;
                }
            }
            return lastConnectedColumn;
        }

        private readonly Table _table;
        private MergedCellList _mergedCells;
        private Dictionary<Cell, FormattedCell> _formattedCells; //SortedList formattedCells;
        private Dictionary<int, XUnit> _bottomBorderMap; //SortedList bottomBorderMap;
        private Dictionary<int, int> _connectedRowsMap; //SortedList connectedRowsMap;
        private Dictionary<int, int> _connectedColumnsMap; //SortedList connectedColumnsMap;

        private int _lastHeaderRow;
        private int _lastHeaderColumn;
        private int _startRow;
        private int _currRow;
        private int _endRow = -1;

        private bool _doHorizontalBreak;
        private XUnit _startX;
        private XUnit _startY;
    }
}
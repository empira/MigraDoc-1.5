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

using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Visitors;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.DocumentObjectModel.Internals;

namespace MigraDoc.RtfRendering
{
    /// <summary>
    ///   Render the format information of a cell.
    /// </summary>
    internal class CellFormatRenderer : RendererBase
    {
        internal CellFormatRenderer(DocumentObject domObj, RtfDocumentRenderer docRenderer)
            : base(domObj, docRenderer)
        {
            _cell = domObj as Cell;
        }

        /// <summary>
        ///   Renders the cell's shading, borders and so on (used by the RowRenderer).
        /// </summary>
        internal override void Render()
        {
            _useEffectiveValue = true;
            _coveringCell = _cellList.GetCoveringCell(_cell);
            Borders borders = _cellList.GetEffectiveBorders(_coveringCell);
            if (_cell.Column.Index != _coveringCell.Column.Index)
                return;

            if (borders != null)
            {
                BordersRenderer brdrsRenderer = new BordersRenderer(borders, _docRenderer);
                brdrsRenderer.LeaveAwayLeft = _cell.Column.Index != _coveringCell.Column.Index;
                brdrsRenderer.LeaveAwayTop = _cell.Row.Index != _coveringCell.Row.Index;
                brdrsRenderer.LeaveAwayBottom = _cell.Row.Index != _coveringCell.Row.Index + _coveringCell.MergeDown;
                brdrsRenderer.LeaveAwayRight = false;
                brdrsRenderer.ParentCell = _cell;
                brdrsRenderer.Render();
            }
            if (_cell == _coveringCell)
            {
                RenderLeftRightPadding();
                Translate("VerticalAlignment", "clvertal");
            }
            object obj = _coveringCell.GetValue("Shading", GV.GetNull);
            if (obj != null)
                new ShadingRenderer((DocumentObject)obj, _docRenderer).Render();

            //Note that vertical and horizontal merging are not symmetrical.
            //Horizontally merged cells are simply rendered as bigger cells.
            if (_cell.Row.Index == _coveringCell.Row.Index && _coveringCell.MergeDown > 0)
                _rtfWriter.WriteControl("clvmgf");

            if (_cell.Row.Index > _coveringCell.Row.Index)
                _rtfWriter.WriteControl("clvmrg");

            _rtfWriter.WriteControl("cellx", GetRightCellBoundary());
        }

        private void RenderLeftRightPadding()
        {
            string clPadCtrl = "clpad";
            string cellPadUnit = "clpadf";
            object cellPdgVal = _cell.Column.GetValue("LeftPadding", GV.GetNull);
            if (cellPdgVal == null)
                cellPdgVal = Unit.FromCentimeter(0.12);

            //Top and left padding are mixed up in word:
            _rtfWriter.WriteControl(clPadCtrl + "t", ToRtfUnit((Unit)cellPdgVal, RtfUnit.Twips));
            //Tells the RTF reader to take it as twips:
            _rtfWriter.WriteControl(cellPadUnit + "t", 3);
            cellPdgVal = _cell.Column.GetValue("RightPadding", GV.GetNull);
            if (cellPdgVal == null)
                cellPdgVal = Unit.FromCentimeter(0.12);

            _rtfWriter.WriteControl(clPadCtrl + "r", ToRtfUnit((Unit)cellPdgVal, RtfUnit.Twips));
            //Tells the RTF reader to take it as Twips:
            _rtfWriter.WriteControl(cellPadUnit + "r", 3);
        }

        /// <summary>
        ///   Gets the right boundary of the cell which is currently rendered.
        /// </summary>
        private int GetRightCellBoundary()
        {
            int rightClmIdx = _coveringCell.Column.Index + _coveringCell.MergeRight;
            double width = RowsRenderer.CalculateLeftIndent(_cell.Table.Rows).Point;
            for (int idx = 0; idx <= rightClmIdx; ++idx)
            {
                object obj = _cell.Table.Columns[idx].GetValue("Width", GV.GetNull);
                if (obj != null)
                    width += ((Unit)obj).Point;
                else
                    width += ((Unit)"2.5cm").Point;
            }
            return ToRtfUnit(new Unit((double)width), RtfUnit.Twips);
        }

        /// <summary>
        ///   Sets the MergedCellList received from the DOM table. This property is set by the RowRenderer.
        /// </summary>
        internal MergedCellList CellList
        {
            set { _cellList = value; }
        }

        private MergedCellList _cellList = null;
        private Cell _coveringCell;
        private readonly Cell _cell;
    }
}
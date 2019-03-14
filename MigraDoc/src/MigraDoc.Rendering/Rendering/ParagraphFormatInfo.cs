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

using System.Collections.Generic;
using MigraDoc.DocumentObjectModel.Shapes;
using PdfSharp.Drawing;
using MigraDoc.DocumentObjectModel;

namespace MigraDoc.Rendering
{
    /// <summary>
    /// Vertical measurements of a paragraph line.
    /// </summary>
    internal struct VerticalLineInfo
    {
        internal VerticalLineInfo(XUnit height, XUnit descent, XUnit inherentlineSpace)
        {
            Height = height;
            Descent = descent;
            InherentlineSpace = inherentlineSpace;
        }

        public XUnit Height;

        public XUnit Descent;

        public XUnit InherentlineSpace;
    }

    /// <summary>
    /// Line info object used by the paragraph format info.
    /// </summary>
    internal struct LineInfo
    {
        public ParagraphIterator StartIter;
        public ParagraphIterator EndIter;
        public XUnit WordsWidth;
        public XUnit LineWidth;
        public int BlankCount;
        public VerticalLineInfo Vertical;
        public List<TabOffset> TabOffsets;
        public bool ReMeasureLine;
        public DocumentObject LastTab;
    }

    /// <summary>
    /// Formatting information for a paragraph.
    /// </summary>
    internal sealed class ParagraphFormatInfo : FormatInfo
    {
        internal ParagraphFormatInfo()
        { }

        internal LineInfo GetLineInfo(int lineIdx)
        {
            return _lineInfos[lineIdx];
        }

        internal LineInfo GetLastLineInfo()
        {
            return _lineInfos[LineCount - 1];
        }

        internal LineInfo GetFirstLineInfo()
        {
            return _lineInfos[0];
        }

        internal void AddLineInfo(LineInfo lineInfo)
        {
            _lineInfos.Add(lineInfo);
        }

        internal int LineCount
        {
            get { return _lineInfos.Count; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mergeInfo"></param>
        /// <returns></returns>
        internal void Append(FormatInfo mergeInfo)
        {
            ParagraphFormatInfo formatInfo = (ParagraphFormatInfo)mergeInfo;
            _lineInfos.AddRange(formatInfo._lineInfos);
        }

        /// <summary>
        /// Indicates whether the paragraph is ending.
        /// </summary>
        /// <returns>True if the paragraph is ending.</returns>
        internal override bool IsEnding
        {
            get { return _isEnding; }
        }
        internal bool _isEnding;

        /// <summary>
        /// Indicates whether the paragraph is starting.
        /// </summary>
        /// <returns>True if the paragraph is starting.</returns>
        internal override bool IsStarting
        {
            get { return _isStarting; }
        }
        internal bool _isStarting;

        internal override bool IsComplete
        {
            get { return _isStarting && _isEnding; }
        }

        internal override bool IsEmpty
        {
            get { return _lineInfos.Count == 0; }
        }

        internal override bool StartingIsComplete
        {
            get
            {
                if (_widowControl)
                    return (IsComplete || (_isStarting && _lineInfos.Count >= 2));
                return _isStarting;
            }
        }

        internal bool _widowControl;

        internal override bool EndingIsComplete
        {
            get
            {
                if (_widowControl)
                    return (IsComplete || (_isEnding && _lineInfos.Count >= 2));
                return _isEnding;
            }
        }

        internal void RemoveEnding()
        {
            if (!IsEmpty)
            {
                if (_widowControl && _isEnding && LineCount >= 2)
                    _lineInfos.RemoveAt(LineCount - 2);
                if (LineCount > 0)
                    _lineInfos.RemoveAt(LineCount - 1);

                _isEnding = false;
            }
        }

        internal string ListSymbol;
        internal XFont ListFont;
        internal Dictionary<Image, RenderInfo> ImageRenderInfos;
        readonly List<LineInfo> _lineInfos = new List<LineInfo>();
    }
}

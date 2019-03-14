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

using System;
using System.Collections.Generic;
using System.Collections;
using MigraDoc.DocumentObjectModel.Internals;

#pragma warning disable 1591

namespace MigraDoc.DocumentObjectModel
{
    /// <summary>
    /// A Borders collection represents the eight border objects used for paragraphs, tables etc.
    /// </summary>
    public class Borders : DocumentObject, IEnumerable
    {
        /// <summary>
        /// Initializes a new instance of the Borders class.
        /// </summary>
        public Borders()
        { }

        /// <summary>
        /// Initializes a new instance of the Borders class with the specified parent.
        /// </summary>
        internal Borders(DocumentObject parent) : base(parent) { }

        /// <summary>
        /// Determines whether a particular border exists.
        /// </summary>
        public bool HasBorder(BorderType type)
        {
            if (!Enum.IsDefined(typeof(BorderType), type))
                throw new /*InvalidEnum*/ArgumentException(DomSR.InvalidEnumValue(type), "type");

            return GetBorderReadOnly(type) != null;
        }

        internal Border GetBorderReadOnly(BorderType type)
        {
            switch (type)
            {
                case BorderType.Bottom:
                    return _bottom;
                case BorderType.DiagonalDown:
                    return _diagonalDown;
                case BorderType.DiagonalUp:
                    return _diagonalUp;
                case BorderType.Horizontal:
                case BorderType.Vertical:
                    return (Border)GetValue(type.ToString(), GV.GetNull);
                case BorderType.Left:
                    return _left;
                case BorderType.Right:
                    return _right;
                case BorderType.Top:
                    return _top;
            }
            if (!Enum.IsDefined(typeof(BorderType), type))
                throw new /*InvalidEnum*/ArgumentException(DomSR.InvalidEnumValue(type), "type");
            return null;
        }

        #region Methods
        /// <summary>
        /// Creates a deep copy of this object.
        /// </summary>
        public new Borders Clone()
        {
            return (Borders)DeepCopy();
        }

        /// <summary>
        /// Implements the deep copy of the object.
        /// </summary>
        protected override object DeepCopy()
        {
            Borders borders = (Borders)base.DeepCopy();
            if (borders._top != null)
            {
                borders._top = borders._top.Clone();
                borders._top._parent = borders;
            }
            if (borders._left != null)
            {
                borders._left = borders._left.Clone();
                borders._left._parent = borders;
            }
            if (borders._right != null)
            {
                borders._right = borders._right.Clone();
                borders._right._parent = borders;
            }
            if (borders._bottom != null)
            {
                borders._bottom = borders._bottom.Clone();
                borders._bottom._parent = borders;
            }
            if (borders._diagonalUp != null)
            {
                borders._diagonalUp = borders._diagonalUp.Clone();
                borders._diagonalUp._parent = borders;
            }
            if (borders._diagonalDown != null)
            {
                borders._diagonalDown = borders._diagonalDown.Clone();
                borders._diagonalDown._parent = borders;
            }
            return borders;
        }

        /// <summary>
        /// Gets an enumerator for the borders object.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            Dictionary<string, Border> ht = new Dictionary<string, Border>();
            ht.Add("Top", _top);
            ht.Add("Left", _left);
            ht.Add("Bottom", _bottom);
            ht.Add("Right", _right);
            ht.Add("DiagonalUp", _diagonalUp);
            ht.Add("DiagonalDown", _diagonalDown);

            return new BorderEnumerator(ht);
        }

        /// <summary>
        /// Clears all Border objects from the collection. Additionally 'Borders = null'
        /// is written to the DDL stream when serialized.
        /// </summary>
        public void ClearAll()
        {
            _clearAll = true;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the top border.
        /// </summary>
        public Border Top
        {
            get { return _top ?? (_top = new Border(this)); }
            set
            {
                SetParent(value);
                _top = value;
            }
        }
        [DV]
        internal Border _top;

        /// <summary>
        /// Gets or sets the left border.
        /// </summary>
        public Border Left
        {
            get { return _left ?? (_left = new Border(this)); }
            set
            {
                SetParent(value);
                _left = value;
            }
        }
        [DV]
        internal Border _left;

        /// <summary>
        /// Gets or sets the bottom border.
        /// </summary>
        public Border Bottom
        {
            get { return _bottom ?? (_bottom = new Border(this)); }
            set
            {
                SetParent(value);
                _bottom = value;
            }
        }
        [DV]
        internal Border _bottom;

        /// <summary>
        /// Gets or sets the right border.
        /// </summary>
        public Border Right
        {
            get { return _right ?? (_right = new Border(this)); }
            set
            {
                SetParent(value);
                _right = value;
            }
        }
        [DV]
        internal Border _right;

        /// <summary>
        /// Gets or sets the diagonalup border.
        /// </summary>
        public Border DiagonalUp
        {
            get { return _diagonalUp ?? (_diagonalUp = new Border(this)); }
            set
            {
                SetParent(value);
                _diagonalUp = value;
            }
        }
        [DV]
        internal Border _diagonalUp;

        /// <summary>
        /// Gets or sets the diagonaldown border.
        /// </summary>
        public Border DiagonalDown
        {
            get { return _diagonalDown ?? (_diagonalDown = new Border(this)); }
            set
            {
                SetParent(value);
                _diagonalDown = value;
            }
        }
        [DV]
        internal Border _diagonalDown;

        /// <summary>
        /// Gets or sets a value indicating whether the borders are visible.
        /// </summary>
        public bool Visible
        {
            get { return _visible.Value; }
            set { _visible.Value = value; }
        }
        [DV]
        internal NBool _visible = NBool.NullValue;

        /// <summary>
        /// Gets or sets the line style of the borders.
        /// </summary>
        public BorderStyle Style
        {
            get { return (BorderStyle)_style.Value; }
            set { _style.Value = (int)value; }
        }
        [DV(Type = typeof(BorderStyle))]
        internal NEnum _style = NEnum.NullValue(typeof(BorderStyle));

        /// <summary>
        /// Gets or sets the standard width of the borders.
        /// </summary>
        public Unit Width
        {
            get { return _width; }
            set { _width = value; }
        }
        [DV]
        internal Unit _width = Unit.NullValue;

        /// <summary>
        /// Gets or sets the color of the borders.
        /// </summary>
        public Color Color
        {
            get { return _color; }
            set { _color = value; }
        }
        [DV]
        internal Color _color = Color.Empty;

        /// <summary>
        /// Gets or sets the distance between text and the top border.
        /// </summary>
        public Unit DistanceFromTop
        {
            get { return _distanceFromTop; }
            set { _distanceFromTop = value; }
        }
        [DV]
        internal Unit _distanceFromTop = Unit.NullValue;

        /// <summary>
        /// Gets or sets the distance between text and the bottom border.
        /// </summary>
        public Unit DistanceFromBottom
        {
            get { return _distanceFromBottom; }
            set { _distanceFromBottom = value; }
        }
        [DV]
        internal Unit _distanceFromBottom = Unit.NullValue;

        /// <summary>
        /// Gets or sets the distance between text and the left border.
        /// </summary>
        public Unit DistanceFromLeft
        {
            get { return _distanceFromLeft; }
            set { _distanceFromLeft = value; }
        }
        [DV]
        internal Unit _distanceFromLeft = Unit.NullValue;

        /// <summary>
        /// Gets or sets the distance between text and the right border.
        /// </summary>
        public Unit DistanceFromRight
        {
            get { return _distanceFromRight; }
            set { _distanceFromRight = value; }
        }
        [DV]
        internal Unit _distanceFromRight = Unit.NullValue;

        /// <summary>
        /// Sets the distance to all four borders to the specified value.
        /// </summary>
        public Unit Distance
        {
            set
            {
                DistanceFromTop = value;
                DistanceFromBottom = value;
                DistanceFromLeft = value;
                DistanceFromRight = value;
            }
        }

        /// <summary>
        /// Gets the information if the collection is marked as cleared. Additionally 'Borders = null'
        /// is written to the DDL stream when serialized.
        /// </summary>
        public bool BordersCleared
        {
            get { return _clearAll; }
            set { _clearAll = value; }
        }
        protected bool _clearAll;
        #endregion

        #region Internal
        /// <summary>
        /// Converts Borders into DDL.
        /// </summary>
        internal override void Serialize(Serializer serializer)
        {
            Serialize(serializer, null);
        }

        /// <summary>
        /// Converts Borders into DDL.
        /// </summary>
        internal void Serialize(Serializer serializer, Borders refBorders)
        {
            if (_clearAll)
                serializer.WriteLine("Borders = null");

            int pos = serializer.BeginContent("Borders");

            if (!_visible.IsNull && (refBorders == null || refBorders._visible.IsNull || (Visible != refBorders.Visible)))
                serializer.WriteSimpleAttribute("Visible", Visible);

            if (!_style.IsNull && (refBorders == null || (Style != refBorders.Style)))
                serializer.WriteSimpleAttribute("Style", Style);

            if (!_width.IsNull && (refBorders == null || (_width.Value != refBorders._width.Value)))
                serializer.WriteSimpleAttribute("Width", Width);

            if (!_color.IsNull && (refBorders == null || ((Color.Argb != refBorders.Color.Argb))))
                serializer.WriteSimpleAttribute("Color", Color);

            if (!_distanceFromTop.IsNull && (refBorders == null || (DistanceFromTop.Point != refBorders.DistanceFromTop.Point)))
                serializer.WriteSimpleAttribute("DistanceFromTop", DistanceFromTop);

            if (!_distanceFromBottom.IsNull && (refBorders == null || (DistanceFromBottom.Point != refBorders.DistanceFromBottom.Point)))
                serializer.WriteSimpleAttribute("DistanceFromBottom", DistanceFromBottom);

            if (!_distanceFromLeft.IsNull && (refBorders == null || (DistanceFromLeft.Point != refBorders.DistanceFromLeft.Point)))
                serializer.WriteSimpleAttribute("DistanceFromLeft", DistanceFromLeft);

            if (!_distanceFromRight.IsNull && (refBorders == null || (DistanceFromRight.Point != refBorders.DistanceFromRight.Point)))
                serializer.WriteSimpleAttribute("DistanceFromRight", DistanceFromRight);

            if (!IsNull("Top"))
                _top.Serialize(serializer, "Top", null);

            if (!IsNull("Left"))
                _left.Serialize(serializer, "Left", null);

            if (!IsNull("Bottom"))
                _bottom.Serialize(serializer, "Bottom", null);

            if (!IsNull("Right"))
                _right.Serialize(serializer, "Right", null);

            if (!IsNull("DiagonalDown"))
                _diagonalDown.Serialize(serializer, "DiagonalDown", null);

            if (!IsNull("DiagonalUp"))
                _diagonalUp.Serialize(serializer, "DiagonalUp", null);

            serializer.EndContent(pos);
        }

        /// <summary>
        /// Gets a name of a border.
        /// </summary>
        internal string GetMyName(Border border)
        {
            if (border == _top)
                return "Top";
            if (border == _bottom)
                return "Bottom";
            if (border == _left)
                return "Left";
            if (border == _right)
                return "Right";
            if (border == _diagonalUp)
                return "DiagonalUp";
            if (border == _diagonalDown)
                return "DiagonalDown";
            return null;
        }

        /// <summary>
        /// Returns an enumerator that can iterate through the Borders.
        /// </summary>
        public class BorderEnumerator : IEnumerator
        {
            int _index;
            readonly Dictionary<string, Border> _ht;

            /// <summary>
            /// Creates a new BorderEnumerator.
            /// </summary>
            public BorderEnumerator(Dictionary<string, Border> ht)
            {
                _ht = ht;
                _index = -1;
            }

            /// <summary>
            /// Sets the enumerator to its initial position, which is before the first element in the border collection.
            /// </summary>
            public void Reset()
            {
                _index = -1;
            }

            /// <summary>
            /// Gets the current element in the border collection.
            /// </summary>
            public Border Current
            {
                get
                {
                    IEnumerator enumerator = _ht.GetEnumerator();
                    enumerator.Reset();
                    for (int idx = 0; idx < _index + 1; idx++)
                        enumerator.MoveNext();
                    return ((DictionaryEntry)enumerator.Current).Value as Border;
                }
            }

            /// <summary>
            /// Gets the current element in the border collection.
            /// </summary>
            object IEnumerator.Current
            {
                get { return Current; }
            }

            /// <summary>
            /// Advances the enumerator to the next element of the border collection.
            /// </summary>
            public bool MoveNext()
            {
                _index++;
                return (_index < _ht.Count);
            }
        }

        /// <summary>
        /// Returns the meta object of this instance.
        /// </summary>
        internal override Meta Meta
        {
            get { return _meta ?? (_meta = new Meta(typeof(Borders))); }
        }
        static Meta _meta;
        #endregion
    }
}

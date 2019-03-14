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
using MigraDoc.DocumentObjectModel.Internals;

namespace MigraDoc.DocumentObjectModel
{
    /// <summary>
    /// Represents one border in a borders collection. The type determines its position in a cell,
    /// paragraph etc.
    /// </summary>
    public class Border : DocumentObject
    {
        /// <summary>
        /// Initializes a new instance of the Border class.
        /// </summary>
        public Border()
        { }

        /// <summary>
        /// Initializes a new instance of the Border class with the specified parent.
        /// </summary>
        internal Border(DocumentObject parent) : base(parent) { }

        #region Methods
        /// <summary>
        /// Creates a deep copy of this object.
        /// </summary>
        public new Border Clone()
        {
            return (Border)DeepCopy();
        }

        /// <summary>
        /// Clears the Border object. Additionally 'Border = null'
        /// is written to the DDL stream when serialized.
        /// </summary>
        public void Clear()
        {
            _fClear.Value = true;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether the border visible is.
        /// </summary>
        public bool Visible
        {
            get { return _visible.Value; }
            set { _visible.Value = value; }
        }
        [DV]
        internal NBool _visible = NBool.NullValue;

        /// <summary>
        /// Gets or sets the line style of the border.
        /// </summary>
        public BorderStyle Style
        {
            get { return (BorderStyle)_style.Value; }
            set { _style.Value = (int)value; }
        }
        [DV(Type = typeof(BorderStyle))]
        internal NEnum _style = NEnum.NullValue(typeof(BorderStyle));

        /// <summary>
        /// Gets or sets the line width of the border.
        /// </summary>
        public Unit Width
        {
            get { return _width; }
            set { _width = value; }
        }
        [DV]
        internal Unit _width = Unit.NullValue;

        /// <summary>
        /// Gets or sets the color of the border.
        /// </summary>
        public Color Color
        {
            get { return _color; }
            set { _color = value; }
        }
        [DV]
        internal Color _color = Color.Empty;

        /// <summary>
        /// Gets the name of this border ("top", "bottom"....).
        /// </summary>
        public string Name
        {
            get { return ((Borders)_parent).GetMyName(this); }
        }

        /// <summary>
        /// Gets the information if the border is marked as cleared. Additionally 'xxx = null'
        /// is written to the DDL stream when serialized.
        /// </summary>
        public bool BorderCleared
        {
            get { return _fClear.Value; }
        }
        internal NBool _fClear = new NBool(false);
        #endregion

        #region Internal
        /// <summary>
        /// Converts Border into DDL.
        /// </summary>
        internal override void Serialize(Serializer serializer)
        {
            throw new Exception("A Border cannot be serialized alone.");
        }

        /// <summary>
        /// Converts Border into DDL.
        /// </summary>
        internal void Serialize(Serializer serializer, string name, Border refBorder)
        {
            if (_fClear.Value)
                serializer.WriteLine(name + " = null");

            int pos = serializer.BeginContent(name);

            if (!_visible.IsNull && (refBorder == null || (Visible != refBorder.Visible)))
                serializer.WriteSimpleAttribute("Visible", Visible);

            if (!_style.IsNull && (refBorder == null || (Style != refBorder.Style)))
                serializer.WriteSimpleAttribute("Style", Style);

            if (!_width.IsNull && (refBorder == null || (Width != refBorder.Width)))
                serializer.WriteSimpleAttribute("Width", Width);

            if (!_color.IsNull && (refBorder == null || (Color != refBorder.Color)))
                serializer.WriteSimpleAttribute("Color", Color);

            serializer.EndContent(pos);
        }

        /// <summary>
        /// Returns the _meta object of this instance.
        /// </summary>
        internal override Meta Meta
        {
            get { return _meta ?? (_meta = new Meta(typeof(Border))); }
        }
        static Meta _meta;
        #endregion
    }
}

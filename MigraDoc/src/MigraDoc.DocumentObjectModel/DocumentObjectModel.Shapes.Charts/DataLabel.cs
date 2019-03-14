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

namespace MigraDoc.DocumentObjectModel.Shapes.Charts
{
    /// <summary>
    /// Represents a DataLabel of a Series
    /// </summary>
    public class DataLabel : DocumentObject
    {
        /// <summary>
        /// Initializes a new instance of the DataLabel class.
        /// </summary>
        public DataLabel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the DataLabel class with the specified parent.
        /// </summary>
        internal DataLabel(DocumentObject parent) : base(parent) { }

        #region Methods
        /// <summary>
        /// Creates a deep copy of this object.
        /// </summary>
        public new DataLabel Clone()
        {
            return (DataLabel)DeepCopy();
        }

        /// <summary>
        /// Implements the deep copy of the object.
        /// </summary>
        protected override object DeepCopy()
        {
            DataLabel dataLabel = (DataLabel)base.DeepCopy();
            if (dataLabel._font != null)
            {
                dataLabel._font = dataLabel._font.Clone();
                dataLabel._font._parent = dataLabel;
            }
            return dataLabel;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a numeric format string for the DataLabel.
        /// </summary>
        public string Format
        {
            get { return _format.Value; }
            set { _format.Value = value; }
        }
        [DV]
        internal NString _format = NString.NullValue;

        /// <summary>
        /// Gets the Font for the DataLabel.
        /// </summary>
        public Font Font
        {
            get { return _font ?? (_font = new Font(this)); }
            set
            {
                SetParent(value);
                _font = value;
            }
        }
        [DV]
        internal Font _font;

        /// <summary>
        /// Gets or sets the Style for the DataLabel.
        /// Only the Font-associated part of the Style's ParagraphFormat is used.
        /// </summary>
        public string Style
        {
            get { return _style.Value; }
            set { _style.Value = value; }
        }
        [DV]
        internal NString _style = NString.NullValue;

        /// <summary>
        /// Gets or sets the position of the DataLabel.
        /// </summary>
        public DataLabelPosition Position
        {
            get { return (DataLabelPosition)_position.Value; }
            set { _position.Value = (int)value; }
        }
        [DV(Type = typeof(DataLabelPosition))]
        internal NEnum _position = NEnum.NullValue(typeof(DataLabelPosition));

        /// <summary>
        /// Gets or sets the type of the DataLabel.
        /// </summary>
        public DataLabelType Type
        {
            get { return (DataLabelType)_type.Value; }
            set { _type.Value = (int)value; }
        }
        [DV(Type = typeof(DataLabelType))]
        internal NEnum _type = NEnum.NullValue(typeof(DataLabelType));
        #endregion

        #region Internal
        /// <summary>
        /// Converts DataLabel into DDL.
        /// </summary>
        internal override void Serialize(Serializer serializer)
        {
            int pos = serializer.BeginContent("DataLabel");

            if (Style != string.Empty)
                serializer.WriteSimpleAttribute("Style", Style);
            if (Format != string.Empty)
                serializer.WriteSimpleAttribute("Format", Format);
            if (!_position.IsNull)
                serializer.WriteSimpleAttribute("Position", Position);
            if (!_type.IsNull)
                serializer.WriteSimpleAttribute("Type", Type);
            if (!IsNull("Font"))
                _font.Serialize(serializer);

            serializer.EndContent(pos);
        }

        /// <summary>
        /// Returns the meta object of this instance.
        /// </summary>
        internal override Meta Meta
        {
            get { return _meta ?? (_meta = new Meta(typeof(DataLabel))); }
        }
        static Meta _meta;
        #endregion
    }
}

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

namespace MigraDoc.DocumentObjectModel
{
    /// <summary>
    /// A ParagraphFormat represents the formatting of a paragraph.
    /// </summary>
    public class ParagraphFormat : DocumentObject
    {
        /// <summary>
        /// Initializes a new instance of the ParagraphFormat class that can be used as a template.
        /// </summary>
        public ParagraphFormat()
        { }

        /// <summary>
        /// Initializes a new instance of the ParagraphFormat class with the specified parent.
        /// </summary>
        internal ParagraphFormat(DocumentObject parent) : base(parent) { }

        #region Methods
        /// <summary>
        /// Creates a deep copy of this object.
        /// </summary>
        public new ParagraphFormat Clone()
        {
            return (ParagraphFormat)DeepCopy();
        }

        /// <summary>
        /// Implements the deep copy of the object.
        /// </summary>
        protected override object DeepCopy()
        {
            ParagraphFormat format = (ParagraphFormat)base.DeepCopy();
            if (format._font != null)
            {
                format._font = format._font.Clone();
                format._font._parent = format;
            }
            if (format._shading != null)
            {
                format._shading = format._shading.Clone();
                format._shading._parent = format;
            }
            if (format._borders != null)
            {
                format._borders = format._borders.Clone();
                format._borders._parent = format;
            }
            if (format._tabStops != null)
            {
                format._tabStops = format._tabStops.Clone();
                format._tabStops._parent = format;
            }
            if (format._listInfo != null)
            {
                format._listInfo = format._listInfo.Clone();
                format._listInfo._parent = format;
            }
            return format;
        }

        /// <summary>
        /// Adds a TabStop object to the collection.
        /// </summary>
        public TabStop AddTabStop(Unit position)
        {
            return TabStops.AddTabStop(position);
        }

        /// <summary>
        /// Adds a TabStop object to the collection and sets its alignment and leader.
        /// </summary>
        public TabStop AddTabStop(Unit position, TabAlignment alignment, TabLeader leader)
        {
            return TabStops.AddTabStop(position, alignment, leader);
        }

        /// <summary>
        /// Adds a TabStop object to the collection and sets its leader.
        /// </summary>
        public TabStop AddTabStop(Unit position, TabLeader leader)
        {
            return TabStops.AddTabStop(position, leader);
        }

        /// <summary>
        /// Adds a TabStop object to the collection and sets its alignment.
        /// </summary>
        public TabStop AddTabStop(Unit position, TabAlignment alignment)
        {
            return TabStops.AddTabStop(position, alignment);
        }

        /// <summary>
        /// Adds a TabStop object to the collection marked to remove the tab stop at
        /// the given position.
        /// </summary>
        public void RemoveTabStop(Unit position)
        {
            TabStops.RemoveTabStop(position);
        }

        /// <summary>
        /// Adds a TabStop object to the collection.
        /// </summary>
        public void Add(TabStop tabStop)
        {
            TabStops.AddTabStop(tabStop);
        }

        /// <summary>
        /// Clears all TapStop objects from the collection. Additionally 'TabStops = null'
        /// is written to the DDL stream when serialized.
        /// </summary>
        public void ClearAll()
        {
            TabStops.ClearAll();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the Alignment of the paragraph.
        /// </summary>
        public ParagraphAlignment Alignment
        {
            get { return (ParagraphAlignment)_alignment.Value; }
            set { _alignment.Value = (int)value; }
        }
        [DV(Type = typeof(ParagraphAlignment))]
        internal NEnum _alignment = NEnum.NullValue(typeof(ParagraphAlignment));

        /// <summary>
        /// Gets the Borders object.
        /// </summary>
        public Borders Borders
        {
            get { return _borders ?? (_borders = new Borders(this)); }
            set
            {
                SetParent(value);
                _borders = value;
            }
        }
        [DV]
        internal Borders _borders;

        /// <summary>
        /// Gets or sets the indent of the first line in the paragraph.
        /// </summary>
        public Unit FirstLineIndent
        {
            get { return _firstLineIndent; }
            set { _firstLineIndent = value; }
        }
        [DV]
        internal Unit _firstLineIndent = Unit.NullValue;

        /// <summary>
        /// Gets or sets the Font object.
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
        /// Gets or sets a value indicating whether to keep all the paragraph's lines on the same page.
        /// </summary>
        public bool KeepTogether
        {
            get { return _keepTogether.Value; }
            set { _keepTogether.Value = value; }
        }
        [DV]
        internal NBool _keepTogether = NBool.NullValue;

        /// <summary>
        /// Gets or sets a value indicating whether this and the next paragraph stay on the same page.
        /// </summary>
        public bool KeepWithNext
        {
            get { return _keepWithNext.Value; }
            set { _keepWithNext.Value = value; }
        }
        [DV]
        internal NBool _keepWithNext = NBool.NullValue;

        /// <summary>
        /// Gets or sets the left indent of the paragraph.
        /// </summary>
        public Unit LeftIndent
        {
            get { return _leftIndent; }
            set { _leftIndent = value; }
        }
        [DV]
        internal Unit _leftIndent = Unit.NullValue;

        /// <summary>
        /// Gets or sets the space between lines on the paragraph.
        /// </summary>
        public Unit LineSpacing
        {
            get { return _lineSpacing; }
            set { _lineSpacing = value; }
        }
        [DV]
        internal Unit _lineSpacing = Unit.NullValue;

        /// <summary>
        /// Gets or sets the rule which is used to define the line spacing.
        /// </summary>
        public LineSpacingRule LineSpacingRule
        {
            get { return (LineSpacingRule)_lineSpacingRule.Value; }
            set { _lineSpacingRule.Value = (int)value; }
        }
        [DV(Type = typeof(LineSpacingRule))]
        internal NEnum _lineSpacingRule = NEnum.NullValue(typeof(LineSpacingRule));

        /// <summary>
        /// Gets or sets the ListInfo object of the paragraph.
        /// </summary>
        public ListInfo ListInfo
        {
            get { return _listInfo ?? (_listInfo = new ListInfo(this)); }
            set
            {
                SetParent(value);
                _listInfo = value;
            }
        }
        [DV]
        internal ListInfo _listInfo;

        /// <summary>
        /// Gets or sets the out line level of the paragraph.
        /// </summary>
        public OutlineLevel OutlineLevel
        {
            get { return (OutlineLevel)_outlineLevel.Value; }
            set { _outlineLevel.Value = (int)value; }
        }
        [DV(Type = typeof(OutlineLevel))]
        internal NEnum _outlineLevel = NEnum.NullValue(typeof(OutlineLevel));

        /// <summary>
        /// Gets or sets a value indicating whether a page break is inserted before the paragraph.
        /// </summary>
        public bool PageBreakBefore
        {
            get { return _pageBreakBefore.Value; }
            set { _pageBreakBefore.Value = value; }
        }
        [DV]
        internal NBool _pageBreakBefore = NBool.NullValue;

        /// <summary>
        /// Gets or sets the right indent of the paragraph.
        /// </summary>
        public Unit RightIndent
        {
            get { return _rightIndent; }
            set { _rightIndent = value; }
        }
        [DV]
        internal Unit _rightIndent = Unit.NullValue;

        /// <summary>
        /// Gets the shading object.
        /// </summary>
        public Shading Shading
        {
            get { return _shading ?? (_shading = new Shading(this)); }
            set
            {
                SetParent(value);
                _shading = value;
            }
        }
        [DV]
        internal Shading _shading;

        /// <summary>
        /// Gets or sets the space that's inserted after the paragraph.
        /// </summary>
        public Unit SpaceAfter
        {
            get { return _spaceAfter; }
            set { _spaceAfter = value; }
        }
        [DV]
        internal Unit _spaceAfter = Unit.NullValue;

        /// <summary>
        /// Gets or sets the space that's inserted before the paragraph.
        /// </summary>
        public Unit SpaceBefore
        {
            get { return _spaceBefore; }
            set { _spaceBefore = value; }
        }
        [DV]
        internal Unit _spaceBefore = Unit.NullValue;

        /// <summary>
        /// Indicates whether the ParagraphFormat has a TabStops collection.
        /// </summary>
        public bool HasTabStops
        {
            get { return _tabStops != null; }
        }

        /// <summary>
        /// Get the TabStops collection.
        /// </summary>
        public TabStops TabStops
        {
            get { return _tabStops ?? (_tabStops = new TabStops(this)); }
            set
            {
                SetParent(value);
                _tabStops = value;
            }
        }
        [DV]
        internal TabStops _tabStops;

        /// <summary>
        /// Gets or sets a value indicating whether a line from the paragraph stays alone in a page.
        /// </summary>
        public bool WidowControl
        {
            get { return _widowControl.Value; }
            set { _widowControl.Value = value; }
        }
        [DV]
        internal NBool _widowControl = NBool.NullValue;
        #endregion

        #region Internal
        /// <summary>
        /// Converts ParagraphFormat into DDL.
        /// </summary>
        internal override void Serialize(Serializer serializer)
        {
            if (_parent is Style)
                Serialize(serializer, "ParagraphFormat", null);
            else
                Serialize(serializer, "Format", null);
        }

        /// <summary>
        /// Converts ParagraphFormat into DDL.
        /// </summary>
        internal void Serialize(Serializer serializer, string name, ParagraphFormat refFormat)
        {
            int pos = serializer.BeginContent(name);

            if (!IsNull("Font") && Parent.GetType() != typeof(Style))
                Font.Serialize(serializer);

            // If a refFormat is specified, it is important to compare the fields and not the properties.
            // Only the fields holds the internal information whether a value is NULL. In contrast to the
            // Efw.Application framework the nullable values and all the meta stuff is kept internal to
            // give the user the illusion of simplicity.

            if (!_alignment.IsNull && (refFormat == null || (_alignment != refFormat._alignment)))
                serializer.WriteSimpleAttribute("Alignment", Alignment);

            if (!_leftIndent.IsNull && (refFormat == null || (_leftIndent != refFormat._leftIndent)))
                serializer.WriteSimpleAttribute("LeftIndent", LeftIndent);

            if (!_firstLineIndent.IsNull && (refFormat == null || _firstLineIndent != refFormat._firstLineIndent))
                serializer.WriteSimpleAttribute("FirstLineIndent", FirstLineIndent);

            if (!_rightIndent.IsNull && (refFormat == null || _rightIndent != refFormat._rightIndent))
                serializer.WriteSimpleAttribute("RightIndent", RightIndent);

            if (!_spaceBefore.IsNull && (refFormat == null || _spaceBefore != refFormat._spaceBefore))
                serializer.WriteSimpleAttribute("SpaceBefore", SpaceBefore);

            if (!_spaceAfter.IsNull && (refFormat == null || _spaceAfter != refFormat._spaceAfter))
                serializer.WriteSimpleAttribute("SpaceAfter", SpaceAfter);

            if (!_lineSpacingRule.IsNull && (refFormat == null || _lineSpacingRule != refFormat._lineSpacingRule))
                serializer.WriteSimpleAttribute("LineSpacingRule", LineSpacingRule);

            if (!_lineSpacing.IsNull && (refFormat == null || _lineSpacing != refFormat._lineSpacing))
                serializer.WriteSimpleAttribute("LineSpacing", LineSpacing);

            if (!_keepTogether.IsNull && (refFormat == null || _keepTogether != refFormat._keepTogether))
                serializer.WriteSimpleAttribute("KeepTogether", KeepTogether);

            if (!_keepWithNext.IsNull && (refFormat == null || _keepWithNext != refFormat._keepWithNext))
                serializer.WriteSimpleAttribute("KeepWithNext", KeepWithNext);

            if (!_widowControl.IsNull && (refFormat == null || _widowControl != refFormat._widowControl))
                serializer.WriteSimpleAttribute("WidowControl", WidowControl);

            if (!_pageBreakBefore.IsNull && (refFormat == null || _pageBreakBefore != refFormat._pageBreakBefore))
                serializer.WriteSimpleAttribute("PageBreakBefore", PageBreakBefore);

            if (!_outlineLevel.IsNull && (refFormat == null || _outlineLevel != refFormat._outlineLevel))
                serializer.WriteSimpleAttribute("OutlineLevel", OutlineLevel);

            if (!IsNull("ListInfo"))
                ListInfo.Serialize(serializer);

            if (!IsNull("TabStops"))
                _tabStops.Serialize(serializer);

            if (!IsNull("Borders"))
            {
                if (refFormat != null)
                    _borders.Serialize(serializer, refFormat.Borders);
                else
                    _borders.Serialize(serializer, null);
            }

            if (!IsNull("Shading"))
                _shading.Serialize(serializer);

            serializer.EndContent(pos);
        }

        /// <summary>
        /// Returns the meta object of this instance.
        /// </summary>
        internal override Meta Meta
        {
            get { return _meta ?? (_meta = new Meta(typeof(ParagraphFormat))); }
        }
        static Meta _meta;
        #endregion
    }
}

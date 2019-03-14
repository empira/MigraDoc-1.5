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
    /// Font represents the formatting of characters in a paragraph.
    /// </summary>
    public sealed class Font : DocumentObject
    {
        /// <summary>
        /// Initializes a new instance of the Font class that can be used as a template.
        /// </summary>
        public Font()
        { }

        /// <summary>
        /// Initializes a new instance of the Font class with the specified parent.
        /// </summary>
        internal Font(DocumentObject parent) : base(parent) { }

        /// <summary>
        /// Initializes a new instance of the Font class with the specified name and size.
        /// </summary>
        public Font(string name, Unit size)
        {
            _name.Value = name;
            _size.Value = size;
        }

        /// <summary>
        /// Initializes a new instance of the Font class with the specified name.
        /// </summary>
        public Font(string name)
        {
            _name.Value = name;
        }

        #region Methods
        /// <summary>
        /// Creates a copy of the Font.
        /// </summary>
        public new Font Clone()
        {
            return (Font)DeepCopy();
        }

        /// <summary>
        /// Applies all non-null properties of a font to this font if the given font's property is different from the given refFont's property.
        /// </summary>
        internal void ApplyFont(Font font, Font refFont)
        {
            if (font == null)
                throw new ArgumentNullException("font");

            if ((!font._name.IsNull && font._name.Value != "") && (refFont == null || font.Name != refFont.Name))
                Name = font.Name;

            if (!font._size.IsNull && (refFont == null || font.Size != refFont.Size))
                Size = font.Size;

            if (!font._bold.IsNull && (refFont == null || font.Bold != refFont.Bold))
                Bold = font.Bold;

            if (!font._italic.IsNull && (refFont == null || font.Italic != refFont.Italic))
                Italic = font.Italic;

            if (!font._subscript.IsNull && (refFont == null || font.Subscript != refFont.Subscript))
                Subscript = font.Subscript;
            else if (!font._superscript.IsNull && (refFont == null || font.Superscript != refFont.Superscript))
                Superscript = font.Superscript;

            if (!font._underline.IsNull && (refFont == null || font.Underline != refFont.Underline))
                Underline = font.Underline;

            if (!font._color.IsNull && (refFont == null || font.Color.Argb != refFont.Color.Argb))
                Color = font.Color;
        }

        /// <summary>
        /// Applies all non-null properties of a font to this font.
        /// </summary>
        public void ApplyFont(Font font)
        {
            if (font == null)
                throw new ArgumentNullException("font");

            if (!font._name.IsNull && font._name.Value != "")
                Name = font.Name;

            if (!font._size.IsNull)
                Size = font.Size;

            if (!font._bold.IsNull)
                Bold = font.Bold;

            if (!font._italic.IsNull)
                Italic = font.Italic;

            if (!font._subscript.IsNull)
                Subscript = font.Subscript;
            else if (!font._superscript.IsNull)
                Superscript = font.Superscript;

            if (!font._underline.IsNull)
                Underline = font.Underline;

            if (!font._color.IsNull)
                Color = font.Color;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the name of the font.
        /// </summary>
        public string Name
        {
            get { return _name.Value; }
            set { _name.Value = value; }
        }
        [DV]
        internal NString _name = NString.NullValue;

        /// <summary>
        /// Gets or sets the size of the font.
        /// </summary>
        public Unit Size
        {
            get { return _size; }
            set { _size = value; }
        }
        [DV]
        internal Unit _size = Unit.NullValue;

        /// <summary>
        /// Gets or sets the bold property.
        /// </summary>
        public bool Bold
        {
            get { return _bold.Value; }
            set { _bold.Value = value; }
        }
        [DV]
        internal NBool _bold = NBool.NullValue;

        /// <summary>
        /// Gets or sets the italic property.
        /// </summary>
        public bool Italic
        {
            get { return _italic.Value; }
            set { _italic.Value = value; }
        }
        [DV]
        internal NBool _italic = NBool.NullValue;

        // THHO4STLA Implementation for Strikethrough in the forum: http://forum.pdfsharp.net/viewtopic.php?p=4636#p4636
        /// <summary>
        /// Gets or sets the underline property.
        /// </summary>
        public Underline Underline
        {
            get { return (Underline)_underline.Value; }
            set { _underline.Value = (int)value; }
        }
        [DV(Type = typeof(Underline))]
        internal NEnum _underline = NEnum.NullValue(typeof(Underline));

        /// <summary>
        /// Gets or sets the color property.
        /// </summary>
        public Color Color
        {
            get { return _color; }
            set { _color = value; }
        }
        [DV]
        internal Color _color = Color.Empty;

        /// <summary>
        /// Gets or sets the superscript property.
        /// </summary>
        public bool Superscript
        {
            get { return _superscript.Value; }
            set
            {
                _superscript.Value = value;
                _subscript.SetNull();
            }
        }
        [DV]
        internal NBool _superscript = NBool.NullValue;

        /// <summary>
        /// Gets or sets the subscript property.
        /// </summary>
        public bool Subscript
        {
            get { return _subscript.Value; }
            set
            {
                _subscript.Value = value;
                _superscript.SetNull();
            }
        }
        [DV]
        internal NBool _subscript = NBool.NullValue;

        //  + .Name = "Arial"
        //  + .Size = 8
        //  + .Bold = False
        //  + .Italic = False
        //  + .Underline = wdUnderlineDouble
        //  * .UnderlineColor = wdColorOrange
        //    .StrikeThrough = False
        //    .DoubleStrikeThrough = False
        //    .Outline = False
        //    .Emboss = False
        //    .Shadow = False
        //    .Hidden = False
        //  * .SmallCaps = False
        //  * .AllCaps = False
        //  + .Color = wdColorAutomatic
        //    .Engrave = False
        //  + .Superscript = False
        //  + .Subscript = False
        //  * .Spacing = 0
        //  * .Scaling = 100
        //  * .Position = 0
        //    .Kerning = 0
        //    .Animation = wdAnimationNone
        #endregion

        /// <summary>
        /// Gets a value indicating whether the specified font exists.
        /// </summary>
        [Obsolete("This function is removed from DocumentObjectModel and always returns false.")]
        public static bool Exists(string fontName)
        {
            //System.Drawing.FontFamily[] families = System.Drawing.FontFamily.Families;
            //foreach (System.Drawing.FontFamily family in families)
            //{
            //  if (String.Compare(family.Name, fontName, true) == 0)
            //    return true;
            //}
            return false;
        }

        #region Internal
        /// <summary>
        /// Get a bitmask of all non-null properties.
        /// </summary>
        private FontProperties CheckWhatIsNotNull()
        {
            FontProperties fp = FontProperties.None;
            if (!_name.IsNull)
                fp |= FontProperties.Name;
            if (!_size.IsNull)
                fp |= FontProperties.Size;
            if (!_bold.IsNull)
                fp |= FontProperties.Bold;
            if (!_italic.IsNull)
                fp |= FontProperties.Italic;
            if (!_underline.IsNull)
                fp |= FontProperties.Underline;
            if (!_color.IsNull)
                fp |= FontProperties.Color;
            if (!_superscript.IsNull)
                fp |= FontProperties.Superscript;
            if (!_subscript.IsNull)
                fp |= FontProperties.Subscript;
            return fp;
        }

        /// <summary>
        /// Converts Font into DDL.
        /// </summary>
        internal override void Serialize(Serializer serializer)
        {
            Serialize(serializer, null);
        }

        /// <summary>
        /// Converts Font into DDL. Properties with the same value as in an optionally given
        /// font are not serialized.
        /// </summary>
        internal void Serialize(Serializer serializer, Font font)
        {
            if (Parent is FormattedText)
            {
                string fontStyle = "";
                if (((FormattedText)Parent)._style.IsNull)
                {
                    // Check if we can use a DDL keyword.
                    FontProperties notNull = CheckWhatIsNotNull();
                    if (notNull == FontProperties.Size)
                    {
                        serializer.Write("\\fontsize(" + _size + ")");
                        return;
                    }
                    if (notNull == FontProperties.Bold && _bold.Value)
                    {
                        serializer.Write("\\bold");
                        return;
                    }
                    if (notNull == FontProperties.Italic && _italic.Value)
                    {
                        serializer.Write("\\italic");
                        return;
                    }
                    if (notNull == FontProperties.Color)
                    {
                        serializer.Write("\\fontcolor(" + _color + ")");
                        return;
                    }
                }
                else
                    fontStyle = "(\"" + ((FormattedText)Parent).Style + "\")";

                //bool needBlank = false;  // nice, but later...
                serializer.Write("\\font" + fontStyle + "[");

                if (!_name.IsNull && _name.Value != "")
                    serializer.WriteSimpleAttribute("Name", Name);

#if DEBUG_ // Test
                if (!_size.IsNull && Size != 0 && Size.Point == 0)
                    GetType();
#endif
                if ((!_size.IsNull))
                    serializer.WriteSimpleAttribute("Size", Size);

                if (!_bold.IsNull)
                    serializer.WriteSimpleAttribute("Bold", Bold);

                if (!_italic.IsNull)
                    serializer.WriteSimpleAttribute("Italic", Italic);

                if (!_underline.IsNull)
                    serializer.WriteSimpleAttribute("Underline", Underline);

                if (!_superscript.IsNull)
                    serializer.WriteSimpleAttribute("Superscript", Superscript);

                if (!_subscript.IsNull)
                    serializer.WriteSimpleAttribute("Subscript", Subscript);

                if (!_color.IsNull)
                    serializer.WriteSimpleAttribute("Color", Color);

                serializer.Write("]");
            }
            else
            {
                int pos = serializer.BeginContent("Font");

#if true
                // Don't write null values if font is null.
                // Do write null values if font is not null!
                if ((!_name.IsNull && Name != String.Empty && font == null) ||
                    (font != null && !_name.IsNull && Name != String.Empty && Name != font.Name))
                    serializer.WriteSimpleAttribute("Name", Name);

#if DEBUG_
        // Test
        if (!_size.IsNull && Size != 0 && Size.Point == 0)
          GetType();
#endif

                if (!_size.IsNull &&
                    (font == null || Size != font.Size))
                    serializer.WriteSimpleAttribute("Size", Size);
                // NBool and NEnum have to be compared directly to check whether the value Null is.
                if (!_bold.IsNull && (font == null || Bold != font.Bold || font._bold.IsNull))
                    serializer.WriteSimpleAttribute("Bold", Bold);

                if (!_italic.IsNull && (font == null || Italic != font.Italic || font._italic.IsNull))
                    serializer.WriteSimpleAttribute("Italic", Italic);

                if (!_underline.IsNull && (font == null || Underline != font.Underline || font._underline.IsNull))
                    serializer.WriteSimpleAttribute("Underline", Underline);

                if (!_superscript.IsNull && (font == null || Superscript != font.Superscript || font._superscript.IsNull))
                    serializer.WriteSimpleAttribute("Superscript", Superscript);

                if (!_subscript.IsNull && (font == null || Subscript != font.Subscript || font._subscript.IsNull))
                    serializer.WriteSimpleAttribute("Subscript", Subscript);

                if (!_color.IsNull && (font == null || Color.Argb != font.Color.Argb))// && Color.RGB != Color.Transparent.RGB)
                    serializer.WriteSimpleAttribute("Color", Color);
#else
        if ((!this .name.IsNull && Name != String.Empty) && (font == null || Name != font.Name))
          serializer.WriteSimpleAttribute("Name", Name);

        if (!this .size.IsNull && (font == null || Size != font.Size))
          serializer.WriteSimpleAttribute("Size", Size);
        //NBool and NEnum have to be compared directly to check whether the value Null is
        if (!this .bold.IsNull && (font == null || Bold != font.Bold))
          serializer.WriteSimpleAttribute("Bold", Bold);

        if (!this .italic.IsNull && (font == null || Italic != font.Italic))
          serializer.WriteSimpleAttribute("Italic", Italic);

        if (!this .underline.IsNull && (font == null || Underline != font.Underline))
          serializer.WriteSimpleAttribute("Underline", Underline);

        if (!this .superscript.IsNull && (font == null || Superscript != font.Superscript))
          serializer.WriteSimpleAttribute("Superscript", Superscript);

        if (!this .subscript.IsNull && (font == null || Subscript != font.Subscript))
          serializer.WriteSimpleAttribute("Subscript", Subscript);

        if (!this .color.IsNull && (font == null || Color.Argb != font.Color.Argb))// && Color.RGB != Color.Transparent.RGB)
          serializer.WriteSimpleAttribute("Color", Color);
#endif
                serializer.EndContent(pos);
            }
        }

        /// <summary>
        /// Returns the meta object of this instance.
        /// </summary>
        internal override Meta Meta
        {
            get { return _meta ?? (_meta = new Meta(typeof(Font))); }
        }
        static Meta _meta;
        #endregion
    }
}

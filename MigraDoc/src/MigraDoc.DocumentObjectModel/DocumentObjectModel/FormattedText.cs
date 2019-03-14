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
using MigraDoc.DocumentObjectModel.Visitors;
using MigraDoc.DocumentObjectModel.Fields;
using MigraDoc.DocumentObjectModel.Shapes;

namespace MigraDoc.DocumentObjectModel
{
    /// <summary>
    /// Represents the format of a text.
    /// </summary>
    public class FormattedText : DocumentObject, IVisitable
    {
        /// <summary>
        /// Initializes a new instance of the FormattedText class.
        /// </summary>
        public FormattedText()
        { }

        /// <summary>
        /// Initializes a new instance of the FormattedText class with the specified parent.
        /// </summary>
        internal FormattedText(DocumentObject parent) : base(parent) { }

        #region Methods
        /// <summary>
        /// Creates a deep copy of this object.
        /// </summary>
        public new FormattedText Clone()
        {
            return (FormattedText)DeepCopy();
        }

        /// <summary>
        /// Implements the deep copy of the object.
        /// </summary>
        protected override object DeepCopy()
        {
            FormattedText formattedText = (FormattedText)base.DeepCopy();
            if (formattedText._font != null)
            {
                formattedText._font = formattedText._font.Clone();
                formattedText._font._parent = formattedText;
            }
            if (formattedText._elements != null)
            {
                formattedText._elements = formattedText._elements.Clone();
                formattedText._elements._parent = formattedText;
            }
            return formattedText;
        }

        /// <summary>
        /// Adds a new Bookmark.
        /// <param name="name">The name of the bookmark.</param>
        /// <param name="prepend">True, if the bookmark shall be inserted at the beginning of the paragraph.</param>
        /// </summary>
        public BookmarkField AddBookmark(string name, bool prepend = true)
        {
            return Elements.AddBookmark(name, prepend);
        }

        /// <summary>
        /// Adds a single character repeated the specified number of times to the formatted text.
        /// </summary>
        public Text AddChar(char ch, int count)
        {
            return Elements.AddChar(ch, count);
        }

        /// <summary>
        /// Adds a single character to the formatted text.
        /// </summary>
        public Text AddChar(char ch)
        {
            return Elements.AddChar(ch);
        }

        /// <summary>
        /// Adds a new PageField.
        /// </summary>
        public PageField AddPageField()
        {
            return Elements.AddPageField();
        }

        /// <summary>
        /// Adds a new PageRefField.
        /// </summary>
        public PageRefField AddPageRefField(string name)
        {
            return Elements.AddPageRefField(name);
        }

        /// <summary>
        /// Adds a new NumPagesField.
        /// </summary>
        public NumPagesField AddNumPagesField()
        {
            return Elements.AddNumPagesField();
        }

        /// <summary>
        /// Adds a new SectionField.
        /// </summary>
        public SectionField AddSectionField()
        {
            return Elements.AddSectionField();
        }

        /// <summary>
        /// Adds a new SectionPagesField.
        /// </summary>
        public SectionPagesField AddSectionPagesField()
        {
            return Elements.AddSectionPagesField();
        }

        /// <summary>
        /// Adds a new DateField.
        /// </summary>
        public DateField AddDateField()
        {
            return Elements.AddDateField();
        }

        /// <summary>
        /// Adds a new DateField.
        /// </summary>
        public DateField AddDateField(string format)
        {
            return Elements.AddDateField(format);
        }

        /// <summary>
        /// Adds a new InfoField.
        /// </summary>
        public InfoField AddInfoField(InfoFieldType iType)
        {
            return Elements.AddInfoField(iType);
        }

        /// <summary>
        /// Adds a new Footnote with the specified text.
        /// </summary>
        public Footnote AddFootnote(string text)
        {
            return Elements.AddFootnote(text);
        }

        /// <summary>
        /// Adds a new Footnote.
        /// </summary>
        public Footnote AddFootnote()
        {
            return Elements.AddFootnote();
        }

        /// <summary>
        /// Adds a text phrase to the formatted text.
        /// </summary>
        /// <param name="text">Content of the new text object.</param>
        /// <returns>Returns a new Text object.</returns>
        public Text AddText(string text)
        {
            return Elements.AddText(text);
        }

        /// <summary>
        /// Adds a new FormattedText.
        /// </summary>
        public FormattedText AddFormattedText()
        {
            return Elements.AddFormattedText();
        }

        /// <summary>
        /// Adds a new FormattedText object with the given format.
        /// </summary>
        public FormattedText AddFormattedText(TextFormat textFormat)
        {
            return Elements.AddFormattedText(textFormat);
        }

        /// <summary>
        /// Adds a new FormattedText with the given Font.
        /// </summary>
        public FormattedText AddFormattedText(Font font)
        {
            return Elements.AddFormattedText(font);
        }

        /// <summary>
        /// Adds a new FormattedText with the given text.
        /// </summary>
        public FormattedText AddFormattedText(string text)
        {
            return Elements.AddFormattedText(text);
        }

        /// <summary>
        /// Adds a new FormattedText object with the given text and format.
        /// </summary>
        public FormattedText AddFormattedText(string text, TextFormat textFormat)
        {
            return Elements.AddFormattedText(text, textFormat);
        }

        /// <summary>
        /// Adds a new FormattedText object with the given text and font.
        /// </summary>
        public FormattedText AddFormattedText(string text, Font font)
        {
            return Elements.AddFormattedText(text, font);
        }

        /// <summary>
        /// Adds a new FormattedText object with the given text and style.
        /// </summary>
        public FormattedText AddFormattedText(string text, string style)
        {
            return Elements.AddFormattedText(text, style);
        }

        /// <summary>
        /// Adds a new Hyperlink of Type "Local", i.e. the target is a Bookmark within the Document.
        /// </summary>
        public Hyperlink AddHyperlink(string bookmarkName)
        {
            return Elements.AddHyperlink(bookmarkName);
        }

        /// <summary>
        /// Adds a new Hyperlink.
        /// </summary>
        public Hyperlink AddHyperlink(string name, HyperlinkType type)
        {
            return Elements.AddHyperlink(name, type);
        }

        /// <summary>
        /// Adds a new Hyperlink of Type "ExternalBookmark", i.e. the target is a Bookmark in an external PDF Document.
        /// </summary>
        /// <param name="filename">The path to the target document.</param>
        /// <param name="bookmarkName">The Named Destination's name in the target document.</param>
        /// <param name="newWindow">Defines if the document shall be opened in a new window.
        /// If not set, the viewer application should behave in accordance with the current user preference.</param>
        public Hyperlink AddHyperlink(string filename, string bookmarkName, HyperlinkTargetWindow newWindow = HyperlinkTargetWindow.UserPreference)
        {
            return Elements.AddHyperlink(filename, bookmarkName, newWindow);
        }

        /// <summary>
        /// Adds a new Hyperlink of Type "EmbeddedDocument".
        /// The target is a Bookmark in an embedded Document in this Document.
        /// </summary>
        /// <param name="destinationPath">The path to the named destination through the embedded documents.
        /// The path is separated by '\' and the last segment is the name of the named destination.
        /// The other segments describe the route from the current (root or embedded) document to the embedded document holding the destination.
        /// ".." references to the parent, other strings refer to a child with this name in the EmbeddedFiles name dictionary.</param>
        /// <param name="newWindow">Defines if the HyperlinkType ExternalBookmark shall be opened in a new window.
        /// If not set, the viewer application should behave in accordance with the current user preference.</param>
        public Hyperlink AddHyperlinkToEmbeddedDocument(string destinationPath, HyperlinkTargetWindow newWindow = HyperlinkTargetWindow.UserPreference)
        {
            return Elements.AddHyperlinkToEmbeddedDocument(destinationPath, newWindow);
        }

        /// <summary>
        /// Adds a new Hyperlink of Type "EmbeddedDocument".
        /// The target is a Bookmark in an embedded Document in an external PDF Document.
        /// </summary>
        /// <param name="filename">The path to the target document.</param>
        /// <param name="destinationPath">The path to the named destination through the embedded documents in the target document.
        /// The path is separated by '\' and the last segment is the name of the named destination.
        /// The other segments describe the route from the root document to the embedded document.
        /// Each segment name refers to a child with this name in the EmbeddedFiles name dictionary.</param>
        /// <param name="newWindow">Defines if the HyperlinkType ExternalBookmark shall be opened in a new window.
        /// If not set, the viewer application should behave in accordance with the current user preference.</param>
        public Hyperlink AddHyperlinkToEmbeddedDocument(string filename, string destinationPath, HyperlinkTargetWindow newWindow = HyperlinkTargetWindow.UserPreference)
        {
            return Elements.AddHyperlinkToEmbeddedDocument(filename, destinationPath, newWindow);
        }

        /// <summary>
        /// Adds a new Image object.
        /// </summary>
        public Image AddImage(string fileName)
        {
            return Elements.AddImage(fileName);
        }

        /// <summary>
        /// Adds a Symbol object.
        /// </summary>
        public Character AddCharacter(SymbolName symbolType)
        {
            return Elements.AddCharacter(symbolType);
        }

        /// <summary>
        /// Adds one or more Symbol objects.
        /// </summary>
        public Character AddCharacter(SymbolName symbolType, int count)
        {
            return Elements.AddCharacter(symbolType, count);
        }

        /// <summary>
        /// Adds a Symbol object defined by a character.
        /// </summary>
        public Character AddCharacter(char ch)
        {
            return Elements.AddCharacter(ch);
        }

        /// <summary>
        /// Adds one or more Symbol objects defined by a character.
        /// </summary>
        public Character AddCharacter(char ch, int count)
        {
            return Elements.AddCharacter(ch, count);
        }

        /// <summary>
        /// Adds one or more Symbol objects defined by a character.
        /// </summary>
        public Character AddSpace(int count)
        {
            return Elements.AddSpace(count);
        }

        /// <summary>
        /// Adds a horizontal tab.
        /// </summary>
        public void AddTab()
        {
            Elements.AddTab();
        }

        /// <summary>
        /// Adds a line break.
        /// </summary>
        public void AddLineBreak()
        {
            Elements.AddLineBreak();
        }

        /// <summary>
        /// Adds a new Bookmark
        /// </summary>
        public void Add(BookmarkField bookmark)
        {
            Elements.Add(bookmark);
        }

        /// <summary>
        /// Adds a new PageField
        /// </summary>
        public void Add(PageField pageField)
        {
            Elements.Add(pageField);
        }

        /// <summary>
        /// Adds a new PageRefField
        /// </summary>
        public void Add(PageRefField pageRefField)
        {
            Elements.Add(pageRefField);
        }

        /// <summary>
        /// Adds a new NumPagesField
        /// </summary>
        public void Add(NumPagesField numPagesField)
        {
            Elements.Add(numPagesField);
        }

        /// <summary>
        /// Adds a new SectionField
        /// </summary>
        public void Add(SectionField sectionField)
        {
            Elements.Add(sectionField);
        }

        /// <summary>
        /// Adds a new SectionPagesField
        /// </summary>
        public void Add(SectionPagesField sectionPagesField)
        {
            Elements.Add(sectionPagesField);
        }

        /// <summary>
        /// Adds a new DateField
        /// </summary>
        public void Add(DateField dateField)
        {
            Elements.Add(dateField);
        }

        /// <summary>
        /// Adds a new InfoField
        /// </summary>
        public void Add(InfoField infoField)
        {
            Elements.Add(infoField);
        }

        /// <summary>
        /// Adds a new Footnote
        /// </summary>
        public void Add(Footnote footnote)
        {
            Elements.Add(footnote);
        }

        /// <summary>
        /// Adds a new Text
        /// </summary>
        public void Add(Text text)
        {
            Elements.Add(text);
        }

        /// <summary>
        /// Adds a new FormattedText
        /// </summary>
        public void Add(FormattedText formattedText)
        {
            Elements.Add(formattedText);
        }

        /// <summary>
        /// Adds a new Hyperlink
        /// </summary>
        public void Add(Hyperlink hyperlink)
        {
            Elements.Add(hyperlink);
        }

        /// <summary>
        /// Adds a new Image
        /// </summary>
        public void Add(Image image)
        {
            Elements.Add(image);
        }

        /// <summary>
        /// Adds a new Character
        /// </summary>
        public void Add(Character character)
        {
            Elements.Add(character);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the font object.
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
        /// Gets or sets the style name.
        /// </summary>
        public string Style
        {
            get { return _style.Value; }
            set { _style.Value = value; }
        }
        [DV]
        internal NString _style = NString.NullValue;

        /// <summary>
        /// Gets or sets the name of the font.
        /// </summary>
        [DV]
        public string FontName
        {
            get { return Font.Name; }
            set { Font.Name = value; }
        }

        /// <summary>
        /// Gets or sets the name of the font.
        /// For internal use only.
        /// </summary>
        [DV]
        internal string Name
        {
            get { return Font.Name; }
            set { Font.Name = value; }
        }

        /// <summary>
        /// Gets or sets the size in point.
        /// </summary>
        [DV]
        public Unit Size
        {
            get { return Font.Size; }
            set { Font.Size = value; }
        }

        /// <summary>
        /// Gets or sets the bold property.
        /// </summary>
        [DV]
        public bool Bold
        {
            get { return Font.Bold; }
            set { Font.Bold = value; }
        }

        /// <summary>
        /// Gets or sets the italic property.
        /// </summary>
        [DV]
        public bool Italic
        {
            get { return Font.Italic; }
            set { Font.Italic = value; }
        }

        /// <summary>
        /// Gets or sets the underline property.
        /// </summary>
        [DV]
        public Underline Underline
        {
            get { return Font.Underline; }
            set { Font.Underline = value; }
        }

        /// <summary>
        /// Gets or sets the color property.
        /// </summary>
        [DV]
        public Color Color
        {
            get { return Font.Color; }
            set { Font.Color = value; }
        }

        /// <summary>
        /// Gets or sets the superscript property.
        /// </summary>
        [DV]
        public bool Superscript
        {
            get { return Font.Superscript; }
            set { Font.Superscript = value; }
        }

        /// <summary>
        /// Gets or sets the subscript property.
        /// </summary>
        [DV]
        public bool Subscript
        {
            get { return Font.Subscript; }
            set { Font.Subscript = value; }
        }

        /// <summary>
        /// Gets the collection of paragraph elements that defines the FormattedText.
        /// </summary>
        public ParagraphElements Elements
        {
            get { return _elements ?? (_elements = new ParagraphElements(this)); }
            set
            {
                SetParent(value);
                _elements = value;
            }
        }
        [DV(ItemType = typeof(DocumentObject))]
        internal ParagraphElements _elements;
        #endregion

        #region Internal
        /// <summary>
        /// Converts FormattedText into DDL.
        /// </summary>
        internal override void Serialize(Serializer serializer)
        {
            bool isFormatted = false;
            if (!IsNull("Font"))
            {
                Font.Serialize(serializer);
                isFormatted = true;
            }
            else
            {
                if (!_style.IsNull)
                {
                    serializer.Write("\\font(\"" + Style + "\")");
                    isFormatted = true;
                }
            }

            if (isFormatted)
                serializer.Write("{");

            if (!IsNull("Elements"))
                Elements.Serialize(serializer);

            if (isFormatted)
                serializer.Write("}");
        }

        /// <summary>
        /// Allows the visitor object to visit the document object and its child objects.
        /// </summary>
        void IVisitable.AcceptVisitor(DocumentObjectVisitor visitor, bool visitChildren)
        {
            visitor.VisitFormattedText(this);

            if (visitChildren && _elements != null)
                ((IVisitable)_elements).AcceptVisitor(visitor, true);
        }

        /// <summary>
        /// Returns the meta object of this instance.
        /// </summary>
        internal override Meta Meta
        {
            get { return _meta ?? (_meta = new Meta(typeof(FormattedText))); }
        }
        static Meta _meta;
        #endregion
    }
}

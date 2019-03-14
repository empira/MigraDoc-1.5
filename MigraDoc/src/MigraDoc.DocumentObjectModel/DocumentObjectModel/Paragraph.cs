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
using MigraDoc.DocumentObjectModel.Internals;
using MigraDoc.DocumentObjectModel.Visitors;
using MigraDoc.DocumentObjectModel.Fields;
using MigraDoc.DocumentObjectModel.Shapes;

namespace MigraDoc.DocumentObjectModel
{
    /// <summary>
    /// Represents a paragraph which is used to build up a document with text.
    /// </summary>
    public class Paragraph : DocumentObject, IVisitable
    {
        /// <summary>
        /// Initializes a new instance of the Paragraph class.
        /// </summary>
        public Paragraph()
        { }

        /// <summary>
        /// Initializes a new instance of the Paragraph class with the specified parent.
        /// </summary>
        internal Paragraph(DocumentObject parent) : base(parent) { }

        #region Methods
        /// <summary>
        /// Creates a deep copy of this object.
        /// </summary>
        public new Paragraph Clone()
        {
            return (Paragraph)DeepCopy();
        }

        /// <summary>
        /// Implements the deep copy of the object.
        /// </summary>
        protected override object DeepCopy()
        {
            Paragraph paragraph = (Paragraph)base.DeepCopy();
            if (paragraph._format != null)
            {
                paragraph._format = paragraph._format.Clone();
                paragraph._format._parent = paragraph;
            }
            if (paragraph._elements != null)
            {
                paragraph._elements = paragraph._elements.Clone();
                paragraph._elements._parent = paragraph;
            }
            return paragraph;
        }

        /// <summary>
        /// Adds a text phrase to the paragraph.
        /// </summary>
        public Text AddText(String text)
        {
            return Elements.AddText(text);
        }

        /// <summary>
        /// Adds a single character repeated the specified number of times to the paragraph.
        /// </summary>
        public Text AddChar(char ch, int count)
        {
            return Elements.AddChar(ch, count);
        }

        /// <summary>
        /// Adds a single character to the paragraph.
        /// </summary>
        public Text AddChar(char ch)
        {
            return Elements.AddChar(ch);
        }

        /// <summary>
        /// Adds one or more Symbol objects.
        /// </summary>
        public Character AddCharacter(SymbolName symbolType, int count)
        {
            return Elements.AddCharacter(symbolType, count);
        }

        /// <summary>
        /// Adds a Symbol object.
        /// </summary>
        public Character AddCharacter(SymbolName symbolType)
        {
            return Elements.AddCharacter(symbolType);
        }

        /// <summary>
        /// Adds one or more Symbol objects defined by a character.
        /// </summary>
        public Character AddCharacter(char ch, int count)
        {
            return Elements.AddCharacter(ch, count);
        }

        /// <summary>
        /// Adds a Symbol object defined by a character.
        /// </summary>
        public Character AddCharacter(char ch)
        {
            return Elements.AddCharacter(ch);
        }

        /// <summary>
        /// Adds a space character as many as count.
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
        /// <param name="newWindow">Defines if the HyperlinkType ExternalBookmark shall be opened in a new window.
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
        /// Adds a new Bookmark.
        /// </summary>
        /// <param name="name">The name of the bookmark.</param>
        /// <param name="prepend">True, if the bookmark shall be inserted at the beginning of the paragraph.</param>
        public BookmarkField AddBookmark(string name, bool prepend = true)
        {
            return Elements.AddBookmark(name, prepend);
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
        /// Adds a new Image object
        /// </summary>
        public Image AddImage(string fileName)
        {
            return Elements.AddImage(fileName);
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
        /// Gets or sets the ParagraphFormat object of the paragraph.
        /// </summary>
        public ParagraphFormat Format
        {
            get { return _format ?? (_format = new ParagraphFormat(this)); }
            set
            {
                SetParent(value);
                _format = value;
            }
        }
        [DV]
        internal ParagraphFormat _format;

        /// <summary>
        /// Gets the collection of document objects that defines the paragraph.
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
        [DV]
        internal ParagraphElements _elements;

        /// <summary>
        /// Gets or sets a comment associated with this object.
        /// </summary>
        public string Comment
        {
            get { return _comment.Value; }
            set { _comment.Value = value; }
        }
        [DV]
        internal NString _comment = NString.NullValue;
        #endregion

        #region Internal
        /// <summary>
        /// Allows the visitor object to visit the document object and its child objects.
        /// </summary>
        void IVisitable.AcceptVisitor(DocumentObjectVisitor visitor, bool visitChildren)
        {
            visitor.VisitParagraph(this);

            if (visitChildren && _elements != null)
                ((IVisitable)_elements).AcceptVisitor(visitor, true);
        }

        /// <summary>
        /// For internal use only.
        /// </summary>
        internal bool SerializeContentOnly
        {
            get { return _serializeContentOnly; }
            set { _serializeContentOnly = value; }
        }
        bool _serializeContentOnly;

        /// <summary>
        /// Converts Paragraph into DDL.
        /// </summary>
        internal override void Serialize(Serializer serializer)
        {
            if (!_serializeContentOnly)
            {
                serializer.WriteComment(_comment.Value);
                serializer.WriteLine("\\paragraph");

                int pos = serializer.BeginAttributes();

                if (_style.Value != "")
                    serializer.WriteLine("Style = \"" + _style.Value + "\"");

                if (!IsNull("Format"))
                    _format.Serialize(serializer, "Format", null);

                serializer.EndAttributes(pos);

                serializer.BeginContent();
                if (!IsNull("Elements"))
                    Elements.Serialize(serializer);
                serializer.CloseUpLine();
                serializer.EndContent();
            }
            else
            {
                Elements.Serialize(serializer);
                serializer.CloseUpLine();
            }
        }

        /// <summary>
        /// Returns the meta object of this instance.
        /// </summary>
        internal override Meta Meta
        {
            get { return _meta ?? (_meta = new Meta(typeof(Paragraph))); }
        }

        /// <summary>
        /// Returns an array of Paragraphs that are separated by parabreaks. Null if no parabreak is found.
        /// </summary>
        internal Paragraph[] SplitOnParaBreak()
        {
            if (_elements == null)
                return null;

            int startIdx = 0;
            List<Paragraph> paragraphs = new List<Paragraph>();
            for (int idx = 0; idx < Elements.Count; ++idx)
            {
                DocumentObject element = Elements[idx];
                if (element is Character)
                {
                    Character character = (Character)element;
                    if (character.SymbolName == SymbolName.ParaBreak)
                    {
                        Paragraph paragraph = new Paragraph();
                        paragraph.Format = Format.Clone();
                        paragraph.Style = Style;
                        paragraph.Elements = SubsetElements(startIdx, idx - 1);
                        startIdx = idx + 1;
                        paragraphs.Add(paragraph);
                    }
                }
            }
            if (startIdx == 0) //No paragraph breaks given.
                return null;
            else
            {
                Paragraph paragraph = new Paragraph();
                paragraph.Format = Format.Clone();
                paragraph.Style = Style;
                paragraph.Elements = SubsetElements(startIdx, _elements.Count - 1);
                paragraphs.Add(paragraph);

                return paragraphs.ToArray();
            }
        }

        /// <summary>
        /// Gets a subset of the paragraphs elements.
        /// </summary>
        /// <param name="startIdx">Start index of the required subset.</param>
        /// <param name="endIdx">End index of the required subset.</param>
        /// <returns>A ParagraphElements object with cloned elements.</returns>
        private ParagraphElements SubsetElements(int startIdx, int endIdx)
        {
            ParagraphElements paragraphElements = new ParagraphElements();
            for (int idx = startIdx; idx <= endIdx; ++idx)
                paragraphElements.Add((DocumentObject)_elements[idx].Clone());
            return paragraphElements;
        }
        static Meta _meta;
        #endregion
    }
}

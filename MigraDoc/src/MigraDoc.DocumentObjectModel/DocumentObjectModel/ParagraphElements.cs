#region MigraDoc - Creating Documents on the Fly
//
// Authors:
//   Stefan Lange
//   Klaus Potzesny
//   David Stephensen
//
// Copyright (c) 2001-2017 empira Software GmbH, Cologne Area (Germany)
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
using MigraDoc.DocumentObjectModel.Fields;
using MigraDoc.DocumentObjectModel.Shapes;

namespace MigraDoc.DocumentObjectModel
{
    /// <summary>
    /// A ParagraphElements collection contains the individual objects of a paragraph.
    /// </summary>
    public class ParagraphElements : DocumentObjectCollection
    {
        /// <summary>
        /// Initializes a new instance of the ParagraphElements class.
        /// </summary>
        public ParagraphElements()
        { }

        /// <summary>
        /// Initializes a new instance of the ParagraphElements class with the specified parent.
        /// </summary>
        internal ParagraphElements(DocumentObject parent) : base(parent) { }

        /// <summary>
        /// Gets a ParagraphElement by its index.
        /// </summary>
        public new DocumentObject this[int index]
        {
            get { return base[index] as DocumentObject; }
        }

        #region Methods
        /// <summary>
        /// Creates a deep copy of this object.
        /// </summary>
        public new ParagraphElements Clone()
        {
            return (ParagraphElements)DeepCopy();
        }

        /// <summary>
        /// Adds a Text object.
        /// </summary>
        /// <param name="text">Content of the new Text object.</param>
        /// <returns>Returns a new Text object.</returns>
        public Text AddText(string text)
        {
            if (text == null)
                throw new ArgumentNullException("text");
#if true
            Text txt = null;
            string[] lines = text.Split('\n');
            int lineCount = lines.Length;
            for (int line = 0; line < lineCount; line++)
            {
                string[] tabParts = lines[line].Split('\t');
                int count = tabParts.Length;
                for (int idx = 0; idx < count; idx++)
                {
                    if (tabParts[idx].Length != 0)
                    {
                        txt = new Text(tabParts[idx]);
                        Add(txt);
                    }
                    if (idx < count - 1)
                        AddTab();
                }
                if (line < lineCount - 1)
                    AddLineBreak();
            }
            return txt;
#else
      Text txt = new Text();
      txt.Content = text;
      Add(txt);
      return txt;
#endif
        }

        /// <summary>
        /// Adds a single character repeated the specified number of times to the paragraph.
        /// </summary>
        public Text AddChar(char ch, int count)
        {
            return AddText(new string(ch, count));
        }

        /// <summary>
        /// Adds a single character to the paragraph.
        /// </summary>
        public Text AddChar(char ch)
        {
            return AddText(new string(ch, 1));
        }

        /// <summary>
        /// Adds a Character object.
        /// </summary>
        public Character AddCharacter(SymbolName symbolType)
        {
            return AddCharacter(symbolType, 1);
        }

        /// <summary>
        /// Adds one or more Character objects.
        /// </summary>
        public Character AddCharacter(SymbolName symbolType, int count)
        {
            Character character = new Character();
            Add(character);
            character.SymbolName = symbolType;
            character.Count = count;
            return character;
        }

        /// <summary>
        /// Adds a Character object defined by a character.
        /// </summary>
        public Character AddCharacter(char ch)
        {
            return AddCharacter((SymbolName)ch, 1);
        }

        /// <summary>
        /// Adds one or more Character objects defined by a character.
        /// </summary>
        public Character AddCharacter(char ch, int count)
        {
            return AddCharacter((SymbolName)ch, count);
        }

        /// <summary>
        /// Adds a space character as many as count.
        /// </summary>
        public Character AddSpace(int count)
        {
            return AddCharacter(SymbolName.Blank, count);
        }

        /// <summary>
        /// Adds a horizontal tab.
        /// </summary>
        public Character AddTab()
        {
            return AddCharacter(SymbolName.Tab, 1);
        }

        /// <summary>
        /// Adds a line break.
        /// </summary>
        public Character AddLineBreak()
        {
            return AddCharacter(SymbolName.LineBreak, 1);
        }

        /// <summary>
        /// Adds a new FormattedText.
        /// </summary>
        public FormattedText AddFormattedText()
        {
            FormattedText formattedText = new FormattedText();
            Add(formattedText);
            return formattedText;
        }

        /// <summary>
        /// Adds a new FormattedText object with the given format.
        /// </summary>
        public FormattedText AddFormattedText(TextFormat textFormat)
        {
            FormattedText formattedText = AddFormattedText();

            if ((textFormat & TextFormat.Bold) == TextFormat.Bold)
                formattedText.Bold = true;
            if ((textFormat & TextFormat.NotBold) == TextFormat.NotBold)
                formattedText.Bold = false;
            if ((textFormat & TextFormat.Italic) == TextFormat.Italic)
                formattedText.Italic = true;
            if ((textFormat & TextFormat.NotItalic) == TextFormat.NotItalic)
                formattedText.Italic = false;
            if ((textFormat & TextFormat.Underline) == TextFormat.Underline)
                formattedText.Underline = Underline.Single;
            if ((textFormat & TextFormat.NoUnderline) == TextFormat.NoUnderline)
                formattedText.Underline = Underline.None;

            return formattedText;
        }

        /// <summary>
        /// Adds a new FormattedText with the given Font.
        /// </summary>
        public FormattedText AddFormattedText(Font font)
        {
            FormattedText formattedText = new FormattedText();
            formattedText.Font.ApplyFont(font);
            Add(formattedText);
            return formattedText;
        }

        /// <summary>
        /// Adds a new FormattedText with the given text.
        /// </summary>
        public FormattedText AddFormattedText(string text)
        {
            FormattedText formattedText = new FormattedText();
            formattedText.AddText(text);
            Add(formattedText);
            return formattedText;
        }

        /// <summary>
        /// Adds a new FormattedText object with the given text and format.
        /// </summary>
        public FormattedText AddFormattedText(string text, TextFormat textFormat)
        {
            FormattedText formattedText = AddFormattedText(textFormat);
            formattedText.AddText(text);
            return formattedText;
        }

        /// <summary>
        /// Adds a new FormattedText object with the given text and font.
        /// </summary>
        public FormattedText AddFormattedText(string text, Font font)
        {
            FormattedText formattedText = AddFormattedText(font);
            formattedText.AddText(text);
            return formattedText;
        }

        /// <summary>
        /// Adds a new FormattedText object with the given text and style.
        /// </summary>
        public FormattedText AddFormattedText(string text, string style)
        {
            FormattedText formattedText = AddFormattedText(text);
            formattedText.Style = style;
            return formattedText;
        }

        /// <summary>
        /// Adds a new Hyperlink of Type "Local", i.e. the Target is a Bookmark within the Document
        /// </summary>
        public Hyperlink AddHyperlink(string name)
        {
            Hyperlink hyperlink = new Hyperlink();
            hyperlink.Name = name;
            Add(hyperlink);
            return hyperlink;
        }

        /// <summary>
        /// Adds a new Hyperlink
        /// </summary>
        public Hyperlink AddHyperlink(string name, HyperlinkType type)
        {
            Hyperlink hyperlink = new Hyperlink();
            hyperlink.Name = name;
            hyperlink.Type = type;
            Add(hyperlink);
            return hyperlink;
        }

        /// <summary>
        /// Adds a new Bookmark.
        /// </summary>
        public BookmarkField AddBookmark(string name)
        {
            BookmarkField fieldBookmark = new BookmarkField();
            fieldBookmark.Name = name;
            Add(fieldBookmark);
            return fieldBookmark;
        }

        /// <summary>
        /// Adds a new PageField.
        /// </summary>
        public PageField AddPageField()
        {
            PageField fieldPage = new PageField();
            Add(fieldPage);
            return fieldPage;
        }

        /// <summary>
        /// Adds a new RefFieldPage.
        /// </summary>
        public PageRefField AddPageRefField(string name)
        {
            PageRefField fieldPageRef = new PageRefField();
            fieldPageRef.Name = name;
            Add(fieldPageRef);
            return fieldPageRef;
        }

        /// <summary>
        /// Adds a new NumPagesField.
        /// </summary>
        public NumPagesField AddNumPagesField()
        {
            NumPagesField fieldNumPages = new NumPagesField();
            Add(fieldNumPages);
            return fieldNumPages;
        }

        /// <summary>
        /// Adds a new SectionField.
        /// </summary>
        public SectionField AddSectionField()
        {
            SectionField fieldSection = new SectionField();
            Add(fieldSection);
            return fieldSection;
        }

        /// <summary>
        /// Adds a new SectionPagesField.
        /// </summary>
        public SectionPagesField AddSectionPagesField()
        {
            SectionPagesField fieldSectionPages = new SectionPagesField();
            Add(fieldSectionPages);
            return fieldSectionPages;
        }

        /// <summary>
        /// Adds a new DateField.
        /// </summary>
        /// 
        public DateField AddDateField()
        {
            DateField fieldDate = new DateField();
            Add(fieldDate);
            return fieldDate;
        }

        /// <summary>
        /// Adds a new DateField with the given format.
        /// </summary>
        public DateField AddDateField(string format)
        {
            DateField fieldDate = new DateField();
            fieldDate.Format = format;
            Add(fieldDate);
            return fieldDate;
        }

        /// <summary>
        /// Adds a new InfoField with the given type.
        /// </summary>
        public InfoField AddInfoField(InfoFieldType iType)
        {
            InfoField fieldInfo = new InfoField();
            fieldInfo.Name = iType.ToString();
            Add(fieldInfo);
            return fieldInfo;
        }

        /// <summary>
        /// Adds a new Footnote with the specified Text.
        /// </summary>
        public Footnote AddFootnote(string text)
        {
            Footnote footnote = new Footnote();
            Paragraph par = footnote.Elements.AddParagraph();
            par.AddText(text);
            Add(footnote);
            return footnote;
        }

        /// <summary>
        /// Adds a new Footnote.
        /// </summary>
        public Footnote AddFootnote()
        {
            Footnote footnote = new Footnote();
            Add(footnote);
            return footnote;
        }

        /// <summary>
        /// Adds a new Image.
        /// </summary>
        public Image AddImage(string name)
        {
            Image image = new Image();
            image.Name = name;
            Add(image);
            return image;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Add(DocumentObject docObj)
        {
            base.Add(docObj);
        }
        #endregion

        #region Internal
        /// <summary>
        /// Converts ParagraphElements into DDL.
        /// </summary>
        internal override void Serialize(Serializer serializer)
        {
            int count = Count;
            for (int index = 0; index < count; ++index)
            {
                DocumentObject element = this[index];
                element.Serialize(serializer);
            }
        }

        /// <summary>
        /// Returns the meta object of this instance.
        /// </summary>
        internal override Meta Meta
        {
            get { return _meta ?? (_meta = new Meta(typeof(ParagraphElements))); }
        }
        static Meta _meta;
        #endregion
    }
}

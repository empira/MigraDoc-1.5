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
using MigraDoc.DocumentObjectModel.Shapes.Charts;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.DocumentObjectModel.Shapes;

namespace MigraDoc.DocumentObjectModel
{
    /// <summary>
    /// A Section is a collection of document objects sharing the same header, footer, 
    /// and page setup.
    /// </summary>
    public class Section : DocumentObject, IVisitable
    {
        /// <summary>
        /// Initializes a new instance of the Section class.
        /// </summary>
        public Section()
        { }

        /// <summary>
        /// Initializes a new instance of the Section class with the specified parent.
        /// </summary>
        internal Section(DocumentObject parent) : base(parent) { }

        #region Methods
        /// <summary>
        /// Creates a deep copy of this object.
        /// </summary>
        public new Section Clone()
        {
            return (Section)DeepCopy();
        }

        /// <summary>
        /// Implements the deep copy of the object.
        /// </summary>
        protected override object DeepCopy()
        {
            Section section = (Section)base.DeepCopy();
            if (section._pageSetup != null)
            {
                section._pageSetup = section._pageSetup.Clone();
                section._pageSetup._parent = section;
            }
            if (section._headers != null)
            {
                section._headers = section._headers.Clone();
                section._headers._parent = section;
            }
            if (section._footers != null)
            {
                section._footers = section._footers.Clone();
                section._footers._parent = section;
            }
            if (section._elements != null)
            {
                section._elements = section._elements.Clone();
                section._elements._parent = section;
            }
            return section;
        }

        /// <summary>
        /// Gets the previous section.
        /// </summary>
        public Section PreviousSection()
        {
            Sections sections = Parent as Sections;
            int index = sections.IndexOf(this);
            if (index > 0)
                return sections[index - 1];
            return null;
        }

        /// <summary>
        /// Adds a new paragraph to the section.
        /// </summary>
        public Paragraph AddParagraph()
        {
            return Elements.AddParagraph();
        }

        /// <summary>
        /// Adds a new paragraph with the specified text to the section.
        /// </summary>
        public Paragraph AddParagraph(string paragraphText)
        {
            return Elements.AddParagraph(paragraphText);
        }

        /// <summary>
        /// Adds a new paragraph with the specified text and style to the section.
        /// </summary>
        public Paragraph AddParagraph(string paragraphText, string style)
        {
            return Elements.AddParagraph(paragraphText, style);
        }

        /// <summary>
        /// Adds a new chart with the specified type to the section.
        /// </summary>
        public Chart AddChart(ChartType type)
        {
            return Elements.AddChart(type);
        }

        /// <summary>
        /// Adds a new chart to the section.
        /// </summary>
        public Chart AddChart()
        {
            return Elements.AddChart();
        }

        /// <summary>
        /// Adds a new table to the section.
        /// </summary>
        public Table AddTable()
        {
            return Elements.AddTable();
        }

        /// <summary>
        /// Adds a manual page break.
        /// </summary>
        public void AddPageBreak()
        {
            Elements.AddPageBreak();
        }

        /// <summary>
        /// Adds a new Image to the section.
        /// </summary>
        public Image AddImage(string fileName)
        {
            return Elements.AddImage(fileName);
        }

        /// <summary>
        /// Adds a new textframe to the section.
        /// </summary>
        public TextFrame AddTextFrame()
        {
            return Elements.AddTextFrame();
        }

        /// <summary>
        /// Adds a new paragraph to the section.
        /// </summary>
        public void Add(Paragraph paragraph)
        {
            Elements.Add(paragraph);
        }

        /// <summary>
        /// Adds a new chart to the section.
        /// </summary>
        public void Add(Chart chart)
        {
            Elements.Add(chart);
        }

        /// <summary>
        /// Adds a new table to the section.
        /// </summary>
        public void Add(Table table)
        {
            Elements.Add(table);
        }

        /// <summary>
        /// Adds a new image to the section.
        /// </summary>
        public void Add(Image image)
        {
            Elements.Add(image);
        }

        /// <summary>
        /// Adds a new text frame to the section.
        /// </summary>
        public void Add(TextFrame textFrame)
        {
            Elements.Add(textFrame);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the PageSetup object
        /// </summary>
        public PageSetup PageSetup
        {
            get { return _pageSetup ?? (_pageSetup = new PageSetup(this)); }
            set
            {
                SetParent(value);
                _pageSetup = value;
            }
        }
        [DV]
        internal PageSetup _pageSetup;

        /// <summary>
        /// Gets the HeadersFooters collection containing the headers.
        /// </summary>
        public HeadersFooters Headers
        {
            get { return _headers ?? (_headers = new HeadersFooters(this)); }
            set
            {
                SetParent(value);
                _headers = value;
            }
        }
        [DV]
        internal HeadersFooters _headers;

        /// <summary>
        /// Gets the HeadersFooters collection containing the footers.
        /// </summary>
        public HeadersFooters Footers
        {
            get { return _footers ?? (_footers = new HeadersFooters(this)); }
            set
            {
                SetParent(value);
                _footers = value;
            }
        }
        [DV]
        internal HeadersFooters _footers;

        /// <summary>
        /// Gets the document elements that build the section's content.
        /// </summary>
        public DocumentElements Elements
        {
            get { return _elements ?? (_elements = new DocumentElements(this)); }
            set
            {
                SetParent(value);
                _elements = value;
            }
        }
        [DV]
        internal DocumentElements _elements;

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

        /// <summary>
        /// Gets the last paragraph of this section, or null, if no paragraph exists is this section.
        /// </summary>
        public Paragraph LastParagraph
        {
            get
            {
                int count = _elements.Count;
                for (int idx = count - 1; idx >= 0; idx--)
                {
                    if (_elements[idx] is Paragraph)
                        return (Paragraph)_elements[idx];
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the last table of this section, or null, if no table exists is this section.
        /// </summary>
        public Table LastTable
        {
            get
            {
                int count = _elements.Count;
                for (int idx = count - 1; idx >= 0; idx--)
                {
                    if (_elements[idx] is Table)
                        return (Table)_elements[idx];
                }
                return null;
            }
        }
        #endregion

        #region Internal
        /// <summary>
        /// Converts Section into DDL.
        /// </summary>
        internal override void Serialize(Serializer serializer)
        {
            serializer.WriteComment(_comment.Value);
            serializer.WriteLine("\\section");

            int pos = serializer.BeginAttributes();
            if (!IsNull("PageSetup"))
                PageSetup.Serialize(serializer);
            serializer.EndAttributes(pos);

            serializer.BeginContent();
            if (!IsNull("headers"))
                _headers.Serialize(serializer);
            if (!IsNull("footers"))
                _footers.Serialize(serializer);
            if (!IsNull("elements"))
                _elements.Serialize(serializer);

            serializer.EndContent();
        }

        /// <summary>
        /// Allows the visitor object to visit the document object and its child objects.
        /// </summary>
        void IVisitable.AcceptVisitor(DocumentObjectVisitor visitor, bool visitChildren)
        {
            visitor.VisitSection(this);

            if (visitChildren && _headers != null)
                ((IVisitable)_headers).AcceptVisitor(visitor, true);
            if (visitChildren && _footers != null)
                ((IVisitable)_footers).AcceptVisitor(visitor, true);
            if (visitChildren && _elements != null)
                ((IVisitable)_elements).AcceptVisitor(visitor, true);
        }

        /// <summary>
        /// Returns the meta object of this instance.
        /// </summary>
        internal override Meta Meta
        {
            get { return _meta ?? (_meta = new Meta(typeof(Section))); }
        }
        static Meta _meta;
        #endregion
    }
}

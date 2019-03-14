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
using System.Collections;
using System.Collections.Generic;
using MigraDoc.DocumentObjectModel.Visitors;

namespace MigraDoc.DocumentObjectModel
{
    /// <summary>
    /// Base class of all collections of the MigraDoc Document Object Model.
    /// </summary>
    public abstract class DocumentObjectCollection : DocumentObject, IList, IVisitable
    {
        /// <summary>
        /// Initializes a new instance of the DocumentObjectCollection class.
        /// </summary>
        internal DocumentObjectCollection()
        {
            _elements = new List<object>();
        }

        /// <summary>
        /// Initializes a new instance of the DocumentObjectCollection class with the specified parent.
        /// </summary>
        internal DocumentObjectCollection(DocumentObject parent)
            : base(parent)
        {
            _elements = new List<object>();
        }

        /// <summary>
        /// Gets the first value in the Collection, if there is any, otherwise null.
        /// </summary>
        public DocumentObject First
        {
            get
            {
                return Count > 0 ? this[0] : null;
            }
        }

        /// <summary>
        /// Creates a deep copy of this object.
        /// </summary>
        public new DocumentObjectCollection Clone()
        {
            return (DocumentObjectCollection)DeepCopy();
        }

        /// <summary>
        /// Implements the deep copy of the object.
        /// </summary>
        protected override object DeepCopy()
        {
            DocumentObjectCollection coll = (DocumentObjectCollection)base.DeepCopy();

            int count = Count;
            coll._elements = new List<object>(count);
            for (int index = 0; index < count; ++index)
            {
                DocumentObject doc = this[index];
                if (doc != null)
                {
                    doc = doc.Clone() as DocumentObject;
                    doc._parent = coll;
                }
                coll._elements.Add(doc);
            }
            return coll;
        }

        /// <summary>
        /// Copies the entire collection to a compatible one-dimensional System.Array,
        /// starting at the specified index of the target array.
        /// </summary>
        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException("TODO");
            //this .elements.CopyTo(array, index);
        }

        /// <summary>
        /// Gets a value indicating whether the Collection is read-only.
        /// </summary>
        bool IList.IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether the Collection has a fixed size.
        /// </summary>
        bool IList.IsFixedSize
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether access to the Collection is synchronized.
        /// </summary>
        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        /// <summary>
        /// Gets an object that can be used to synchronize access to the collection.
        /// </summary>
        object ICollection.SyncRoot
        {
            get { return null; }
        }

        /// <summary>
        /// Gets the number of elements actually contained in the collection.
        /// </summary>
        public int Count
        {
            get { return _elements.Count; }
        }

        /// <summary>
        /// Removes all elements from the collection.
        /// </summary>
        public void Clear()
        {
            ((IList)this).Clear();
        }

        /// <summary>
        /// Inserts an object at the specified index.
        /// </summary>
        public virtual void InsertObject(int index, DocumentObject val)
        {
            SetParent(val);
            ((IList)this).Insert(index, val);
            // Call ResetCachedValues for all objects moved by the Insert operation.
            int count = ((IList)this).Count;
            for (int idx = index + 1; idx < count; ++idx)
            {
                DocumentObject obj = (DocumentObject)((IList)this)[idx];
                obj.ResetCachedValues();
            }
        }

        /// <summary>
        /// Determines the index of a specific item in the collection.
        /// </summary>
        public int IndexOf(DocumentObject val)
        {
            return ((IList)this).IndexOf(val);
        }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        public virtual DocumentObject this[int index]
        {
            get { return _elements[index] as DocumentObject; }
            set
            {
                SetParent(value);
                _elements[index] = value;
            }
        }

        /// <summary>
        /// Gets the last element or null, if no such element exists.
        /// </summary>
        public DocumentObject LastObject
        {
            get
            {
                int count = _elements.Count;
                if (count > 0)
                    return (DocumentObject)_elements[count - 1];
                return null;
            }
        }

        /// <summary>
        /// Removes the element at the specified index.
        /// </summary>
        public void RemoveObjectAt(int index)
        {
            ((IList)this).RemoveAt(index);
            // Call ResetCachedValues for all objects moved by the RemoveAt operation.
            int count = ((IList)this).Count;
            for (int idx = index; idx < count; ++idx)
            {
                DocumentObject obj = (DocumentObject)((IList)this)[idx];
                obj.ResetCachedValues();
            }
        }

        /// <summary>
        /// Inserts the object into the collection and sets its parent.
        /// </summary>
        public virtual void Add(DocumentObject value)
        {
            SetParent(value);
            _elements.Add(value);
        }

        /// <summary>
        /// Determines whether this instance is null.
        /// </summary>
        public override bool IsNull()
        {
            if (!Meta.IsNull(this))
                return false;
            if (_elements == null)
                return true;
            foreach (DocumentObject docObject in _elements)
            {
                if (docObject != null && !docObject.IsNull())
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Allows the visitor object to visit the document object and its child objects.
        /// </summary>
        void IVisitable.AcceptVisitor(DocumentObjectVisitor visitor, bool visitChildren)
        {
            visitor.VisitDocumentObjectCollection(this);

            foreach (DocumentObject docobj in this)
            {
                IVisitable visitable = docobj as IVisitable;
                if (visitable != null)
                    visitable.AcceptVisitor(visitor, visitChildren);
            }
        }

        /// <summary>
        /// Returns an enumerator that can iterate through this collection.
        /// </summary>
        public IEnumerator GetEnumerator()
        {
            return _elements.GetEnumerator();
        }

        List<object> _elements;

        #region IList Members
        /// <summary>
        /// Gets or sets the element at the specified index. 
        /// </summary>
        object IList.this[int index]
        {
            get { return _elements[index]; }
            set { _elements[index] = value; }
        }

        /// <summary>
        /// Removes the item at the specified index from the Collection.
        /// </summary>
        void IList.RemoveAt(int index)
        {
            _elements.RemoveAt(index);
        }

        /// <summary>
        /// Inserts an object at the specified index.
        /// </summary>
        void IList.Insert(int index, object value)
        {
            _elements.Insert(index, value);
        }

        /// <summary>
        /// Removes the first occurrence of the specific object.
        /// </summary>
        void IList.Remove(object value)
        {
            _elements.Remove(value);
        }

        /// <summary>
        /// Determines whether an element exists.
        /// </summary>
        bool IList.Contains(object value)
        {
            return _elements.Contains(value);
        }

        /// <summary>
        /// Determines the index of a specific item in the Collection.
        /// </summary>
        int IList.IndexOf(object value)
        {
            return _elements.IndexOf(value);
        }

        /// <summary>
        /// Adds an item to the Collection.
        /// </summary>
        int IList.Add(object value)
        {
            _elements.Add(value);
            return _elements.Count - 1;
        }

        /// <summary>
        /// Removes all items from the Collection.
        /// </summary>
        void IList.Clear()
        {
            _elements.Clear();
        }
        #endregion
    }
}

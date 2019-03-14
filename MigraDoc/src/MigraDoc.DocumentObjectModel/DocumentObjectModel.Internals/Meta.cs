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
using System.Reflection;

namespace MigraDoc.DocumentObjectModel.Internals
{
    /// <summary>
    /// Meta class for document objects.
    /// </summary>
    public sealed class Meta
    {
        /// <summary>
        /// Initializes a new instance of the DomMeta class.
        /// </summary>
        public Meta(Type documentObjectType)
        {
            AddValueDescriptors(this, documentObjectType);
        }

        /// <summary>
        /// Gets the meta object of the specified document object.
        /// </summary>
        /// <param name="documentObject">The document object the meta is returned for.</param>
        public static Meta GetMeta(DocumentObject documentObject)
        {
            return documentObject.Meta;
        }

        /// <summary>
        /// Gets the object specified by name from dom.
        /// </summary>
        public object GetValue(DocumentObject dom, string name, GV flags)
        {
            int dot = name.IndexOf('.');
            if (dot == 0)
                throw new ArgumentException(DomSR.InvalidValueName(name));
            string trail = null;
            if (dot > 0)
            {
                trail = name.Substring(dot + 1);
                name = name.Substring(0, dot);
            }
            ValueDescriptor vd = _vds[name];
            if (vd == null)
                throw new ArgumentException(DomSR.InvalidValueName(name));

            object value = vd.GetValue(dom, flags);
            if (value == null && flags == GV.GetNull)  //??? also for GV.ReadOnly?
                return null;

            //REVIEW DaSt: Create object in case of GV.ReadWrite?
            if (trail != null)
            {
                if (value == null || trail == "")
                    throw new ArgumentException(DomSR.InvalidValueName(name));
                DocumentObject doc = value as DocumentObject;
                if (doc == null)
                    throw new ArgumentException(DomSR.InvalidValueName(name));
                value = doc.GetValue(trail, flags);
            }
            return value;
        }

        /// <summary>
        /// Sets the member of dom specified by name to val.
        /// If a member with the specified name does not exist an ArgumentException will be thrown.
        /// </summary>
        public void SetValue(DocumentObject dom, string name, object val)
        {
            int dot = name.IndexOf('.');
            if (dot == 0)
                throw new ArgumentException(DomSR.InvalidValueName(name));
            string trail = null;
            if (dot > 0)
            {
                trail = name.Substring(dot + 1);
                name = name.Substring(0, dot);
            }
            ValueDescriptor vd = _vds[name];
            if (vd == null)
                throw new ArgumentException(DomSR.InvalidValueName(name));

            if (trail != null)
            {
                //REVIEW DaSt: dom.GetValue(name) and call SetValue recursively,
                //             or dom.GetValue(name.BisVorletzteElement) and then call SetValue?
                DocumentObject doc = (DocumentObject)dom.GetValue(name);
                doc.SetValue(trail, val);
            }
            else
                vd.SetValue(dom, val);
        }

        /// <summary>
        /// Determines whether this meta contains a value with the specified name.
        /// </summary>
        public bool HasValue(string name)
        {
            ValueDescriptor vd = _vds[name];
            return vd != null;
        }

        /// <summary>
        /// Sets the member of dom specified by name to null.
        /// If a member with the specified name does not exist an ArgumentException will be thrown.
        /// </summary>
        public void SetNull(DocumentObject dom, string name)
        {
            ValueDescriptor vd = _vds[name];
            if (vd == null)
                throw new ArgumentException(DomSR.InvalidValueName(name));

            vd.SetNull(dom);
        }

        /// <summary>
        /// Determines whether the member of dom specified by name is null.
        /// If a member with the specified name does not exist an ArgumentException will be thrown.
        /// </summary>
        public /* not virtual */ bool IsNull(DocumentObject dom, string name)
        {
            //bool isNull = false;
            int dot = name.IndexOf('.');
            if (dot == 0)
                throw new ArgumentException(DomSR.InvalidValueName(name));
            string trail = null;
            if (dot > 0)
            {
                trail = name.Substring(dot + 1);
                name = name.Substring(0, dot);
            }
            ValueDescriptor vd = _vds[name];
            if (vd == null)
                throw new ArgumentException(DomSR.InvalidValueName(name));

            if (vd is NullableDescriptor || vd is ValueTypeDescriptor)
            {
                if (trail != null)
                    throw new ArgumentException(DomSR.InvalidValueName(name));
                return vd.IsNull(dom);
            }
            DocumentObject docObj = (DocumentObject)vd.GetValue(dom, GV.ReadOnly);
            if (docObj == null)
                return true;
            if (trail != null)
                return docObj.IsNull(trail);
            return docObj.IsNull();

            //      DomValueDescriptor vd = vds[name];
            //      if (vd == null)
            //        throw new ArgumentException(DomSR.InvalidValueName(name));
            //      
            //      return vd.IsNull(dom);
        }

        /// <summary>
        /// Sets all members of the specified dom to null.
        /// </summary>
        public /*virtual*/ void SetNull(DocumentObject dom)
        {
            int count = _vds.Count;
            for (int index = 0; index < count; index++)
            {
                if (!_vds[index].IsRefOnly)
                    _vds[index].SetNull(dom);
            }
        }

        /// <summary>
        /// Determines whether all members of the specified dom are null. If dom contains no members IsNull
        /// returns true.
        /// </summary>
        public bool IsNull(DocumentObject dom)
        {
            int count = _vds.Count;
            for (int index = 0; index < count; index++)
            {
                ValueDescriptor vd = _vds[index];
                if (vd.IsRefOnly)
                    continue;
                if (!vd.IsNull(dom))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Gets the DomValueDescriptor of the member specified by name from the DocumentObject.
        /// </summary>
        public ValueDescriptor this[string name]
        {
            get { return _vds[name]; }
        }

        /// <summary>
        /// Gets the DomValueDescriptorCollection of the DocumentObject.
        /// </summary>
        public ValueDescriptorCollection ValueDescriptors
        {
            get { return _vds; }
        }

        readonly ValueDescriptorCollection _vds = new ValueDescriptorCollection();

        /// <summary>
        /// Adds a value descriptor for each field and property found in type to meta.
        /// </summary>
        static void AddValueDescriptors(Meta meta, Type type)
        {
#if !NETFX_CORE
            FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
#else
            var fieldInfos = type.GetTypeInfo().DeclaredFields;
#endif
            foreach (FieldInfo fieldInfo in fieldInfos)
            {
#if DEBUG_
                string name = fieldInfo.Name;
                if (name == "parent")
                    name.GetType();
#endif
                DVAttribute[] dvs = (DVAttribute[])fieldInfo.GetCustomAttributes(typeof(DVAttribute), false);
                if (dvs.Length == 1)
                {
                    ValueDescriptor vd = ValueDescriptor.CreateValueDescriptor(fieldInfo, dvs[0]);
                    meta.ValueDescriptors.Add(vd);
                }
            }

#if !NETFX_CORE
            PropertyInfo[] propInfos = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
#else
            var propInfos = type.GetTypeInfo().DeclaredProperties;
#endif
            foreach (PropertyInfo propInfo in propInfos)
            {
#if DEBUG_
        string name = propInfo.Name;
        if (name == "Font")
          name.GetType();
#endif
                DVAttribute[] dvs = (DVAttribute[])propInfo.GetCustomAttributes(typeof(DVAttribute), false);
                if (dvs.Length == 1)
                {
                    ValueDescriptor vd = ValueDescriptor.CreateValueDescriptor(propInfo, dvs[0]);
                    meta.ValueDescriptors.Add(vd);
                }
            }
        }
    }
}
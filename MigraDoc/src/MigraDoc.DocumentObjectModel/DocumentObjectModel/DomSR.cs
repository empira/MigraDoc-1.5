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
using System.Diagnostics;
using System.Reflection;
using System.Resources;
#if DEBUG
using System.Text.RegularExpressions;
#endif

namespace MigraDoc.DocumentObjectModel
{
    /// <summary>
    /// String resources of MigraDoc.DocumentObjectModel. Provides all localized text strings
    /// for this assembly.
    /// </summary>
    static class DomSR
    {
        /// <summary>
        /// Loads the message from the resource associated with the enum type and formats it
        /// using 'String.Format'. Because this function is intended to be used during error
        /// handling it never raises an exception.
        /// </summary>
        /// <param name="id">The type of the parameter identifies the resource
        /// and the name of the enum identifies the message in the resource.</param>
        /// <param name="args">Parameters passed through 'String.Format'.</param>
        /// <returns>The formatted message.</returns>
        public static string FormatMessage(DomMsgID id, params object[] args)
        {
            string message;
            try
            {
                message = GetString(id);
                if (message != null)
                {
#if DEBUG
                    if (Regex.Matches(message, @"\{[0-9]\}").Count > args.Length)
                    {
                        //TODO too many placeholders or too less args...
                    }
#endif
                    message = String.Format(message, args);
                }
                else
                    message = "<<<error: message not found>>>";
                return message;
            }
            catch (Exception ex)
            {
                message = "INTERNAL ERROR while formatting error message: " + ex;
            }
            return message;
        }

        public static string CompareJustCells
        {
            get { return "Only cells can be compared by this Comparer."; }
        }

        /// <summary>
        /// Gets the localized message identified by the specified DomMsgID.
        /// </summary>
        public static string GetString(DomMsgID id)
        {
            return ResMngr.GetString(id.ToString());
        }
        #region How to use
#if true_
    // Message with no parameter is property.
    public static string SampleMessage1
    {
      // In the first place English only
      get { return "This is sample message 1."; }
    }

    // Message with no parameter is property.
    public static string SampleMessage2
    {
      // Then localized:
      get { return DomSR.GetString(DomMsgID.SampleMessage1); }
    }

    // Message with parameters is function.
    public static string SampleMessage3(string parm)
    {
      // In the first place English only
      //return String.Format("This is sample message 2: {0}.", parm);
    }
    public static string SampleMessage4(string parm)
    {
      // Then localized:
      return String.Format(GetString(DomMsgID.SampleMessage2), parm);
    }
#endif
        #endregion

        #region General Messages

        public static string StyleExpected
        {
            get { return GetString(DomMsgID.StyleExpected); }
        }

        public static string BaseStyleRequired
        {
            get { return GetString(DomMsgID.BaseStyleRequired); }
        }

        public static string EmptyBaseStyle
        {
            get { return GetString(DomMsgID.EmptyBaseStyle); }
        }

        public static string InvalidFieldFormat(string format)
        {
            return FormatMessage(DomMsgID.InvalidFieldFormat, format);
        }

        public static string InvalidInfoFieldName(string name)
        {
            return FormatMessage(DomMsgID.InvalidInfoFieldName, name);
        }

        public static string UndefinedBaseStyle(string baseStyle)
        {
            return FormatMessage(DomMsgID.UndefinedBaseStyle, baseStyle);
        }

        public static string InvalidValueName(string name)
        {
            return FormatMessage(DomMsgID.InvalidValueName, name);
        }

        public static string InvalidUnitValue(string unitValue)
        {
            return FormatMessage(DomMsgID.InvalidUnitValue, unitValue);
        }

        public static string InvalidUnitType(string unitType)
        {
            return FormatMessage(DomMsgID.InvalidUnitType, unitType);
        }

        public static string InvalidEnumValue<T>(T value) where T : struct
        {
            // ... where T : enum
            //   -or-
            // ... where T : Enum
            // is not implemented in C# because nobody has done this.
            // See Eric Lippert on this topic: http://stackoverflow.com/questions/1331739/enum-type-constraints-in-c-sharp
#if !NETFX_CORE
            Debug.Assert(typeof (T).IsSubclassOf(typeof(Enum)));
#else
            Debug.Assert(typeof(T).GetTypeInfo().IsSubclassOf(typeof(Enum)));
#endif
            return FormatMessage(DomMsgID.InvalidEnumValue, value, typeof(T).Name);
        }

        public static string InvalidEnumForLeftPosition
        {
            get { return GetString(DomMsgID.InvalidEnumForLeftPosition); }
        }

        public static string InvalidEnumForTopPosition
        {
            get { return GetString(DomMsgID.InvalidEnumForTopPosition); }
        }

        public static string InvalidColorString(string colorString)
        {
            return FormatMessage(DomMsgID.InvalidColorString, colorString);
        }

        public static string InvalidFontSize(double value)
        {
            return FormatMessage(DomMsgID.InvalidFontSize, value);
        }

        public static string InsertNullNotAllowed()
        {
            return "Insert null not allowed.";
        }

        public static string ParentAlreadySet(DocumentObject value, DocumentObject docObject)
        {
            return String.Format("Value of type '{0}' must be cloned before set into '{1}'.",
              value.GetType(), docObject.GetType());
        }

        public static string MissingObligatoryProperty(string propertyName, string className)
        {
            return String.Format("ObigatoryProperty '{0}' not set in '{1}'.", propertyName, className);
        }

        public static string InvalidDocumentObjectType
        {
            get
            {
                return "The given document object is not valid in this context.";
            }
        }
        #endregion

        #region DdlReader Messages

        #endregion

        #region Resource Manager

        public static ResourceManager ResMngr
        {
            // ReSharper disable ConvertIfStatementToNullCoalescingExpression
            get
            {
                if (_resmngr == null)
                {
#if !NETFX_CORE
                    _resmngr = new ResourceManager("MigraDoc.DocumentObjectModel.Resources.Messages", Assembly.GetExecutingAssembly());
#else
                    _resmngr = new ResourceManager("MigraDoc.DocumentObjectModel.Resources.Messages", typeof(DomSR).GetTypeInfo().Assembly);
#endif
                }
                return _resmngr;
            }
            // ReSharper restore ConvertIfStatementToNullCoalescingExpression
        }

#if !SILVERLIGHT
        /// <summary>
        /// Writes all messages defined by DomMsgID.
        /// </summary>
        [Conditional("DEBUG")]
        public static void TestResourceMessages()
        {
            string[] names = Enum.GetNames(typeof(DomMsgID));
            foreach (string name in names)
            {
                string message = String.Format("{0}: '{1}'", name, ResMngr.GetString(name));
                Debug.WriteLine(message);
            }
        }
#endif
        static ResourceManager _resmngr;

        #endregion
    }
}
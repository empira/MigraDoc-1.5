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

#pragma warning disable 1591

namespace MigraDoc.DocumentObjectModel
{
    /// <summary>
    /// Represents a special character in paragraph text.
    /// </summary>
    // TODO: So ändern, dass symbolName und char in unterschiedlichen Feldern gespeichert werden.
    // TODO Remove German remarks!
    public class Character : DocumentObject
    {
        // \space
        public static readonly Character Blank = new Character(SymbolName.Blank);
        public static readonly Character En = new Character(SymbolName.En);
        public static readonly Character Em = new Character(SymbolName.Em);
        public static readonly Character EmQuarter = new Character(SymbolName.EmQuarter);
        public static readonly Character Em4 = new Character(SymbolName.Em4);

        // used to serialize as \tab, \linebreak
        public static readonly Character Tab = new Character(SymbolName.Tab);
        public static readonly Character LineBreak = new Character(SymbolName.LineBreak);
        //public static readonly Character MarginBreak         = new Character(SymbolName.MarginBreak);

        // \symbol
        public static readonly Character Euro = new Character(SymbolName.Euro);
        public static readonly Character Copyright = new Character(SymbolName.Copyright);
        public static readonly Character Trademark = new Character(SymbolName.Trademark);
        public static readonly Character RegisteredTrademark = new Character(SymbolName.RegisteredTrademark);
        public static readonly Character Bullet = new Character(SymbolName.Bullet);
        public static readonly Character Not = new Character(SymbolName.Not);
        public static readonly Character EmDash = new Character(SymbolName.EmDash);
        public static readonly Character EnDash = new Character(SymbolName.EnDash);
        public static readonly Character NonBreakableBlank = new Character(SymbolName.NonBreakableBlank);
        public static readonly Character HardBlank = new Character(SymbolName.HardBlank);

        /// <summary>
        /// Initializes a new instance of the Character class.
        /// </summary>
        public Character()
        { }

        /// <summary>
        /// Initializes a new instance of the Character class with the specified parent.
        /// </summary>
        internal Character(DocumentObject parent) : base(parent) { }

        /// <summary>
        /// Initializes a new instance of the Character class with the specified SymbolName.
        /// </summary>
        Character(SymbolName name)
            : this()
        {
            // uint does not work, need cast to int.
            //SetValue("SymbolName", (int)(uint)name);
            _symbolName.Value = (int)name;
        }

        #region Properties
        /// <summary>
        /// Gets or sets the SymbolName. Returns 0 if the type is defined by a character.
        /// </summary>
        public SymbolName SymbolName
        {
            get { return (SymbolName)_symbolName.Value; }
            set { _symbolName.Value = (int)value; }
        }
        [DV(Type = typeof(SymbolName))]
        internal NEnum _symbolName = NEnum.NullValue(typeof(SymbolName));

        /// <summary>
        /// Gets or sets the SymbolName as character. Returns 0 if the type is defined via an enum.
        /// </summary>
        public char Char
        {
            get
            {
                if (((uint)_symbolName.Value & 0xF0000000) == 0)
                    return (char)_symbolName.Value;
                return '\0';
            }
            set { _symbolName.Value = value; }
        }

        /// <summary>
        /// Gets or sets the number of times the character is repeated.
        /// </summary>
        public int Count
        {
            get { return _count.Value; }
            set { _count.Value = value; }
        }
        [DV]
        internal NInt _count = new NInt(1);
        #endregion

        #region Internal
        /// <summary>
        /// Converts Character into DDL.
        /// </summary>
        internal override void Serialize(Serializer serializer)
        {
            string text = String.Empty;
            if (_count == 1)
            {
                if ((SymbolName)_symbolName.Value == SymbolName.Tab)
                    text = "\\tab ";
                else if ((SymbolName)_symbolName.Value == SymbolName.LineBreak)
                    text = "\\linebreak\x0D\x0A";
                else if ((SymbolName)_symbolName.Value == SymbolName.ParaBreak)
                    text = "\x0D\x0A\x0D\x0A";
                //else if (symbolType == SymbolName.MarginBreak)
                //  text = "\\marginbreak ";

                if (text != "")
                {
                    serializer.Write(text);
                    return;
                }
            }

            if (((uint)_symbolName.Value & 0xF0000000) == 0xF0000000)
            {
                // SymbolName == SpaceType?
                if (((uint)_symbolName.Value & 0xF1000000) == 0xF1000000)
                {
                    if ((SymbolName)_symbolName.Value == SymbolName.Blank)
                    {
                        //Note: Don't try to optimize it by leaving away the braces in case a single space is added.
                        //This would lead to confusion with '(' in directly following text.
                        text = "\\space(" + Count + ")";
                    }
                    else
                    {
                        if (_count == 1)
                            text = "\\space(" + SymbolName + ")";
                        else
                            text = "\\space(" + SymbolName + ", " + Count + ")";
                    }
                }
                else
                {
                    text = "\\symbol(" + SymbolName + ")";
                }
            }
            else
            {
                // symbolType is a (unicode) character
                text = " \\chr(0x" + _symbolName.Value.ToString("X") + ")";
            }

            serializer.Write(text);
        }

        /// <summary>
        /// Returns the meta object of this instance.
        /// </summary>
        internal override Meta Meta
        {
            get { return _meta ?? (_meta = new Meta(typeof(Character))); }
        }
        static Meta _meta;
        #endregion
    }
}

#region MigraDoc - Creating Documents on the Fly
//
// Authors:
//   Klaus Potzesny
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

using MigraDoc.DocumentObjectModel;
using System.Collections.Generic;

namespace MigraDoc.RtfRendering
{
    /// <summary>
    /// ListInfoRenderer.
    /// </summary>
    internal class ListInfoRenderer : RendererBase
    {
        public ListInfoRenderer(DocumentObject domObj, RtfDocumentRenderer docRenderer)
            : base(domObj, docRenderer)
        {
            _listInfo = domObj as ListInfo;
        }

        public static void Clear()
        {
            _idList.Clear();
            _listID = 1;
            _templateID = 2;
        }
        /// <summary>
        /// Renders a ListIfo to RTF.
        /// </summary>
        internal override void Render()
        {
            if (_prevListInfoID.Key != null && _listInfo.ContinuePreviousList)
            {
                _idList.Add(_listInfo, _prevListInfoID.Value);
                return;
            }
            _idList.Add(_listInfo, _listID);

            _rtfWriter.StartContent();
            _rtfWriter.WriteControl("list");
            // rtfWriter.WriteControl("listtemplateid", templateID.ToString());
            _rtfWriter.WriteControl("listsimple", 1);
            WriteListLevel();
            _rtfWriter.WriteControl("listrestarthdn", 0);
            _rtfWriter.WriteControl("listid", _listID.ToString());
            _rtfWriter.EndContent();

            _prevListInfoID = new KeyValuePair<ListInfo, int>(_listInfo, _listID);
            _listID += 2;
            _templateID += 2;
        }
        private static KeyValuePair<ListInfo, int> _prevListInfoID = new KeyValuePair<ListInfo, int>();
        private readonly ListInfo _listInfo;
        private static int _listID = 1;
        private static int _templateID = 2;
        private static readonly Dictionary<ListInfo, int> _idList = new Dictionary<ListInfo, int>();

        /// <summary>
        /// Gets the corresponding List ID of the ListInfo Object.
        /// </summary>
        internal static int GetListID(ListInfo li)
        {
            if (_idList.ContainsKey(li))
                return (int)_idList[li];

            return -1;
        }

        private void WriteListLevel()
        {
            ListType listType = _listInfo.ListType;
            string levelText1 = "";
            string levelText2 = "";
            string levelNumbers = "";
            int fontIdx = -1;
            switch (listType)
            {
                case ListType.NumberList1:
                    levelText1 = "'02";
                    levelText2 = "'00.";
                    levelNumbers = "'01";
                    break;

                case ListType.NumberList2:
                case ListType.NumberList3:
                    levelText1 = "'02";
                    levelText2 = "'00)";
                    levelNumbers = "'01";
                    break;

                //levelText1 = "'02";
                //levelText2 = "'00)";
                //levelNumbers = "'01";
                //break;

                case ListType.BulletList1:
                    levelText1 = "'01";
                    levelText2 = "u-3913 ?";
                    fontIdx = _docRenderer.GetFontIndex("Symbol");
                    break;

                case ListType.BulletList2:
                    levelText1 = "'01o";
                    levelText2 = "";
                    fontIdx = _docRenderer.GetFontIndex("Courier New");
                    break;

                case ListType.BulletList3:
                    levelText1 = "'01";
                    levelText2 = "u-3929 ?";
                    fontIdx = _docRenderer.GetFontIndex("Wingdings");
                    break;
            }
            WriteListLevel(levelText1, levelText2, levelNumbers, fontIdx);
        }

        private void WriteListLevel(string levelText1, string levelText2, string levelNumbers, int fontIdx)
        {
            _rtfWriter.StartContent();
            _rtfWriter.WriteControl("listlevel");
            // Start
            Translate("ListType", "levelnfcn", RtfUnit.Undefined, "4", false);
            Translate("ListType", "levelnfc", RtfUnit.Undefined, "4", false);
            _rtfWriter.WriteControl("leveljcn", 0);
            _rtfWriter.WriteControl("levelstartat", 1); //Start-At immer auf 1.

            _rtfWriter.WriteControl("levelold", 0); //Kompatibel mit Word 2000?

            _rtfWriter.StartContent();
            _rtfWriter.WriteControl("leveltext");
            _rtfWriter.WriteControl("leveltemplateid", _templateID);
            _rtfWriter.WriteControl(levelText1);
            if (levelText2 != "")
                _rtfWriter.WriteControl(levelText2);

            _rtfWriter.WriteSeparator();

            _rtfWriter.EndContent();
            _rtfWriter.StartContent();
            _rtfWriter.WriteControl("levelnumbers");
            if (levelNumbers != "")
                _rtfWriter.WriteControl(levelNumbers);

            _rtfWriter.WriteSeparator();
            _rtfWriter.EndContent();

            if (fontIdx >= 0)
                _rtfWriter.WriteControl("f", fontIdx);

            _rtfWriter.WriteControl("levelfollow", 0);

            _rtfWriter.EndContent();
        }
    }
}

using ImageMetaExtractor.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ImageMetaExtractor.Reader
{
    class MnoteProperties
    {
        public static readonly string TagName = "Makernote";

        private readonly MetaItemList _mnoteItemList;

        public MnoteProperties(MetaItemList mnoteItemList)
        {
            _mnoteItemList = mnoteItemList;
        }

    }
}

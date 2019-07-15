using System;
using System.IO;
using System.Collections.Generic;
using Metadata = MetadataExtractor; // System.IOとDirectoryが被るので別名を付ける
using ImageMetaExtractor.Reader;

namespace ImageMetaExtractor
{
    public class MetaExtractor
    {


        public MetaExtractor(string imagePath)
        {
            var imageMeta = ReaderFactory.GetInstance(imagePath);
            if (imageMeta is null) return;

            //File
            var fileMetaList = imageMeta.GetFileMetaItemList();
            Console.WriteLine(fileMetaList);

            //Exif
            if (imageMeta.HasExifMeta)
            {
                var exifMetaListGroup = imageMeta.GetExifMetaListGroup();
                foreach (var metaList in exifMetaListGroup)
                {
                    Console.WriteLine(metaList);
                }
            }

            //Makernote

        }

    }
}

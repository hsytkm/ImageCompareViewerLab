using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Metadata = MetadataExtractor; // System.IOとDirectoryが被るので別名を付ける

namespace ImageMetaExtractor.Reader
{
    /// <summary>
    /// MetadataExtractor の IReadOnlyList<Directory> を持つ
    /// </summary>
    abstract class ImageMetaExtractorBase : ImageMetaBase
    {
        internal readonly IReadOnlyList<Metadata.Directory> _directories;

        public ImageMetaExtractorBase(string imagePath) : base(imagePath)
        {
            using (var stream = new FileStream(imagePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                _directories = Metadata.ImageMetadataReader.ReadMetadata(stream);
            }
        }

        /// <summary>
        /// 引数文字列を含むDirectoryを返す
        /// </summary>
        /// <param name="tagName"></param>
        /// <returns></returns>
        internal Metadata.Directory GetDirectory(string tagName)
            => _directories.FirstOrDefault(d => d.Name.Contains(tagName));

    }
}

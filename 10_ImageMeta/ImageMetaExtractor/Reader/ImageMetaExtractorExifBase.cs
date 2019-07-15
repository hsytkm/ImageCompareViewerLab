using ImageMetaExtractor.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using MetadataExtractor;

namespace ImageMetaExtractor.Reader
{
    /// <summary>
    /// MetadataExtractor の IReadOnlyList<Directory> を持ってて Exif を含む(Jpeg/Tiff)
    /// </summary>
    abstract class ImageMetaExtractorExifBase : ImageMetaExtractorBase
    {
        internal override bool GetHasExifMeta() => true;

        // EXIFタグ番号
        private enum EXIF_MAIN_ID : int
        {
            MAKER = 0x010f,
            MODEL = 0x0110,
            EXPO_PROG = 0x8822,
            FVAL = 0x829d,
            SSPEED = 0x829a,
            SSPEED2 = 0x9201,       // 詳細版
            FVAL2 = 0x9202,         // 詳細版
            ISO_2BYTE = 0x8827,
            SENSITIVITY_TYPE = 0x8830,
            ISO_4BYTE_SOS = 0x8831,
            ISO_4BYTE_REI = 0x8832,
            //ISO_4BYTE_SPEED = 0x8833,
            EVCORR = 0x9204,
            OPEN_FVAL = 0x9205,
            FLASH = 0x9209,
            FLENGTH = 0x920a,
            FLEN35M = 0xa405,
            EXPO_CORRECT = 0x9204,
        };

        private static readonly string[] ExifTagName = new string[]
        {
            "Exif IFD0",
            "Exif SubIFD"
        };

        // 何度も取得するのでコンストラクタで用意する
        private readonly MetaItemList ExifMainItemList;
        private readonly MetaItemList ExifSubItemList;

        public ImageMetaExtractorExifBase(string imagePath)
            : base(imagePath)
        {
            var exif0 = GetMetaItemList(ExifTagName[0], "EXIF 1");
            
            // PCで加工されてEXIFが消えたJPEG対応
            ExifMainItemList = exif0 ?? throw new KeyNotFoundException(ExifTagName[0]);

            var exif1 = GetMetaItemList(ExifTagName[1], "EXIF 2");

            // PCで加工されてEXIFが消えたJPEG対応
            ExifSubItemList = exif1 ?? throw new KeyNotFoundException(ExifTagName[1]);
        }

        /// <summary>
        /// DirectoryからExifを読み出す
        /// </summary>
        /// <returns></returns>
        public override IList<MetaItemList> GetExifMetaListGroup()
            => new List<MetaItemList>() { ExifMainItemList, ExifSubItemList };

        /// <summary>
        /// 対象Dictionaryの全要素を取得
        /// </summary>
        /// <param name="targetTag"></param>
        /// <param name="newTag"></param>
        /// <param name="sort_id_list"></param>
        /// <returns></returns>
        internal MetaItemList GetMetaItemList(string targetTag, string newTag, IEnumerable<int> sort_id_list = null)
        {
            var directory = GetDirectory(targetTag);
            if (directory == null) return null;

            // MetadataExtractorの情報をMyXML定義に変換して設定
            var metas = directory.Tags.Select(t => GetMetaItemFromLibTag(directory, t)).ToList();
            if (!metas.Any()) return null;

            // ID順にソート
            metas.Sort((a, b) => a.Id - b.Id);

            // 指定IDを上に移動
            if (sort_id_list != null)
            {
                throw new NotImplementedException(); //◆未実装
                //foreach (var id in sort_id_list.Reverse())  // 後で実行するほど上に配置される
                //{
                //    var ofst = (from m in metas.Select((v, i) => new { v, i })
                //                where m.v.Id == id
                //                select m.i).FirstOrDefault();
                //    metas.Insert(0, metas[ofst]);
                //    metas.RemoveAt(ofst + 1);      // 追加したのでヒット時+1の要素を削除
                //}
            }
            return new MetaItemList(newTag, metas);
        }

        /// <summary>
        /// MetadataExtractorの情報をMyXML定義に変換して設定
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        private MetaItem GetMetaItemFromLibTag(Directory dir, Tag tag)
        {
            int id = tag.Type;
            string key = tag.Name;
            string value = tag.Description;
            object obj = dir.GetObject(id);
            return new MetaItem(id, key, value, obj);

            throw new NotImplementedException();
            //return GetResolvedMetaItem(id, name, content, obj);
        }

        #region メタ情報

        private string GetMetaItemValueString(EXIF_MAIN_ID id)
            => ExifMainItemList.GetMetaItem((int)id).Value.Trim();

        public string Maker
        {
            get
            {
                if (_maker == null) _maker = GetMetaItemValueString(EXIF_MAIN_ID.MAKER);
                return _maker;
            }
        }
        private string _maker;

        public string Model
        {
            get
            {
                if (_model == null) _model = GetMetaItemValueString(EXIF_MAIN_ID.MODEL);
                return _model;
            }
        }
        private string _model;

        public string ExpoProg
        {
            get
            {
                if (_expoProg == null) _expoProg = GetMetaItemValueString(EXIF_MAIN_ID.EXPO_PROG);
                return _expoProg;
            }
        }
        private string _expoProg;

        public string Fvalue
        {
            get
            {
                if (_fvalue == null)
                {
                    // 詳細情報を優先して表示する
                    //_fvalue = GetMetaItemString(EXIF_MAIN_ID.FVAL2);

                    if (string.IsNullOrWhiteSpace(_fvalue))
                        _fvalue = GetMetaItemValueString(EXIF_MAIN_ID.FVAL);
                }
                return _fvalue;
            }
        }
        private string _fvalue;

        public string SSpeed
        {
            get
            {
                if (_sSpeed == null)
                {
                    // 詳細情報を優先して表示する
                    //_sSpeed = GetMetaItemString(EXIF_MAIN_ID.SSPEED2);

                    if (string.IsNullOrWhiteSpace(_sSpeed))
                        _sSpeed = GetMetaItemValueString(EXIF_MAIN_ID.SSPEED);
                }
                return _sSpeed;
            }
        }
        private string _sSpeed;

        public string ISO
        {
            get
            {
                string GetIsoValue()
                {
                    var meta = ExifMainItemList.GetMetaItem((int)EXIF_MAIN_ID.ISO_2BYTE);
                    if (string.IsNullOrWhiteSpace(meta.Value)) return "";
                    string iso = meta.Value;

                    //◆未実装
                    //int? val2b = MetasBase.ConvObjectToInt(meta.Content);
                    //if (val2b != null && val2b == 0xffff)
                    //{
                    //    object obj = GetMetaItem(EXIF_MAIN_ID.SENSITIVITY_TYPE).Source;
                    //    int? val4b = MetasBase.ConvObjectToInt(obj);

                    //    // "感度種別"のenum値を直見
                    //    switch (val4b)
                    //    {
                    //        case 1: iso = GetMetaItemValueString(EXIF_MAIN_ID.ISO_4BYTE_SOS); break;
                    //        case 2: iso = GetMetaItemValueString(EXIF_MAIN_ID.ISO_4BYTE_REI); break;
                    //    }
                    //}
                    return $"ISO{iso}";
                }

                if (_iso == null) _iso = GetIsoValue();
                return _iso;
            }
        }
        private string _iso;

        public double Av
        {
            get
            {
                if (!_Av.HasValue) _Av = ExposureApexAv.Fval2Av(Fvalue);
                return _Av.Value;
            }
        }
        private double? _Av;

        public double Tv
        {
            get
            {
                if (!_Tv.HasValue) _Tv = ExposureApexTv.Sspeed2Tv(SSpeed);
                return _Tv.Value;
            }
        }
        private double? _Tv;

        public double Sv
        {
            get
            {
                if (!_Sv.HasValue) _Sv = ExposureApexSv.Iso2Sv(ISO);
                return _Sv.Value;
            }
        }
        private double? _Sv;

        public double Ev
        {
            get
            {
                if (!_Ev.HasValue) _Ev = Av + Tv + Sv;
                return _Ev.Value;
            }
        }
        private double? _Ev;

        public string ExpoCorrect
        {
            get
            {
                if (_ExpoCorrect == null)
                {
                    _ExpoCorrect = GetMetaItemValueString(EXIF_MAIN_ID.EXPO_CORRECT);
                    if (!string.IsNullOrWhiteSpace(_ExpoCorrect)) _ExpoCorrect += "EV";
                }
                return _ExpoCorrect;
            }
        }
        private string _ExpoCorrect;

        public string Flash
        {
            get
            {
                if (_flash == null) _flash = GetMetaItemValueString(EXIF_MAIN_ID.FLASH);
                return _flash;
            }
        }
        private string _flash;

        public string FLen35m
        {
            get
            {
                string GetFLength35mm()
                {
                    var s = GetMetaItemValueString(EXIF_MAIN_ID.FLEN35M);
                    if (string.IsNullOrEmpty(s)) return "";
                    if (s.Contains("Unknown")) return s;
                    return s.Replace("mm", "").Trim() + "mm";
                }

                if (_fLen35m == null) _fLen35m = GetFLength35mm();
                return _fLen35m;
            }
        }
        private string _fLen35m;

        // EXIF情報からLumixのJPEGかを判定
        public bool IsLumixJpeg
        {
            get
            {
                if (!_isLumixJpeg.HasValue)
                {
                    _isLumixJpeg = false;
                    if (Maker.Equals("Panasonic"))
                        if (Model.Contains("DC-") || Model.Contains("DMC-"))
                            _isLumixJpeg = true;
                }
                return _isLumixJpeg.Value;
            }
        }
        private bool? _isLumixJpeg;

        #endregion

    }
}

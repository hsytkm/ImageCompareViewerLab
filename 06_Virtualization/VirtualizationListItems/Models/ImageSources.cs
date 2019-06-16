using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace VirtualizationListItems.Models
{
    class ImageSources : BindableBase
    {
        private const string DirPath = @"C:\data";

        public ObservableCollection<ImageSource> Sources { get; } = new ObservableCollection<ImageSource>();

        private string _SelectedImagePath;
        public string SelectedImagePath
        {
            get => _SelectedImagePath;
            set => SetProperty(ref _SelectedImagePath, value);
        }

        public ImageSources() { }

        public void Initialize(string dirPath = DirPath)
        {
            Sources.Clear();

            // ImageSourceのコンストラクタ内ではサムネイルを読み込まない
            foreach (var path in GetImagePaths(dirPath))
            {
                Sources.Add(new ImageSource(path));
            }

            // とりあえず先頭画像を選択しとく
            if (Sources.Any())
            {
                SelectedImagePath = Sources.First().FilePath;
            }
        }

        private static IEnumerable<string> GetImagePaths(string directoryPath)
        {
            var pat = ".jpg";
            var images = new List<string>();

            // patは "*.jpg" の形式にする
            foreach (var file in new DirectoryInfo(directoryPath).GetFiles($"*{pat}", SearchOption.TopDirectoryOnly))
            {
                // tiff画像が ".tif" と ".tiff" で二重検出されるので完全一致をチェック
                if (Path.GetExtension(file.FullName).ToLower() == pat)
                    images.Add(file.FullName);
            }
            return images.OrderBy(f => f);
        }

        // 表示候補画像の解放と読み出し
        public void UpdateThumbnail(double centerRatio, double viewportRatio)
        {
            if (centerRatio == 0) throw new ArgumentException(nameof(centerRatio));
            if (viewportRatio == 0) throw new ArgumentException(nameof(viewportRatio));

            var list = Sources;
            int length = list.Count;
            if (length == 0) return;

            //Debug.WriteLine($"Thumbnail Update() center={centerRatio:f2} viewport={viewportRatio:f2}");

            int margin = 1; // 表示マージン(左右に1個余裕持たせる)
            int centerIndex = (int)Math.Floor(length * centerRatio);        // 切り捨て
            int countRaw = (int)Math.Ceiling(length * viewportRatio);       // 切り上げ
            int start = Math.Max(0, centerIndex - (countRaw / 2) - margin); // 一つ余分に描画する
            int end = Math.Min(length - 1, start + countRaw + margin);      // 一つ余分に描画する
            int count = end - start + 1;
            //Debug.WriteLine($"Thumbnail Update() total={length} start={start} end={end} count={count}");

            // 解放リスト(表示範囲外で読込み中)
            var unloads = Enumerable.Range(0, length)
                .Where(x => !(start <= x && x <= end))
                .Select(x => list[x])
                .Where(x => !x.IsThumbnailEmpty);
            foreach (var source in unloads)
            {
                //Debug.WriteLine($"Thumbnail Update() Unload: {source.FilePath}");
                source.UnloadThumbnail();
            }

            // 読込みリスト(表示範囲の未読込みを対象)
            var loads = Enumerable.Range(start, count)
                .Select(x => list[x])
                .Where(x => x.IsThumbnailEmpty);
            foreach (var source in loads)
            {
                //Debug.WriteLine($"Thumbnail Update() Load: {source.FilePath}");
                source.LoadThumbnail();
            }

            // 読み込み状況の表示テスト
            LoadedItemText();
        }

        #region テスト

        private string _LoadStatus;
        public string LoadStatus
        {
            get => _LoadStatus;
            private set => SetProperty(ref _LoadStatus, value);
        }

        private void LoadedItemText()
        {
            var sb = new System.Text.StringBuilder();
            foreach(var source in Sources)
            {
                sb.Append(source.IsThumbnailEmpty ? "□" : "■");
            }
            LoadStatus = sb.ToString();
        }

        #endregion

    }
}

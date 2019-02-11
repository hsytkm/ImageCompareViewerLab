using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace VirtualizationListItems.Models
{
    class ImageSources : BindableBase
    {
        // Singleton
        public static ImageSources Instance { get; } = new ImageSources();

        private const string DirPath = @"C:\data";

        public ObservableCollection<ImageSource> Sources = new ObservableCollection<ImageSource>();

        private string _SelectedImagePath;
        public string SelectedImagePath
        {
            get => _SelectedImagePath;
            set => SetProperty(ref _SelectedImagePath, value);
        }

        private ImageSources() { }

        public void Initialize()
        {
            // 全削除(ClearだとEventListenerの解除できないので1つずつ削除)
            while (Sources.Any())
            {
                Sources.RemoveAt(0);
            }

            // コンストラクタ内ではサムネイルを読み込まない
            foreach (var path in GetImages(DirPath))
            {
                Sources.Add(new ImageSource(path));
            }

            if (Sources.Any())
            {
                SelectedImagePath = Sources[0].FilePath;
            }
        }

        private static IEnumerable<string> GetImages(string directoryPath)
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
            int count = (int)Math.Ceiling(length * viewportRatio);          // 切り上げ
            int start = Math.Max(0, centerIndex - (count / 2) - margin);    // 一つ余分に描画する
            int end = Math.Min(length - 1, start + count + margin);         // 一つ余分に描画する
            //Debug.WriteLine($"Thumbnail Update() total={length} start={start} end={end} num={end - start + 1}");

            // 解放リスト(表示範囲外で読込み中)
            var disposes = Enumerable.Range(0, length)
                .Where(x => !(start <= x && x <= end))
                .Select(x => list[x])
                .Where(x => !x.IsThumbnailEmpty);
            foreach (var source in disposes)
            {
                //Debug.WriteLine($"Thumbnail Update() Unload: {source.FilePath}");
                source.UnloadThmbnail();
            }

            // 読込みリスト(表示範囲の未読込みを対象)
            var readTargets = Enumerable.Range(start, end - start + 1)
                .Select(x => list[x])
                .Where(x => x.IsThumbnailEmpty);
            foreach (var source in readTargets)
            {
                //Debug.WriteLine($"Thumbnail Update() Load: {source.FilePath}");
                source.LoadThmbnail();
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

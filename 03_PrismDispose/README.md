# PrismDispose

## 実現したいこと
Moduleの破棄時に、ViewとViewModelのDisposeをコールしたい。

## サンプル説明

### Dispose対応

App.xaml.cs にて Dispose するための IRegionBehavior (DisposeRegionBehavior) を登録している。

DisposeRegionBehaviorでは、Region.Viewsコレクションが Remove された場合に、View/ViewModel を Dispose している。

View/ViewModel が IDisposable を継承していなければ何も起きない。

### ×ボタンによるアプリ終了

Disposeを呼ぶために別途対応が必要。

MainWindow の Closedイベントで対象RegionのViewを全て Remove することで対応した。


## 参考にしたページ

https://github.com/runceel/PrismEdu/tree/master/09.RegionBehavior

https://stackoverflow.com/questions/18086195/is-there-any-way-to-remove-a-view-by-name-from-a-prism-region-when-the-view-wa



以上
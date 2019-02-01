# PrismDispose

## 実現したいこと
Moduleの破棄時に、ViewとViewModelのDisposeをコールしたい。

## サンプル内容

App.xaml.cs にて Dispose するための IRegionBehavior (DisposeRegionBehavior) を登録している。

DisposeRegionBehaviorでは、Region.Viewsコレクションが Remove された場合に、View/ViewModel を Dispose している。

View/ViewModel が IDisposable を継承していなければ何も起きない。



## 参考にしたページ

https://github.com/runceel/PrismEdu/tree/master/09.RegionBehavior

https://stackoverflow.com/questions/18086195/is-there-any-way-to-remove-a-view-by-name-from-a-prism-region-when-the-view-wa



以上
# Virtualization

## 実現したいこと

サムネイル画像を複数並べたコントロールを仮想化したい。

調べたところ仮想化には、"UIの仮想化" と "データの仮想化" の2種があるらしい。

## プロジェクト

### DataVirtualization

CodeProjectから拾った仮想化のサンプルプロジェクト
難しくて内容を理解できていない…

### VirtualizationListItems

1. UIの仮想化

	ListBox を VirtualizingStackPanel で仮想化。真っ当な実装。

2. データの仮想化

	UIのViewportなどから無理やり仮想化
	◆サムネイル表示が2次元になると対応できずイマイチ…

## 参考にさせて頂いたページ

- [ListView で表示用データを仮想化する](https://clown.hatenablog.jp/entry/20130327/listview_virtualizing)

- [WPFでコレクションの表示を高速化するオプションとか](http://yuuxxxx.hatenablog.com/entry/2014/02/01/232320)

- [Msを16倍出し抜くwpf開発2回目](https://www.slideshare.net/cct-inc/ms16wpf2)

- [UI に表示されるデータの仮想化（１）](https://www.matatabi-ux.com/entry/2014/08/04/120000)

- [UI に表示されるデータの仮想化（２）](https://www.matatabi-ux.com/entry/2014/08/08/120000)

- [WPFアプリケーションを高速化、仮想化Canvasを使ってみた。](https://qiita.com/takanemu/items/6f9ea70a0b5425b67f75)

- [簡素な仮想化パネル](http://proprogrammer.hatenadiary.jp/entry/2014/10/09/182618)

以上
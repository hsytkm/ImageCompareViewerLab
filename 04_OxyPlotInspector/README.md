# OxyPlotInspector

## 実現したいこと
画像にラインを引いて、線上の画素レベルをグラフに表示したい。

## 対応したこと
- Viewで引いたライン上の画素値を取得する
- OxyPlotでRGB値のグラフを表示する
- カスタムポップアップウィンドウにOxyPlotグラフを表示する
- OxyPlot.dllの読み出しをポップアップ表示まで行わない(起動時に読まない)
- グラフViewの読み出し中は全画素値をメモリに読み出しておく(何度もディスクアクセスしない)
- グラフViewの非表示で読み出し全画素値を解放する(不要なメモリ使用しない)
- 解析Lineを矢印にして方向が分かるようにする


## 参考にさせて頂いたページ

https://github.com/oxyplot/oxyplot


![Sample](https://github.com/hsytkm/ImageCompareViewer/blob/master/04_OxyPlotInspector/OxyPlotInspector/capture.gif)

以上
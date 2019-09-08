# 09_WPFGraphs

Excelのように、値に応じてセルの色を変えたい。



## Graph2D

何となく完成したけど、値が255超だと動作せず、使い物にならない。

思い出として削除せずに残しておく。



## Graph2D_2

前回対応できなかった、255超の値でも動作するようになった。（ViewModelからFore/Backの色が指定できる）

けど、セルのWidthが異様に長いし、セルをコピーすると色情報までコピーされており、やっぱり使い物にならない。おしい。


## Graph2D_3

セルのWidthは適正やし、セルをコピーも意図通りまで来た。

xamlだけで実現できなかった(コードビハインド使った)のは残念やけど、やりたいことできて良かった。

![Sample](https://github.com/hsytkm/ImageCompareViewer/blob/master/09_WPFGraphs/Graph2D_3/capture.gif)

以上


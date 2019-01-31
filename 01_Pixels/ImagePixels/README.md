# ImagePixels

JPEG全画素のRGB平均値を求める検討リポジトリ

## 画素値の読み出し結果

Start
LoopCount: 30
ImageSize: W=4592 H=3448

BitmapImage0                       : Y=86.58 Time=00:00:12.4736726 Ratio=110.3%
BitmapImage0                       : Y=86.58 Time=00:00:11.3104130 Ratio=100.0%  2回目の方が高速
Bitmap1(Lockbits)                  : Y=86.58 Time=00:00:10.0139894 Ratio=88.5%
Bitmap2(Lockbits&Unsafe)           : Y=86.58 Time=00:00:08.3101715 Ratio=73.5%    通常最速
Bitmap3(Lockbits&Unsafe&Para)      : Y=31.66 Time=00:00:20.1460909 Ratio=178.1%   バグってる
Bitmap4(Lockbits&Unsafe&Span)      : Y=86.58 Time=00:00:14.0653608 Ratio=124.4%
Bitmap5(Bitmap2+Parallel2)         : Y=86.58 Time=00:00:07.7947307 Ratio=68.9%
Bitmap5(Bitmap2+Parallel4)         : Y=86.58 Time=00:00:07.3467357 Ratio=65.0%    Para8より早い
Bitmap5(Bitmap2+Parallel8)         : Y=86.58 Time=00:00:07.4916510 Ratio=66.2%

Finish

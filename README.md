udoya @2025/2/7

## custom operator and stamp
exeファイルと同じディレクトリにあるoperators/atk, defにあるpngをリストにしています．
ガジェットのstampに関しても，gadgets/general, def_gadget, atk_gadget, generalに分けています．これらのディレクトリを作成し，そこにpngファイルを入れることで，リスト表示が可能です．
ファイル名やサイズは僕もよくわかっていません．ただ，ファイル名はリストに表示される名前になります．


## required for build
- Visual Studio (I use 2022 pro)
- .NET Framework 4.7.2

## このforkで改造したいこと
- mapとstampとリスト表示(Done)
 - mapは<map_name>_<attribute>_<floor>.png　のような形式でmapフォルダにあれば、勝手にリスト表示してくれるようにしたい。
 - stampはGeneral/Defender/Atackkerの3つに分けて色々区分したい
- stampの移動


## future work
- realtime share
- release exe file for windows


## How to use

### other component
ここにあるものは現状slnファイルのビルドに使用せず，別途データを用意するために利用したものである．


`ClassifyOperator.js` は，input_dir/にまとまっている.pngファイルを攻撃，防御に分類し直しているものである．これらは[r6operators ライブラリ](https://github.com/marcopixel/r6operators)を利用してsvgアイコンを入手し，何らかの形で([ImageMagick](https://imagemagick.org/index.php)など)pngファイルへとコンバートすることを前提としている．そのため，npmが動かせる環境でないといけない．

#### download champion icon from "r6soperators" liblary

npm install r6operators

icon path(SVG) is "r6operators\node_modules\r6operators\dist\icons"

#### install ImageMagick to translate SVG -> png
download ImageMagick

in windows powershell...
```shell
$inputDir = "C:\svg_files"

$outputDir = "C:\png_output"

magick -background none -density 300 "$inputDir\*.svg" -set filename: "%t" "$outputDir\%[filename:].png"
```
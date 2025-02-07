udoya @2025/2/7

## How to use
releaseを見よう！[releaseはこちらから](https://github.com/udoya/TacticalBoard/releases)
exeファイルをダウンロードし、そのまま起動する。
各種リソース、ファイル、ディレクトリは個々人で用意する。以下のディレクトリ構成にすることが望ましい。
```txt
├── TacticalBoard.exe
├── operators/
│   ├── atk/
│   │   └── (データ.png)
│   └── def/
│       └── (データ.png)
├── gadgets/
│   ├── atk_gadget/
│   │   └── (このあたりはpngで)
│   ├── def_gadget/
│   │   └── (お好みに合わせて)
│   ├── favolite/
│   │   └── (カスタマイズ)
│   └── general/
│       └── (してください)
└── maps/
    └── (好きなマップたち)/
        ├── 0.png / 0.jpg
        ├── 1.png / 1.jpg
        ├── 2.png / 2.jpg
        └── 3.png / 3.jpg
```

## custom operator and stamp
exeファイルと同じディレクトリにあるoperators/atk, defにあるpngをリストにしています．
ガジェットのstampに関しても，gadgets/general, def_gadget, atk_gadget, generalに分けています．これらのディレクトリを作成し，そこにpngファイルを入れることで，リスト表示が可能です．
ファイル名やサイズは僕もよくわかっていません．ただ，ファイル名はリストに表示される名前になります．


## required for build
- Visual Studio (I use 2022 pro)
- .NET Framework 4.7.2
- Windows

## このforkで改造したいこと
- mapとstampとリスト表示(Done)
 - mapは<map_name>_<attribute>_<floor>.png　のような形式でmapフォルダにあれば、勝手にリスト表示してくれるようにしたい。
 - stampはGeneral/Defender/Atackkerの3つに分けて色々区分したい
- stampの移動(Done)


## future work
- realtime share
- release exe file for windows (Done)


## other component
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

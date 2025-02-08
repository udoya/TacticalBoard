udoya @2025/2/7

## How to use
releaseを見よう！[releaseはこちらから．](https://github.com/udoya/TacticalBoard/releases)

exeファイルをダウンロードし、そのまま起動する．
各種リソース、ファイル、ディレクトリは個々人で用意する。以下のディレクトリ構成にすることが望ましい．
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

### そこまで教える気のない簡単な操作方法説明
- 基本的には左クリックで大丈夫！
- 右クリックはメニュー操作，スタンプの削除などなどに使います．
- リスト内はダブルクリックでイベントを起こすことができます．


### custom operator and stamp
- ディレクトリ内にある画像をリストにしているので，自分のお好みで上のディレクトリ配置図に合わせて画像を用意してください．多分pngファイルで用意したほうがいいと思います．
- 画像は再配布すると権利的に怪しそうなので，自分で見つけていただくようにお願いします．(オペレーターをatk/defに分けるのはめんどくさいと思いますが，一応other componentに自分用のオペレーターデータ収集用スクリプトをのせてあります．)
 - 自分でやったほう方法はあまり合理的でないかもしれないので，シェルや環境構築に慣れていない人は手動で探すことをおすすめします．  


## required for build
- Visual Studio (I use 2022 pro)
- .NET Framework 4.7.2
- Windows


## future work
- realtime share


## other component
ここにあるものは現状slnファイルのビルドに使用せず，別途データを用意するために利用したものである．

`ClassifyOperator.js` は，input_dir/にまとまっている.pngファイルを攻撃，防御に分類し直しているものである．これらは[r6operators ライブラリ](https://github.com/marcopixel/r6operators)を利用してsvgアイコンを入手し，何らかの形で([ImageMagick](https://imagemagick.org/index.php)など)pngファイルへとコンバートすることを前提としている．そのため，npmが動かせる環境でないといけない．

#### download champion icon from "r6soperators" liblary

```shell
npm install r6operators
```

icon path(SVG) is "r6operators\node_modules\r6operators\dist\icons"

#### install ImageMagick to translate SVG -> png
download ImageMagick

in windows powershell...
```shell
$inputDir = "C:\svg_files"

$outputDir = "C:\png_output"

magick -background none -density 300 "$inputDir\*.svg" -set filename: "%t" "$outputDir\%[filename:].png"
```

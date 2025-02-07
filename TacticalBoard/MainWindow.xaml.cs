using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using System.Linq;
using System.Windows.Ink;


namespace TacticalBoard
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        //直線書いてる最中かどうかの判定
        bool IsDraw;
        //現在選択中のコマの直線の名前
        String lineName;
        //コマのリストとレイヤー
        List<Thumb> thumbs = new List<Thumb>();
        String[] thumbsLayer = new string[10];
        //コマの初期配置座標リスト
        private PointCollection thumbDefaultPoints = new PointCollection();

        //スタンプ押してる最中判定用
        bool IsStamp;
        //スタンプのリスト
        List<Image> stampImages = new List<Image>();
        //スタンプリストのIndex
        int stampIndex = 0;

        //テキストスタンプ判定
        bool IsTextStamp;
        //文字列スタンプのリスト
        List<Canvas> TextStampCanvas = new List<Canvas>();
        //文字列スタンプのIndex
        int sTBIndex = 0;

        //現在のレイヤーのインクキャンバス
        InkCanvas nowLayerInk;
        //現在のレイヤーのスタンプ用キャンバス
        Canvas nowLayerStamp;
        //現在のレイヤー
        String nowLayer = "Layer1";

        //現在のインク色
        Button nowInkButton;

        //メニュー項目用
        List<MenuItem> CMItems = new List<MenuItem>();
        //private ListBox ObjectList = null;

        DrawingAttributes inkDA;

        private bool isDragging = false;
        private Point startPoint;
        private Image currentStamp;

        public MainWindow()
        {
            InitializeComponent();

            nowLayerInk = inkCanvasLayer1;
            nowLayerStamp = StampCanvasLayer1;
            nowInkButton = YellowInk;

            thumbsLayer = new string[]{
                "Layer1","Layer1","Layer1","Layer1","Layer1","Layer1","Layer1","Layer1","Layer1","Layer1"
            };

        }

        //Thumbドラッグ開始
        private void Thumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            var thumb = sender as Thumb;
            lineName = thumb.Name + "Line";
            int i;
            thumb.Opacity = 1;

            try
            {
                //直線を消す
                Line line = PeaceCanvas.FindName(lineName) as Line;
                line.Visibility = Visibility.Collapsed;
                line.Opacity = 1;
            }
            catch (Exception)
            {

            }

            //透過設定
            for (i = 0; i < 10; i++)
            {
                if (thumbs[i].Equals(thumb))
                {
                    thumbsLayer[i] = nowLayer;
                }
            }
        }

        //Thumbドラッグ中
        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (sender is Thumb thumb)
            {
                string[] split = thumb.Name.Split('_');
                Thumb hiddenthumb;

                if (split.Length > 1)
                {
                    hiddenthumb = FindName(split[0]) as Thumb;
                }
                else
                {
                    hiddenthumb = FindName(split[0] + "_dead") as Thumb;
                }

                var x = Canvas.GetLeft(thumb) + e.HorizontalChange;
                var y = Canvas.GetTop(thumb) + e.VerticalChange;

                var canvas = thumb.Parent as Canvas;
                if (null != canvas)
                {
                    x = Math.Max(x, 0);
                    y = Math.Max(y, 0);
                    x = Math.Min(x, canvas.ActualWidth - thumb.ActualWidth);
                    y = Math.Min(y, canvas.ActualHeight - thumb.ActualHeight);
                }

                Canvas.SetLeft(thumb, x);
                Canvas.SetTop(thumb, y);
                Canvas.SetLeft(hiddenthumb, x);
                Canvas.SetTop(hiddenthumb, y);
            }
        }

        //背景画像(MAP画像)をファイルから選択
        private void MAPButton_Click(object sender, RoutedEventArgs e)
        {
            // ダイアログのインスタンスを生成
            var dialog = new OpenFileDialog
            {

                //初期ディレクトリを設定
                InitialDirectory = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.IO.Path.GetFullPath(Environment.GetCommandLineArgs()[0])), "maps"),
                // ファイルの種類を設定
                Filter = "イメージファイル (*.png, *.jpg)|*.png;*.jpg"
            };

            // ダイアログを表示する
            if (dialog.ShowDialog() == true)
            {
                ImageBrush ib = new ImageBrush
                {
                    ImageSource = new BitmapImage(new Uri(dialog.FileName, UriKind.Relative)),
                    Stretch = Stretch.Uniform
                };
                nowLayerInk.Background = ib;
            }
        }

        //コマから右ドラッグで直線を引くためのメソッド(右クリック押し込み動作)
        private void ThumbRightDown(object sender, MouseButtonEventArgs e)
        {
            EraseButton.IsChecked = false;
            nowLayerInk.EditingMode = InkCanvasEditingMode.None;
            MoveButton.IsChecked = false;
            isDragging = false;

            var Thumb = sender as Thumb;
            Point thumbPoint = Thumb.TranslatePoint(new Point(0, 0), PeaceCanvas);
            Point mousePoint = Mouse.GetPosition(PeaceCanvas);
            lineName = Thumb.Name + "Line";
            Line line = PeaceCanvas.FindName(lineName) as Line;
            if (line != null)
            {
                line.X1 = thumbPoint.X + 14;
                line.Y1 = thumbPoint.Y + 14;
            }

            if (IsDraw)
            {
                IsDraw = false;
                line.X2 = mousePoint.X;
                line.Y2 = mousePoint.Y;
            }
            else
            {
                IsDraw = true;
            }

        }

        //キャンバス上のマウス移動関係
        private void peaceMouseMove(object sender, MouseEventArgs e)
        {
            Point mousePoint = Mouse.GetPosition(PeaceCanvas);
            //直線描画部分
            Line line = PeaceCanvas.FindName(lineName) as Line;
            if (IsDraw)
            {

                line.X2 = mousePoint.X;
                line.Y2 = mousePoint.Y;
                line.Visibility = Visibility.Visible;
            }

            //Stamp押す部分
            if (IsStamp)
            {
                stampImages[stampIndex - 1].Margin = new Thickness(mousePoint.X, mousePoint.Y, 0, 0);
            }
            if (IsTextStamp)
            {
                TextStampCanvas[sTBIndex - 1].Margin = new Thickness(mousePoint.X, mousePoint.Y, 0, 0);
            }
        }

        //thumbから右クリックを離した場合
        private void peaceMouseRightUp(object sender, MouseButtonEventArgs e)
        {
            Line line = PeaceCanvas.FindName(lineName) as Line;
            Point mousePoint = Mouse.GetPosition(PeaceCanvas);
            if (IsDraw)
            {
                IsDraw = false;
                line.X2 = mousePoint.X;
                line.Y2 = mousePoint.Y;
            }
        }

        //リセットボタンの動作
        private void resetButton(object sender, RoutedEventArgs e)
        {
            Line line;
            int i;

            //ボタン周りのリセット
            EraseButton.IsChecked = false;
            nowLayerInk.EditingMode = InkCanvasEditingMode.None;
            MoveButton.IsChecked = false;
            isDragging = false;

            //コマのリセット
            for (i = 0; i < 10; i++)
            {
                Canvas.SetLeft(thumbs[i], thumbDefaultPoints[i].X);
                Canvas.SetTop(thumbs[i], thumbDefaultPoints[i].Y);
                lineName = thumbs[i].Name + "Line";
                line = PeaceCanvas.FindName(lineName) as Line;
                line.Visibility = Visibility.Collapsed;
                thumbsLayer[i] = nowLayer;
                thumbs[i].Opacity = 1;
                line.Opacity = 1;
            }

            //インクのリセット
            inkCanvasLayer1.Strokes.Clear();
            inkCanvasLayer2.Strokes.Clear();
            inkCanvasLayer3.Strokes.Clear();
            inkCanvasLayer4.Strokes.Clear();

            //スタンプのリセット
            for (; stampIndex > 0; stampIndex--)
            {
                stampImages[stampIndex - 1].Visibility = Visibility.Collapsed;
            }
            for (; sTBIndex > 0; sTBIndex--)
            {
                TextStampCanvas[sTBIndex - 1].Visibility = Visibility.Collapsed;
            }
            TextStampCanvas.Clear();
            stampImages.Clear();
        }

        //Thumbをロードしたらリストに入れて管理しやすくする
        private void thumbLoaded(object sender, RoutedEventArgs e)
        {
            var thumb = sender as Thumb;
            thumbs.Add((Thumb)sender);
            thumbDefaultPoints.Add(thumb.TranslatePoint(new Point(0, 0), PeaceCanvas));
        }

        //消しゴムモードの切替
        private void EraseChecked(object sender, RoutedEventArgs e)
        {
            if (EraseButton == null)
            {
                return;
            }

            EraseButton.IsChecked = true;
            CMItems[0].IsChecked = false;
            CMItems[1].IsChecked = true;
            CMItems[2].IsChecked = false;
            nowLayerInk.EditingMode = InkCanvasEditingMode.EraseByStroke;
            nowInkButton.BorderBrush = new SolidColorBrush(Colors.LightGray);
            MoveButton.IsChecked = false;
            isDragging = false;
        }
        private void EraseUnchecked(object sender, RoutedEventArgs e)
        {
            nowLayerInk.EditingMode = InkCanvasEditingMode.None;
            EraseButton.IsChecked = false;
            CMItems[1].IsChecked = false;

        }

        //画面上でのクリック時の動作
        private void peaceMouseClick(object sender, MouseButtonEventArgs e)
        {
            //スタンプ押す状態のとき
            if (IsStamp || IsTextStamp)
            {
                IsStamp = false;
                IsTextStamp = false;
            }
        }

        //スタンプ押下用メソッド
        private void PressStamp(String StampType)
        {
            Image stamp = new Image
            {
                Source = new BitmapImage(new Uri(StampType, UriKind.RelativeOrAbsolute)),
                Width = 50
            };
            IsStamp = true;
            stampIndex++;


            //マウスカーソルの位置に画像をセットする
            Point mousePoint = Mouse.GetPosition(nowLayerInk);
            stamp.Margin = new Thickness(mousePoint.X, mousePoint.Y, 0, 0);

            //右クリックで削除できるようにイベントハンドラを用意する
            stamp.MouseLeftButtonDown += new MouseButtonEventHandler(Stamp_MouseLeftButtonDown);
            stamp.MouseMove += (Stamp_MouseMove);
            stamp.MouseLeftButtonUp += new MouseButtonEventHandler(Stamp_MouseLeftButtonUp);
            stamp.MouseRightButtonUp += new MouseButtonEventHandler(StampClear);

            //スタンプを配置
            nowLayerStamp.Children.Add(stamp);
            stampImages.Add(stamp);

            CMItems[2].IsChecked = false;
            MenuPenClick(null, null);
        }

        private void PressStamp(Image _stamp)
        {
            // スタンプのコピーを作成
            Image stamp = new Image
            {
                Source = new BitmapImage(new Uri((_stamp.Source as BitmapImage).UriSource.ToString(), UriKind.RelativeOrAbsolute)),
                Width = 50
            };
            IsStamp = true;
            stampIndex++;

            // 右クリックで削除できるようにイベントハンドラを用意する
            stamp.MouseLeftButtonDown += new MouseButtonEventHandler(Stamp_MouseLeftButtonDown);
            stamp.MouseMove += (Stamp_MouseMove);
            stamp.MouseLeftButtonUp += new MouseButtonEventHandler(Stamp_MouseLeftButtonUp);
            stamp.MouseRightButtonUp += new MouseButtonEventHandler(StampClear);

            // マウスカーソルの位置に画像をセットする
            Point mousePoint = Mouse.GetPosition(nowLayerInk);
            stamp.Margin = new Thickness(mousePoint.X, mousePoint.Y, 0, 0);

            // スタンプを配置
            nowLayerStamp.Children.Add(stamp);
            stampImages.Add(stamp);

            CMItems[2].IsChecked = false;
            MenuPenClick(null, null);
        }

        //グレスタンプボタン
        private void fragButtonClick(object sender, RoutedEventArgs e)
        {
            //ペン関係をオフにする
            EraseButton.IsChecked = false;
            nowLayerInk.EditingMode = InkCanvasEditingMode.None;
            MoveButton.IsChecked = false;
            isDragging = false;
            CMItems[2].IsChecked = false;


            //スタンプ押下メソッドを呼び出す
            PressStamp("Resources/frag.png");
        }

        //スモークスタンプボタン
        private void smokeButtonClick(object sender, RoutedEventArgs e)
        {
            //ペン関係をオフにする
            EraseButton.IsChecked = false;
            nowLayerInk.EditingMode = InkCanvasEditingMode.None;
            MoveButton.IsChecked = false;
            isDragging = false;
            CMItems[2].IsChecked = false;

            //スタンプ押下メソッドを呼び出す。
            PressStamp("Resources/smoke.png");
        }

        //スタンスタンプボタン
        private void stunButtonClick(object sender, RoutedEventArgs e)
        {
            //ペン関係をオフにする
            EraseButton.IsChecked = false;
            nowLayerInk.EditingMode = InkCanvasEditingMode.None;
            MoveButton.IsChecked = false;
            isDragging = false;
            CMItems[2].IsChecked = false;

            //スタンプ押下メソッドを呼び出す。
            PressStamp("Resources/stun.png");
        }

        //その他スタンプボタン
        private void stampButtonClick(object sender, RoutedEventArgs e)
        {
            isDragging = false;
            CMItems[2].IsChecked = false;

            // ダイアログのインスタンスを生成
            var dialog = new OpenFileDialog
            {
                //初期ディレクトリを設定
                InitialDirectory = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.IO.Path.GetFullPath(Environment.GetCommandLineArgs()[0])), "stamps"),
                // ファイルの種類を設定
                Filter = "イメージファイル (*.png, *.jpg)|*.png;*.jpg"
            };

            // ダイアログを表示する
            if (dialog.ShowDialog() == true)
            {
                //ダイアログから選択された画像でスタンプを押下する。
                PressStamp(dialog.FileName);
            }
        }

        //レイヤー関連
        private void layerButtonClick(object sender, RoutedEventArgs e)
        {
            //現在のレイヤー関連を保持
            Button beforeLayerButton = FindName(nowLayer + "Button") as Button;
            InkCanvas beforeLayerInk = nowLayerInk;
            Canvas beforeLayerStamp = nowLayerStamp;

            //クリックされたレイヤー情報
            Button nowLayerButton = sender as Button;
            String clickedLayer = nowLayerButton.Content.ToString();
            nowLayer = clickedLayer;
            nowLayerInk = FindName("inkCanvas" + clickedLayer) as InkCanvas;
            nowLayerStamp = FindName("StampCanvas" + clickedLayer) as Canvas;

            //表示系の切り替え
            nowLayerInk.Visibility = Visibility.Visible;
            nowLayerStamp.Visibility = Visibility.Visible;
            beforeLayerInk.Visibility = Visibility.Collapsed;
            beforeLayerInk.EditingMode = InkCanvasEditingMode.None;
            nowInkButton.BorderBrush = new SolidColorBrush(Colors.LightGray);
            CMItems[0].IsChecked = false;
            CMItems[1].IsChecked = false;
            CMItems[2].IsChecked = false;

            EraseButton.IsChecked = false;
            beforeLayerStamp.Visibility = Visibility.Collapsed;
            nowLayerButton.IsEnabled = false;
            beforeLayerButton.IsEnabled = true;

            //コマの透過関連
            int i;
            Line line;
            for (i = 0; i < 10; i++)
            {
                line = FindName(thumbs[i].Name + "Line") as Line;
                if (thumbsLayer[i].Equals(nowLayer))
                {
                    thumbs[i].Opacity = 1;
                    line.Opacity = 1;
                }
                else
                {
                    thumbs[i].Opacity = 0.4;
                    line.Opacity = 0.4;
                }
            }
        }

        //スタンプを右クリックしたときの動作(右クリックしたスタンプを消す)
        private void StampClear(object sender, RoutedEventArgs e)
        {
            if (IsStamp || EraseButton.IsChecked == false)
            {
                return;
            }
            try
            {
                var Stamp = sender as Image;
                Stamp.Visibility = Visibility.Collapsed;
            }
            catch (Exception)
            {
                var Stamp = sender as TextBlock;
                var textcanvas = Stamp.Parent as Canvas;
                textcanvas.Visibility = Visibility.Collapsed;
            }
        }

        //インクのカラー設定
        private void ColorButton(object sender, RoutedEventArgs e)
        {
            int actualDrawingSize = 3;
            int previewDrawingSize = 15;
            //現在のインクと入れ替えたりする
            Button beforeInkButton = nowInkButton;
            nowInkButton = sender as Button;
            CMItems[0].IsChecked = true;

            //インクモード時に同じボタンを押したらインクオフ、それ以外は押した色でインクモード
            if (nowInkButton.BorderBrush.ToString().Equals(Colors.Black.ToString()))
            {
                nowLayerInk.EditingMode = InkCanvasEditingMode.None;
                nowInkButton.BorderBrush = new SolidColorBrush(Colors.LightGray);
                CMItems[0].IsChecked = false;
            }
            else
            {
                EraseButton.IsChecked = false;
                nowLayerInk.EditingMode = InkCanvasEditingMode.Ink;
                beforeInkButton.BorderBrush = new SolidColorBrush(Colors.LightGray);
                nowInkButton.BorderBrush = new SolidColorBrush(Colors.Black);

                MoveButton.IsChecked = false;
                isDragging = false;

            }

            //ボタンの背景色をインク色にする
            SolidColorBrush colorBrush = nowInkButton.Background as SolidColorBrush;
            inkCanvasLayer1.DefaultDrawingAttributes.Color = colorBrush.Color;
            inkCanvasLayer2.DefaultDrawingAttributes.Color = colorBrush.Color;
            inkCanvasLayer3.DefaultDrawingAttributes.Color = colorBrush.Color;
            inkCanvasLayer4.DefaultDrawingAttributes.Color = colorBrush.Color;

            inkDA = new DrawingAttributes
            {
                Color = colorBrush.Color,
                Width = previewDrawingSize,
                Height = previewDrawingSize,
                FitToCurve = false,
                IsHighlighter = false,
                StylusTip = StylusTip.Ellipse,
                IgnorePressure = false
            };

            nowLayerInk.DefaultDrawingAttributes = inkDA;

            // Set the actual drawing size smaller
            nowLayerInk.PreviewMouseLeftButtonDown += (s, args) =>
            {
                inkDA.Width = actualDrawingSize;
                inkDA.Height = actualDrawingSize;
            };

            nowLayerInk.PreviewMouseLeftButtonUp += (s, args) =>
            {
                inkDA.Width = previewDrawingSize;
                inkDA.Height = previewDrawingSize;
            };
        }

        private void TextButtonClick(object sender, RoutedEventArgs e)
        {
            //ペン関係をオフにする
            EraseButton.IsChecked = false;
            nowLayerInk.EditingMode = InkCanvasEditingMode.None;
            MoveButton.IsChecked = false;
            isDragging = false;
            int i;


            TextStampCanvas.Add(new Canvas());

            Point mousePoint = Mouse.GetPosition(nowLayerInk);

            List<TextBlock> stampTextBlocks = new List<TextBlock>();
            TextStampCanvas.Add(new Canvas
            {
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(mousePoint.X, mousePoint.Y, 0, 0)
            }
            );

            for (i = 0; i < 5; i++)
            {
                stampTextBlocks.Add
                    (new TextBlock
                    {
                        Text = StampText.Text,
                        Foreground = new SolidColorBrush(Colors.White),
                        FontSize = 18,
                    }
                    );

                //右クリックで削除できるようにイベントハンドラを用意する
                stampTextBlocks[i].MouseRightButtonUp += new MouseButtonEventHandler(StampClear);
            }

            //一番表の文字だけ赤色、残りは白色で縁取り
            stampTextBlocks[0].Foreground = new SolidColorBrush(Colors.Red);

            //マウスカーソルの位置に画像をセットする
            stampTextBlocks[1].Margin = new Thickness(1, 0, 0, 0);
            stampTextBlocks[2].Margin = new Thickness(-1, 0, 0, 0);
            stampTextBlocks[3].Margin = new Thickness(0, 1, 0, 0);
            stampTextBlocks[4].Margin = new Thickness(0, -1, 0, 0);

            //スタンプを配置
            for (; i > 0; i--)
            {
                TextStampCanvas[sTBIndex].Children.Add(stampTextBlocks[i - 1]);
            }
            nowLayerStamp.Children.Add(TextStampCanvas[sTBIndex]);

            sTBIndex++;
            IsTextStamp = true;
        }

        //右クリックメニュー関連：ペン
        private void MenuPenClick(object sender, RoutedEventArgs e)
        {
            if (CMItems[0].IsChecked == true)
            {
                EraseButton.IsChecked = false;
                nowLayerInk.EditingMode = InkCanvasEditingMode.Ink;
                nowInkButton.BorderBrush = new SolidColorBrush(Colors.Black);
                isDragging = false;
                MoveButton.IsChecked = false;
                CMItems[2].IsChecked = false;
            }
            else
            {
                EraseButton.IsChecked = false;
                nowLayerInk.EditingMode = InkCanvasEditingMode.None;
                nowInkButton.BorderBrush = new SolidColorBrush(Colors.LightGray);
                                isDragging = false;
                MoveButton.IsChecked = false;
                CMItems[2].IsChecked = false;
            }
        }

        //メニュー項目初期化時にリスト化
        private void MenuItemInitialized(object sender, EventArgs e)
        {
            CMItems.Add((MenuItem)sender);
        }

        private void ThumbDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Thumb thumb = sender as Thumb;
            Thumb nextthumb = FindName(thumb.Name + "_dead") as Thumb;
            thumb.Visibility = Visibility.Collapsed;
            nextthumb.Visibility = Visibility.Visible;
        }

        private void ThumbDeadDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Thumb thumb = sender as Thumb;
            String[] split = thumb.Name.Split('_');
            Thumb nextthumb = FindName(split[0]) as Thumb;
            thumb.Visibility = Visibility.Collapsed;
            nextthumb.Visibility = Visibility.Visible;
        }


        // udoya func
        private void OperatorButtonClick(object sender, RoutedEventArgs e)
        {
            string role = (sender as Button).Content.ToString().ToLower();
            string directoryPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "operators", role);
            DisplayIcons(directoryPath);
        }


        private void GadgetButtonClick(object sender, RoutedEventArgs e)
        {
            string name = (sender as Button).Content.ToString().ToLower();
            string directoryPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "gadgets", name);
            DisplayIcons(directoryPath);
        }

        private void DisplayIcons(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                MessageBox.Show("Directory not found: " + directoryPath);
                return;
            }

            var files = Directory.GetFiles(directoryPath, "*.png", SearchOption.AllDirectories);
            if (files.Length == 0)
            {
                MessageBox.Show("No PNG files found in directory: " + directoryPath);
                return;
            }

            ObjectList.Items.Clear();

            foreach (var file in files)
            {
                StackPanel panel = new StackPanel { Orientation = Orientation.Horizontal };
                Image image = new Image
                {
                    Source = new BitmapImage(new Uri(file, UriKind.Absolute)),
                    Width = 30,
                    Height = 30,
                };
                TextBlock textBlock = new TextBlock
                {
                    // size
                    FontSize = 16,
                    Text = System.IO.Path.GetFileNameWithoutExtension(file),
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(5, 0, 0, 0)
                };

                panel.Children.Add(image);
                panel.Children.Add(textBlock);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ObjectList.Items.Add(panel);
                });
            }
            ObjectList.UpdateLayout();
        }
        private void ObjectList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ObjectList.SelectedItem is StackPanel panel)
            {
                Image image = panel.Children.OfType<Image>().FirstOrDefault();
                //ペン関係をオフにする
                EraseButton.IsChecked = false;
                nowLayerInk.EditingMode = InkCanvasEditingMode.None;
                MoveButton.IsChecked = false;
                isDragging = false;

                //スタンプ押下メソッドを呼び出す。
                PressStamp(image);
            }
        }

        private void UpdateAllLayerBackGroundWithFileName(string filePath, int i)
        //
        {
            if (File.Exists(filePath))
            {
                ImageBrush ib = new ImageBrush
                {
                    ImageSource = new BitmapImage(new Uri(filePath, UriKind.Relative)),
                    Stretch = Stretch.Uniform
                };
                switch (i)
                {
                    case 0:
                        inkCanvasLayer1.Background = ib;
                        break;
                    case 1:
                        inkCanvasLayer2.Background = ib;
                        break;
                    case 2:
                        inkCanvasLayer3.Background = ib;
                        break;
                    case 3:
                        inkCanvasLayer4.Background = ib;
                        break;
                }
            }

        }

        private void mapFolderButton_Click(object sender, RoutedEventArgs e)
        {
            // inkcanvaslayer1~4のbackgroundを初期化
            inkCanvasLayer1.Background = null;
            inkCanvasLayer2.Background = null;
            inkCanvasLayer3.Background = null;
            inkCanvasLayer4.Background = null;

            var dialog = new System.Windows.Forms.FolderBrowserDialog
            {
                Description = "使用したいマップが入っているフォルダを選択してください。0,1,2,3.jpg/pngが各レイヤに読みこまれます。",
                // 初期フォルダを実行ディレクトリ+mapに設定する
                SelectedPath = AppDomain.CurrentDomain.BaseDirectory + "maps"
            };

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    string selectedPath = dialog.SelectedPath;
                    // フォルダ内の画像ファイルを取得 (seletcedPath+ "/<0,1,2,3>.png")の4枚、ただし存在しない場合は無視
                    for (int i = 0; i < 4; i++)
                    {
                        string filePath = System.IO.Path.Combine(selectedPath, i + ".png");
                        UpdateAllLayerBackGroundWithFileName(filePath, i);

                        // jpgに対してもやりたかった
                        filePath = System.IO.Path.Combine(selectedPath, i + ".jpg");
                        UpdateAllLayerBackGroundWithFileName(filePath, i);
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("エラーが発生しました。");
                }
            }
        }

        // マウス左ボタン押下時
        private void Stamp_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(isDragging)
            {
                currentStamp = sender as Image;
                startPoint = e.GetPosition(nowLayerStamp);
                currentStamp.CaptureMouse();
                e.Handled = true;
            }
        }

        // マウス移動時
        private void Stamp_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging && currentStamp != null)
            {
                Point currentPoint = e.GetPosition(nowLayerStamp);
                double offsetX = currentPoint.X - startPoint.X;
                double offsetY = currentPoint.Y - startPoint.Y;

                // スタンプの位置を更新
                double newX = currentStamp.Margin.Left + offsetX;
                double newY = currentStamp.Margin.Top + offsetY;
                currentStamp.Margin = new Thickness(newX, newY, 0, 0);

                startPoint = currentPoint;
            }
        }

        // マウス左ボタン解放時
        private void Stamp_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isDragging && currentStamp != null)
            {
                currentStamp.ReleaseMouseCapture();
                currentStamp = null;
                e.Handled = true;
            }
        }

        private void IsMoveButton_Click(object sender, RoutedEventArgs e)
        {

            if (isDragging)
            {
                isDragging = false;
                MoveButton.IsChecked = false;
                CMItems[2].IsChecked = false;

            }
            else
            {
                isDragging = true;
                MoveButton.IsChecked = true;
                EraseButton.IsChecked = false;
                CMItems[0].IsChecked = false;
                CMItems[1].IsChecked = false;
                CMItems[2].IsChecked = true;

                //nowInkButton.BorderBrush = new SolidColorBrush(Colors.LightGray);
            }
        }
    }

}

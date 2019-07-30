# AiForms.Dialogs for  Xamarin.Forms

AiForms.Dialogs は Xamarin.Forms で カスタムダイアログを XAML を使って簡単に作成して自由な位置に表示できるライブラリです。Android と iOS に対応しています。

![Build status](https://kamusoft.visualstudio.com/NugetCI/_apis/build/status/AiForms.Dialogs)

## 機能

* [Dialog](#dialog)
* [Toast](#toast)
* [Loading](#loading)

全てのダイアログは、.NETStandard プロジェクトで XAML または c# コードを使って定義できます。またこれらのダイアログはLayoutAlignmentやOffsetプロパティを指定して好きな位置に表示させることができます。
任意でダイアログの開始・終了時にアニメーションロジックを埋め込むことも可能です。

## API Reference

* [IDialog](#idialog)
* [IReusableDialog](#ireusabledialog)
* [IToast](#itoast)
* [ILoading](#iloading)
* [IReusableLoading](#ireusableloading)
* [ExtraView](#extraview)
* [DialogView](#dialogview)
* [ToastView](#toastview)
* [LoadingView](#loadingview)

## Minimum Platform OS Version 

iOS: iOS10  
Android: version 5.1.1 (only FormsAppcompatActivity) / API22

## Demo

<a href="https://www.youtube.com/watch?feature=player_embedded&v=-DzMvQCQU1Q
" target="_blank"><img src="https://img.youtube.com/vi/-DzMvQCQU1Q/0.jpg" 
alt="" width="480" height="360" border="0" /></a>

# Get Started

## Nuget Installation

[https://www.nuget.org/packages/AiForms.Dialogs/](https://www.nuget.org/packages/AiForms.Dialogs/)

```bash
Install-Package AiForms.Dialogs -pre
```

.NETStandard プロジェクトと各プラットフォームのプロジェクトにインストールする必要があります。

### 各プラットフォームの設定

#### iOS AppDelegate.cs

```csharp
public override bool FinishedLaunching(UIApplication app, NSDictionary options) {
    global::Xamarin.Forms.Forms.Init();
    AiForms.Dialogs.Dialogs.Init(); //need to write here
    ...
}
```

#### Android MainActivity.cs

```csharp
protected override void OnCreate(Bundle bundle)
{
    base.OnCreate(bundle);

    global::Xamarin.Forms.Forms.SetFlags("FastRenderers_Experimental");
    global::Xamarin.Forms.Forms.Init(this, bundle);
    AiForms.Dialogs.Dialogs.Init(this); //need to write here
    ...
}
```

## Dialog

XAML や c# コードで定義したダイアログを呼び出す機能です。
このダイアログを使えば、Custom Renderer や Effects を使わずにダイアログを自由にデザインし好きな場所に配置することができます。

<img src="./images/dialog.png" width="600" />

### DialogView の定義

Dialog機能を利用するには、ContentViewを継承した専用の DialogView を作成する必要があります。

```csharp
public partial class MyDialogView : DialogView
{
    public MyDialogView()
    {
        InitializeComponent();
    }

    public override void SetUp()
    {
        // ダイアログがオープンする度に最初に呼び出されます
    }

    public override void TearDown()
    {
        // ダイアログがクローズする度に呼び出されます
    }

    public override void RunPresentationAnimation()
    {
        // 必要に応じてダイアログ表示時のアニメーション処理を記述します。
    }

    public override void RunDismissalAnimation()
    {
        // 必要に応じてダイアログが閉じる時のアニメーション処理を記述します。
    }

    public override void Destroy()
    {
        // DialogViewのクリーンアップ処理を記述します。ダイアログが破棄された時に呼び出されます。
    }

    void Handle_OK_Clicked(object sender, System.EventArgs e)
    {
        // ダイアログに完了を通知します。
        DialogNotifier.Complete();
    }

    void Handle_Cancel_Clicked(object sender, System.EventArgs e)
    {
        // ダイアログにキャンセルを通知します。
        DialogNotifier.Cancel();
    }
}
```

```xml
<extra:DialogView
    xmlns="http://xamarin.com/schemas/2014/forms" 
    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml" 
    xmlns:extra="clr-namespace:AiForms.Dialogs.Abstractions;assembly=AiForms.Dialogs.Abstractions"
    x:Class="Sample.Views.MyDialogView"
    CornerRadius="10" OffsetX="0" OffsetY="0" UseCurrentPageLocation="false" 
    VerticalLayoutAlignment="Center" HorizontalLayoutAlignment="Center" >

    <StackLayout WidthRequest="200">
        <Label Text="{Binding Title}" />
        <Button Text="Cancel" Clicked="Handle_Cancel_Clicked"/>
        <Button Text="OK" Clicked="Handle_OK_Clicked" />
    </StackLayout>

</extra:DialogView>
```

> Visual Studio の ContentView Xaml のテンプレートから作成するのが良いかもしれません。

### Dialog の表示

DialogViewを定義したら、IDialogのメソッドをどこからでも呼び出せるようになります。
ダイアログを表示するには、AiForms.Dialogs.Dialog.Instance で IDialog のインスタンスを取得して、ShowAsync メソッドを呼び出します。

```csharp
using AiForms.Dialogs;

public async Task SomeMethod()
{
    var ret = await Dialog.Instance.ShowAsync<MyDialogView>(new{Title="Hello"});
    // キャンセルされた場合はfalse, 完了した場合はtrueを返します。
    // 任意で ViewModel を DialogView に渡すことができます。
    // ダイアログが閉じられたら、全ての関連リソースは自動的に破棄されます。

    // また状態を保つことができる IReusableDialog インスタンスを生成することもできます。
    var reusableDialog = Dialog.Instance.Create<MyDialogView>();

    ret = await reusableDialog.ShowAsync();
    // この場合はダイアログが閉じられてもDispose処理は行われません。

    // ShowAsync はDisposeを実行するまで何回でも使用できます。
    reusableDialog.Dispose();
}
```

## Toast

これは、Androidのネイティブの トースト によく似た（というかAndroid実装はToastそのもの）、数秒で消えるダイアログを表示する機能です。
この機能を使えば、Toast の内容を XAML や c# コードで自由にデザインし、好きな場所に配置することができます。

<img src="./images/toast.png" width="600" />

### ToastView の定義

Toast を利用するには、ContentViewを継承した専用の ToastView を定義する必要があります。

```csharp
public partial class MyToastView : ToastView
{
    public MyToastView()
    {
        InitializeComponent();
    }

    // 開始アニメーションの定義
    public override void RunPresentationAnimation() {}

    // 終了アニメーションの定義
    public override void RunDismissalAnimation() {}

    // クリーンアップ処理の定義
    public override void Destroy() {}
}
```

```xml
<extra:ToastView xmlns="http://xamarin.com/schemas/2014/forms"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml" 
             xmlns:extra="clr-namespace:AiForms.Dialogs.Abstractions;assembly=AiForms.Dialogs.Abstractions"
             x:Class="Sample.Views.MyToastView"
             HorizontalLayoutAlignment="Center"
             VerticalLayoutAlignment="Center"
             OffsetX="50" OffsetY="50" Duration="{Binding Duration}" >

    <StackLayout WidthRequest="150" Spacing="0">
        <Label Text="{Binding Title}" />
    </StackLayout>
</extra:ToastView>
```

### Toast の表示

ToastView を定義したら、IToast のメソッドをどこからでも利用できるようになります。
Toast を表示するには、AiForms.Dialogs.Toast.Instance から IToast インスタンスを取得して、Show メソッドを呼びます。

```csharp
using AiForms.Dialogs;

public async Task SomeMethod()
{
    Toast.Instance.Show<MyToastView>(new{Title="Hello",Duration=1500});
    // 任意で、ViewModel をToastViewに渡すことができます。
}
```

## Loading

これは、ローディング状況を通知するための、ユーザーが閉じることができないダイアログを表示する機能です。
これには2つのモードがあり、ひとつは、デフォルトの組み込みダイアログで、もうひとつは、DialogやToastと同じような LoadingView を使ったカスタムダイアログです。
LoadingView によるカスタムダイアログを利用すると、自由に内容をデザインして好きな位置に配置することが可能です。
どちらのモードもViewに進捗を通知する機能を持っています。

<img src="./images/loading.png" width="600" />

<img src="./images/customloading.png" width="600" />

### デフォルトの Loading Dialog の表示

Default の Loading Dialog を表示するには、AiForms.Dialogs.Loading.Instanceから ILoading インスタンスを取得して、StartAsync メソッドを呼び出します。

以下のコードはデフォルトの Loading Dialog の使用方法を示しています。

```csharp
using AiForms.Dialogs;
using AiForms.Dialogs.Abstractions;
public async Task SomeMethod()
{
    // Default Loading Dialog 用の設定が可能です。
    Configurations.LoadingConfig = new LoadingConfig {
        IndicatorColor = Color.White,
        OverlayColor = Color.Black,
        Opacity = 0.4,
        DefaultMessage = "Loading...",
    };

    await Loading.Instance.StartAsync(async progress =>{
        // 重い処理など
        for (var i = 0; i < 100; i++)
        {
            await Task.Delay(50);
            // IProgress<double>の値を使ってDialogに進捗を送信することができます。
            progress.Report((i + 1) * 0.01d);
        }
    });
}
```

### カスタムの LoadingView の定義

カスタムの Loading Dialog には、ContentView を継承した専用の LoadingView を定義する必要があります。

```csharp
public partial class MyLoadingView : LoadingView
{
    Animation animation;

    public MyLoadingView()
    {
        InitializeComponent();

        animation = new Animation(v=>{
            image.Rotation = 360 * v;

            if(v <= 0.3){
                image.Scale = 1.0 - 0.5 * v / 0.3;
            }
            else if(v <= 0.6){
                image.Scale = 0.5 + 0.8 * (v - 0.3) / 0.3;
            }
            else {
                image.Scale = 1.3 - 0.2 * (v - 0.6) / 0.4;
            }
        },0,1,Easing.Linear,()=>{
            
        });
    }

    // 必要に応じて進捗用のループアニメーションなどを定義します。
    public override void RunPresentationAnimation()
    {
        this.Animate("sample", animation, 16, 1440, null, (v, c) => {
            image.Rotation = 0;
            image.Scale = 1;
        }, () => true);
    }

    // アニメーションの停止を指示します。
    public override void RunDismissalAnimation()
    {
        this.AbortAnimation("sample");
    }

    // クリーンアップ処理を定義します。
    public override void Destroy() {}
}
```

```xml
<extra:LoadingView 
    xmlns="http://xamarin.com/schemas/2014/forms"
    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml" 
    xmlns:extra="clr-namespace:AiForms.Dialogs.Abstractions;assembly=AiForms.Dialogs.Abstractions"
    x:Class="Sample.Views.MyLoadingView" 
    x:Name="Me" 
    OverlayColor="#2000FF00" >
    <StackLayout WidthRequest="60" HeightRequest="60" Spacing="0">
        <Image x:Name="image" WidthRequest="36" HeightRequest="36" Source="icon.png" />
        <Label Text="{Binding Message}" FontSize="Micro" />
        <Label Text="{Binding Progress,Source={x:Reference Me},StringFormat='{0:P0}'}" FontSize="Micro" />        
    </StackLayout>
</extra:LoadingView>
```

> LoadingView は double型のProgressプロパティを持っています。このプロパティを自身にバインディングすれば、進捗を表現することが可能です。

### カスタムの Loading Dialog の表示

LoadingView を定義すると、ILoading.Create メソッドを使って、IReusableLoading のインスタンスを生成することができます。
IReusableLoading インスタンスの StartAsync メソッドを使って、カスタム Loading Dialog を表示することができます。

```csharp
using AiForms.Dialogs;

public async Task SomeMethod()
{
    // IReusableLoading インスタンスを生成
    var reusableLoading = Loading.Instance.Create<MyLoadingView>();
    // 任意で ViewModel を LoadingView に渡すことができます。

    await reusableLoading.StartAsync(async progress => {
        // 重い処理など
        await Task.Delay(1000);
        // IProgress を使って LoadingView に進捗を送ることができます。
        progress.Report(1.0);
    });

    // StartAsync メソッドはDisposeするまで何回でも使用できます。
    reusableLoading.Dispose();
}
```

# API Reference

## IDialog

### Methods

#### Task`<bool>` ShowAsync`<T>`(object viewModel = null)
#### Task`<bool>` ShowAsync(DialogView view, object viewModel = null)
    
* 型またはDialogViewのインスタンスを指定してダイアログを表示します。任意でBindingするためのViewModelを渡すことができます。
* ダイアログが閉じられたとき、全ての関連リソースは自動的に破棄されます。
* Cancelの場合はfalse, Completeの場合はtrueを返します。


#### IReusableDialog Create`<TView>`(object viewModel = null) 
#### IReusableDialog Create(DialogView view, object viewModel = null)

* 型またはDialogViewのインスタンスを指定して IReusableDialog のインスタンスを生成します。任意でBindingするためのViewModelを渡すことができます。

## IReusableDialog

### Methods

#### Task`<bool>` ShowAsync()

* ダイアログを表示します。
* Cancelの場合はfalse, Completeの場合はtrueを返します。
* ダイアログを閉じてもDisposeを実行するまでリソースを破棄しません。

#### Dispose()

## IToast

### Methods

#### void Show`<TView>`(object viewModel = null)

* 型を指定して Toast を表示します。任意でViewModelを渡すことができます。

## ILoading

### Methods

#### Task StartAsync(Func`<IProgress<double>, Task>` action, string message = null, bool isCurrentScope = false)

* Loading Dialog を表示し、処理を待機します。
* action には 目的の処理を行いTaskを返すFuncを指定します。このFuncはIProgressのインスタンスのパラメータがあり、これを利用して進捗をダイアログに通知することが可能です。
* message にはLoadingアイコンの下に表示する文字列を指定します。
* ダイアログが属する親Viewを KeyWindow から 現在のViewController の View に変更する場合は isCurrentScope に true を指定します。iOS でのみ有効で、これによってモーダルの範囲をアプリケーションレベルではなくカレントページの範囲に限定することができます。

#### void Show(string message = null, bool isCurrentScope = false)

* 待機なしで Loading Dialog を表示します。

#### void Hide()

* Loading Dialog を閉じます。

#### void SetMessage(string message)

* ローディングの文字列を変更します。

#### IReusableLoading Create`<TView>`(object viewModel = null)
#### IReusableLoading Create(LoadingView view, object viewModel = null)

* 型または LoadingView のインスタンスを指定して IReusableLoading インスタンスを生成します。任意で ViewModel を渡すこともできます。

## IReusableLoading

### Methods

#### Task StartAsync(Func`<IProgress<double>, Task>` action, bool isCurrentScope = false)

* カスタムの Loading Dialog を表示して処理を待ちます。

#### void Show(bool isCurrentScope = false)

* 待機なしでカスタムの Loading Dialog を表示します。

#### void Hide()

* カスタムの Loading Dialog を閉じます。


## LoadingConfig

デフォルトのLoading Dialogのグローバル設定です。
AiForms.Dialogs.Abstractions.Configurations プロパティにこのインスタンスをセットすると設定が有効になります。

> カスタム Loading Dialog を使用する場合は、この設定は使われません。

### Properties

* OffsetX
    * 水平方向の位置調整の値。
* OffsetY
    * 垂直方向の位置調整の値。
* IndicatorColor
    * Loading アイコンの色。
* FontSize
    * ローディング文字列のフォントサイズ。
* FontColor
    * ローディング文字列の文字色。
* OverlayColor
    * ダイアログのオーバレイの背景色。
* Opacity
    * 全体の透過値。 (0-1)
* DefaultMessage
    * 規定のローディング文字列。
* ProgressMessageFormat
    * ローディング文字列のフォーマット。c# の string format に準拠します。
    * 例） ``{0}\n{1:P0}`` 0がmessage 1がprogressになっています。
* IsReusable
    * LoadingView を再利用するかどうか。 (default: false)
    * 通常は、ダイアログが閉じられる度にインスタンスは破棄されますが、trueにするとインスタンスが再利用されるようになり、閉じた時に破棄されず、次に開くときに再生成しません。

## ExtraView

これは、DialogView, ToastView, and LoadingView の基底クラスです。

このViewは、HorizontalLayoutAlignment、VerticalLayoutAlignment、OffsetX、OffsetXなどの自身の位置を決定するプロパティが定義されており、全てのViewで利用することができます。

ContentViewを継承しているので任意の VisualElement を配置できます。

またダイアログの表示時、消滅時のアニメーションやダイアログが破棄される時のクリーンアップ処理などを定義できる virtual method が用意されています。

### 共通プロパティ

* ProportionalWidth
    * 画面幅に対する割合で幅を指定します。(0-1)
    * 指定しなければ、幅には WidthRequestの値が自動サイズが使われます。
* ProportionalHeight
    * 画面の高さに対する割合で高さを指定します。(0-1)
    * 指定しなければ、幅には HeightRequestの値が自動サイズが使われます。
* VerticalLayoutAlignment
    * 垂直位置を指定を Start / Center / End / Fill から指定します。 (default: Center)
* HorizontalLayoutAlignment
    * 水平位置を Start / Center / End / Fill から指定します。 (default: Center)
* OffsetX
    * LayoutAlignmentからの水平方向の調整位置を相対値で指定します。
* OffsetY
    * LayoutAlignmentからの垂直方向の調整位置を相対値で指定します。
* CornerRadius
    * ダイアログの角を丸める場合の値。

### 共通 Virtual Methods

* RunPresentationAnimation
    * ダイアログが表示されるときのアニメーションを記述します。
* RunDismissalAnimation
    * ダイアログが閉じられるときのアニメーションを記述します。
* Destroy
    * 自身のクリーンアップする処理などを記述します。ダイアログが破棄された時に呼ばれます。

> アニメーション の時間は現在のところ 250ms を想定して設計されていますので、これ以上長い時間を設定するとアニメーションが途中で中止される可能性があります。

## DialogView

この View は、IDialog または IReusableDialog が表示するダイアログの中身を定義するものです。
全ての ExtraView のプロパティとメソッドと以下のプロパティとメソッドが利用可能です。

### Properties

* IsCanceledOnTouchOutside
    * ダイアログの外側をタッチした場合に、キャンセル扱いにするかどうか。(default: ture)
* OverlayColor
    * ダイアログの外側のオーバーレイの色。
* UseCurrentPageLocation
    * HorizontalLayoutAlignment と VerticalLayoutAlignment で配置する位置の基準をWindowエリアにするか現在のページのエリアにするかの値。
    * Trueにすると、各辺はページ内に限定されます。ステータスバー、ナビゲーションバー、タブバーなどを含まない範囲になります。

#### DialogNotifier

* DialogView から IDialog インスタンスへ キャンセルか完了の通知を送るためのオブジェクト。
* DialogView内でボタンをタップした時にダイアログを完了させたりキャンセルさせたりするには以下のコードのようにします。

```csharp
void Handle_OK_Clicked(object sender, System.EventArgs e)
{
    DialogNotifier.Complete();
}

void Handle_Cancel_Clicked(object sender, System.EventArgs e)
{
    DialogNotifier.Cancel();
}
```

また ViewModelからバインディングを通じて使用することもできます。

```Xml
<ex:DialogView DialogNotifier="{Binding Notifier}">
```

```csharp
public IDialogNotifier Notifier { get; set; }

async void Show()
{
    await Dialog.Instance.ShowAsync<MyDialogView>(this);
}

void Complete()
{
    Notifier.Complete();
}
void Cancel()
{
    Notifier.Cancel();
}
```

### Virtual Methods

* SetUp
    * ダイアログが開かれる度に毎回呼ばれます。ここに自身の準備処理を記述します。IReusableDialog用です。
* TearDown
    * ダイアログが閉じられる度に毎回呼ばれます。ここに後始末用の処理を記述します。IReusableDialog用です。

## ToastView

このViewは、IToast が表示するトーストの内容を定義するためのものです。
全てのExtraViewのプロパティとメソッドと以下のプロパティが利用できます。

> Androidでは Toast は OffsetX や OffsetY の値を使って Windows の端を越えることはできません。

### Properties

* Duration
    * トーストが消えるまでのミリ妙。(1-3500) (default:1500)

## LoadingView

このViewは、IReusableLoading が表示するカスタムの Loading dialog の内容を定義するためのものです。
全てのExtraViewのプロパティとメソッドと以下のプロパティが利用できます。

### Properties

* Progress
    * 進捗を表現するためのdouble型の値。
    * この値を使ってカスタムViewで、UIで進捗を表現することができます。
* OverlayColor
    * ダイアログの全体の背景色。

## License

The MIT Licensed.

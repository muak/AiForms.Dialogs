# AiForms.Dialogs for  Xamarin.Forms

This is a collection of Custom Dialogs that can be defined with XAML for Xamarin.Forms (Android / iOS).

[Japanese](./README-ja.md)

## Available Features

* [Dialog](#dialog)
* [Toast](#toast)
* [Loading](#loading)

All dialogs can be created with XAML or c# code in a NETStandard project.
Also, they can be arranged the position where you want by specifying LayoutAlignment and Offset properties.
Optionally, the animations when the dialog is appearing or disappearing can be set.

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

# Get Started

## Nuget Installation

[https://www.nuget.org/packages/AiForms.Dialogs/](https://www.nuget.org/packages/AiForms.Dialogs/)

```bash
Install-Package AiForms.Dialogs -pre
```

You need to install this nuget package to .NETStandard project and each platform project.

### For each platform project

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

This is the dialog that can be defined with XAML or c# code.
This dialog allows you to freely design the dialog content and arrange anywhere without custom renderer and effects.

### Create a DialogView

It is necessary for Dialog to create the dedicated DialogView derived from ConentView as the following:

```csharp
public partial class MyDialogView : DialogView
{
    public MyDialogView()
    {
        InitializeComponent();
    }

    public override void SetUp()
    {
        // called each opening dialog
    }

    public override void TearDwon()
    {
        // called each closing dialog
    }

    public override void RunPresentationAnimation()
    {
        // define opening animation
    }

    public override void RunDismissalAnimation()
    {
        // define closing animation
    }

    public override void Destroy()
    {
        // define clean up process.
    }

    void Handle_OK_Clicked(object sender, System.EventArgs e)
    {
        // send complete notification to the dialog.
        DialogNotifier.Complete();
    }

    void Handle_Cancel_Clicked(object sender, System.EventArgs e)
    {
        // send cancel notification to the dialog.
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

> You should make use of ContentView Xaml Template with Visual Studio.

### Show Dialog

Once a DialogView defined, Dialog methods can be used from wherever.

```csharp
using AiForms.Dialogs;

public async Task SomeMethod()
{
    var ret = await Dialog.Instance.ShowAsync<MyDialogView>(new{Title="Hello"});
    // If canceled, ret is false; Otherwise is true.
    // Optionally, view model can be passed to the dialog view instance.
    // When the dialog is closed, all related resource automatically are disposed.

    // Also, IReusableDialog that keeps the state can be created.
    var reusableDialog = Dialog.Instance.Create<MyDialogView>();

    ret = await reusableDialog.ShowAsync();
    // Even if the dialog is closed, disposing process is not executed.

    // ShowAsync can be used any number of times until disposing it.
    reusableDialog.Dispose();
}
```

## Toast

This is the dialog that disappears after some second, which is similar to Android Native Toast Control.
This can be defined with XAML or c# code and allows you to freely design the content and arrange anywhere.

### Create a ToastView

It is necessary for Toast to create the dedicated ToastView derived from ConentView as the following:

```csharp
public partial class MyToastView : ToastView
{
    public MyToastView()
    {
        InitializeComponent();
    }

    // define appearing animation
    public override void RunPresentationAnimation() {}

    // define disappearing animation
    public override void RunDismissalAnimation() {}

    // define clean up process.
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

### Show Toast

Once a ToastView is defined, Toast methods can be used from wherever.

```csharp
using AiForms.Dialogs;

public async Task SomeMethod()
{
    Toast.Instance.Show<MyToastView>(new{Title="Hello",Duration=1500});
    // Optionally, view model can be passed to the toast view instance.
}
```

## Loading

This is the dialog that can't be close by a user, which is appropriate for the notification of loading progress.
This has two modes, one is default built-in dialog, the other is custom dialog using LoadingView the same as Dialog or Toast.
LoadingView allows you to freely design the content and arrange anywhere.
Both of these have the function that notifies progress to the view.

### Show default Loading Dialog

The following code describes how to use default Loading Dialog:

```csharp
using AiForms.Dialogs;
using AiForms.Dialogs.Abstractions;
public async Task SomeMethod()
{
    // Loading settings
    Configurations.LoadingConfig = new LoadingConfig {
        IndicatorColor = Color.White,
        OverlayColor = Color.Black,
        Opacity = 0.4,
        DefaultMessage = "Loading...",
    };

    await Loading.Instance.StartAsync(async progress =>{
        // some heavy process.
        for (var i = 0; i < 100; i++)
        {
            await Task.Delay(50);
            // can send progress to the dialog with the IProgress.
            progress.Report((i + 1) * 0.01d);
        }
    });
}
```

### Create custom loading view

It is necessary for Custom Loading to create the dedicated LoadingView derived from ConentView as the following:

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

    // Start Loading animation 
    public override void RunPresentationAnimation()
    {
        this.Animate("sample", animation, 16, 1440, null, (v, c) => {
            image.Rotation = 0;
            image.Scale = 1;
        }, () => true);
    }

    // Stop animation
    public override void RunDismissalAnimation()
    {
        this.AbortAnimation("sample");
    }

    // Clean up
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

> LoadingView has a Progress property of type double, the progress can be expressed by binding the Progress property of itself.

### Show a custom loading dialog

Once a LoadingView is defined, Create method can create a IReusableLoading instance. Using the StartAsync method of the IReusableLoading instance can show a custom loading dialog.

```csharp
using AiForms.Dialogs;

public async Task SomeMethod()
{
    // Create a IReusableLoading instance.
    var reusableLoading = Loading.Instance.Create<MyLoadingView>();
    // Optionally, view model can be passed to the dialog view instance.

    await reusableLoading.StartAsync(async progress => {
        // heavy process
        await Task.Delay(1000);
        // can send progress to the LoadingView with the IProgress.
        progress.Report(1.0);
    });

    // StartAsync can be used any number of times until disposing it.
    reusableLoading.Dispose();
}
```

# API Reference

## IDialog

### Methods

* Task`<bool>` ShowAsync`<T>`(object viewModel = null)
* Task`<bool>` ShowAsync(DialogView view, object viewModel = null)
    * A dialog is shown by specifying a Type or a view instance. Optionally, A view model can be passed to in order to bind.
    * When the dialog is closed, all related resource automatically are disposed.

* IReusableDialog Create`<TView>`(object viewModel = null) 
* IReusableDialog Create(DialogView view, object viewModel = null)
    * A IReusableDialog instance is created by specifying a Type or a view instance. Optionally, A view model can be passed to in order to bind.

## IReusableDialog

### Methods

* Task`<bool>` ShowAsync()
    * A dialog is shown.
    * Even if the dialog is closed, disposing process is not executed.
* Dispose()

## IToast

### Methods

* void Show`<TView>`(object viewModel = null)

## ILoading

### Methods

* void Show(string message = null, bool isCurrentScope = false)
* void Hide()
* void SetMessage(string message)
* Task StartAsync(Func`<IProgress<double>, Task>` action, string message = null, bool isCurrentScope = false)
* IReusableLoading Create`<TView>`(object viewModel = null)
* IReusableLoading Create(LoadingView view, object viewModel = null)

## IReusableLoading

### Methods

* void Show(bool isCurrentScope = false)
* void Hide()
* Task StartAsync(Func`<IProgress<double>, Task>` action, bool isCurrentScope = false)


## LoadingConfig

### Properties

* OffsetX
* OffsetY
* IndicatorColor
* FontSize
* FontColor
* OverlayColor
* Opacity
* DefaultMessage
* ProgressMessageFormat
* IsReusable

## ExtraView

This is base class of DialogView, ToastView, and LoadingView.

### Common Propoerties

* ProportionalWidth
* ProportionalHeight
* VerticalLayoutAlignment
* HorizontalLayoutAlignment
* OffsetX
* OffsetY
* CornerRadius

### Common Virtual Methods

* RunPresentationAnimation
* RunDismissalAnimation
* Destroy

## DialogView

### Properties
* IsCanceledOnTouchOutside
* OverlayColor
* UseCurrentPageLocation
* DialogNotifier

### Virtual Methods

* SetUp
* TearDown

## ToastView

### Properties

* Duration

## LoadingView

### Properties

* Progress
* OverlayColor

## License

The MIT Licensed.
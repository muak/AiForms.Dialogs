using AiForms.Extras.Abstractions;
using Xamarin.Forms;

namespace Sample.Views
{
    public partial class MyIndicatorView : ContentView,IIndicatorView
    {
        Animation animation;

        public MyIndicatorView()
        {
            InitializeComponent();

            image.Source = ImageSource.FromResource("Sample.Resources.ios7-paw-outline.png");

            animation = new Animation(v=>{
                this.Rotation = 360 * v;

                if(v <= 0.3){
                    this.Scale = 1.0 - 0.5 * v / 0.3;
                }
                else if(v <= 0.6){
                    this.Scale = 0.5 + 0.8 * (v - 0.3) / 0.3;
                }
                else {
                    this.Scale = 1.3 - 0.2 * (v - 0.6) / 0.4;
                }


            },0,1,Easing.Linear,()=>{
                
            });
        }

        public void StartAnimating()
        {
            this.Animate("sample", animation,16,1440,null,(v,c)=>{
                this.Rotation = 0;
                this.Scale = 1;
            },()=>true);
        }

        public void StopAnimating()
        {
            this.AbortAnimation("sample");
        }
    }
}

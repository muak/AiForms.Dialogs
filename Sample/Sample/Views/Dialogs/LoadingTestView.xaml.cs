using System;
using System.Collections.Generic;
using Xamarin.Forms;
using AiForms.Dialogs.Abstractions;

namespace Sample.Views
{
    public partial class LoadingTestView : LoadingView
    {
        public int seq = 1;
        public int progressCnt = 0;
        public List<bool> Assert = new List<bool>();

        public LoadingTestView()
        {
            InitializeComponent();
            this.PropertyChanged += (sender, e) => {
                if(e.PropertyName == LoadingView.ProgressProperty.PropertyName)
                {
                    progressCnt++;
                }
            };
        }

        public override void RunPresentationAnimation()
        {
            Assert.Add(seq++ == 1); // is first
        }

        public override void RunDismissalAnimation()
        {
            Assert.Add(seq++ == 2); // is second
            Assert.Add(progressCnt == 4); // called four times(system call 1)
        }

        public override void Destroy()
        {
            Assert.Add(seq++ == 3); // is third           
        }
    }
}

using System;
using System.Collections.Generic;
using Xamarin.Forms;
using AiForms.Dialogs.Abstractions;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Sample.Views
{
    public partial class ToastTestView : ToastView
    {
        public int seq = 1;
        public List<bool> Assert = new List<bool>();
        public Stopwatch Stopwatch;
        public TaskCompletionSource<bool> Tcs { get; set; }

        public ToastTestView()
        {
            InitializeComponent();
            Stopwatch = new Stopwatch();
        }

        public override void RunPresentationAnimation()
        {
            Stopwatch.Start();
            Assert.Add(seq++ == 1); // is first
        }

        public override void RunDismissalAnimation()
        {
            Assert.Add(seq++ == 2); // is second
        }

        public override void Destroy()
        {
            Stopwatch.Stop();
            Assert.Add(seq++ == 3); // is third

            // is approximate
            Assert.Add(Stopwatch.ElapsedMilliseconds > this.Duration - 150 &&
                       Stopwatch.ElapsedMilliseconds < this.Duration + 150);
            Debug.WriteLine(Stopwatch.ElapsedMilliseconds);
            Tcs.SetResult(true);
        }



        public void Clear()
        {
            seq = 1;
            Assert.Clear();
            Stopwatch.Reset();
        }
    }
}

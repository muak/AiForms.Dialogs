using System;
using System.Collections.Generic;
using AiForms.Dialogs.Abstractions;
using Xamarin.Forms;

namespace Sample.Views
{
    public partial class DialogTestView : DialogView
    {
        public int seq = 1;
        public List<bool> Assert = new List<bool>();

        public DialogTestView()
        {
            InitializeComponent();
        }

        public override void SetUp()
        {
            seq = 1;
            Assert.Clear();
            Assert.Add(seq++ == 1); // is first
        }

        public override void TearDown()
        {
            Assert.Add(seq++ == 4);
        }

        public override void RunPresentationAnimation()
        {
            Assert.Add(seq++ == 2);
        }

        public override void RunDismissalAnimation()
        {
            Assert.Add(seq++ == 3);
        }

        public override void Destroy()
        {
            Assert.Add(seq++ == 5);
        }

    }
}

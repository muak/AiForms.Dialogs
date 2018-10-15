using System;
using XF = Xamarin.Forms;
using Pc = Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

namespace Sample.Views
{
    public class MyTabbedPage:XF.TabbedPage
    {
        public MyTabbedPage()
        {
            On<Xamarin.Forms.PlatformConfiguration.Android>()
                      .SetToolbarPlacement(ToolbarPlacement.Bottom);
        }
    }
}

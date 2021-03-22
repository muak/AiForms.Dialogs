using System;
using Xamarin.Forms;
namespace AiForms.Dialogs.Abstractions
{
    internal static class PageExtensions
    {
        public static Page GetActivePage(this Page page)
        {
            if(page is MasterDetailPage)
            {
                var mdPage = page as MasterDetailPage;
                return mdPage.Detail.GetActivePage();
            }

            if(page is TabbedPage)
            {
                var tabbedPage = page as TabbedPage;
                return tabbedPage.CurrentPage.GetActivePage();
            }

            if(page is NavigationPage)
            {
                var navPage = page as NavigationPage;
                return navPage.CurrentPage.GetActivePage();
            }

            return page;
        }
    }
}

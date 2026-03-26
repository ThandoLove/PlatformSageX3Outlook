
using Microsoft.AspNetCore.Components;

namespace OperationalWorkspaceUI.UIServices.System

{
    public class NavigationService
    {
        private readonly NavigationManager _nav;

        public NavigationService(NavigationManager nav)
        {
            _nav = nav;
        }

        public void NavigateTo(string url)
        {
            _nav.NavigateTo(url);
        }

        public void NavigateTo(string url, bool forceLoad)
        {
            _nav.NavigateTo(url, forceLoad);
        }
    }
}
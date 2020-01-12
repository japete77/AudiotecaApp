using System;

using Xamarin.Forms;

namespace audioteca
{
    public class AudioLibraryPage : ContentPage
    {
        public AudioLibraryPage()
        {
            Content = new StackLayout
            {
                Children = {
                    new Label { Text = "Hello ContentPage" }
                }
            };
        }
    }
}


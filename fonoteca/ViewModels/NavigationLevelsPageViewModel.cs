﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using fonoteca.Models.Player;

namespace fonoteca.ViewModels
{
    public partial class NavigationLevelsPageViewModel : ObservableObject
    {
        [ObservableProperty]
        private List<NavigationLevel> items;

        [ObservableProperty]
        private bool loading;

        [RelayCommand]
        public async Task GoToBack()
        {
            await Shell.Current.Navigation.PopAsync(true);
        }
    }
}

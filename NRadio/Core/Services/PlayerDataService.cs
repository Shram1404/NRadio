using NRadio.Core.Models;
using NRadio.ViewModels;
using NRadio.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NRadio.Core.Services
{
    //public class PlayerDataService
    //{
    //    public event PropertyChangedEventHandler PropertyChanged;

    //    public MainViewModel MainVM { get; set; }
    //    public PlayerViewModel PlayerVM { get; private set; }

    //    public PlayerPage MainPlayerView { get; private set; }
    //    private MiniPlayerPage _miniPlayerView;
    //    public MiniPlayerPage MiniPlayerView
    //    {
    //        get => _miniPlayerView;
    //        private set
    //        {
    //            if (_miniPlayerView != value)
    //            {
    //                _miniPlayerView = value;
    //                OnPropertyChanged(nameof(MiniPlayerView));
    //            }
    //        }
    //    }
    //    private bool _isPlayerCreated = false;
    //    public bool IsPlayerCreated
    //    {
    //        get => _isPlayerCreated;
    //        private set
    //        {
    //            if (_isPlayerCreated != value)
    //            {
    //                _isPlayerCreated = value;
    //                OnPropertyChanged(nameof(IsPlayerCreated));
    //            }
    //        }
    //    }

    //    public void StartPlay(ObservableCollection<RadioStation> radioStations, int currentSongIndex)
    //    {
    //        if (PlayerVM == null)
    //        {
    //            PlayerVM = new PlayerViewModel(radioStations, currentSongIndex);

    //            MainPlayerView = new PlayerPage();
    //            MainPlayerView.DataContext = PlayerVM;

    //            MiniPlayerView = new MiniPlayerPage();
    //            MiniPlayerView.DataContext = PlayerVM;

    //            IsPlayerCreated = true;
    //            PlayerVM.PlayPause();
    //        }
    //        else PlayerVM.ChangePlaylist(radioStations, currentSongIndex);
    //    }
    //    private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(null, new PropertyChangedEventArgs(propertyName));
    //}
}

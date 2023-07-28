using System;

using Microsoft.Toolkit.Mvvm.ComponentModel;

using Windows.Media.Core;
using Windows.Media.Playback;

namespace NRadio.ViewModels
{
    public class MediaPlayerViewModel : ObservableObject
    {
        private const string DefaultSource = "https://www.learningcontainer.com/wp-content/uploads/2020/05/sample-mp4-file.mp4";
        private const string DefaultPoster = "https://www.learningcontainer.com/wp-content/uploads/2020/05/Sample-jpg-image-500kb.jpg";


        private IMediaPlaybackSource _source;

        public IMediaPlaybackSource Source
        {
            get { return _source; }
            set { SetProperty(ref _source, value); }
        }

        private string _posterSource;

        public string PosterSource
        {
            get { return _posterSource; }
            set { SetProperty(ref _posterSource, value); }
        }

        public MediaPlayerViewModel()
        {
            Source = MediaSource.CreateFromUri(new Uri(DefaultSource));
            PosterSource = DefaultPoster;
        }

        public void DisposeSource()
        {
            var mediaSource = Source as MediaSource;
            mediaSource?.Dispose();
            Source = null;
        }
    }
}

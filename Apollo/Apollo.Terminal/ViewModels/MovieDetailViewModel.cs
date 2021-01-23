using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Apollo.Core.Dto;
using Apollo.Core.Interfaces;
using Apollo.Terminal.Commands;
using Apollo.Terminal.Common;
using Apollo.Terminal.Types;
using Apollo.Terminal.Types.TransferObject;
using Apollo.Terminal.ViewModels.Base;
using Apollo.Util;
using LibVLCSharp.Shared;
using MaterialDesignThemes.Wpf;
using MediaPlayer = LibVLCSharp.Shared.MediaPlayer;

namespace Apollo.Terminal.ViewModels
{
    public class MovieDetailViewModel : BasePageViewModel
    {

        #region Member

        private const string KeyPattern = @"https:[~{\a-zA-Z]*.mp4\?[(~{\a-zA-Z]*";

        private readonly IServiceFactory _serviceFactory;

        private PackIcon _content;
        private string _streamUrl;
        private bool _playButtonVisible;
        private MovieDto _movieDto;
        private IEnumerable<ActorDto> _actors;
        private LibVLC _libVlc;

        private bool _videoPaused;
        private readonly long _playButtonInactiveTimeout;
        private readonly DispatcherTimer _playButtonInactiveTimer;
        private IList<ScheduleDto> _schedules;
        private ScheduleDto _selectedSchedule;

        #endregion

        #region Properties

        public string Actors => $"{_actors.Select(a => a.Name).Aggregate((current, next) => $"{current}, {next}")}";
        public int MaxActorLength => 200;
        public int MaxDescriptionLength => 665;
        public string ShortenText => "...";

        public string Description => _movieDto?.Description;

        public string Genre => _movieDto?.Genre?.Name;

        public byte[] Image => _movieDto?.Image;

        public int Rating => _movieDto.Rating;

        public string Title => _movieDto?.Title;

        public string Duration => $"{_movieDto.Duration} min.";

        public string TrailerUrl => _movieDto?.Trailer;

        public MediaPlayer MediaPlayer { get; private set; }

        public bool PlayButtonVisible
        {
            get => _playButtonVisible;
            set => Set(ref _playButtonVisible, value);
        }

        public string StreamUrl
        {
            get => _streamUrl;
            private set => Set(ref _streamUrl, value);
        }

        public PackIcon ButtonContent
        {
            get => _content;
            set => Set(ref _content, value);
        }

        public ObservableCollection<ScheduleDto> Schedules { get; set; }

        public ScheduleDto SelectedSchedule
        {
            get => _selectedSchedule;
            set
            {
                Set(ref _selectedSchedule, value);
                OnNavigateToWizard(value);
            }
        }

        #endregion

        #region Commands

        public ICommand VideoPlayerTouch { get; set; }

        public ICommand TogglePlay { get; set; }

        public ICommand NavigateBack { get; set; }

        #endregion

        #region Constructors
        public MovieDetailViewModel(IServiceFactory serviceFactory)
        {
            _playButtonInactiveTimeout = long.Parse(ConfigurationHelper.GetValues("Play_Button_Inactive_Timeout").First());

            _serviceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));

            _playButtonInactiveTimer = new DispatcherTimer();

            Init();
            SetupInactiveTimer();
        }
        #endregion

        #region Methods

        private void Init()
        {
            LibVLCSharp.Shared.Core.Initialize();
            _libVlc = new LibVLC();
            MediaPlayer = new MediaPlayer(_libVlc);
            Schedules = new ObservableCollection<ScheduleDto>();
            MediaPlayer.EndReached += OnTrailerEndReached;
            VideoPlayerTouch = new DelegateCommand<object>(OnVideoPlayerTouch);
            TogglePlay = new DelegateCommand<object>(OnTogglePlay);
            NavigateBack = new DelegateCommand<object>(OnNavigateBack);
            ButtonContent = new PackIcon { Kind = PackIconKind.Play, Height = 40, Width = 40 };
        }

        private void SetupInactiveTimer()
        {
            _playButtonInactiveTimer.Interval = TimeSpan.FromMilliseconds(_playButtonInactiveTimeout);
            _playButtonInactiveTimer.Tick += (sender, e) => OnPlayButtonTimeout();

            _playButtonInactiveTimer.IsEnabled = false;
        }

        private void OnNavigateToWizard(ScheduleDto scheduleDto)
        {
            if (scheduleDto != null)
            {
                RequestNewPage(PageViewType.Wizard, new MovieDetailToWizard(scheduleDto, _schedules));
            }
        }

        private void OnNavigateBack(object obj)
        {
            RequestNewPage(PageViewType.MovieList);
        }

        private void OnVideoPlayerTouch(object arg)
        {
            if (!MediaPlayer.IsPlaying)
            {
                return;
            }

            PlayButtonVisible = true;
            _playButtonInactiveTimer.IsEnabled = false;
            _playButtonInactiveTimer.IsEnabled = true;
        }

        private void OnPlayButtonTimeout()
        {
            PlayButtonVisible = false;
            _playButtonInactiveTimer.IsEnabled = false;
        }

        private void OnTogglePlay(object arg)
        {
            if (MediaPlayer.IsPlaying)
            {
                MediaPlayer.Pause();
                ButtonContent.Kind = PackIconKind.Play;
                _videoPaused = true;
            }
            else
            {
                if (_videoPaused)
                {
                    MediaPlayer.Play();
                }
                else
                {
                    MediaPlayer.Play(new Media(_libVlc, new Uri(StreamUrl)));
                }
                ButtonContent.Kind = PackIconKind.Pause;
                PlayButtonVisible = false;
            }
            _playButtonInactiveTimer.IsEnabled = false;
        }

        public override async Task InitializeAsync(object argument)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(nameof(argument));
            }

            _schedules = (List<ScheduleDto>)argument;
            var schedule = _schedules.First();
            _movieDto = await _serviceFactory.CreateMovieService().GetActiveMovieByIdAsync(schedule.MovieId);
            _actors = await _serviceFactory.CreateMovieService().GetActorsByMovieIdAsync(schedule.MovieId);
            _streamUrl = await DownloadHelper.FetchStreamUrl(TrailerUrl, KeyPattern);
        }

        public override void InitializeDone()
        {
            Schedules.Clear();
            Schedules.AddRange(_schedules);
            PlayButtonVisible = false;
            PlayButtonVisible = StreamUrl != null;
            TriggerPropertyChanged(this, nameof(StreamUrl));
            _videoPaused = false;
        }

        private void OnTrailerEndReached(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke((Action)(() =>
           {
               ButtonContent.Kind = PackIconKind.Play;
               PlayButtonVisible = true;
               _playButtonInactiveTimer.IsEnabled = false;
           }));
        }

        public override void ResetDone()
        {
            MediaPlayer.Stop();
            ButtonContent.Kind = PackIconKind.Play;
            PlayButtonVisible = false;
            _playButtonInactiveTimer.IsEnabled = false;
            SelectedSchedule = null;
        }
        #endregion

    }
}

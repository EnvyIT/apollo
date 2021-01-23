using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using Apollo.Core.Dto;
using Apollo.Core.Interfaces;
using Apollo.Terminal.Commands;
using Apollo.Terminal.Types;
using Apollo.Terminal.ViewModels.Base;
using Apollo.Util;
using static System.Int32;

namespace Apollo.Terminal.ViewModels
{
    public class MovieListViewModel : BasePageViewModel
    {
        private const string GenrePrefix = "Genre_";
        private const string AllGenreKey = GenrePrefix + "All";
        private const string ReservationBufferMinutesKey = "ReservationBufferMinutes";
        private const string BookableDaysKey = "Bookable_Days";

        private readonly int _selectableDaysConfiguration;
        private readonly IScheduleService _scheduleService;
        private readonly IMovieService _movieService;

        private readonly ICollectionView _moviesView;

        private IList<ScheduleDto> _scheduleData;
        private IDictionary<long, IEnumerable<ScheduleDto>> _movieSchedules;
        private IList<string> _genreKeyData;

        private bool _isLoading;

        private ISet<DateTime> _selectableDays;
        private IList<string> _genreKeys;
        private ObservableCollection<MovieDto> _movies;

        private DateTime _selectedDay;
        private string _selectedGenreKey;
        private string _searchText;

        public ICommand NextPageCommand { get; }
        public int ReservationBufferMinutes { get; }

        public bool IsLoading
        {
            get => _isLoading;
            private set => Set(ref _isLoading, value);
        }

        public bool ContainsMovies => !_moviesView.IsEmpty;

        public IList<string> GenreKeys
        {
            get => _genreKeys;
            private set => Set(ref _genreKeys, value);
        }

        public ISet<DateTime> SelectableDays
        {
            get => _selectableDays;
            private set => Set(ref _selectableDays, value);
        }

        public DateTime SelectedDay
        {
            get => _selectedDay;
            set
            {
                Set(ref _selectedDay, value);
                RefreshData();
            }
        }

        public string SelectedGenreKey
        {
            get => _selectedGenreKey;
            set
            {
                if (value == _selectedGenreKey)
                {
                    return;
                }

                Set(ref _selectedGenreKey, value);
                RefreshMovieView();
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (value == _searchText)
                {
                    return;
                }

                Set(ref _searchText, value);
                RefreshMovieView();
            }
        }

        public ObservableCollection<MovieDto> Movies
        {
            get => _movies;
            private set => Set(ref _movies, value);
        }

        public MovieListViewModel(IServiceFactory serviceFactory)
        {
            ReservationBufferMinutes = Parse(ConfigurationHelper.GetValues(ReservationBufferMinutesKey)[0]);
            _selectableDaysConfiguration = Parse(ConfigurationHelper.GetValues(BookableDaysKey)[0]);

            _scheduleService = serviceFactory.CreateScheduleService();
            _movieService = serviceFactory.CreateMovieService();

            _movies = new ObservableCollection<MovieDto>();
            _moviesView = CollectionViewSource.GetDefaultView(_movies);
            _moviesView.Filter = MoviesViewFilter;

            _selectableDays = GetSelectableDays();
            _selectedDay = SelectableDays.FirstOrDefault();

            NextPageCommand = new DelegateCommand<long>(id =>
                RequestNewPage(PageViewType.MovieDetail, _movieSchedules[id].ToList()));
        }


        public override void ResetDone()
        {
            Movies.Clear();
        }

        public override async Task InitializeAsync(object argument)
        {
            _genreKeyData = (await _movieService.GetActiveGenresAsync()).Select(genre => $"{GenrePrefix}{genre.Name}")
                .ToList();
            _genreKeyData.Insert(0, AllGenreKey);

            await LoadFilteredData();
        }

        public override void InitializeDone()
        {
            DataLoaded();
        }

        private async void RefreshData()
        {
            IsLoading = true;
            Movies.Clear();

            var taskHelper = new TaskHelper();
            taskHelper.OnSuccess += DataLoaded;
            taskHelper.OnFail += e => ShowSnackBar("Movies_Failed_Loading", e);
            taskHelper.OnComplete += () => IsLoading = false;

            await taskHelper.Run(LoadFilteredData);
        }

        private void RefreshMovieView()
        {
            _moviesView.Refresh();
            TriggerPropertyChanged(this, nameof(ContainsMovies));
        }

        private async Task LoadFilteredData()
        {
            var from = new DateTime(_selectedDay.Year, _selectedDay.Month, _selectedDay.Day, 00, 00, 00);
            if (_selectedDay.IsSameDay(DateTime.Now))
            {
                from = DateTime.Now.AddMinutes(ReservationBufferMinutes);
            }

            var to = new DateTime(_selectedDay.Year, _selectedDay.Month, _selectedDay.Day, 23, 59, 59);

            _scheduleData = (await _scheduleService.GetInTimeRangeAsync(from, to)).ToList();
            foreach (var schedule in _scheduleData)
            {
                schedule.Movie.Image = await _movieService.FetchImageByMovieId(schedule.MovieId);
            }
            _movieSchedules = _scheduleData.GroupBy(schedule => schedule.MovieId)
                .ToDictionary(
                    entry => entry.Key,
                    entry => entry.Select(schedule => schedule)
                );
        }

        private void DataLoaded()
        {
            GenreKeys = _genreKeyData;
            SelectedGenreKey = GenreKeys.FirstOrDefault();

            Movies.Clear();
            if (SelectedGenreKey != null && _movieSchedules != null)
            {
                _movieSchedules.Select(ms => ms.Value.First())
                    .Select(s => s.Movie).ToList()
                    .ForEach(m => Movies.Add(m));
            }

            TriggerPropertyChanged(this, nameof(ContainsMovies));

            IsLoading = false;
        }

        private bool MoviesViewFilter(object data)
        {
            if (!(data is MovieDto movie))
            {
                return true;
            }

            return (_searchText == null || movie.Title.ToLower().Contains(_searchText.ToLower())) &&
                   (_selectedGenreKey == AllGenreKey ||
                    movie.Genre.Name == SelectedGenreKey.Remove(0, GenrePrefix.Length));
        }

        private ISet<DateTime> GetSelectableDays()
        {
            var result = new HashSet<DateTime>();

            if (_selectableDaysConfiguration <= 0)
            {
                return result;
            }

            var today = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 0, 0, 0);
            for (var i = 0; i < _selectableDaysConfiguration; i++)
            {
                result.Add(today.AddDays(i));
            }

            return result;
        }
    }
}
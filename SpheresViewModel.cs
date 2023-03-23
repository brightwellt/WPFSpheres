using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows.Input;

namespace WpfSpheres
{
    /// <summary>ViewModel logic for spheres project - houses top level commands, engine for maintaining spheres</summary>
    /// <remarks>Ordinarily I'd add an interface here and have a design viewmodel if appropriate. It also allows for unit testing of the VM.</remarks>
    public class SpheresViewModel : ViewModelBase, IDisposable
    {
        #region Private Member Variables

        private readonly SpheresModel _model;

        private bool _isPlaying;

        #endregion

        #region Public Properties

        /// <summary>Our collection of spheres</summary>
        public ObservableCollection<SphereViewModel> Spheres { get; set; } = new ObservableCollection<SphereViewModel>();

        public bool IsPlaying
        {
            get { return _isPlaying; }
            set
            {
                _isPlaying = value;
                RaisePropertyChanged(nameof(IsPlaying));
            }
        }

        public double NewXValue { get; set; } = 50;
        public double NewYValue { get; set; } = 80;
        public uint NewSizeValue { get; set; } = 25;

        #endregion

        #region Construction and Destruction

        /// <summary>Big fan of constructor injection - I can mock the incoming arguments and unit test the class easily.
        /// This model does practically nothing - just houses a single setting - but is there to complete a bit of MVVM demoing.</summary>
        public SpheresViewModel(SpheresModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model), "Null Model passed to SpheresViewModel");
            _model = model;
        }

        private bool _disposed;

        public void Dispose()
        {
            if (_disposed) return;
            Stop(null);

            _disposed = true;
        }
        #endregion

        #region Commands

        public ICommand AddCommand => new RelayCommand(Add);

        private void Add(object parameter)
        {
           AddSphere(NewXValue, NewYValue, NewSizeValue);
            // N.B. The sort of unit test you could add here would be verifying that the above arguments go in correctly e.g. X goes to X.
        }

        public void AddSphere(double x, double y, uint size, double dx = 2, double dy = 1.5d)
        {
            var sphere = new SphereViewModel(x, y, size, dx, dy);

            if (_model.RandomiseSpeeds)
            {
                Random rnd = new Random(DateTime.Now.Millisecond);
                sphere.dX += rnd.Next(-10, 10) / 5d;
                sphere.dY += rnd.Next(-10, 10) / 5d;
            }

            Spheres.Add(sphere);

            NewSizeValue++; // The demo is all about spheres eating each other, so I want to encourage different size spheres.
            RaisePropertyChanged(nameof(NewSizeValue));
        }

        /// <summary>Note you could have a single command e.g. TogglePlayStateCommand here</summary>
        public ICommand PlayCommand => new RelayCommand(Play, CanPlay);
        public ICommand StopCommand => new RelayCommand(Stop, CanStop);

        private void Play(object parameter)
        {
            if (!IsPlaying)
            {
                IsPlaying = true;
                // Start timer, background task etc.
                _timer = new Timer(Timer_Elapsed, null, 10, 10);
            }
        }

        private bool CanPlay(object parameter) => !IsPlaying;

        private void Stop(object parameter)
        {
            if (IsPlaying)
            {
                IsPlaying = false;
                // Stop timer, background task etc.
                _timer?.Dispose();
            }
        }

        private bool CanStop(object parameter) => IsPlaying;

        #endregion

        #region Timing
        ///<summary>Done with a very simple timer. Could use timers, threads, tasks, but went for quick demo</summary>
        private Timer _timer; 

        public event EventHandler PlayTimerElapsed;

        private void Timer_Elapsed(object state)
        {
            PlayTimerElapsed?.Invoke(this, null);
        }

        #endregion
    }
}
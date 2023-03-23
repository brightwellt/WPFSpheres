namespace WpfSpheres
{
    /// <summary>Represents a single sphere</summary>
    public class SphereViewModel : ViewModelBase
    {
        private uint _size;
        private double _x;
        private double _y;
        private double _dX;
        private double _dY;
        private uint _spheresEaten;

        public uint Size
        {
            get { return _size; }
            set
            {
                _size = value; 
                RaisePropertyChanged(nameof(Size));
            }
        }


        public double X
        {
            get { return _x; }
            set
            {
                _x = value; 
                RaisePropertyChanged(nameof(X));
            }
        }

        public double Y
        {
            get { return _y; }
            set
            {
                _y = value; 
                RaisePropertyChanged(nameof(Y));
            }
        }

        public double dX
        {
            get { return _dX; }
            set
            {
                _dX = value; 
                RaisePropertyChanged(nameof(dX));
            }
        }

        public double dY
        {
            get { return _dY; }
            set
            {
                _dY = value; 
                RaisePropertyChanged(nameof(dY));
            }
        }

        /// <summary>Used to flag we should delete this sphere next update</summary>
        public bool ToBeEaten { get; set; }

        public uint SpheresEaten
        {
            get { return _spheresEaten; }
            set
            {
                _spheresEaten = value; 
                RaisePropertyChanged(nameof(SpheresEaten));
            }
        }

        public SphereViewModel(double x, double y, uint size, double dx, double dy)
        {
            X = x;
            Y = y;
            Size = size;
            dX = dx;
            dY = dy;
        }

    }
}
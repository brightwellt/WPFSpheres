using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WpfSpheres
{
    /// <summary>
    /// Interaction logic for SpheresCanvas.xaml
    /// </summary>
    public partial class SpheresCanvas
    {
        public SpheresCanvas()
        {
            InitializeComponent();
        }
        
        private SpheresViewModel SpheresVM => DataContext as SpheresViewModel;
        private ObservableCollection<SphereViewModel> Spheres => SpheresVM.Spheres;

        /// <summary>Very lazy way to link my drawing code to my viewmodel. Not normal approach but you did say 1-2 hours.</summary>
        private void SpheresCanvas_OnLoaded(object sender, RoutedEventArgs e)
        {
            Spheres.CollectionChanged += Spheres_CollectionChanged;
            SpheresVM.PlayTimerElapsed += SpheresVM_PlayTimerElapsed; ;
        }

        private void Spheres_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateSpheres(false); // Don't try and eat spheres - we're already changing a collection
        }

        private void SpheresVM_PlayTimerElapsed(object sender, System.EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                UpdateSpheres(true); // Eat the spheres - we're allowed to change the collection
            });
        }

        /// <summary>Code behind is nice for mouse work - draw a new sphere where the cursor is</summary>
        private void SphereCanvas_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
        
            if (Spheres == null) return;
            var pos = e.GetPosition(this);
            SpheresVM.AddSphere(pos.X, pos.Y, SpheresVM.NewSizeValue);
        }

        private void UpdateSpheres(bool eatSpheres)
        {
            // Very inefficient - clearing all the children and recreating them.
            // With more time I'd potentially skip canvas entirely and use a Visual / DrawingContext combo.
            // I've used such an approach when needing to draw 1000s of vessels off Singapore Harbour.
            // But this shows some logic all in one place.

            // Game here is that the larger spheres are going to eat the smaller ones if we are playing and grow by their size.
            bool isPlaying = SpheresVM.IsPlaying;

            Children.Clear();

            if (eatSpheres) EatSpheres();

            // Red and blue are a reference to Maxwell's Demon https://en.wikipedia.org/wiki/Maxwell%27s_demon
            var brush = isPlaying ? Brushes.Red : Brushes.Blue;
            foreach (var sphere in Spheres.OrderBy(x => x.Size))
            {
                double radius = sphere.Size / 2d;

                if (isPlaying) // Update the positions
                {
                    sphere.X += sphere.dX;
                    sphere.Y += sphere.dY;
                    if (sphere.X < radius || sphere.X > ActualWidth - radius) sphere.dX = -sphere.dX;
                    if (sphere.Y < radius || sphere.Y > ActualHeight - radius) sphere.dY = -sphere.dY;
                }

                var ellipse = new Ellipse {Width = sphere.Size, Height = sphere.Size, Stroke = brush};
                SetLeft(ellipse, sphere.X - radius);
                SetTop(ellipse, sphere.Y - radius);
                Children.Add(ellipse);
            }
        }

        /// <summary>Big circles eat little circles they encounter</summary>
        private void EatSpheres()
        {
            var query = Spheres.OrderBy(x => x.Size);
            // Faster ways to iterate yes, but just for demo purposes here
            foreach (var sphere1 in query)
            {
                foreach (var sphere2 in query)
                {
                    if (sphere1 == sphere2 || sphere1.Size <= sphere2.Size) continue;
                    if (Circle1IntersectsCircle2(sphere1.X, sphere1.Y, sphere2.X, sphere2.Y, sphere1.Size, sphere2.Size))
                    {
                        // Grow by half the size of the eaten sphere...could slow down, or properly add areas, or do other collision logic here
                        sphere1.Size += sphere2.Size / 2;
                        
                        sphere1.SpheresEaten++;

                        sphere2.ToBeEaten = true;
                    }
                }
            }

            // Clear the eaten spheres
            var eaten = Spheres.ToList().Where(x => x.ToBeEaten);
            foreach (var sphere in eaten)
            {
                Spheres.Remove(sphere);
            }
        }

        private bool Circle1IntersectsCircle2(double x1, double y1, double x2, double y2, double r1, double r2)
        {
            // Stolen from https://www.geeksforgeeks.org/check-two-given-circles-touch-intersect/
            // Pretty sure its not right but consider it placeholder physics
            double d = Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
           
            return d <= r1 + r2;
        }
    }
}

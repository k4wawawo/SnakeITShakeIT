using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;



namespace SnakeGame
{
    /// <summary>
    /// Logika interakcji  MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Opis czerwonych kawałków na planszy
        private readonly List<Point> _bonusPoints = new List<Point>();

        // Opis Snake-a na planszy
        private readonly List<Point> _snakePoints = new List<Point>();

        private readonly Brush _snakeColor = Brushes.Purple;
        private enum SnakeSize
        {
            Small = 4,
            Normal = 6,
            Thick = 8
        };
        private enum KierunekRuchu
        {
            Upwards = 8,
            Downwards = 2,
            Toleft = 4,
            Toright = 6
        };

        // Wartości TimeSpan
        private enum SzybkośćGry
        {
            Fast = 1,
            Moderate = 10000,
            Slow = 9999,
            DamnSlow = 500000,
        };

        private readonly Point _startingPoint = new Point(100, 100);
        private Point _currentPosition = new Point();

        // Inicjalizacja kierunku
        private int _direction = 0;

        /* Snake unika własnego ciała  */
        private int _previousDirection = 0;

        /* Zmiana rozmiaru snake-a 
         Dostępne możliwości SMALL, NORMAL, THICK*/
        private readonly int _headSize = (int)SnakeSize.Thick;

        private int _length = 10;
        private int _score = 0;
        private int score;
        private readonly Random _rnd = new Random();


#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public MainWindow()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        {
            InitializeComponent();
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += new EventHandler(timer_Tick);

            /* Szybkość gry. 
             * Dostępnt możliwości FAST, MODERATE, SLOW, DAMNSLOW */
            timer.Interval = new TimeSpan((int)SzybkośćGry.Moderate);
            timer.Start();

            this.KeyDown += new KeyEventHandler(OnButtonKeyDown);
            PainSnake(_startingPoint);
            _currentPosition = _startingPoint;

            // Tworzenie pokarmu dla węża 
            for (var n = 0; n < 10; n++)
            {
                DrukujBonus(n);
            }
        }
        private void PainSnake(Point currentposition)
        {

            // Tworzenie obramowania Snake-a.

            Ellipse newEllipse = new Ellipse
            {
                Fill = _snakeColor,
                Width = _headSize,
                Height = _headSize
            };

            Canvas.SetTop(newEllipse, currentposition.Y);
            Canvas.SetLeft(newEllipse, currentposition.X);

            int count = FieldPaint.Children.Count;
            FieldPaint.Children.Add(newEllipse);
            _snakePoints.Add(currentposition);

            // Ograniczenie ogona.
            if (count > _length)
            {
                FieldPaint.Children.RemoveAt(count - _length + 9);
                _snakePoints.RemoveAt(count - _length);
            }
        }

        private void DrukujBonus(int index)
        {
            Point bonusPoint = new Point(_rnd.Next(10, 780), _rnd.Next(10, 480));

            Ellipse newEllipse = new Ellipse
            {
                Fill = Brushes.Yellow,
                Width = _headSize,
                Height = _headSize

            };

            Canvas.SetTop(newEllipse, bonusPoint.Y);
            Canvas.SetLeft(newEllipse, bonusPoint.X);
            FieldPaint.Children.Insert(index, newEllipse);
            _bonusPoints.Insert(index, bonusPoint);
            
        }
        
        private void timer_Tick(object sender, EventArgs e)
        {
            // Ogon kieruje się za głową węża

            switch (_direction)
            {
                case (int)KierunekRuchu.Downwards:
                    _currentPosition.Y += 1;
                    PainSnake(_currentPosition);
                    break;
                case (int)KierunekRuchu.Upwards:
                    _currentPosition.Y -= 1;
                    PainSnake(_currentPosition);
                    break;
                case (int)KierunekRuchu.Toleft:
                    _currentPosition.X -= 1;
                    PainSnake(_currentPosition);
                    break;
                case (int)KierunekRuchu.Toright:
                    _currentPosition.X += 1;
                    PainSnake(_currentPosition);
                    break;
            }

            // Tworzenie scian.
            if ((_currentPosition.X < 5) || (_currentPosition.X > 780) ||
                (_currentPosition.Y < 5) || (_currentPosition.Y > 480))
                KoniecGry();

         // Gdy Snake zjada jedzenie wydłuża się.
            int n = 0;
            foreach (Point point in _bonusPoints)
            {

                if ((Math.Abs(point.X - _currentPosition.X) < _headSize) &&
                    (Math.Abs(point.Y - _currentPosition.Y) < _headSize))
                {
                    _length += 10;
                    _score += 1;

                    // Gdy snake zjada pokarm- znika on z pola + aktualny wynik.
                    _bonusPoints.RemoveAt(n);
                    FieldPaint.Children.RemoveAt(n);
                    DrukujBonus(n);
                    score++;
                    txtbScore.Text = score.ToString();

                    break;
                }
                n++;
            }

            // Ograniczenie trafienia w ogon.
            for (int q = 0; q < (_snakePoints.Count - _headSize * 2); q++)
            {
                Point point = new Point(_snakePoints[q].X, _snakePoints[q].Y);
                if ((Math.Abs(point.X - _currentPosition.X) < (_headSize)) &&
                     (Math.Abs(point.Y - _currentPosition.Y) < (_headSize)))
                {
                    KoniecGry();
                    break;
                }

            }

        }

        private void OnButtonKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                    case Key.Down:
                    if (_previousDirection != (int)KierunekRuchu.Upwards)
                        _direction = (int)KierunekRuchu.Downwards;
                    break;
                case Key.Up:
                    if (_previousDirection != (int)KierunekRuchu.Downwards)
                            _direction = (int)KierunekRuchu.Upwards;
                        break;
                case Key.Left:
                      if (_previousDirection != (int)KierunekRuchu.Toright)
                        _direction = (int)KierunekRuchu.Toleft;
                    break;
                case Key.Right:
                  if (_previousDirection != (int)KierunekRuchu.Toleft)
                        _direction = (int)KierunekRuchu.Toright;
                    break;

            }
            _previousDirection = _direction;

        }
        private void KoniecGry()
        {
            MessageBox.Show($@"Przegrałeś! Twój wynik to: { _score}", "Koniec:(", MessageBoxButton.OK, MessageBoxImage.Warning);
            Restart();
        }
        private void Restart()
        {
            Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }


    }
}

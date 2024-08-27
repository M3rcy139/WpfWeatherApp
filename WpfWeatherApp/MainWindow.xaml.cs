using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Windows;
using System.Windows.Input;
using static WpfWeatherApp.WeatherInfo;
using WpfWeatherApp.Infrastructure;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace WpfWeatherApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void OnGetWeatherClick(object sender, RoutedEventArgs e)
        {
            string city = CityTextBox.Text;
            if (!string.IsNullOrEmpty(city))
            {
                await GetWeatherData(city);
            }
            else
            {
                MessageBox.Show("Please enter a city name.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public async Task GetWeatherData(string city)
        {
            WeatherGetter wGetter = new WeatherGetter(city);

            await wGetter.GetWeatherAsync();
            ImageSourceSet(wGetter);
            DisplayWeather(wGetter);
            UpdateSunPosition(null, null, wGetter);
        }
        private void DisplayWeather(WeatherGetter wG)
        {
            //WeatherMainTextBlock.Text = wG.Main;
            WeatherDescriptionTextBlock.Text = wG.Description;
            TempTextBlock.Text = $"{wG.Temp} °C";
            FeelsLikeTextBlock.Text = $"{wG.FeelsLike} °C";
            PressureTextBlock.Text = $"{wG.Pressure} hPa";
            HumidityTextBlock.Text = $"{wG.Humidity} %";
            WindSpeedTextBlock.Text = $"{wG.WindSpeed} м/с";
            WindDirectionTextBlock.Text = $"{wG.WindDirection}";

            SunriseTextBlock.Text = wG.Sunrise.ToShortTimeString();
            SunsetTextBlock.Text = wG.Sunset.ToShortTimeString();

            CurrentTime.Text = wG.CurrentTime.ToShortTimeString();
        }

        private void ImageSourceSet(WeatherGetter wg)
        {
            BackgroundImage bgImage = new BackgroundImage();

            BitmapImage bitmap = new BitmapImage();
            string imagePath = bgImage.BackgroundImageSet(wg.Main, wg.Image);
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(imagePath, UriKind.RelativeOrAbsolute);
            bitmap.EndInit();
            Image.Source = bitmap;
        }

        private void UpdateSunPosition(object sender, EventArgs e, WeatherGetter wG)
        {
            DateTime sunrise = wG.Sunrise;
            DateTime sunset = wG.Sunset;
            DateTime currentTime = wG.CurrentTime;

            if (currentTime < sunrise || currentTime > sunset)
            {
                Sun.Opacity = 0.3;
                Canvas.SetLeft(Sun, 1 * (SunCanvas.ActualWidth - Sun.Width));
                Image.Opacity = 0.3;
                DoubleAnimation opacityAnimation = new DoubleAnimation(0.0, 1.0, TimeSpan.FromSeconds(2));
                Night.BeginAnimation(OpacityProperty, opacityAnimation);
                return;
            }

            Image.Opacity = 0.7;
            DoubleAnimation reverseOpacityAnimation = new DoubleAnimation(Night.Opacity, 0.0, TimeSpan.FromSeconds(2));
            Night.BeginAnimation(OpacityProperty, reverseOpacityAnimation);
            Sun.Opacity = 1;

            // Вычисляем общее время дня и текущее время в этом интервале
            double totalTime = (sunset - sunrise).TotalMinutes;
            double elapsedTime = (currentTime - sunrise).TotalMinutes;

            double percentageOfDay = elapsedTime / totalTime;

            double canvasWidth = SunCanvas.ActualWidth;
            double canvasHeight = SunCanvas.ActualHeight;

            double sunX = (percentageOfDay * (canvasWidth - Sun.Width)) / 1;
            double sunY = (canvasHeight * Math.Sin(percentageOfDay * Math.PI)) / 1; 

            Canvas.SetLeft(Sun, sunX);
            //Canvas.SetTop(Sun, canvasHeight - sunY - Sun.Height); // Положение сверху вниз
        }

        private void OnMinimizeClick(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void OnCloseClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void DockPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
    }
}

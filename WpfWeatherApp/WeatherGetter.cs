using Newtonsoft.Json;
using System.Net.Http;

namespace WpfWeatherApp
{
    public class WeatherGetter
    {
        string APIKey = "ef0333d33c01144073c62d570a00ef7d";
        string City = "";

        public string Main { get; set; }
        public string Description { get; set; }
        public DateTime Sunset { get; set; }
        public DateTime Sunrise { get; set; }
        public string Temp { get; set; }
        public string Pressure { get; set; }
        public string Humidity { get; set; }
        public string WindSpeed { get; set; }
        public string WindDirection { get; set; }
        public string FeelsLike { get; set; }
        public string Image {  get; set; }
        public DateTime CurrentTime { get; set; }
        public WeatherGetter(string city)
        {
            City = city;
        }

        public async Task GetWeatherAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                string url = string.Format("https://api.openweathermap.org/data/2.5/weather?q={0}&appid={1}&units=metric&lang=ru", City, APIKey);
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    WeatherInfo.root info = JsonConvert.DeserializeObject<WeatherInfo.root>(json);
                    Image = info.weather[0].icon + ".png";

                    Main = info.weather[0].main;
                    Description = ConvertDescription(info.weather[0].description);
                    Sunset = ConvertDateTime(info.sys.sunset, info.timezone);
                    Sunrise = ConvertDateTime(info.sys.sunrise, info.timezone);

                    Temp = info.main.temp.ToString();
                    FeelsLike = info.main.feels_like.ToString();
                    Pressure = info.main.pressure.ToString();
                    Humidity = info.main.humidity.ToString();
                    WindSpeed = info.wind.speed.ToString();
                    WindDirection = ConvertWindDirection(info.wind.deg);

                    CurrentTime = GetCurrentTimeByTimezoneOffset(info.timezone);
                }
                else throw new Exception($"Ошибка при получении данных: {response.ReasonPhrase}");
            }
        }

        DateTime ConvertDateTime(long sec, int timezone)
        {
            DateTime day = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc).ToUniversalTime();
            day = day.AddSeconds(sec).ToUniversalTime();
            day = day.AddSeconds(timezone);

            return day;
        }

        string ConvertTemp(double temp)
        {
            string result = Math.Round(temp - 273.15, 1).ToString();
            return result;
        }

        string ConvertDescription(string description)
        {
            return char.ToUpper(description[0]) + description.Substring(1);
        }

        string ConvertWindDirection(double deg)
        {
            if (deg >= 337.5 || deg < 22.5)
                return "С";
            else if (deg >= 22.5 && deg < 67.5)
                return "СВ";
            else if (deg >= 67.5 && deg < 112.5)
                return "В";
            else if (deg >= 112.5 && deg < 157.5)
                return "ЮВ";
            else if (deg >= 157.5 && deg < 202.5)
                return "Ю";
            else if (deg >= 202.5 && deg < 247.5)
                return "ЮЗ";
            else if (deg >= 247.5 && deg < 292.5)
                return "З";
            else if (deg >= 292.5 && deg < 337.5)
                return "СЗ";
            else return "";
        }

        DateTime GetCurrentTimeByTimezoneOffset(int timezoneOffsetInSeconds)
        {
            DateTime utcNow = DateTime.UtcNow;

            DateTime localTime = utcNow.AddSeconds(timezoneOffsetInSeconds);

            return localTime;
        }
    }
}

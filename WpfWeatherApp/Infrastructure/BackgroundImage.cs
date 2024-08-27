using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfWeatherApp.Infrastructure
{
    class BackgroundImage
    {
        public string BackgroundImageSet(string main, string image)
        {
            main = main.ToLower();

            if (main == "snow") return "Images/snow.jpg";
            if (main == "thunderstorm") return "Images/thunderstorm.jpg";
            if (main == "drizzle") return "Images/mist.jpg";
            if (main == "rain") return "Images/rain.jpg";
            if (main == "clouds")
            {
                if (image == "04d.png" || image == "04n.png" || image == "03d.png" || image == "03n.png")
                    return "Images/bclouds.jpg";
                else return "Images/clouds.jpg";
            }
            else return "Images/sunny.jpg";
        }
    }
}

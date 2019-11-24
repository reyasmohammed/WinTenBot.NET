using System.Threading.Tasks;

namespace WinTenBot.Services
{
    interface IWeatherService
    {
        Task<CurrentWeather> GetWeatherAsync(float lat, float lon);
    }
}
using System.Threading.Tasks;
using WinTenBot.Model;

namespace WinTenBot.Interfaces
{
    interface IWeatherService
    {
        Task<CurrentWeather> GetWeatherAsync(float lat, float lon);
    }
}
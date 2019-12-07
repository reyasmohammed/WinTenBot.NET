using System.Data;
using System.Threading.Tasks;

namespace WinTenBot.Interfaces
{
    public interface ITagsService
    {
        Task<DataTable> GetTagsAsync();
    }
}
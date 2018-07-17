using System.Threading.Tasks;

namespace Gaois.QueryLogger.AspNetCore
{
    public interface IQueryLogger
    {
        void Log(params Query[] queries);
        Task<int> LogAsync(params Query[] queries);
    }
}
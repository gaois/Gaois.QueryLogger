namespace Gaois.QueryLogger.AspNetCore
{
    public interface IQueryLogger
    {
        void Log(params Query[] queries);
    }
}
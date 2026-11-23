using System.Collections.Generic;
using System.Threading.Tasks;

namespace HotelingFunctionApp
{
    public interface IDataAccess
    {
        Task<List<SeatsModel>> GetSeatsDataAccessAsync();
    }
}

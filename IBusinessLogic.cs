using System.Collections.Generic;
using System.Threading.Tasks;

namespace HotelingFunctionApp
{
    public interface IBusinessLogic
    {
        Task<List<SeatsModel>> GetSeatsBusinessLogicAsync();
    }
}


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace HotelingFunctionApp
{
    public class BusinessLogic : IBusinessLogic
    {
        private readonly IDataAccess _dataAccess;
        private readonly ILogger<BusinessLogic> _logger;

        public BusinessLogic(IDataAccess dataAccess, ILogger<BusinessLogic> logger)
        {
            _dataAccess = dataAccess ?? throw new ArgumentNullException(nameof(dataAccess));
            _logger = logger;
        }

        public async Task<List<SeatsModel>> GetSeatsBusinessLogicAsync()
        {
            try
            {
                var sw = Stopwatch.StartNew();
                var seats = await _dataAccess.GetSeatsDataAccessAsync();
                sw.Stop();
                _logger.LogInformation($"BusinessLogic retrieved {seats?.Count ?? 0} seats in {sw.ElapsedMilliseconds}ms");
                return seats;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Method}", nameof(GetSeatsBusinessLogicAsync));
                throw; // preserve original exception and stack
            }
        }
    }
}

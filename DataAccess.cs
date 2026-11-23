using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace HotelingFunctionApp
{
    public class DataAccess : IDataAccess
    {
        private readonly ILogger<DataAccess> _logger;

        public DataAccess(ILogger<DataAccess> logger)
        {
            _logger = logger;
        }

        public async Task<List<SeatsModel>> GetSeatsDataAccessAsync()
        {
            var seats = new List<SeatsModel>();

            // Read connection string from environment (local.settings.json -> Values)
            var connectionString = Environment.GetEnvironmentVariable("SqlConnectionString");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("Connection string 'SqlConnectionString' not found.");
            }

            // Simple retry policy with exponential backoff
            const int maxAttempts = 3;
            var delay = 200; // milliseconds

            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    var sw = Stopwatch.StartNew();
                    using (var conn = new SqlConnection(connectionString))
                    {
                        await conn.OpenAsync();

                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = "SELECT SeatId, SeatName FROM seats";
                            using (var reader = await cmd.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    var seat = new SeatsModel
                                    {
                                        SeatId = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                                        SeatName = reader.IsDBNull(1) ? string.Empty : reader.GetString(1)
                                    };
                                    seats.Add(seat);
                                }
                            }
                        }
                    }
                    sw.Stop();
                    _logger.LogInformation($"Retrieved {seats.Count} seats in {sw.ElapsedMilliseconds}ms (attempt {attempt})");
                    break; // success
                }
                catch (SqlException sqlEx) when (attempt < maxAttempts)
                {
                    // transient SQL exception - retry
                    _logger.LogWarning(sqlEx, "Transient SQL error on attempt {Attempt}. Retrying after {Delay}ms.", attempt, delay);
                    await Task.Delay(delay);
                    delay *= 2;
                    continue;
                }
                catch (TimeoutException timeoutEx) when (attempt < maxAttempts)
                {
                    _logger.LogWarning(timeoutEx, "Timeout on attempt {Attempt}. Retrying after {Delay}ms.", attempt, delay);
                    await Task.Delay(delay);
                    delay *= 2;
                    continue;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to retrieve seats from database.");
                    throw; // preserve stack
                }
            }

            return seats;
        }
    }
}

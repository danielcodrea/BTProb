using BT_DataAccess.Interfaces;
using BT_DataModels;
using Dapper;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Linq;

namespace BT_DataAccess.Repositories
{
    public class JobsRepository : IRepository
    {
        private readonly string _connectionString;

        public JobsRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Database");
        }
        public async Task<int> Create(Job entity)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                    int id = await connection.QuerySingleOrDefaultAsync<int>(
                                "[dbo].[InsertJob]",
                                new { entity.Name, entity.Type, entity.FileAttached, entity.File },
                                commandType: CommandType.StoredProcedure);
                    return id;
            }
        }

        public async Task<bool> Delete(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                 connection.ExecuteAsync(
                    "[dbo].[DeleteJob]",
                    new { id, isDeleted = true },
                    commandType: CommandType.StoredProcedure);
                return true;
            }
        }

        public async Task<List<Job>> GetAll()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var result = await connection.QueryAsync<Job>(
                    "[dbo].[GetAllJobs]",
                    commandType: CommandType.StoredProcedure);
                return result.ToList();
            }
        }

        public async Task<Job> GetById(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var result = await connection.QuerySingleOrDefaultAsync<Job>(
                    "[dbo].[SelectJobById]",
                    new { id },
                    commandType: CommandType.StoredProcedure);
                return result;
            }
        }

        public async Task<int> Update(Job entity)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var result = await connection.ExecuteAsync(
                    "[dbo].[UpdateJob]",
                    new {entity.ID, entity.Name, entity.Type, entity.FileAttached, entity.File, entity.Processed, entity.ProcessedDate },
                    commandType: CommandType.StoredProcedure);
                return 1;
            }
        }

    }
}

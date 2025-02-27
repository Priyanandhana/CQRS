using Domain.Entities;
using Infrastructure.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Persistence;
using Dapper;
using System.Text;
using Z.Dapper.Plus;

namespace Infrastructure.Data
{
    public class PurchaseOrderRepository : IPurchaseOrderRepository
    {
        private readonly string _connectionString;
        private readonly AppDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public PurchaseOrderRepository(IConfiguration configuration, AppDbContext dbContext)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Database connection string is not set.");
            _dbContext = dbContext;
        }

        public async Task<int> BulkInsertlinqAsync(List<PurchaseOrderHeader> purchaseOrders)
        {
            var stopwatch = Stopwatch.StartNew();
            var purchaseOrderList = new List<PurchaseOrderDetail>();

            foreach (var purchaseOrder in purchaseOrders)
            {
                purchaseOrderList.AddRange(purchaseOrder.PODetailinfo);
            }

            stopwatch.Stop();
            return purchaseOrderList.Count;
        }

        public async Task BulkInsertUsingTVPAsync(List<PurchaseOrderHeader> headers, List<PurchaseOrderDetail> details)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            using var cmd = new SqlCommand("dbo.BulkInsertPurchaseOrders", conn)
            {
                CommandType = CommandType.StoredProcedure,
                CommandTimeout = 60
            };

            cmd.Parameters.Add(new SqlParameter("@HeaderTVP", SqlDbType.Structured)
            {
                TypeName = "dbo.PurchaseOrderHeaderType",
                Value = TVPHelper.ToPurchaseOrderHeaderDataTable(headers)
            });

            cmd.Parameters.Add(new SqlParameter("@DetailTVP", SqlDbType.Structured)
            {
                TypeName = "dbo.PurchaseOrderDetailType",
                Value = TVPHelper.ToPurchaseOrderDetailDataTable(details)
            });

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task BulkInsertUsingSqlBulkCopy(List<PurchaseOrderHeader> headers, List<PurchaseOrderDetail> details)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                await BulkInsertDataAsync(connection, transaction, TVPHelper.ToPurchaseOrderHeaderDataTable(headers), "PurchaseOrderHeader");
                await BulkInsertDataAsync(connection, transaction, TVPHelper.ToPurchaseOrderDetailDataTable(details), "PurchaseOrderDetails");

                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception("Bulk insert failed", ex);
            }
        }

        private async Task BulkInsertDataAsync(SqlConnection connection, SqlTransaction transaction, DataTable table, string tableName)
        {
            using var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.KeepIdentity | SqlBulkCopyOptions.TableLock, transaction)
            {
                DestinationTableName = tableName,
                BatchSize = 25000,
                BulkCopyTimeout = 60,
                EnableStreaming = true
            };

            foreach (DataColumn col in table.Columns)
            {
                bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
            }

            await bulkCopy.WriteToServerAsync(table);
        }

        public async Task BulkInsertUsingDapperAsync(List<PurchaseOrderHeader> headers, List<PurchaseOrderDetail> details)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        Task insertHeadersTask = Task.CompletedTask;
                        Task insertDetailsTask = Task.CompletedTask;

                        // Bulk Insert into PurchaseOrderHeader using parameterized query
                        if (headers.Any())
                        {
                            string insertHeaderQuery = @"
                        INSERT INTO PurchaseOrderHeader 
                        (OUInstance, PurchaseOrder, NumberingType, PODate, POPriority, PartType, Remarks1, Supplier, POCurrency, AddressID, PoStatus, AmendNo, Role, [User], Language) 
                        VALUES 
                        (@OUInstance, @PurchaseOrder, @NumberingType, @PODate, @POPriority, @PartType, @Remarks1, @Supplier, @POCurrency, @AddressID, 'D', 0, 'role', 'user', 1)";

                            insertHeadersTask = connection.ExecuteAsync(insertHeaderQuery, headers, transaction);
                        }

                        // Bulk Insert into PurchaseOrderDetails using parameterized query
                        if (details.Any())
                        {
                            string insertDetailQuery = @"
                        INSERT INTO PurchaseOrderDetails 
                        (OUInstance, Part, OrderQty, PurchaseUOM, Cost, CostPer, Condition, CertificateType, Warehouse, Remarks2, Role, [User], Language) 
                        VALUES 
                        (@OUInstance, @Part, @OrderQty, @PurchaseUOM, @Cost, @CostPer, @Condition, @CertificateType, @Warehouse, @Remarks2, 'role', 'user', 1)";

                            insertDetailsTask = connection.ExecuteAsync(insertDetailQuery, details, transaction);
                        }

                        // Execute both bulk inserts simultaneously to reduce response time
                        await Task.WhenAll(insertHeadersTask, insertDetailsTask);

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public async Task<bool> BulkInsertUsingDapperPlusAsync(List<PurchaseOrderHeader> headers, List<PurchaseOrderDetail> details)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Configure Dapper Plus
                        DapperPlusManager.Entity<PurchaseOrderHeader>().Table("PurchaseOrderHeader");
                        DapperPlusManager.Entity<PurchaseOrderDetail>().Table("PurchaseOrderDetails");

                        // Perform bulk insert with transaction
                        connection.UseBulkOptions(options => options.Transaction = transaction) 
                                  .BulkInsert(headers)
                                  .ThenBulkInsert(headers => details);

                        transaction.Commit(); 
                        return true; 
                    }
                    catch
                    {
                        transaction.Rollback(); 
                        return false; 
                    }
                }
            }
        }


    }
}

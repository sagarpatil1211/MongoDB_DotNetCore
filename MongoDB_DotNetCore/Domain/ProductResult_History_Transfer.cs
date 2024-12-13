using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MongoDB_DotNetCore.data;
using MongoDB_DotNetCore.Entities.MongoDB;
using MongoDB_DotNetCore.Entities.MSSQL;
using System.Globalization;

namespace MongoDB_DotNetCore.Domain
{
    public class ProductResult_History_Transfer
    {
        private readonly MongoDbContext _mongoDbContext;

        private readonly SQLDatabaseContext _sqlDbContext;
        private readonly IMongoCollection<ProductResult_History> _ProductResult_History;
        public ProductResult_History_Transfer(MongoDbContext mongoDbContext, SQLDatabaseContext sqlDbContext)
        {
            _mongoDbContext = mongoDbContext;
            _ProductResult_History = mongoDbContext.GetCollection<ProductResult_History>("ProductResult_History");
            _sqlDbContext = sqlDbContext;
        }
        public async Task addProductResult_HistoryRecentRecords(DateTimeOffset input1)
        {
            // Get the current UTC date and time
            var currentUtcDate = DateTime.UtcNow;
            //var currentUtcDate = DateTime.ParseExact(
            //    "2019-09-04T10:53:55.500+00:00",
            //    "yyyy-MM-ddTHH:mm:ss.fffK",
            //    System.Globalization.CultureInfo.InvariantCulture,
            //    System.Globalization.DateTimeStyles.AdjustToUniversal
            //);

            // Convert the UTC time to Indian Standard Time (IST) - UTC + 5:30
            var indianTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            var currentIndianDate = TimeZoneInfo.ConvertTimeFromUtc(currentUtcDate, indianTimeZone);

            // For test start

            // ================== test 1

            //string input = "2019-11-09T06:22:20.500+00:00";
            //string input = "2019-09-04T10:53:28.000+00:00";
            // Parse the string into a DateTime object  
            //DateTime dateTimeUtc = DateTime.Parse(input).ToUniversalTime();
            //currentUtcDate = dateTimeUtc;
            //var currentIndianDate = dateTimeUtc;

            //================== test 2 by parameter

            ////string input = "04-09-2019 10:56:28 +00:00";
            //string input = input1.ToString();
            //// Parse the input string into DateTime using custom format
            //DateTime parsedDate = DateTime.ParseExact(input, "dd-MM-yyyy HH:mm:ss zzz", CultureInfo.InvariantCulture);

            //// Format the DateTime object into the desired format
            //string output = parsedDate.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz");


            //// Parse the string into a DateTime object  
            //DateTime dateTimeUtc = DateTime.Parse(output).ToUniversalTime();
            //currentUtcDate = dateTimeUtc;
            //var currentIndianDate = dateTimeUtc;

            // For test end


            // Calculate the date and time 5 minutes before the current time

            var fiveMinutesAgo = currentUtcDate.AddMinutes(-5);
            //var fiveMinutesAgo = currentIndianDate.AddMinutes(-5);


            // Format currentDate and fiveMinutesAgo in the exact required format (ISO 8601)
            //string strCurrentDate = currentDate.ToString("yyyy-MM-ddTHH:mm:ss.fff+00:00");
            //string strCurrentDate = currentIndianDate.ToString("yyyy-MM-ddTHH:mm:ss.fff+00:00");
            string strCurrentDate = currentUtcDate.ToString("yyyy-MM-ddTHH:mm:ss.fff+00:00");
            string strFiveMinutesAgo = fiveMinutesAgo.ToString("yyyy-MM-ddTHH:mm:ss.fff+00:00");

            // Create a filter to match records where enddate > fiveMinutesAgo && enddate <= currentDate
            var filter = Builders<ProductResult_History>.Filter.And(
                Builders<ProductResult_History>.Filter.Gte(x => x.enddate, Convert.ToDateTime(strFiveMinutesAgo)), // Greater than or equal to
                Builders<ProductResult_History>.Filter.Lte(x => x.enddate, Convert.ToDateTime(strCurrentDate))    // Less than or equal to
            );

            DateTime? latestCaptuteEndDate;
            FilterDefinition<ProductResult_History> filterByLatestdate = null;
            var latestCaptuteEndDateRecord = await _sqlDbContext.LatestCapturedEndDate.FirstOrDefaultAsync(e => e.collectionName == "ProductResult_History");
            if (latestCaptuteEndDateRecord != null)
            {
                latestCaptuteEndDate = latestCaptuteEndDateRecord.LatestCapturedEndDate;
                if (latestCaptuteEndDate != DateTime.MinValue)
                {
                    string strlatestCaptuteEndDate = latestCaptuteEndDate?.ToString("yyyy-MM-ddTHH:mm:ss.fff+00:00");
                     filterByLatestdate = Builders<ProductResult_History>.Filter.And(
                        Builders<ProductResult_History>.Filter.Gt(x => x.enddate, Convert.ToDateTime(strlatestCaptuteEndDate)) // Greater than   
                        );

                }
            }

            FilterDefinition<ProductResult_History> combinedFilter = null;


            if (filterByLatestdate is not null)
            {
                combinedFilter = Builders<ProductResult_History>.Filter.And(
                filterByLatestdate,
                filter 
                );
            }
            else
            {
                combinedFilter = Builders<ProductResult_History>.Filter.And(
                filter
                );
            }



            // Execute the query and return the matching records
            var records = await _ProductResult_History.Find(combinedFilter)
                .Sort(Builders<ProductResult_History>.Sort.Descending(x => x.enddate)) // Sort by enddate descending
                //.Limit(25)
                .ToListAsync();


            var a = new List<ProductResult_History>();

            // Assuming _sqlDbContext is your Entity Framework context for SQL Server
            using (var transaction = await _sqlDbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    if (records.Count > 0)
                    {
                        // Loop through the MongoDB records and insert them into SQL Server
                        foreach (var record in records)
                        {
                            var sqlRecord = new Sql_ProductResult_History
                            {
                                // Map the fields from MongoDB record to SQL Server record
                                ObjectId = record.Id,
                                L1Name = record.L1Name,
                                productname = record.productname,
                                updatedate = record.updatedate,
                                enddate = record.enddate,
                                timespan = record.timespan,
                                productresult = record.productresult,
                                productresult_accumulate = record.productresult_accumulate,
                                resultflag =Convert.ToInt16(record.resultflag) 


                            };

                            // Insert into SQL Server table
                            await _sqlDbContext.ProductResult_History.AddAsync(sqlRecord);
                        }

                        // Commit the transaction
                        await _sqlDbContext.SaveChangesAsync();
                        //await transaction.CommitAsync();
                    }
                }
                catch (Exception ex)
                {
                    // Rollback in case of error
                    await transaction.RollbackAsync();
                    throw new Exception("Error occurred while inserting records into SQL Server", ex);
                }

                try {
                    if(records.Count > 0) {
                    var existingRecord = await _sqlDbContext.LatestCapturedEndDate.FirstOrDefaultAsync(e => e.collectionName == "ProductResult_History");

                    if (existingRecord != null)
                    {
                        // If the record exists, update it
                        existingRecord.LatestCapturedEndDate = records[0].enddate?.ToUniversalTime(); // Update EndDate (or other fields as needed)
                        //existingRecord.LatestCaptuteEndDate = records[0].enddate; // Update EndDate (or other fields as needed)


                        _sqlDbContext.LatestCapturedEndDate.Update(existingRecord); // Mark it for update
                    }
                    else
                    {
                        // If the record doesn't exist, insert a new one
                        var latestDateRecord = new Sql_LatestCapturedEndDate
                        {
                            // Map the fields from MongoDB record to SQL Server record
                            collectionName = "ProductResult_History",
                            LatestCapturedEndDate = records[0].enddate,


                        };

                        await _sqlDbContext.LatestCapturedEndDate.AddAsync(latestDateRecord); // Insert new record
                    }

                    // Commit the transaction
                    await _sqlDbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                    }

                }
                catch (Exception ex)
                {
                    // Rollback in case of error
                    await transaction.RollbackAsync();
                    throw new Exception("Error occurred while inserting records into SQL Server", ex);
                }
            }


            //return a;
        }

    }
}

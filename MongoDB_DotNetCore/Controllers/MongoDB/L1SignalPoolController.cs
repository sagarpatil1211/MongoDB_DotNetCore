using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB_DotNetCore.data;
using MongoDB_DotNetCore.Entities;
using MongoDB_DotNetCore.Entities.MongoDB;
using MongoDB_DotNetCore.Entities.MSSQL;
using System.Collections;

namespace MongoDB_DotNetCore.Controllers.MongoDB
{
    [Route("api/[controller]")]
    [ApiController]
    public class L1SignalPoolController : ControllerBase
    {
        private readonly MongoDbContext _mongoDbContext;

        private readonly SQLDatabaseContext _sqlDbContext;
        private readonly IMongoCollection<L1Signal_Pool> _L1Signal_Pool;
        private readonly IMongoCollection<L1Signal_Pool_Active> _L1Signal_Pool_Active;
        public L1SignalPoolController(MongoDbContext mongoDbContext, SQLDatabaseContext sqlDbContext)
        {
            _mongoDbContext = mongoDbContext;
            _L1Signal_Pool = mongoDbContext.GetCollection<L1Signal_Pool>("L1Signal_Pool");
            _L1Signal_Pool_Active = mongoDbContext.GetCollection<L1Signal_Pool_Active>("L1Signal_Pool_Active");
            _sqlDbContext = sqlDbContext;
        }

        [HttpGet("AllL1Signal_Pool")]
        public async Task<IEnumerable<L1Signal_Pool>> GetL1Signal_Pool()
        {
            return await _L1Signal_Pool.Find(FilterDefinition<L1Signal_Pool>.Empty).ToListAsync();
        }

         [HttpGet("AllL1Signal_Pool_Active")]
        public async Task<IEnumerable<L1Signal_Pool_Active>> GetL1Signal_Pool_Active()
        {
            var result = await _L1Signal_Pool_Active.Find(FilterDefinition<L1Signal_Pool_Active>.Empty).ToListAsync();
            return result;

        }
        //[HttpGet("Filter")]
        //public async Task<List<L1Signal_Pool>> GetTop5RecordsAsync()
        //{
        //    // Use Find() to get records and limit the result to 5.
        //    var top5Records = await _L1Signal_Pool
        //        .Find(FilterDefinition<L1Signal_Pool>.Empty)  // No filter, get all records
        //        .Limit(5)  // Limit to 5 records
        //        .ToListAsync();

        //    return top5Records;
        //}

        [HttpGet("FilterByName")]
        public async Task<IEnumerable<L1Signal_Pool>> GetL1_Pool_OpenedByFilter(string Name)
        {
            // Create a filter to find records with the specified L1Name
            var filter = Builders<L1Signal_Pool>.Filter.Eq(x => x.L1Name, Name);
            // Use Find() with the filter to get records

            var projection = Builders<L1Signal_Pool>.Projection.Include(doc => doc.L1Name)
                                                       .Include(doc => doc.signalname)
                                                       .Include(doc => doc.updatedate)
                                                       .Include(doc =>doc.timespan)
                                                       .Include(doc => doc.enddate);
            var records = await _L1Signal_Pool
                .Find(filter)  // Filter to match L1Name
                .Limit(10)        
                .ToListAsync();  // Convert result to a list

            return records;
            //return await _L1_Pool_Opened.Find(FilterDefinition<L1_Pool_Opened>.Empty).ToListAsync();
        }

        //[HttpGet("Recent")]
        //public async Task<List<L1Signal_Pool>> GetRecentRecords()
        //{
        //    // Get the current UTC date and time
        //    var currentDate = DateTime.UtcNow;

        //    //// Convert the UTC time to Indian Standard Time (IST) - UTC + 5:30
        //    //var indianTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        //    //var currentIndianDate = TimeZoneInfo.ConvertTimeFromUtc(currentDate, indianTimeZone);

        //    // For test start

        //    string input = "2019-11-09T06:38:20.500+00:00";
        //    // Parse the string into a DateTime object  
        //    DateTime dateTimeUtc = DateTime.Parse(input).ToUniversalTime();
        //    currentDate = dateTimeUtc;
        //   var currentIndianDate = dateTimeUtc;

        //    // For test end


        //    // Calculate the date and time 5 minutes before the current time
        //    var fiveMinutesAgo = currentIndianDate.AddMinutes(-5);

        //    //// Convert the UTC time to Indian Standard Time (IST) - UTC + 5:30
        //    //var indianTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        //    //var currentIndianDate = TimeZoneInfo.ConvertTimeFromUtc(currentUtcDate, indianTimeZone);

        //    // Format currentDate and fiveMinutesAgo in the exact required format (ISO 8601)
        //    //string strCurrentDate = currentDate.ToString("yyyy-MM-ddTHH:mm:ss.fff+00:00");
        //    string strCurrentDate = currentIndianDate.ToString("yyyy-MM-ddTHH:mm:ss.fff+00:00");
        //    string strFiveMinutesAgo = fiveMinutesAgo.ToString("yyyy-MM-ddTHH:mm:ss.fff+00:00");
        //    //var c = Convert.ToDateTime(strCurrentDate);
        //    //var b = Convert.ToDateTime(strFiveMinutesAgo);

        //    // Create a filter to match records where enddate > fiveMinutesAgo && enddate <= currentDate
        //    var filter = Builders<L1Signal_Pool>.Filter.And(
        //        Builders<L1Signal_Pool>.Filter.Gt(x => x.enddate, Convert.ToDateTime(strFiveMinutesAgo)), // Greater than
        //        Builders<L1Signal_Pool>.Filter.Lte(x => x.enddate, Convert.ToDateTime(strCurrentDate))    // Less than or equal to
        //    );

        //    // Execute the query and return the matching records
        //    var records = await _L1Signal_Pool.Find(filter)
        //        .Sort(Builders<L1Signal_Pool>.Sort.Descending(x => x.enddate)) // Sort by enddate descending
        //        .Limit(20)
        //        .ToListAsync();

        //    var a = new List<L1Signal_Pool>();

        //    return records;
        //    //return a;
        //}

        #region Recent_L1_Signal_pool
        [HttpGet("Recent_L1_Signal_pool")]
        public async Task GetRecentRecords()
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
            var filter = Builders<L1Signal_Pool>.Filter.And(
                Builders<L1Signal_Pool>.Filter.Gte(x => x.enddate, Convert.ToDateTime(strFiveMinutesAgo)), // Greater than or equal to
                Builders<L1Signal_Pool>.Filter.Lte(x => x.enddate, Convert.ToDateTime(strCurrentDate))    // Less than or equal to
            );

            DateTime? latestCaptuteEndDate ;
            FilterDefinition<L1Signal_Pool> filterByLatestdate = null;
            var latestCaptuteEndDateRecord = await _sqlDbContext.LatestCapturedEndDate.FirstOrDefaultAsync(e => e.collectionName == "L1Signal_Pool");
            if (latestCaptuteEndDateRecord != null)
            {
                latestCaptuteEndDate = latestCaptuteEndDateRecord.LatestCapturedEndDate;
                if (latestCaptuteEndDate != DateTime.MinValue)
                {
                    string strlatestCaptuteEndDate = latestCaptuteEndDate?.ToString("yyyy-MM-ddTHH:mm:ss.fff+00:00");
                     filterByLatestdate = Builders<L1Signal_Pool>.Filter.And(
                        Builders<L1Signal_Pool>.Filter.Gt(x => x.enddate, Convert.ToDateTime(strlatestCaptuteEndDate)) // Greater than   
                        );

                }
            }

            FilterDefinition<L1Signal_Pool> combinedFilter = null;


            if (filterByLatestdate is not null)
            {
                combinedFilter = Builders<L1Signal_Pool>.Filter.And(
                filterByLatestdate,
                filter 
                );
            }
            else
            {
                combinedFilter = Builders<L1Signal_Pool>.Filter.And(
                filter
                );
            }



            // Execute the query and return the matching records
            var records = await _L1Signal_Pool.Find(combinedFilter)
                .Sort(Builders<L1Signal_Pool>.Sort.Descending(x => x.enddate)) // Sort by enddate descending
                //.Limit(25)
                .ToListAsync();


            var a = new List<L1Signal_Pool>();

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
                            var sqlRecord = new Sql_L1Signal_Pool
                            {
                                // Map the fields from MongoDB record to SQL Server record
                                ObjectId = record.Id,
                                L1Name = record.L1Name,
                                updatedate = record.updatedate,
                                enddate = record.enddate,
                                timespan = record.timespan,
                                signalname = record.signalname,
                                value = Convert.ToString(record.value),

                            };

                            // Insert into SQL Server table
                            await _sqlDbContext.L1Signal_Pool.AddAsync(sqlRecord);
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
                    var existingRecord = await _sqlDbContext.LatestCapturedEndDate.FirstOrDefaultAsync(e => e.collectionName == "L1Signal_Pool");

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
                            collectionName = "L1Signal_Pool",
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

        #endregion

        //#region Recent_L1_Signal_pool
        //[HttpGet("Recent_L1_Signal_pool2")]
        //public async Task GetRecentRecord()
        //{
        //    // Get the current UTC date and time
        //    var currentDate = DateTime.UtcNow;

        //    //// Convert the UTC time to Indian Standard Time (IST) - UTC + 5:30
        //    //var indianTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        //    //var currentIndianDate = TimeZoneInfo.ConvertTimeFromUtc(currentDate, indianTimeZone);

        //    // For test start

        //    string input = "2019-11-09T06:22:20.500+00:00";
        //    // Parse the string into a DateTime object  
        //    DateTime dateTimeUtc = DateTime.Parse(input).ToUniversalTime();
        //    currentDate = dateTimeUtc;
        //    var currentIndianDate = dateTimeUtc;

        //    // For test end


        //    // Calculate the date and time 5 minutes before the current time
        //    var fiveMinutesAgo = currentIndianDate.AddMinutes(-5);

        //    //// Convert the UTC time to Indian Standard Time (IST) - UTC + 5:30
        //    //var indianTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        //    //var currentIndianDate = TimeZoneInfo.ConvertTimeFromUtc(currentUtcDate, indianTimeZone);

        //    // Format currentDate and fiveMinutesAgo in the exact required format (ISO 8601)
        //    //string strCurrentDate = currentDate.ToString("yyyy-MM-ddTHH:mm:ss.fff+00:00");
        //    string strCurrentDate = currentIndianDate.ToString("yyyy-MM-ddTHH:mm:ss.fff+00:00");
        //    string strFiveMinutesAgo = fiveMinutesAgo.ToString("yyyy-MM-ddTHH:mm:ss.fff+00:00");
        //    //var c = Convert.ToDateTime(strCurrentDate);
        //    //var b = Convert.ToDateTime(strFiveMinutesAgo);

        //    // Create a filter to match records where enddate > fiveMinutesAgo && enddate <= currentDate
        //    var filter = Builders<L1Signal_Pool>.Filter.And(
        //        Builders<L1Signal_Pool>.Filter.Gte(x => x.enddate, Convert.ToDateTime(strFiveMinutesAgo)), // Greater than or equal to
        //        Builders<L1Signal_Pool>.Filter.Lte(x => x.enddate, Convert.ToDateTime(strCurrentDate))    // Less than or equal to
        //    );

        //    DateTime latestCaptuteEndDate;
        //    FilterDefinition<L1Signal_Pool> filterByLatestdate = null;
        //    var latestCaptuteEndDateRecord = await _sqlDbContext.LatestCapturedEndDate.FirstOrDefaultAsync(e => e.collectionName == "L1Signal_Pool");
        //    if (latestCaptuteEndDateRecord != null)
        //    {
        //        latestCaptuteEndDate = latestCaptuteEndDateRecord.LatestCapturedEndDate;
        //        if (latestCaptuteEndDate != DateTime.MinValue)
        //        {
        //            string strlatestCaptuteEndDate = latestCaptuteEndDate.ToString("yyyy-MM-ddTHH:mm:ss.fff+00:00");
        //            filterByLatestdate = Builders<L1Signal_Pool>.Filter.And(
        //               Builders<L1Signal_Pool>.Filter.Gt(x => x.enddate, Convert.ToDateTime(strlatestCaptuteEndDate)) // Greater than   
        //               );

        //        }
        //    }

        //    FilterDefinition<L1Signal_Pool> combinedFilter = null;


        //    if (filterByLatestdate is not null)
        //    {
        //        combinedFilter = Builders<L1Signal_Pool>.Filter.And(
        //        filterByLatestdate,
        //        filter
        //        );
        //    }
        //    else
        //    {
        //        combinedFilter = Builders<L1Signal_Pool>.Filter.And(
        //        filter
        //        );
        //    }



        //    // Execute the query and return the matching records
        //    var records = await _L1Signal_Pool.Find(combinedFilter)
        //        .Sort(Builders<L1Signal_Pool>.Sort.Descending(x => x.enddate)) // Sort by enddate descending
        //                                                                       //.Limit(25)
        //        .ToListAsync();


        //    var a = new List<L1Signal_Pool>();

        //    // Assuming _sqlDbContext is your Entity Framework context for SQL Server
        //    using (var transaction = await _sqlDbContext.Database.BeginTransactionAsync())
        //    {
        //        try
        //        {
        //            if (records.Count > 0)
        //            {
        //                // Loop through the MongoDB records and insert them into SQL Server
        //                foreach (var record in records)
        //                {
        //                    var sqlRecord = new Sql_L1Signal_Pool
        //                    {
        //                        // Map the fields from MongoDB record to SQL Server record
        //                        ObjectId = record.Id,
        //                        L1Name = record.L1Name,
        //                        updatedate = record.updatedate,
        //                        enddate = record.enddate,
        //                        timespan = record.timespan,
        //                        signalname = record.signalname,
        //                        value = Convert.ToString(record.value),

        //                    };

        //                    // Insert into SQL Server table
        //                    await _sqlDbContext.L1Signal_Pool.AddAsync(sqlRecord);
        //                }

        //                // Commit the transaction
        //                await _sqlDbContext.SaveChangesAsync();
        //                //await transaction.CommitAsync();
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            // Rollback in case of error
        //            await transaction.RollbackAsync();
        //            throw new Exception("Error occurred while inserting records into SQL Server", ex);
        //        }

        //        try
        //        {
        //            if (records.Count > 0)
        //            {
        //                var existingRecord = await _sqlDbContext.LatestCapturedEndDate.FirstOrDefaultAsync(e => e.collectionName == "L1Signal_Pool");

        //                if (existingRecord != null)
        //                {
        //                    // If the record exists, update it
        //                    existingRecord.LatestCapturedEndDate = records[0].enddate.ToUniversalTime(); // Update EndDate (or other fields as needed)
        //                                                                                                //existingRecord.LatestCaptuteEndDate = records[0].enddate; // Update EndDate (or other fields as needed)


        //                    _sqlDbContext.LatestCapturedEndDate.Update(existingRecord); // Mark it for update
        //                }
        //                else
        //                {
        //                    // If the record doesn't exist, insert a new one
        //                    var latestDateRecord = new Sql_LatestCapturedEndDate
        //                    {
        //                        // Map the fields from MongoDB record to SQL Server record
        //                        collectionName = "L1Signal_Pool",
        //                        LatestCapturedEndDate = records[0].enddate,


        //                    };

        //                    await _sqlDbContext.LatestCapturedEndDate.AddAsync(latestDateRecord); // Insert new record
        //                }

        //                // Commit the transaction
        //                await _sqlDbContext.SaveChangesAsync();
        //                await transaction.CommitAsync();
        //            }

        //        }
        //        catch (Exception ex)
        //        {
        //            // Rollback in case of error
        //            await transaction.RollbackAsync();
        //            throw new Exception("Error occurred while inserting records into SQL Server", ex);
        //        }
        //    }


        //    //return a;
        //}

        //#endregion
    }
}

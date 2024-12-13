//using Microsoft.EntityFrameworkCore;
//using MongoDB.Driver;
//using MongoDB_DotNetCore.data;
//using MongoDB_DotNetCore.Entities.MongoDB;
//using MongoDB_DotNetCore.Entities.MSSQL;
//using System.Globalization;

//namespace MongoDB_DotNetCore.Domain
//{
//    public class SystemDiagnosis_Machine_Transfer
//    {
//        private readonly MongoDbContext _mongoDbContext;

//        private readonly SQLDatabaseContext _sqlDbContext;
//        private readonly IMongoCollection<SystemDiagnosis_Machine> _SystemDiagnosis_Machine;
//        public SystemDiagnosis_Machine_Transfer(MongoDbContext mongoDbContext, SQLDatabaseContext sqlDbContext)
//        {
//            _mongoDbContext = mongoDbContext;
//            _SystemDiagnosis_Machine = mongoDbContext.GetCollection<SystemDiagnosis_Machine>("SystemDiagnosis_Machine");
//            _sqlDbContext = sqlDbContext;
//        }
//        public async Task addSystemDiagnosis_MachineRecentRecords(DateTimeOffset input1)
//        {
//            // Get the current UTC date and time
//            var currentDate = DateTime.UtcNow;

//            // Convert the UTC time to Indian Standard Time (IST) - UTC + 5:30
//            var indianTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
//            var currentIndianDate = TimeZoneInfo.ConvertTimeFromUtc(currentDate, indianTimeZone);

//            // For test start

//            // test 1

//            //string input = "2019-11-09T06:22:20.500+00:00";
//            //string input = "2019-09-04T10:53:28.000+00:00";
//            // Parse the string into a DateTime object  
//            //DateTime dateTimeUtc = DateTime.Parse(input).ToUniversalTime();
//            //currentDate = dateTimeUtc;
//            //var currentIndianDate = dateTimeUtc;

//            // test 2 by parameter
//            ////string input = "04-09-2019 10:56:28 +00:00";
//            //string input = input1.ToString();
//            //// Parse the input string into DateTime using custom format
//            //DateTime parsedDate = DateTime.ParseExact(input, "dd-MM-yyyy HH:mm:ss zzz", CultureInfo.InvariantCulture);

//            //// Format the DateTime object into the desired format
//            //string output = parsedDate.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz");


//            //// Parse the string into a DateTime object  
//            //DateTime dateTimeUtc = DateTime.Parse(output).ToUniversalTime();
//            //currentDate = dateTimeUtc;
//            //var currentIndianDate = dateTimeUtc;

//            // For test end


//            // Calculate the date and time 5 minutes before the current time
//            var fiveMinutesAgo = currentIndianDate.AddMinutes(-5);

//            //// Convert the UTC time to Indian Standard Time (IST) - UTC + 5:30
//            //var indianTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
//            //var currentIndianDate = TimeZoneInfo.ConvertTimeFromUtc(currentUtcDate, indianTimeZone);

//            // Format currentDate and fiveMinutesAgo in the exact required format (ISO 8601)
//            //string strCurrentDate = currentDate.ToString("yyyy-MM-ddTHH:mm:ss.fff+00:00");
//            string strCurrentDate = currentIndianDate.ToString("yyyy-MM-ddTHH:mm:ss.fff+00:00");
//            string strFiveMinutesAgo = fiveMinutesAgo.ToString("yyyy-MM-ddTHH:mm:ss.fff+00:00");
//            //var c = Convert.ToDateTime(strCurrentDate);
//            //var b = Convert.ToDateTime(strFiveMinutesAgo);

//            // Create a filter to match records where enddate > fiveMinutesAgo && enddate <= currentDate
//            var filter = Builders<SystemDiagnosis_Machine>.Filter.And(
//                Builders<SystemDiagnosis_Machine>.Filter.Gte(x => x.enddate, Convert.ToDateTime(strFiveMinutesAgo)), // Greater than or equal to
//                Builders<SystemDiagnosis_Machine>.Filter.Lte(x => x.enddate, Convert.ToDateTime(strCurrentDate))    // Less than or equal to
//            );

//            DateTime latestCaptuteEndDate;
//            FilterDefinition<SystemDiagnosis_Machine> filterByLatestdate = null;
//            var latestCaptuteEndDateRecord = await _sqlDbContext.LatestCapturedEndDate.FirstOrDefaultAsync(e => e.collectionName == "SystemDiagnosis_Machine");
//            if (latestCaptuteEndDateRecord != null)
//            {
//                latestCaptuteEndDate = latestCaptuteEndDateRecord.LatestCapturedEndDate;
//                if (latestCaptuteEndDate != DateTime.MinValue)
//                {
//                    string strlatestCaptuteEndDate = latestCaptuteEndDate.ToString("yyyy-MM-ddTHH:mm:ss.fff+00:00");
//                     filterByLatestdate = Builders<SystemDiagnosis_Machine>.Filter.And(
//                        Builders<SystemDiagnosis_Machine>.Filter.Gt(x => x.enddate, Convert.ToDateTime(strlatestCaptuteEndDate)) // Greater than   
//                        );

//                }
//            }

//            FilterDefinition<SystemDiagnosis_Machine> combinedFilter = null;


//            if (filterByLatestdate is not null)
//            {
//                combinedFilter = Builders<SystemDiagnosis_Machine>.Filter.And(
//                filterByLatestdate,
//                filter 
//                );
//            }
//            else
//            {
//                combinedFilter = Builders<SystemDiagnosis_Machine>.Filter.And(
//                filter
//                );
//            }



//            // Execute the query and return the matching records
//            var records = await _SystemDiagnosis_Machine.Find(combinedFilter)
//                .Sort(Builders<SystemDiagnosis_Machine>.Sort.Descending(x => x.enddate)) // Sort by enddate descending
//                //.Limit(25)
//                .ToListAsync();


//            var a = new List<SystemDiagnosis_Machine>();

//            // Assuming _sqlDbContext is your Entity Framework context for SQL Server
//            using (var transaction = await _sqlDbContext.Database.BeginTransactionAsync())
//            {
//                try
//                {
//                    if (records.Count > 0)
//                    {
//                        // Loop through the MongoDB records and insert them into SQL Server
//                        foreach (var record in records)
//                        {
//                            var sqlRecord = new Sql_SystemDiagnosis_Machine
//                            {
//                                // Map the fields from MongoDB record to SQL Server record
//                                ObjectId = record.Id,
//                                TagName = record.TagName,
//                                updatedate = record.updatedate,
//                                enddate = record.enddate,
//                                timespan = record.timespan,
//                                signalname = record.signalname,
//                                value = Convert.ToString(record.value),

//                            };

//                            // Insert into SQL Server table
//                            await _sqlDbContext.SystemDiagnosis_Machine.AddAsync(sqlRecord);
//                        }

//                        // Commit the transaction
//                        await _sqlDbContext.SaveChangesAsync();
//                        //await transaction.CommitAsync();
//                    }
//                }
//                catch (Exception ex)
//                {
//                    // Rollback in case of error
//                    await transaction.RollbackAsync();
//                    throw new Exception("Error occurred while inserting records into SQL Server", ex);
//                }

//                try {
//                    if(records.Count > 0) {
//                    var existingRecord = await _sqlDbContext.LatestCapturedEndDate.FirstOrDefaultAsync(e => e.collectionName == "SystemDiagnosis_Machine");

//                    if (existingRecord != null)
//                    {
//                        // If the record exists, update it
//                        existingRecord.LatestCapturedEndDate = records[0].enddate.ToUniversalTime(); // Update EndDate (or other fields as needed)
//                        //existingRecord.LatestCaptuteEndDate = records[0].enddate; // Update EndDate (or other fields as needed)


//                        _sqlDbContext.LatestCapturedEndDate.Update(existingRecord); // Mark it for update
//                    }
//                    else
//                    {
//                        // If the record doesn't exist, insert a new one
//                        var latestDateRecord = new Sql_LatestCapturedEndDate
//                        {
//                            // Map the fields from MongoDB record to SQL Server record
//                            collectionName = "SystemDiagnosis_Machine",
//                            LatestCapturedEndDate = records[0].enddate,


//                        };

//                        await _sqlDbContext.LatestCapturedEndDate.AddAsync(latestDateRecord); // Insert new record
//                    }

//                    // Commit the transaction
//                    await _sqlDbContext.SaveChangesAsync();
//                    await transaction.CommitAsync();
//                    }

//                }
//                catch (Exception ex)
//                {
//                    // Rollback in case of error
//                    await transaction.RollbackAsync();
//                    throw new Exception("Error occurred while inserting records into SQL Server", ex);
//                }
//            }


//            //return a;
//        }

//    }
//}

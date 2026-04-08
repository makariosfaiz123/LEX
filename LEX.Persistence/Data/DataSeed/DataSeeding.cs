using LEX.Domain.Entities;
using LEX.Persistence.Data.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LEX.Persistence.Data.DataSeed
{



    public class DataSeeding(AppDbContext _context)
    {
        public async Task DataSeed()
        {
            try
            {
                if ((await _context.Database.GetPendingMigrationsAsync()).Any())
                    await _context.Database.MigrateAsync();
                if (!_context.Lawyers.Any())
                {
                    var LawyersFile = File.OpenRead("..\\LEX.Persistence\\Data\\DataSeed\\Lawyers.json");
                    var finallawyes = await JsonSerializer.DeserializeAsync<List<Lawyer>>(LawyersFile );
                    if (finallawyes is not null && finallawyes.Any())
                        await _context.AddRangeAsync(finallawyes);
                    await _context.SaveChangesAsync();

                }

                if (!_context.Customers.Any())
                {
                    var CustomersrFile = File.OpenRead("..\\LEX.Persistence\\Data\\DataSeed\\Customersr.json");
                    var finallCustomersr = await JsonSerializer.DeserializeAsync<List<Customer>>(CustomersrFile);
                    if (finallCustomersr is not null && finallCustomersr.Any())
                        await _context.AddRangeAsync(finallCustomersr);
                    await _context.SaveChangesAsync();

                }


                if (!_context.Documents.Any())
                {
                    var DocumentsFile = File.OpenRead("..\\LEX.Persistence\\Data\\DataSeed\\Documents.json");
                    var finalDocuments = await JsonSerializer.DeserializeAsync<List<Documents>>(DocumentsFile);
                    if (finalDocuments is not null && finalDocuments.Any())
                        await _context.AddRangeAsync(finalDocuments);
                    await _context.SaveChangesAsync();

                }

                if (!_context.CaseRequests.Any())
                {
                    var CaseRequestsFile = File.OpenRead("..\\LEX.Persistence\\Data\\DataSeed\\CaseRequests.json");
                    var finalCaseRequests = await JsonSerializer.DeserializeAsync<List<CaseRequests>>(CaseRequestsFile);
                    if (finalCaseRequests is not null && finalCaseRequests.Any())
                        await _context.AddRangeAsync(finalCaseRequests);
                    await _context.SaveChangesAsync();

                }

                if (!_context.Reviews.Any())
                {
                    var ReviewsFile = File.OpenRead("..\\LEX.Persistence\\Data\\DataSeed\\Reviews.json");
                    var finalReviews = await JsonSerializer.DeserializeAsync<List<Review>>(ReviewsFile);
                    if (finalReviews is not null && finalReviews.Any())
                        await _context.AddRangeAsync(finalReviews);
                    await _context.SaveChangesAsync();

                }
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
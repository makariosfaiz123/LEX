using LEX.Domain.Entities;
using LEX.Persistence.Data.Context;
using LEX.Shared.DTOs.LawyerDtos;
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

                // 1. عمل Seed للأماكن (Countries -> Governorates -> Cities)
                if (!_context.Countries.Any())
                {
                    var egypt = new Country
                    {
                        Name = "Egypt",
                        Governorates = new List<Governorate>
                        {
                            new Governorate
                            {
                                Name = "Sohag",
                                Cities = new List<City>
                                {
                                    new City { Name = "Jirja" },
                                    new City { Name = "Sohag City" },
                                }
                            },
                            new Governorate
                            {
                                Name = "Cairo",
                                Cities = new List<City>
                                {
                                    new City { Name = "Nasr City" },
                                    new City { Name = "Maadi" },
                                }
                            },
                            new Governorate
                            {
                                Name = "Alexandria",
                                Cities = new List<City>
                                {
                                    new City { Name = "Smouha" }
                                }
                            },
                            new Governorate
                            {
                                Name = "Giza",
                                Cities = new List<City>
                                {
                                    new City { Name = "Dokki" }
                                }
                            }
                        }
                    };

                    await _context.Countries.AddAsync(egypt);
                    await _context.SaveChangesAsync();
                }

                // 2. عمل Seed للمحامين
                if (!_context.Lawyers.Any())
                {
                    var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var lawyersPath = "..\\LEX.Persistence\\Data\\DataSeed\\Lawyers.json";

                    if (File.Exists(lawyersPath))
                    {
                        using var lawyersStream = File.OpenRead(lawyersPath);
                        var lawyersData = await JsonSerializer.DeserializeAsync<List<LawyerSeedDto>>(lawyersStream, jsonOptions);

                        if (lawyersData is not null && lawyersData.Any())
                        {
                            var lawyersToInsert = new List<Lawyer>();

                            // 💡 التعديل هنا: عملنا Include للمحافظة عشان نقدر نجيب الـ CountryId
                            var dbCities = await _context.Cities
                                .Include(c => c.Governorate)
                                .ToListAsync();

                            foreach (var item in lawyersData)
                            {
                                var city = dbCities.FirstOrDefault(c => c.Name.Equals(item.City, StringComparison.OrdinalIgnoreCase));

                                lawyersToInsert.Add(new Lawyer
                                {
                                    Fname = item.Fname,
                                    Lname = item.Lname,
                                    Email = item.Email,
                                    Phone = item.Phone,
                                    Porfile_Image = item.Porfile_Image,

                                    // ==========================================
                                    // التعيين المباشر (عشان إنت ظبطت الـ DTO خلاص)
                                    // ==========================================
                                    Date_Of_Birth = item.Date_Of_Birth,       // مباشر
                                    Start_Practice = item.Start_Practice,     // مباشر

                                    gender = item.Gender,                     // مباشر
                                    RoleS = item.RoleS,                       // مباشر
                                    Status = item.Status,                     // مباشر

                                    // الوحيدة اللي هتحتاج Cast هي الـ Specialization عشان إنت سايبها int في الـ DTO
                                    Specialization = (LEX.Domain.EnumS.Specializations)item.Specialization,

                                    Location = new Location
                                    {
                                        Street = item.Street,

                                        // ربط المدينة
                                        CityId = city?.Id ?? 0,

                                        // 💡 السطر اللي كان ناقص (ربط المحافظة عشان الـ Constraint ميضربش)
                                        GovernorateId = city?.GovernorateId ?? 0,

                                        // ربط الدولة
                                        CountryId = city?.Governorate?.CountryId ?? 0
                                    }
                                });
                            }

                            await _context.Lawyers.AddRangeAsync(lawyersToInsert);
                            await _context.SaveChangesAsync();
                        }
                    }
                }

                if (!_context.Customers.Any())
                {
                    var CustomersrFile = File.OpenRead("..\\LEX.Persistence\\Data\\DataSeed\\Customersr.json");
                    var finallCustomersr = await JsonSerializer.DeserializeAsync<List<Customer>>(CustomersrFile);
                    if (finallCustomersr is not null && finallCustomersr.Any())
                        await _context.Customers.AddRangeAsync(finallCustomersr);
                    await _context.SaveChangesAsync();

                }


                //if (!_context.Documents.Any())
                //{
                //    var DocumentsFile = File.OpenRead("..\\LEX.Persistence\\Data\\DataSeed\\Documents.json");
                //    var finalDocuments = await JsonSerializer.DeserializeAsync<List<Documents>>(DocumentsFile);
                //    if (finalDocuments is not null && finalDocuments.Any())
                //        await _context.AddRangeAsync(finalDocuments);
                //    await _context.SaveChangesAsync();

                //}

                //if (!_context.CaseRequests.Any())
                //{
                //    var CaseRequestsFile = File.OpenRead("..\\LEX.Persistence\\Data\\DataSeed\\CaseRequests.json");
                //    var finalCaseRequests = await JsonSerializer.DeserializeAsync<List<CaseRequests>>(CaseRequestsFile);
                //    if (finalCaseRequests is not null && finalCaseRequests.Any())
                //        await _context.AddRangeAsync(finalCaseRequests);
                //    await _context.SaveChangesAsync();

                //}

                //if (!_context.Reviews.Any())
                //{
                //    var ReviewsFile = File.OpenRead("..\\LEX.Persistence\\Data\\DataSeed\\Reviews.json");
                //    var finalReviews = await JsonSerializer.DeserializeAsync<List<Review>>(ReviewsFile);
                //    if (finalReviews is not null && finalReviews.Any())
                //        await _context.AddRangeAsync(finalReviews);
                //    await _context.SaveChangesAsync();

                //}
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
using CityInfo.API.DbContexts;
using CityInfo.API.Entities;
using CityInfo.API.Models;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.Services
{
    public class CityInfoRepository : ICityInfoRepository
    {
        private readonly CityInfoContext _context;
        public CityInfoRepository(CityInfoContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<City>> GetCitiesAsync()
        {
            return await _context.Cities.ToListAsync();
        }

        public async Task<IEnumerable<City>> GetCitiesAsync(string? name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return await GetCitiesAsync();
            }

            name = name.Trim();
            return await _context.Cities
                        .Where(c => c.Name == name)
                        .OrderBy(c => c.Name)
                        .ToListAsync();
        }

        public async Task<IEnumerable<City>> GetCitiesAsync(string? name, string? searchQuery)
        {
            if (string.IsNullOrEmpty (name)
                && string.IsNullOrWhiteSpace(searchQuery))
            {
                return await GetCitiesAsync();
            }

            // collection to start from
            var collection = _context.Cities as IQueryable<City>;

            if (!string.IsNullOrWhiteSpace(name)){
                name = name.Trim();
                collection = collection.Where(c => c.Name == name);
            }
            
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                searchQuery = searchQuery.Trim();
                collection = collection.Where(a => a.Name.Contains(searchQuery)
                                || (a.Description != null && a.Description.Contains(searchQuery)));
            }
            return await collection.OrderBy(c => c.Name).ToListAsync();
        }

        public async Task<(IEnumerable<City>, PaginationMetaData)> GetCitiesAsync(string? name, string? searchQuery, int pageNumber, int pageSize)
        {
           

            // collection to start from
            var collection = _context.Cities as IQueryable<City>;
            if (!string.IsNullOrWhiteSpace(name))
            {
                name = name.Trim();
                collection = collection.Where(c => c.Name == name);
            }

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                searchQuery = searchQuery.Trim();
                collection = collection.Where(a => a.Name.Contains(searchQuery)
                                || (a.Description != null && a.Description.Contains(searchQuery)));
            }

            var totalItemCount = await collection.CountAsync();

            var paginationMetaData = new PaginationMetaData(totalItemCount, pageSize, pageNumber);
            var collectionToReturn = await collection.OrderBy(c => c.Name)
                          .Skip(pageSize*(pageNumber-1))
                          .Take(pageSize)
                          .ToListAsync();
            return (collectionToReturn, paginationMetaData);
        }


        public async Task<City?> GetCityAsync(int cityId, bool includePointsOfInterest)
        {
           if (includePointsOfInterest)
            {
                return await _context.Cities.Include(x => x.PointOfInterest)
                                      .Where(x => x.Id == cityId)
                                      .FirstOrDefaultAsync();
            }
            return await _context.Cities.FirstOrDefaultAsync(x => x.Id == cityId);
        }

        public async Task<bool> CityExistsAsync(int cityId)
        {
            return await _context.Cities.AnyAsync(c => c.Id == cityId);
        }

        public async Task<PointOfInterest?> GetPointOfInterestForCityAsync(int cityId, int pointOfInterestId)
        {
            return await _context.PointsOfInterests
                            .FirstOrDefaultAsync(x => x.CityId == cityId && x.Id == pointOfInterestId);
        }

        public async Task<IEnumerable<PointOfInterest>> GetPointOfInterestsForCityAsync(int cityId)
        {
            return await _context.PointsOfInterests.Where(x => x.CityId == cityId).ToListAsync();
        }

        public async Task AddPointOfInterestForCityAsync(int cityId, PointOfInterest pointOfInterest)
        {
            var city = await GetCityAsync(cityId, false);
            if (city != null)
            {
                city.PointOfInterest.Add(pointOfInterest);
            }
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() >= 0);
        }

        public void DeletePointOfInterest(PointOfInterest pointOfInterest)
        {
            _context.PointsOfInterests.Remove(pointOfInterest);
        }

        public async Task<bool> CityNameMatchesCityId(string? cityName, int cityId)
        {
            return await _context.Cities.AnyAsync(c => c.Id == cityId && c.Name == cityName);
        }
    }
}

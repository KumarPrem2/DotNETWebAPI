using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CitiesController: ControllerBase
    {
        private readonly CitiesDataStore _citiesDataStore;
        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;
        const int maxCitiesPageSize = 20;

        public CitiesController(CitiesDataStore citiesDataStore, ICityInfoRepository cityInfoRepository, IMapper mapper)
        {
            _citiesDataStore = citiesDataStore ?? throw new ArgumentNullException(nameof(citiesDataStore));
            _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
           
        }
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<CityWithoutPointOfInterestDto>>> GetCities()
        //{
        //   var cities = await _cityInfoRepository.GetCitiesAsync();

        //    return Ok(_mapper.Map<IEnumerable<CityWithoutPointOfInterestDto>>(cities));
        //}

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCity(int id, bool includePointOfInterest = false)
        {


            var city = await _cityInfoRepository.GetCityAsync(id, includePointOfInterest);
            if (city == null)
            {
                return NotFound();
            }

            if (includePointOfInterest)
            {
                return Ok(_mapper.Map<CityDto>(city));
            }
            return Ok(_mapper.Map<CityWithoutPointOfInterestDto>(city));
        }

        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<CityWithoutPointOfInterestDto>>> GetCities([FromQuery(Name ="name")] string? name)
        //{
        //    var cities = await _cityInfoRepository.GetCitiesAsync(name);
        //    return Ok(_mapper.Map<IEnumerable<CityWithoutPointOfInterestDto>>(cities));
        //}

        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<CityWithoutPointOfInterestDto>>> GetCities([FromQuery(Name = "name")] string? name, 
        //   [FromQuery(Name = "searchQuery")] string? searchQuery)
        //{
        //    var cities = await _cityInfoRepository.GetCitiesAsync(name, searchQuery);
        //    return Ok(_mapper.Map<IEnumerable<CityWithoutPointOfInterestDto>>(cities));
        //}

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CityWithoutPointOfInterestDto>>> GetCities([FromQuery(Name = "name")] string? name,
          [FromQuery(Name = "searchQuery")] string? searchQuery, int pageNumber = 1, int pageSize = 10)
        {
            if (pageSize > maxCitiesPageSize)
            {
                pageSize = maxCitiesPageSize;
            }

            var (cities, paginationMetaData) = await _cityInfoRepository.GetCitiesAsync(name, searchQuery, pageNumber, pageSize);
            Response.Headers.Add("X-Pagination",
                JsonSerializer.Serialize(paginationMetaData)
                );

            return Ok(_mapper.Map<IEnumerable<CityWithoutPointOfInterestDto>>(cities));
        }


    }
}

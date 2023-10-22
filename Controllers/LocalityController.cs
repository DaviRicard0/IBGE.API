using IBGE.API.Context;
using IBGE.API.Domain;
using IBGE.API.Domain.DTOs;
using IBGE.API.Domain.DTOs.Common;
using IBGE.API.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Data;
using System.Net;

namespace IBGE.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LocalityController : ControllerBase
    {
        private readonly ILogger<LocalityController> _logger;
        private readonly ResponseDto _responseDto;

        public LocalityController(ILogger<LocalityController> logger)
        {
            _logger = logger;
            _responseDto = new();
        }

        //[HttpGet]
        //public IEnumerable<WeatherForecast> Get()
        //{
        //    throw new NotImplementedException();
        //}

        [HttpGet("results")]
        public async Task<IActionResult> Get([FromQuery(Name = "search_query")] string query = "")
        {
            try
            {
                var dt = await DbContext.SelectAsync(new Locality { Filter = query }, Operating.Select);

                var listLocality = new List<Locality>();

                foreach (DataRow line in dt.Rows)
                {
                    listLocality.Add(new Locality()
                    {

                    });
                }

                _responseDto.Status = HttpStatusCode.OK;
                _responseDto.Data = listLocality;

                return Ok(_responseDto);
            }
            catch (Exception ex)
            {
                _responseDto.Message = ex.Message;
                _responseDto.Status = HttpStatusCode.BadRequest;

                return BadRequest(_responseDto);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(LocalityDto req)
        {
            try
            {
                var localityDomain = Locality.ConvertLocalityDtoToLocality(req);

                await DbContext.SaveAsync(localityDomain, Operating.Create);
                localityDomain.Id = DbContext.GetReturn<int>();

                _responseDto.Status = HttpStatusCode.Created;
                _responseDto.Data = new Locality { Id = localityDomain.Id };

                return Created("uri", _responseDto);
            }
            catch (Exception ex)
            {
                _responseDto.Message = ex.Message;
                _responseDto.Status = HttpStatusCode.BadRequest;

                return BadRequest(_responseDto);
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put([FromRoute] int id,LocalityDto req)
        {
            try
            {
                var localityDomain = Locality.ConvertLocalityDtoToLocality(req);
                localityDomain.Id = id;

                await DbContext.SaveAsync(localityDomain, Operating.Update);

                _responseDto.Status = HttpStatusCode.OK;
                _responseDto.Data = localityDomain;

                return Ok(req);
            }
            catch (Exception ex)
            {
                _responseDto.Message = ex.Message;
                _responseDto.Status = HttpStatusCode.BadRequest;

                return BadRequest(_responseDto);
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                var localityDomain = new Locality { Id = id };

                await DbContext.SaveAsync(localityDomain, Operating.Delete);

                _responseDto.Status = HttpStatusCode.OK;
                _responseDto.Data = localityDomain;

                return Ok(_responseDto);
            }
            catch (Exception ex)
            {
                _responseDto.Message = ex.Message;
                _responseDto.Status = HttpStatusCode.BadRequest;

                return BadRequest(_responseDto);
            }
        }
    }
}
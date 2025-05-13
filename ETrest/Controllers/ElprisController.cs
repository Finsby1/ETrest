using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using ETlib;
using ETlib.Repository;
using Newtonsoft.Json;

namespace ETrest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ElprisController : ControllerBase
    {

        private EnergyPriceRepository _repo;

        public ElprisController(EnergyPriceRepository repo)
        {
            _repo = repo;
        }
        
        private static readonly HttpClient client = new HttpClient();
        private List<double> Elpriser = new List<double>();

        private string GetUrlForToday(string _region)
        {
            var today = DateTime.Now;
            string year = today.Year.ToString();
            string month = today.Month.ToString("D2"); // to cifre, fx "05"
            string day = today.Day.ToString("D2");     // to cifre, fx "12"
            string region = _region; // Eller "DK1" hvis du vil ændre det
            

            return $"https://www.elprisenligenu.dk/api/v1/prices/{year}/{month}-{day}_{region}.json";
        }
 
        
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("fromAPI/{zone}")]
        public async Task<ActionResult<IEnumerable<EnergyPrice>>> GetAllItems(string zone)
        {
            HttpResponseMessage response;
            if (zone == "Vest")
            {
                response = await client.GetAsync(GetUrlForToday("DK1"));
            }
            else if (zone == "Øst")
            {
                response = await client.GetAsync(GetUrlForToday("DK2")); 
            }
            else
            {
                response = null;
                return BadRequest("Invalid zone");
            }
            
            try
            {
                IEnumerable<EnergyPrice>? list = await response.Content.ReadFromJsonAsync<IEnumerable<EnergyPrice>>();

                if (list.Count() > 0 && zone == "Vest")
                {
                    foreach (EnergyPrice ep in list)
                    {
                        _repo.Add(ep, 1);
                    }
                }
                else if (list.Count() > 0 && zone == "Øst")
                {
                    foreach (EnergyPrice ep in list)
                    {
                        _repo.Add(ep, 2);
                    }
                }
                else
                {
                    return BadRequest("Invalid zone");
                }
                return Ok(list);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            
        }   
        


        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("{zone}/{hour}")]
        public ActionResult<EnergyPrice> GetPrisById(int hour, string zone)
        {
            try
            {
                EnergyPrice ep;
                if (zone == "Vest")
                {
                    ep = _repo.GetByHour(hour, 1);
                    if (ep != null)
                    {
                        return Ok(ep);
                    }
                }
                else if (zone == "Øst")
                {
                    ep = _repo.GetByHour(hour, 2);
                    if (ep != null)
                    {
                        return Ok(ep);
                    }
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return BadRequest();
        }
        
        [HttpGet("All/{zone}")]
        public ActionResult<IEnumerable<EnergyPrice>> Get(string zone)
        {
            if(zone == "Vest")
            {
                return _repo.GetSavedPrices(1).ToList();
            }
            else if (zone == "Øst")
            {
                return _repo.GetSavedPrices(2).ToList();
            }
            else
            {
                return BadRequest("Invalid zone");
            }
            
        }
        
        
    }
}



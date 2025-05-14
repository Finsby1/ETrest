using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using ETlib;
using ETlib.Repository;
using Newtonsoft.Json;

namespace ETrest.Controllers
{    [Route("api/[controller]")]
    [ApiController]
    public class ElprisController : ControllerBase
    {

        private EnergyPriceRepository _repo;

        public ElprisController(EnergyPriceRepository repo)
        {
            _repo = repo;
        }
        
        private static readonly HttpClient client = new HttpClient();

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
        [HttpGet("FromAPI")]
        public async Task<ActionResult> GetAllItems()
        {
            _repo.restart();
            HttpResponseMessage West;
            HttpResponseMessage East;

            West = await client.GetAsync(GetUrlForToday("DK1"));
            East = await client.GetAsync(GetUrlForToday("DK2")); 
            
            try
            {
                IEnumerable<EnergyPrice>? WestList = await West.Content.ReadFromJsonAsync<IEnumerable<EnergyPrice>>();
                IEnumerable<EnergyPrice>? EastList = await East.Content.ReadFromJsonAsync<IEnumerable<EnergyPrice>>();
                if (WestList.Count() > 0)
                {
                    foreach (EnergyPrice epWest in WestList)
                    {
                        _repo.Add(epWest, 1);
                    }
                }
                else if (EastList.Count() > 0)
                {
                    foreach (EnergyPrice epEast in EastList)
                    {
                        _repo.Add(epEast, 2);
                    }
                }
                else
                {
                    return BadRequest("Something went wrong while getting data from api");
                }
                return Ok();
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
                if (zone == "West")
                {
                    ep = _repo.GetByHour(hour, 1);
                    if (ep != null)
                    {
                        return Ok(ep);
                    }
                }
                else if (zone == "East")
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
            if(zone == "West")
            {
                return _repo.GetSavedPrices(1).ToList();
            }
            else if (zone == "East")
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



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
        [HttpGet("Vest")]
        public async Task<ActionResult<IEnumerable<EnergyPrice>>> GetAllItemsVest()
        {
            HttpResponseMessage response = await client.GetAsync(GetUrlForToday("DK1"));
            try
            {
                IEnumerable<EnergyPrice>? list = await response.Content.ReadFromJsonAsync<IEnumerable<EnergyPrice>>();

                if (list.Count() > 0)
                {
                    foreach (EnergyPrice ep in list)
                    {
                        _repo.Add(ep, 1);
                    }
                }
                return Ok(list);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            
        }   
        
        [HttpGet("Øst")]
        
        public async Task<ActionResult<IEnumerable<EnergyPrice>>> GetAllItemsØSt()
        {
            HttpResponseMessage response = await client.GetAsync(GetUrlForToday("DK2"));
            try
            {
                IEnumerable<EnergyPrice>? list = await response.Content.ReadFromJsonAsync<IEnumerable<EnergyPrice>>();

                if (list.Count() > 0)
                {
                    foreach (EnergyPrice ep in list)
                    {
                        _repo.Add(ep, 2);
                    }
                }
                return Ok(list);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }   
        

        [HttpGet("{id}")]
        public IActionResult GetPrisById(int id)
        {
            return null;
        }
        
        [HttpGet]
        public ActionResult<IEnumerable<EnergyPrice>> Get()
        {
            return _repo.GetEnergyPricesWest().ToList();
        }
        
        
    }
}



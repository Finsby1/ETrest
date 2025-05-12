using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using ETrest.Models;

namespace ETrest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ElprisController : ControllerBase
    {
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
 
        
        
        [HttpGet("Vest")]
        public async Task<IActionResult> GetAllItemsVest()
        {
            string url = GetUrlForToday("DK1"); // <-- dynamisk URL
            HttpResponseMessage response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                string jsonDK1 = await response.Content.ReadAsStringAsync();
                return Content(jsonDK1, "application/json");
            }
            else
            {
                return StatusCode((int)response.StatusCode, "Error retrieving data");
            }
            
        }   
        
        [HttpGet("Øst")]
        
        public async Task<IActionResult> GetAllItemsØSt()
        {
            string url = GetUrlForToday("DK2"); // <-- dynamisk URL
            HttpResponseMessage response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                string jsonDK2 = await response.Content.ReadAsStringAsync();
                return Content(jsonDK2, "application/json");
            }
            else
            {
                return StatusCode((int)response.StatusCode, "Error retrieving data");
            }
            
        }   
        

        [HttpGet("{id}")]
        public IActionResult GetPrisById(int id)
        {
            return GetPrisByTime(id);
        }

        [HttpPost]
        public IActionResult Post([FromBody] double nyPris)
        {
            if (nyPris < 0 || nyPris > 10)
                return BadRequest("Pris skal være mellem 0 og 10");

            Elpriser.Add(nyPris);
            return Ok(new { besked = "Pris tilføjet", nyPris, indeks = Elpriser.Count - 1 });
        }

        private IActionResult GetPrisByTime(int time)
        {
            if (time < 0 || time >= Elpriser.Count)
                return NotFound("Ingen pris tilgængelig for time " + time);

            double pris = Elpriser[time];
            string tidspunkt = $"{time}:00";
            string vurdering = VurderPris(pris);

            return Ok(new[] { tidspunkt, vurdering });
        }

        private string VurderPris(double pris)
        {
            if (pris < 0.80) return "Lav";
            if (pris < 1.20) return "Middel";
            return "Høj";
        }
    }
}



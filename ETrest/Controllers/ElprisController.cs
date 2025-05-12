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
        private const string URL = "https://www.elprisenligenu.dk/api/v1/prices/2025/05-12_DK2.json";
        private List<double> Elpriser = new List<double>();

        
        
        [HttpGet]
        public async Task<IActionResult> GetAllItems()
        {
            HttpResponseMessage response = await client.GetAsync(URL);

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                return Content(json, "application/json");
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



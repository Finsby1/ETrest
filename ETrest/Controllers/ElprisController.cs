using Microsoft.AspNetCore.Mvc;

namespace ETrest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ElprisController : ControllerBase
    {
        private static readonly List<double> Elpriser = new()
        {
            0.75, 0.65, 0.82, 1.10, 1.55, 1.30, 0.90,
            0.78, 0.69, 0.60, 1.40, 1.25, 0.95, 0.88,
            1.10, 1.35, 1.60, 1.20, 1.00, 0.85, 0.70,
            0.65, 0.80, 0.90
        };

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



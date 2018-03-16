using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using dashboard.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace dashboard.Controllers
{

    [Route("api/railway/v1")]
    public class TrainController : Controller
    {
        private readonly string RailApiKey;

        public TrainController(IOptions<ApiKeys> options)
        {
            RailApiKey = options.Value.RailApiKey;
        }

        [HttpGet]
        public async Task<ActionResult> GetRailwayInfo()
        {
            try
            {
                var accountBytes = Encoding.ASCII.GetBytes(RailApiKey);
                var base64AuthHeaderValue = Convert.ToBase64String(accountBytes);

                var url = "http://webservices.ns.nl/ns-api-avt?station=Koog+aan+de+Zaan";

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("basic", base64AuthHeaderValue);
                    var result = await client.GetStringAsync(url);
                    var document = XDocument.Parse(result);

                    var resultData = document.Root.Descendants("VertrekkendeTrein").Take(5).Select(tr => new
                    {
                        type = tr.Descendants().FirstOrDefault(d => d.Name == "TreinSoort")?.Value.Trim() ?? "Trein",
                        destination = tr.Descendants().FirstOrDefault(d => d.Name == "EindBestemming")?.Value.Trim() ?? "(Onbekend)",
                        route = tr.Descendants().FirstOrDefault(d => d.Name == "RouteTekst")?.Value.Trim() ?? string.Empty,
                        departureTime = tr.Descendants().FirstOrDefault(d => d.Name == "VertrekTijd")?.Value.Trim() ?? null,
                        track = tr.Descendants().FirstOrDefault(d => d.Name == "VertrekSpoor")?.Value.Trim() ?? "?",
                        delay = tr.Descendants().FirstOrDefault(d => d.Name == "VertrekVertragingTekst")?.Value.Trim() ?? null,
                        notes = tr.Descendants("Opmerkingen").Select(o => o.Value.Trim())
                    });

                    return Ok(resultData);
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to load railway info: { ex.Message }");
            }
        }
    }
}
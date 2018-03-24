using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;
using System.Linq;
using Newtonsoft.Json;

namespace dashboard.Controllers 
{
    [Route("api/xkcd/v1")]
    public class XkcdController : Controller
    {
        [HttpGet]
        public async Task<ActionResult> LoadStrip()
        {
            using (var client = new HttpClient())
            {
                var xkcdResponse = await client.GetStringAsync("https://xkcd.com/info.0.json");
                var comicData = JsonConvert.DeserializeObject(xkcdResponse);

                return Ok(comicData);
            }
        }
    }
}
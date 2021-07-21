using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WondyBotAPI.Supervisors;

namespace WondyBotAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly CompanySupervisor _supervisor;

        public CompanyController(CompanySupervisor supervisor)
        {
            _supervisor = supervisor;
        }

        [HttpGet("companyId")]
        public async Task<ActionResult<IEnumerable<string>>> GetCompanyId(string companyId)
        {
            //gets a list of all companys that match the user input with either ticker or company name
            var companyNameMatches = await _supervisor.GetCompanyNameMatches(companyId);

            if(companyNameMatches.Count() > 0)
            {
                return Ok(companyNameMatches);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("ticker")]
        public async Task<ActionResult<string>> GetCompanyDetail(string ticker)
        {
            //returns company details based on ticker input
            var companyDetails = await _supervisor.GetCompanyDetails(ticker);

            return companyDetails;
        }
    }
}

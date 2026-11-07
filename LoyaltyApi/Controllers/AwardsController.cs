using LoyaltyApi.Contracts;
using LoyaltyApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace LoyaltyApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public sealed class AwardsController (IAwardService awardService): ControllerBase
    {
        [HttpPost]
        public ActionResult<AwardResponse> CreateAward([FromBody] AwardRequest? request)
        {
            var errors = AwardRequestValidator.Validate(request);

            if(errors.Any())
                return BadRequest(errors);

            var result = awardService.CreateAward(request!);

            return result.Status switch
            {
                AwardResultStatus.Created =>
                    StatusCode(StatusCodes.Status201Created, result.Award),

                AwardResultStatus.ExistingMatch =>
                    Ok(result.Award),

                AwardResultStatus.Conflict =>
                    Conflict(new { message = "This award is claimed already" }),

                AwardResultStatus.WalletNotFound =>
                    NotFound(new { message = $"Wallet not found" }),

                _ => StatusCode(500, new { message = "An unexpected error occurred" })
            };
        }
    
    }
}

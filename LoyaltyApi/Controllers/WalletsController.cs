using LoyaltyApi.Contracts;
using LoyaltyApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace LoyaltyApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public sealed class WalletsController (IAwardService awardService) : ControllerBase
    {

        [HttpGet("{walletId}")]
        public ActionResult<WalletResponse> GetWallet(string walletId)
        {
            var wallet = awardService.GetWallet(walletId);
            if (wallet is null)
                return NotFound();

            return Ok(wallet);
        }
    }
}

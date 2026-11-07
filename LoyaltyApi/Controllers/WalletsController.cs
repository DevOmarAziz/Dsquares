using LoyaltyApi.Contracts;
using LoyaltyApi.Data;
using LoyaltyApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace LoyaltyApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public sealed class WalletsController (IAwardService awardService, InMemoryStore store) : ControllerBase
    {

        [HttpGet("{walletId}")]
        public ActionResult<WalletResponse> GetWallet(string walletId)
        {
            var wallet = awardService.GetWallet(walletId);
            if (wallet is null)
                return NotFound();

            return Ok(wallet);
        }

        [HttpPost("reset")]
        public IActionResult Reset()
        {
            store.Reset();
            return Ok();
        }
    }
}

using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.FeatureManagement;

namespace RaffleApp.Pages
{
    public class IndexModel(IVariantFeatureManagerSnapshot featureManager) : PageModel
    {
        public bool IsWinner { get; set; }
        public string Message { get; set; } = string.Empty;

        public async Task OnGet()
        {
            Variant variant = await featureManager.GetVariantAsync("Raffle");
            IsWinner = variant.Name == "Winner";
            Message = variant.Configuration?.Get<string>() ?? "Welcome to the Raffle!";
        }
    }
}

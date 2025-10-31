using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ShoraWorkManager.Areas.Identity.Pages.Account.Manage
{
    public class TablesManagement : PageModel
    {
        private readonly ILogger<TablesManagement> _logger;

        public TablesManagement(ILogger<TablesManagement> logger)
        {
            _logger = logger;
        }
    }
}

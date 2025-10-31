using Microsoft.AspNetCore.Html;

namespace Application.Contracts.Response
{
    public class EditedResult
    {
        public bool IsSuccess { get; set; }
        public string ReturnUrl { get; set; }
    }
}

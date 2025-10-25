namespace ShoraWorkManager.Models
{
    public class HomeEditSectionViewModel
    {
        public string Title { get; set; } = "";
        public string HtmlText { get; set; } = "";
        public IList<string> Photos { get; set; } = new List<string>();

        public string UpdateAction { get; set; } = "";
        public string AddPhotoAction { get; set; } = "";
        public string DeletePhotoAction { get; set; } = "";
        public string Controller { get; set; } = "Home";

        public string HtmlFieldName { get; set; } = "HtmlText";
        public string UploadFieldName { get; set; } = "photos";
    }
}

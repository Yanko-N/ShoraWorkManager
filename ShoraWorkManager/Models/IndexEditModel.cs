namespace ShoraWorkManager.Models
{
    public class IndexEditModel
    {
        public string SectionOneHtmlText { get; set; } = "";
        public List<string> SectionOnePhotos { get; set; } = new List<string>();

        public string SectionTwoHtmlText { get; set; } = "";
        public List<string> SectionTwoPhotos { get; set; } = new List<string>();

        public string SectionThreeHtmlText { get; set; } = "";
        public List<string> SectionThreePhotos { get; set; } = new List<string>();
    }
}

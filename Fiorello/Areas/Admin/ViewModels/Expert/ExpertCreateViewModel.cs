namespace Fiorello.Areas.Admin.ViewModels.Expert
{
    public class ExpertCreateViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Position { get; set; }



        public IFormFile Photo { get; set; }
    }
}

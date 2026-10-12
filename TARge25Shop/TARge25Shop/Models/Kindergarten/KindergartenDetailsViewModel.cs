namespace TARge25Shop.Models.Kindergarten
{
    internal class KindergartenDetailsViewModel
    {
        public Guid? Id { get; set; }
        public string GroupName { get; set; }
        public string KindergartenName { get; set; }
        public string TeacherName { get; set; }
        public int ChildrenCount { get; set; }

        public List<KindergartenImageViewModel> Image { get; set; }
            = new List<KindergartenImageViewModel>();

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
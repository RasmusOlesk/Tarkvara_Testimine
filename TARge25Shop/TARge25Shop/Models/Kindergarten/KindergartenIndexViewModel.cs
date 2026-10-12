namespace TARge25Shop.Models.Kindergarten
{
    public class KindergartenIndexViewModel
    {
        public Guid? Id { get; set; }
        public string GroupName { get; set; } = string.Empty;
        public string KindergartenName { get; set; } = string.Empty;
        public string TeacherName { get; set; } = string.Empty;
        public int ChildrenCount { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

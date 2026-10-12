namespace TARge25Shop.Models.Kindergarten
{
    public class KindergartenImageViewModel
    {
        public Guid ImageId { get; set; }
        public string? ImageTitle { get; set; }
        public byte[]? ImageData { get; set; }
        public string? Image { get; set; }
        public Guid? KindergartenId { get; set; }
    }
}

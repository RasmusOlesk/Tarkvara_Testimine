using TARge25SHop.Core.Dto;
using Microsoft.AspNetCore.Http;


namespace TARge25Shop.Core.Dto
{
    public class KindergartenDto
    {
        public Guid? Id { get; set; }
        public string GroupName { get; set; } = string.Empty;
        public string KindergartenName { get; set; } = string.Empty;

        public string TeacherName { get; set; } = string.Empty;
        public int ChildrenCount { get; set; }

        public List<IFormFile> Files { get; set; }
        public IEnumerable<FileToDatabaseDto> Image { get; set; }
            = new List<FileToDatabaseDto>();

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

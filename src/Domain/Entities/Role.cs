using Crm.Domain.Common;

namespace Crm.Domain.Entities
{
    public sealed class Role : BaseEntity
    {
        public required string Name { get; set; }

        public string? Description { get; set; }

        public ICollection<User> Users { get; set; } = new List<User>();
    }
}

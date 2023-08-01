namespace webapi.Entity
{
    public class BaseEntity
    {
        public int Id { get; set; }
        public int? Creator { get; set; }
        public int? Updater { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}

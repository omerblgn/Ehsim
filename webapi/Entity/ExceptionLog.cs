using System.ComponentModel.DataAnnotations;

namespace webapi.Entity.Log
{
    public class ExceptionLog
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        [StringLength(1000)]
        public string RequestUrl { get; set; }
        public DateTime Date { get; set; }
        public string Exception { get; set; }
        public string StackTrace { get; set; }
        public int HttpStatusCode { get; set; }
    }
}

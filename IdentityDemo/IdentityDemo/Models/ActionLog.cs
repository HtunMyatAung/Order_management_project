using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityDemo.Models
{
    [Table("actionlog")]
    public class ActionLog
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("user_id")]
        public string UserId { get; set; }
        [Column("UserName")]
        public string UserName { get; set; }
        [Column("ActionName")]
        public string ActionName { get; set; }
        [Column("ControllerName")]
        public string ControllerName { get; set; }
        [Column("Timestamp")]
        public DateTime Timestamp { get; set; }
        [Column("RequestData")]
        public string RequestData { get; set; }
    }
}

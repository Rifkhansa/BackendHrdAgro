using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BackendHrdAgro.Models.Database.MySql.Master
{
    [Table("login_session")]
    public partial class LoginSession
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("user_id")]
        [StringLength(21)]
        public string UserId { get; set; }

        [Column("access_token")]
        [StringLength(250)]
        public string AccessToken { get; set; }

        [Column("created_at", TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at", TypeName = "datetime")]
        public DateTime? UpdatedAt { get; set; }

        [Column("deleted_at", TypeName = "datetime")]
        public DateTime? DeletedAt { get; set; }

        [Column("test")]
        public sbyte? Test { get; set; }
    }
}

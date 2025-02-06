using System;
using System.Collections.Generic;
using BackendHrdAgro.Models.Database.MySql;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BackendHrdAgro.Models.Master
{

    public class WarehouseDB
    {
        DatabaseContext context = new DatabaseContext();
        public IEnumerable<TmThirdParty> WarehouseFindAlls()
        {
            var result = context.TmThirdParties.Where(x=> x.DeletedAt == null && x.DeletedBy == null).ToList();
            return result;
        }
    }

    [Keyless]
    public partial class WharehouseFindAll
    {
        [Required]
        [Column("id")]
        [StringLength(21)]
        [Unicode(false)]
        public string Id { get; set; }

        [Required]
        [Column("name")]
        [StringLength(50)]
        [Unicode(false)]
        public string Name { get; set; }

        [Column("created_at", TypeName = "datetime")]
        public DateTime? CreatedAt { get; set; }

        [Column("created_by")]
        [StringLength(21)]
        [Unicode(false)]
        public string CreatedBy { get; set; }
    }

}

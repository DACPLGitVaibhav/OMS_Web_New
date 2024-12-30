using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DATA.Models
{
    [Table("tbl_VariantCode")]
    public class VariantCode 
    {
        [Key]
        public int Id { get; set; }    
        public string Erp_Vcode { get; set; }
        public string Description { get; set; }
        public int Mes_Vcode { get; set; }

    }
}

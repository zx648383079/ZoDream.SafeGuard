using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.SafeGuard.DataNet
{
    public class CheckData
    {
       
        [Column("1")]
        public string Extension { get; set; } = string.Empty;

        [Column("2")]
        public string Text { get; set; } = string.Empty;

        [Column("0")]
        [ColumnName("Label")]
        public bool Label { get; set; }
    }
}

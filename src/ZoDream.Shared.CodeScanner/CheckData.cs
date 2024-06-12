using Microsoft.ML.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZoDream.Shared.CodeScanner
{
    public class CheckData
    {
       
        [Column("1")]
        public string Extension { get; set; } = string.Empty;

        [Column("2")]
        public string Text { get; set; } = string.Empty;

        [Column("0")]
        [ColumnName("Label")]
        public string Label { get; set; } = string.Empty;
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNetLinkApp.Models.SGLogData
{
    public class SGTransferLogData
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public int GroupId { get; set; }
        public string UserSeq { get; set; }
        public string LogDate { get; set; }
        public string LogDatetime { get; set; } = String.Empty;
        public string Log { get; set; } = String.Empty;
        public DateTime? Time { get; set; } = null;
    }
}

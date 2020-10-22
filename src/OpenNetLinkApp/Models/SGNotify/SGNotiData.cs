using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using OpenNetLinkApp.Models.SGSideBar;

namespace OpenNetLinkApp.Models.SGNotify
{
    public enum NOTI_TYPE : int
    {
        USER,
        SYSTEM,
        ALL,
    }

    public class SGNotiData
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long         Id { get; set; }
        public NOTI_TYPE    Type { get; set; }
        public int          GroupId { get; set; }
        public string       UserSeq { get; set; }
        public string       Seq { get; set; }
        public LSIDEBAR     CategoryId { get; set; }
        public string       Path { get; set; } = String.Empty;
        public string       IconImage { get; set; } = String.Empty;
        public string       Head { get; set; } = String.Empty;
        public string        Body { get; set; } = String.Empty;
        public DateTime? Time { get; set; } = null; 
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OpenNetLinkApp.Models.SGNotify
{
    public class SGAlarmData
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long   AlarmId { get; private set; }
        public int    GroupId { get; private set; }
        public string Path { get; private set; } = String.Empty;
        public string IconImage { get; private set; } = String.Empty;
        public string Head { get; private set; } = String.Empty;
        public string Body { get; private set; } = String.Empty;
        public DateTime? Time { get; private set; } = null; 
    }
}
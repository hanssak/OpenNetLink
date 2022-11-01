using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using OpenNetLinkApp.Models.SGSideBar;

namespace OpenNetLinkApp.Models.SGNotify
{
    public class SGReSendData
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //Primary Key
        public int RESENDID { get; set; }
        //망구분 GroupId
        public int GROUPID { get; set; }
        //유저 Seq 넘버
        public string USERSEQ { get; set; }
        //사용자 ID 
        public string CLIENTID { get; set; }
        //MID
        public string MID { get; set; }
        //파일네임
        public string HSZNAME { get; set; }
        //완료여부
        public bool ISEND { get; set; }
        //전송정보
        public object TRANSINFO { get; set; }


        
    }
}

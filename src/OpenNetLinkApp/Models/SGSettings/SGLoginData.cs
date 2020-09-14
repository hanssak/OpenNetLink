using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using OpenNetLinkApp.Models.SGSideBar;

namespace OpenNetLinkApp.Models.SGSettings
{
    /*
        1. groupID 정보 (태그명 : GROUPID)                  ->     최대길이 : 3
        2. ID  정보(태그명 : UID)                           ->     최대 길이 : 128 
        3. PW 정보(태그명 : UPW)                            ->     최대 길이 : 128 
        4. 결재라인 정보(태그명 : APPRLINE)                   ->     최대 길이 : 2048  ( 복합결재 사용 시 암호화 할 경우 길어질 것으로 판단.)
        5. 패스워드 Delay 정보(태그명 : DELAYDISPLAYPW)       ->     최대길이 : 128 (DateTime 형태의 문자열)
        6. 자동로그인 사용/해제 (태그명  : AUTOLOGINING)       ->     최대길이 : 1 ( 0 : 해제, 1: 사용)
    */
    public class SGLoginData
    {
        [Key]
        public int    GROUPID { get; set; }
        public string UID { get; set; } = String.Empty;
        public string UPW { get; set; } = String.Empty;
        public string APPRLINE { get; set; } = String.Empty;
        public string DELAYDISPLAYPW { get; set; } = String.Empty;
        public int    AUTOLOGINING { get; set; }
    }
}
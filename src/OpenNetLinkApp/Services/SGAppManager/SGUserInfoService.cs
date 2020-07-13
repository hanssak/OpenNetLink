using System;
using OpenNetLinkApp.Models.SGUserInfo;

namespace OpenNetLinkApp.Services.SGAppManager
{
#if false
    public enum SD_POLICY : int
    {
        ONLY_IN 	    = 1,
        ONLY_OUT 	    = 2,
        ALLOW_ALL	    = 3,
        DENY_ALL       	= 4,
        INHERIT_DEPT   	= 5
    }

    /// <summary>
    /// Provides methods for getting User Info that logined user.
    /// </summary>
    public interface ISGUserInfo
    {
        
    }
    internal class SGUserInfo : ISGUserInfo
    {
        /// <summary>
        /// Gets or sets the UserId of Current User.
        /// </summary>
        public string UserId { get; private set; } = String.Empty;
        public string UserName { get; private set; } = String.Empty; 
        public string UserSeq { get; private set; } = String.Empty;        
        public string DeptName { get; private set; } = String.Empty;
        public string DeptSeq { get; private set; } = String.Empty;
        public string Email { get; private set; } = String.Empty;
        public string Position { get; private set; } = String.Empty;    /* 직책 */
        public string Rank { get; private set; } = String.Empty;        /* 직위 */
        public int ManOrSteff { get; private set; } = 1;  /* 1: 팀원, 2: 팀장 */
        public int ApproveAllow { get; private set; } = 0;  /* 0:일반사용자(결재를 받아야하는직원), 1:결재자, 2:전결자 */
        public int SecApproveAllow { get; private set; } = 0;  /* 0: 일반사용자, 1: 정보보안결재권자 */
        public int AddApproveAllow { get; private set; } = 0;  /* 0: 일반사용자, 1: 다른부서 결재 가능 */ 

        public SGUserInfoAdded UserInfoAdded { get; private set; } = null;
    }
#endif
}
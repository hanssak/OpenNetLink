using System;

namespace OpenNetLinkApp.Models.SGUserInfo
{
    public enum SD_POLICY : int
    {
        ONLY_IN = 1,
        ONLY_OUT = 2,
        ALLOW_ALL = 3,
        DENY_ALL = 4,
        INHERIT_DEPT = 5
    }

    /// <summary>
    /// Provides methods for getting User Info that logined user.
    /// </summary>
    public interface ISGUserInfo
    {
        string UserId { get; }
        string UserName { get; }

        string UserSeq { get; }
        string DeptName { get; }
        //string DeptSeq { get; }
        //string Email { get; }             -> 사용자의 이메일 정보는 수신받지 않음.
        string Position { get; }            /* 직책 */
        string Rank { get; }                /* 직위 */
        /// <summary>
        /// 0:일반사용자(결재를 받아야하는직원), 1:결재자, 2:전결자 */
        /// </summary>
        int ApprPos { get; }         /* 0:일반사용자(결재를 받아야하는직원), 1:결재자, 2:전결자 */

        //int SecApproveAllow { get; }      /* 0: 일반사용자, 1: 정보보안결재권자 */
        //int AddApproveAllow { get; }      /* 0: 일반사용자, 1: 다른부서 결재 가능 */ 
        string HeaderUserName { get; }      //상위 해더 부분에 유저 이름 및 대결재자 일 경우 표시

        ISGUserInfoAdded UserInfoAdded { get; }

        /// <summary>
        /// 권한 및 위치에 대한 정보 (Approve Position)
        /// </summary>
        /// <param name="isRealApprPos">true: 대결재 위임과 상관없이 사용자의 실제 권한 반환 여부</param>
        /// <param name="isAllDelegatedFromProxy">true:원결재자의 모든 권한을 위임받는 경우 / false : 원결자와 대결자 중 높은 권한을 가진 권한을 반환</param>
        /// <returns>0:일반, 1:결재자, 2:전결자</returns>
        public int GetUserApprPos(bool isRealApprPos, bool isAllDelegatedFromProxy);
    }
    internal class SGUserInfo : ISGUserInfo
    {
        /// <summary>
        /// Gets or sets the UserId of Current User.
        /// </summary>
        public string UserId { get; set; } = String.Empty;
        public string UserName { get; set; } = String.Empty;
        public string UserSeq { get; set; } = String.Empty;
        public string DeptName { get; set; } = String.Empty;
        //public string DeptSeq { get; private set; } = String.Empty;
        //public string Email { get; set; } = String.Empty;         -> 사용자의 이메일 정보는 수신받지 않음.
        public string Position { get; set; } = String.Empty;    /* 직책 */
        public string Rank { get; set; } = String.Empty;        /* 직위 */

        /// <summary>
        ///  0:일반사용자(결재를 받아야하는직원), 1:결재자, 2:전결자
        /// </summary>
        public int ApprPos { get; set; } = 0;  /* 0:일반사용자(결재를 받아야하는직원), 1:결재자, 2:전결자 */

        //public int SecApproveAllow { get; private set; } = 0;  /* 0: 일반사용자, 1: 정보보안결재권자 */
        //public int AddApproveAllow { get; private set; } = 0;  /* 0: 일반사용자, 1: 다른부서 결재 가능 */ 

        public string HeaderUserName { get; set; } = String.Empty; //상위 해더 부분에 유저 이름 및 대결재자 일 경우 표시
        public ISGUserInfoAdded UserInfoAdded { get; set; } = null;

        /// <summary>
        /// 권한 및 위치에 대한 정보 (Approve Position)
        /// </summary>
        /// <param name="isRealApprPos">true: 대결재 위임과 상관없이 사용자의 실제 권한 반환 여부</param>
        /// <param name="isAllDelegatedFromProxy">true:원결재자의 모든 권한을 위임받는 경우 / false : 원결자와 대결자 중 높은 권한을 가진 권한을 반환</param>
        /// <returns>0:일반, 1:결재자, 2:전결자</returns>
        public int GetUserApprPos(bool isRealApprPos, bool isAllDelegatedFromProxy)
        {
            if (isRealApprPos)
                return ApprPos;

            if (!isAllDelegatedFromProxy)       //대결자에게 위임 받지 않는 경우, 실제 권한 반환
                return ApprPos;

            //대결자에게 위임받는 경우, 실제 권한과 대결 권한 중 높은 권한 반환
            if (ApprPos >= UserInfoAdded.SFMRight)
                return ApprPos;
            else
                return UserInfoAdded.SFMRight;
        }
    }
}

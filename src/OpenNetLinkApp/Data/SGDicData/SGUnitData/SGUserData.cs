using System;
using System.Collections.Generic;
using System.Text;
using HsNetWorkSGData;
using HsNetWorkSG;

namespace OpenNetLinkApp.Data.SGDicData.SGUnitData
{
    public class SGUserData : SGData
    {
        public SGUserData()
        {

        }
        ~SGUserData()
        {

        }
        override public void Copy(HsNetWork hs, SGData data)
        {
            SetSessionKey(hs.GetSeedKey());
            m_DicTagData = new Dictionary<string, string>(data.m_DicTagData);
            m_DicRecordData = new List<Dictionary<int, string>>(data.m_DicRecordData);
        }

        /**
		*@biref 사용자의 팀 이름을 반환한다..
		*@return 사용자 팀 이름
		*/
        public string GetTeamName()
        {
            string strData = GetUserTagData("TEAMNAME");
            return strData;
        }
        /**
		*@biref 사용자의 결재권한 여부를 반환한다.
		*@return 0 : 일반사용자, 1: 결재권자, 2: 전결자
		*/
        public int GetUserApprpos()
        {
            string strData = GetUserTagData("APPRPOS");
            int nApprPos = Convert.ToInt32(strData);
            return nApprPos;
        }
        /**
        *@biref 사용자의 보안결재권한 여부를 반환한다.
        *@return true : 보안결재자
        */
        public bool GetUserPrivacyApprPos()
        {
            string strData = GetUserTagData("DLPAPPRPOS");
            int iValue = Convert.ToInt32(strData);
            if (iValue == 1)
                return true;
            return false;
        }
        /**
        *@biref 사용자의 이름을 반환한다.
        *@return 사용자 이름
        */
        public string GetUserName()
        {
            string strData = GetUserTagData("USERNAME");
            return strData;
        }
        /**
        *@biref 사용자의 Sequence 정보를 반환한다.
        *@return 사용자 Sequence
        */
        public string GetUserSequence()
        {
            string strData = GetUserTagData("SEQ");
            return strData;
        }

        /**
        *@biref 사용자의 Position 정보를 반환한다.
        *@return 사용자 Position
        */
        public string GetUserPosition()
        {
            string strData = GetUserTagData("POSITION");
            return strData;
        }
        /**
        *@biref 사용자의 Rank 정보를 반환한다.
        *@return 사용자 Rank
        */
        public string GetRank()
        {
            string strData = GetUserTagData("RANK");
            return strData;
        }

        /**
        *@biref 사용자가 팀원인지 팀장인지 정보를 반환한다.
        *@return 사용자가 팀원인지 팀장인지 여부 (1:팀원, 2:팀장)
        */
        public int GetPartOwner()
        {
            string strData = GetUserTagData("PARTOWNER");
            int iValue = Convert.ToInt32(strData);
            return iValue;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using HsNetWorkSG;
using System.Threading;
using SGData;

namespace OpenNetLinkApp.Services
{
    class CmdRecvParser
    {

        private SGCmdDef sGCmd;

        public SGDicData m_sgLoginData;
        public SGDicData m_sgUserInfo;
        public SGDicData m_sgSystemEnv;
        public SGDicData m_sgTransSearchData;
        public SGDicData m_sgApprSearchData;
        public SGDicData m_sgDetailSearchData;
        public SGDicData m_sgApproveLine;
        public SGDicData m_sgData;

        public CmdRecvParser()
        {
            sGCmd = new SGCmdDef();
            m_sgLoginData = new SGDicData();
            m_sgUserInfo = new SGDicData();
            m_sgSystemEnv = new SGDicData();
            m_sgTransSearchData = new SGDicData();
            m_sgApprSearchData = new SGDicData();
            m_sgDetailSearchData = new SGDicData();
            m_sgApproveLine = new SGDicData();
            m_sgData = new SGDicData();
        }

        ~CmdRecvParser()
        {

        }

        public void Noti(object obj, SGEventArgs e)
        {
            RecvSGCmd(e);
        }

        public SGDicData GetSGData(string strCmd)
        {
            SGDicData dicData;

            int CmdID = sGCmd.RecvCmdtoID(strCmd);
            switch(CmdID)
            {
                case 1001:                                          // user bind(connect) 인증 응답
                    dicData = m_sgLoginData;
                    break;

                case 1030:                                          // 사용자 정보 응답.
                    dicData = m_sgUserInfo;
                    break;

                case 1040:                                          // 결재관리 조회 리스트 요청 응답.
                    dicData = m_sgApprSearchData;
                    break;

                case 1041:                                          // 환경변수 시스템 설정값 요청 응답.
                    dicData = m_sgSystemEnv;
                    break;

                case 1042:                                          // 상세보기 조회 요청 응답.
                    dicData = m_sgDetailSearchData;
                    break;

                case 1043:                                          // 전송관리 조회 리스트 요청 응답.
                    dicData = m_sgTransSearchData;
                    break;

                case 1075:                                          // 사용자 기본결재정보조회.
                    dicData = m_sgApproveLine;
                    break;

                default:
                    dicData = m_sgData;
                    break;
            }

            return dicData;
        }
       
        public void RecvSGCmd(SGEventArgs args)
        {
            int CmdID = sGCmd.RecvCmdtoID(args.strCmd);
            switch (CmdID)
            {
                case 1000:                                                  // SEEDKEY_ACK : seed key 요청 응답
                    break;

                case 1001:                                                  // BIND_ACK : user bind(connect) 인증 응답
                    RecvConnect(args);
                    break;

                case 1002:                                                  // LOGOUT : LogOut 응답.
                    break;

                case 1003:                                                  // LINK_ACK : link check 응답
                    break;

                case 1004:                                                  // CHANGEPASSWD_ACK : 비밀번호 변경 요청 응답.
                    break;

                case 1005:                                                  // DEPTINFO_ACK : 부서정보 조회 응답.
                    break;

                case 1006:                                                  // USERINFO_ACK : 사용자 정보 조회 응답.
                    break;

                case 1007:                                                  // USERINFO2_ACK : 사용자 정보 조회 응답(팀장여부 추가).
                    break;

                case 1030:                                                  // USERINFOEX : 사용자 정보 응답.
                    RecvParser(m_sgUserInfo,args);
                    break;

                case 1040:                                                  // 결재관리 조회 리스트 요청 응답.
                    RecvParser(m_sgApprSearchData,args);
                    break;

                case 1041:                                                  // 환경변수 시스템 설정값 요청 응답.
                    RecvParser(m_sgSystemEnv, args);
                    break;

                case 1042:                                                  // 상세보기 조회 요청 응답.
                    RecvParser(m_sgDetailSearchData, args);
                    break;

                case 1043:                                                  // 전송관리 조회 리스트 요청 응답.
                    RecvParser(m_sgTransSearchData, args);
                    break;

                case 1075:                                                  // 사용자 기본결재정보조회.
                    RecvParser(m_sgApproveLine, args);
                    break;
            }


            CancellationTokenSource ct = null;
            ct = CmdSendParser.m_DicCt[CmdID];
            if (ct != null)
            {
                ct.Cancel();
                CmdSendParser.m_DicCt[CmdID] = null;
            }
        }

        public string GetUserID()
        {
            return m_sgLoginData.GetTagData("CLIENTID");
        }

        public int GetResult(Dictionary<string, string> dic)
        {
            string strResult = dic["RESULT"];
            int iResult = Convert.ToInt32(strResult);
            return iResult;
        }

        public void RecvConnect(SGEventArgs args)
        {
            Dictionary<string, string> data = args.MsgRecode;
            int ret = GetResult(data);
            if (ret == 0)
            {
                foreach (var item in data)
                {
                    m_sgLoginData.SetTagData(item.Key, item.Value);
                }

                CmdSendParser sendParser = new CmdSendParser();                         // 추후 서비스로 등록할 것이므로 수정 필요.
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic["CLIENTID"] = GetUserID();
                sendParser.RequestCmd("CMD_STR_USERINFOEX", dic);
                sendParser.RequestCmd("CMD_STR_APPROVELINE", dic);
                sendParser.RequestCmd("CMD_STR_APPROVELINE", dic);
                sendParser.RequestCmd("CMD_STR_APPRINSTCUR", dic);
                sendParser.RequestCmd("CMD_STR_SYSTEMRUNENV", dic);
                sendParser.RequestCmd("CMD_STR_URLLIST", dic);
            }
            else
            {

            }
        }

        public void RecvParser(SGDicData dic, SGEventArgs args)
        {
            dic.Clear();
            Dictionary<string, string> data = args.MsgRecode;
            int ret = GetResult(data);
            if (ret == 0)
            {
                foreach (var item in data)
                {
                    dic.SetTagData(item.Key, item.Value);
                }
            }
            else
            {

            }
        }
        
        
    }
}

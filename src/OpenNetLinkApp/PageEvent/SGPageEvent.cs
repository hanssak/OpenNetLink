using System;
using System.Collections.Generic;
using System.Text;

namespace OpenNetLinkApp.PageEvent
{
    public class PageEventArgs : EventArgs
    {
        public string strMsg { get; set; }
        public int result { get; set; }
    }
    public delegate void SideBarEvent(int groupid, PageEventArgs e);
    // 로그인
    public delegate void LoginEvent(int groupid, PageEventArgs e);

    // 전송관리 
    public delegate void TransSearchEvent(int groupid, PageEventArgs e);
    public delegate void TransCancelEvent(int groupid, PageEventArgs e);

    // 전송관리 상세보기
    public delegate void TransDetailCancelEvent(int groupid, PageEventArgs e);

    // 결재관리
    public delegate void ApprSearchEvent(int groupid, PageEventArgs e);
    public delegate void ApprApproveEvent(int groupid, PageEventArgs e);
    public delegate void ApprRejectEvent(int groupid, PageEventArgs e);

    // 결재관리 상세보기
    public delegate void ApprDetailApproveEvent(int groupid, PageEventArgs e);
    public delegate void ApprDetailRejectEvent(int groupid, PageEventArgs e);
    public delegate void ApprDetailFilePrevEvent(int groupid, PageEventArgs e);
}

namespace OpenNetLinkApp.PageEvent
{
    public class SGPageEvent
    {
       // public event LoginEvent LoginResult_Event;
        public Dictionary<int, LoginEvent> DicLoginEvent = new Dictionary<int, LoginEvent>(); // 로그인

        public Dictionary<int, TransSearchEvent> DicTransSearchEvent = new Dictionary<int, TransSearchEvent>(); // 전송관리 조회
        public Dictionary<int, TransCancelEvent> DicTransCancelEvent = new Dictionary<int, TransCancelEvent>(); // 전송관리 전송취소

        public Dictionary<int, TransDetailCancelEvent> DicTransDetailCancelEvent = new Dictionary<int, TransDetailCancelEvent>(); // 전송상세보기 전송취소.

        public Dictionary<int, ApprSearchEvent> DicApprSearchEvent = new Dictionary<int, ApprSearchEvent>();         // 결재관리 조회
        public Dictionary<int, ApprApproveEvent> DicApprApproveEvent = new Dictionary<int, ApprApproveEvent>();      // 결재관리 승인 
        public Dictionary<int, ApprRejectEvent> DicApprRejectEvent = new Dictionary<int, ApprRejectEvent>();         // 결재관리 반려

        public Dictionary<int, ApprDetailApproveEvent> DicApprDetailApproveEvent = new Dictionary<int, ApprDetailApproveEvent>();       // 결재상세보기 승인
        public Dictionary<int, ApprDetailRejectEvent> DicApprDetailRejectEvent = new Dictionary<int, ApprDetailRejectEvent>();          // 결재상세보기 반려
        public Dictionary<int, ApprDetailFilePrevEvent> DicApprDetailFilePrevEvent = new Dictionary<int, ApprDetailFilePrevEvent>();              // 결재상세보기 미리보기
        public SGPageEvent()
        {

        }
        ~SGPageEvent()
        {

        }

        public void SetLoginEventAdd(int groupid, LoginEvent e)
        {
            DicLoginEvent[groupid] = e;
        }
        public LoginEvent GetLoginEvent(int groupid)
        {
            LoginEvent e = null;
            if (DicLoginEvent.TryGetValue(groupid,out e) == true)
                e = DicLoginEvent[groupid];
            return e;
        }

        public void SetTransSearchEventAdd(int groupid, TransSearchEvent e)
        {
            DicTransSearchEvent[groupid] = e;
        }
        public TransSearchEvent GetTransSearchEvent(int groupid)
        {
            TransSearchEvent e = null;
            if (DicTransSearchEvent.TryGetValue(groupid, out e) == true)
                e = DicTransSearchEvent[groupid];
            return e;
        }

        public void SetTransCancelEventAdd(int groupid, TransCancelEvent e)
        {
            DicTransCancelEvent[groupid] = e;
        }
        public TransCancelEvent GetTransCancelEvent(int groupid)
        {
            TransCancelEvent e = null;
            if (DicTransCancelEvent.TryGetValue(groupid, out e) == true)
                e = DicTransCancelEvent[groupid];
            return e;
        }

        public void SetTransDetailCancelEventAdd(int groupid, TransDetailCancelEvent e)
        {
            DicTransDetailCancelEvent[groupid] = e;
        }
        public TransDetailCancelEvent GetTransDetailCancelEvent(int groupid)
        {
            TransDetailCancelEvent e = null;
            if (DicTransDetailCancelEvent.TryGetValue(groupid, out e) == true)
                e = DicTransDetailCancelEvent[groupid];
            return e;
        }

        public void SetApprSearchEventAdd(int groupid, ApprSearchEvent e)
        {
            DicApprSearchEvent[groupid] = e;
        }
        public ApprSearchEvent GetApprSearchEvent(int groupid)
        {
            ApprSearchEvent e = null;
            if (DicApprSearchEvent.TryGetValue(groupid, out e) == true)
                e = DicApprSearchEvent[groupid];
            return e;
        }

        public void SetApprApproveEvent(int groupid, ApprApproveEvent e)
        {
            DicApprApproveEvent[groupid] = e;
        }
        public ApprApproveEvent GetApprApproveEvent(int groupid)
        {
            ApprApproveEvent e = null;
            if (DicApprApproveEvent.TryGetValue(groupid, out e) == true)
                e = DicApprApproveEvent[groupid];
            return e;
        }

        public void SetApprRejectEvent(int groupid, ApprRejectEvent e)
        {
            DicApprRejectEvent[groupid] = e;
        }
        public ApprRejectEvent GetApprRejectEvent(int groupid)
        {
            ApprRejectEvent e = null;
            if (DicApprRejectEvent.TryGetValue(groupid, out e) == true)
                e = DicApprRejectEvent[groupid];
            return e;
        }

        public void SetApprDetailApproveEvent(int groupid, ApprDetailApproveEvent e)
        {
            DicApprDetailApproveEvent[groupid] = e;
        }
        public ApprDetailApproveEvent GetApprDetailApproveEvent(int groupid)
        {
            ApprDetailApproveEvent e = null;
            if (DicApprDetailApproveEvent.TryGetValue(groupid, out e) == true)
                e = DicApprDetailApproveEvent[groupid];
            return e;
        }

        public void SetApprDetailRejectEvent(int groupid, ApprDetailRejectEvent e)
        {
            DicApprDetailRejectEvent[groupid] = e;
        }
        public ApprDetailRejectEvent GetApprDetailRejectEvent(int groupid)
        {
            ApprDetailRejectEvent e = null;
            if (DicApprDetailRejectEvent.TryGetValue(groupid, out e) == true)
                e = DicApprDetailRejectEvent[groupid];
            return e;
        }

        public void SetApprDetailFilePrevEvent(int groupid, ApprDetailFilePrevEvent e)
        {
            DicApprDetailFilePrevEvent[groupid] = e;
        }
        public ApprDetailFilePrevEvent GetApprDetailFilePrevEvent(int groupid)
        {
            ApprDetailFilePrevEvent e = null;
            if (DicApprDetailFilePrevEvent.TryGetValue(groupid, out e) == true)
                e = DicApprDetailFilePrevEvent[groupid];
            return e;
        }
    }
}

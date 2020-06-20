using System;
using System.Collections.Generic;
using System.Text;

namespace OpenNetLinkApp.Components.Grid
{
    public class ApproveData
    {
        public ApproveData(string title, string register, bool isapprove, string approver)
        {
            Title = title;
            RegDate = DateTime.Now;
            Register = register;
            IsApprove = isapprove;
            ApproveDate = DateTime.Now;
            Approver = approver;
        }

        public string Title { get; set; }
        public DateTime RegDate { get; set; }
        public string Register { get; set; }
        public bool IsApprove { get; set; }
        public DateTime ApproveDate { get; set; }
        public string Approver { get; set; }
    }
}

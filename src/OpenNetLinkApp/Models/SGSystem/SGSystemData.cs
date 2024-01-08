using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNetLinkApp.Models.SGSystem
{
    public class SGSystemData
    {
        /// <summary>
        /// OpenNetLink 시작 인자
        /// </summary>
        public string[] StartArg { get; set; }

        /// <summary>
        /// 'NAC' 파일 Value 값(실행 시점에 저장)
        /// </summary>
        public string NacFileValue { get; set; }
    }
}

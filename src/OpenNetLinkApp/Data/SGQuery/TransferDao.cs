using OpenNetLinkApp.Services;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using static OpenNetLinkApp.Common.Enums;

namespace OpenNetLinkApp.Data.SGQuery
{
    class TransferDao
    {
		/// <summary>
		/// 클립보드 데이터 타입에 따른 조건절
		/// </summary>
		/// <param name="strArrClipDataType">클립보드 데이터타입</param>
		/// <returns>조건절 쿼리문</returns>
		public string GetClipDataSearch(string[] strArrClipDataType)
		{
			StringBuilder sb = new StringBuilder();

			//sb.Append("  AND a.data_type IN ('1', '2')");
			if (strArrClipDataType == null || strArrClipDataType.Length < 1)
				sb.Append("  AND a.data_type!='0'");
			else
			{
				sb.Append("  AND a.data_type IN (");
				int nIdx = 0;
				for (; nIdx < strArrClipDataType.Length; nIdx++)
				{
					sb.Append("'");
					sb.Append(strArrClipDataType[nIdx]);
					sb.Append("'");
					if (nIdx < strArrClipDataType.Length - 1)
						sb.Append(",");
				}
				sb.Append(")");
			}

			return sb.ToString();
		}

		/// <summary>
		/// 쿼리 결과 가져오기
		/// </summary>
		/// <param name="tParam">쿼리 매개변수</param>
		/// <param name="bNoClipboard">(true : 일반파일  false : 클립보드)</param>
		/// <param name="strArrClipDataType">클립보드 타입</param>
		/// <returns>쿼리문</returns>
		public string List(TransferParam tParam, bool bNoClipboard, string[] strArrClipDataType = null)
		{
			//단일망, 다중망일 경우 보낸 곳과 받는 곳의 정보를 받아 결재 리스트를 보여준다.
			string dataTypeCheckString = String.Empty;
			if (bNoClipboard)
				dataTypeCheckString = "AND A.DATA_TYPE=0";
			else
				dataTypeCheckString = GetClipDataSearch(strArrClipDataType);

			string dateTimeCheckString = String.Empty;
			if (!(tParam.SearchFromDay.Equals("")) && (tParam.SearchToDay.Equals("")))
			{
				dateTimeCheckString = $"AND A.REQUEST_TIME >= '{tParam.SearchFromDay}'";
			}
			else if ((tParam.SearchFromDay.Equals("")) && !(tParam.SearchToDay.Equals("")))
			{
				dateTimeCheckString = $"AND A.REQUEST_TIME <= '{tParam.SearchToDay}'";
			}
			else if (!(tParam.SearchFromDay.Equals("")) && !(tParam.SearchToDay.Equals("")))
			{
				dateTimeCheckString = $"AND A.REQUEST_TIME >= '{tParam.SearchFromDay}' AND A.REQUEST_TIME <= '{tParam.SearchToDay}'";
			}

			Dictionary<string, string> param = SQLXmlService.Instanse.ConvertClassToDictionary(tParam);
			param.Add("DataTypeCheckString", dataTypeCheckString);
			param.Add("DateTimeCheckString", dateTimeCheckString);

			return SQLXmlService.Instanse.GetSqlQuery("TransferDaoList", param);
        }

		/// <summary>
		/// 쿼리 결과 행 수 가져오기
		/// </summary>
		/// <param name="tParam">쿼리 매개변수</param>
		/// <param name="bNoClipboard">(true : 일반파일  false : 클립보드)</param>
		/// <param name="strArrClipDataType">클립보드</param>
		/// <returns>쿼리문</returns>
		public string TotalCount(TransferParam tParam, bool bNoClipboard, string[] strArrClipDataType = null)
		{
            //단일망, 다중망일 경우 보낸 곳과 받는 곳의 정보를 받아 결재 리스트를 보여준다.
            
			string dataTypeCheckString = String.Empty;
			if (bNoClipboard)
				dataTypeCheckString = "AND A.DATA_TYPE=0";
			else
				dataTypeCheckString = GetClipDataSearch(strArrClipDataType);

			string dateTimeCheckString = String.Empty;
			if (!(tParam.SearchFromDay.Equals("")) && (tParam.SearchToDay.Equals("")))
			{
				dateTimeCheckString = $"AND A.REQUEST_TIME >= '{tParam.SearchFromDay}'";
			}
			else if ((tParam.SearchFromDay.Equals("")) && !(tParam.SearchToDay.Equals("")))
			{
				dateTimeCheckString = $"AND A.REQUEST_TIME <= '{tParam.SearchToDay}'";
			}
			else if (!(tParam.SearchFromDay.Equals("")) && !(tParam.SearchToDay.Equals("")))
			{
				dateTimeCheckString = $"AND A.REQUEST_TIME >= '{tParam.SearchFromDay}' AND A.REQUEST_TIME <= '{tParam.SearchToDay}'";
			}

            Dictionary<string, string> param = SQLXmlService.Instanse.ConvertClassToDictionary(tParam);
            param.Add("DataTypeCheckString", dataTypeCheckString);
            param.Add("DateTimeCheckString", dateTimeCheckString);

            return SQLXmlService.Instanse.GetSqlQuery("TransferDaoCount", param);

		}
    }
}

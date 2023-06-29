using HsNetWorkSG;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace OpenNetLinkApp.Data.SGQuery
{
    class SQLMapping
    {
        /// <summary>
        /// 쿼리문 가져오기
        /// </summary>
        /// <param name="xmlContent"></param>
        /// <param name="id"></param>
        /// <param name="param"></param>
        /// <param name="sb"></param>
        public static void GetSqlQuery(ref byte[] xmlContent, string id, Dictionary<string, string> param, ref StringBuilder sb)
        {

            byte[] dXmlContent = null;
            SGCrypto.AESDecrypt256WithDEK(xmlContent, ref dXmlContent);

            XmlDocument xdoc = new XmlDocument();

            try
            {
                using (MemoryStream ms = new MemoryStream(dXmlContent))
                {
                    xdoc.Load(ms);

                    XmlNodeList nodes = xdoc.SelectNodes("statements/statement");

                    foreach (XmlNode node in nodes)
                    {
                        if (node.Attributes["id"].Value == id)
                        {
                            GetRecursiveNode(node.FirstChild, param, ref sb);
                            break;
                        }
                    }

                    foreach (string str in param.Keys)
                    {
                        string temp = $"#{str}#";
                        sb.Replace(temp, param[str]);
                    }

                    //ms.Position = 0;
                    //for(int i = 0; i < ms.Length; i++)
                    //{
                    //    ms.WriteByte(0);
                    //}
                }
            }
            catch(Exception ex)
            {

            }
            finally
            {
                if(dXmlContent != null)
                    dXmlContent.hsClear(3);
                xdoc = null;
            }
        }
        /// <summary>
        /// xml에서 자식 노드를 돌면서 sql 문을 sb 버퍼에 삽입
        /// </summary>
        /// <param name="node"></param>
        /// <param name="param"></param>
        /// <param name="sb"></param>
        public static void GetRecursiveNode(XmlNode node, Dictionary<string, string> param, ref StringBuilder sb)
        {
            if (node.Name == "if")
            {
                GetIFElement(node, param, ref sb);
                if (node.NextSibling != null)
                {
                    if (node.NextSibling.Name == "else")
                    {
                        if (node.NextSibling.NextSibling != null)
                            GetRecursiveNode(node.NextSibling.NextSibling, param, ref sb);
                    }
                    else
                        GetRecursiveNode(node.NextSibling, param, ref sb);
                }
            }
            else if (node.Name == "case")
            {
                GetCaseElement(node, param, ref sb);
                if (node.NextSibling != null)
                    GetRecursiveNode(node.NextSibling, param, ref sb);
            }
            else
            {
                sb.Append(node.InnerText);
                if (node.NextSibling != null)
                    GetRecursiveNode(node.NextSibling, param, ref sb);
            }
        }
        /// <summary>
        /// IF Element 조건식 찾아서 처리
        /// </summary>
        /// <param name="node"></param>
        /// <param name="param"></param>
        /// <param name="sb"></param>
        public static void GetIFElement(XmlNode node, Dictionary<string, string> param, ref StringBuilder sb)
        {

            string terms = node.Attributes["test"].Value;
            bool subTest = GetCheckTerm(terms, param);
            if (subTest)
            {

                GetRecursiveNode(node.FirstChild, param, ref sb);

            }
            else
            {
                if (node.NextSibling != null)
                {
                    if (node.Name == "else")
                    {
                        GetRecursiveNode(node.NextSibling.FirstChild, param, ref sb);
                    }
                }
            }
        }
        /// <summary>
        /// CASE Element 조건식 찾아서 처리
        /// </summary>
        /// <param name="node"></param>
        /// <param name="param"></param>
        /// <param name="sb"></param>
        public static void GetCaseElement(XmlNode node, Dictionary<string, string> param, ref StringBuilder sb)
        {
            XmlNode childNode = node.FirstChild;

            do
            {
                string terms = childNode.Attributes["test"].Value;
                bool checkTest = GetCheckTerm(terms, param);

                if (checkTest)
                {
                    sb.Append(childNode.InnerText);
                    break;
                }
                else
                {
                    if (childNode.NextSibling != null)
                        childNode = childNode.NextSibling;
                    else
                        break;
                }

            } while (childNode.NextSibling != null);
        }
        /// <summary>
        /// 조건식 처리
        /// </summary>
        /// <param name="term"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static bool GetCheckTerm(string term, Dictionary<string, string> param)
        {
            bool result = false;

            List<string> splitTerm = GetSplitTerm(term);

            int count = splitTerm.Count() / 4;
            for (int i = 0; i < count + 1; i++)
            {
                string[] terms = new string[3];
                terms[0] = splitTerm[i * 4];
                terms[1] = splitTerm[i * 4 + 1];
                terms[2] = splitTerm[i * 4 + 2];

                bool subResult = GetCheckSubTerm(terms, param);

                if (i == 0)
                {
                    result = subResult;
                }
                else
                {
                    switch (splitTerm[i * 4 - 1])
                    {
                        case "AND":
                            result = subResult && result;
                            break;
                        case "OR":
                            result = subResult || result;
                            break;
                    }
                }
            }

            return result;
        }
        /// <summary>
        /// 조건식을 Splite
        /// </summary>
        /// <param name="term"></param>
        /// <returns></returns>
        public static List<string> GetSplitTerm(string term)
        {
            List<string> splitList = new List<string>();
            string temp = "";
            bool firstSingleQuote = false;
            for (int i = 0; i < term.Length; i++)
            {
                if (term[i] == '\'')
                {
                    if (!firstSingleQuote)
                        firstSingleQuote = !firstSingleQuote;
                    else
                    {
                        firstSingleQuote = !firstSingleQuote;
                        splitList.Add(temp);

                        temp = "";
                    }

                }
                else
                {
                    if (firstSingleQuote)
                    {
                        temp += term[i];
                    }
                    else
                    {
                        if (term[i] != ' ')
                            temp += term[i];
                        else
                        {
                            if (temp != "")
                                splitList.Add(temp);
                            temp = "";
                        }
                    }
                }
            }

            return splitList;
        }
        /// <summary>
        /// 조건식을 3를 한 묶음으로 생각하고 처리
        /// </summary>
        /// <param name="term"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static bool GetCheckSubTerm(string[] term, Dictionary<string, string> param)
        {
            bool result = false;
            switch (term[1])
            {
                case "==":
                    if (param[term[0].Trim()] == term[2].Trim())
                        result = true;
                    break;
                case ">=":
                    if (Convert.ToDouble((param[term[0]])) >= Convert.ToDouble(term[2]))
                        result = true;
                    break;
                case "<=":
                    if (Convert.ToDouble((param[term[0]])) <= Convert.ToDouble(term[2]))
                        result = true;
                    break;
                case ">":
                    if (Convert.ToDouble((param[term[0]])) > Convert.ToDouble(term[2]))
                        result = true;
                    break;
                case "<":
                    if (Convert.ToDouble((param[term[0]])) < Convert.ToDouble(term[2]))
                        result = true;
                    break;
                case "!=":
                    if (param[term[0]] != term[2])
                        result = true;
                    break;
            }

            return result;

        }
    }
}

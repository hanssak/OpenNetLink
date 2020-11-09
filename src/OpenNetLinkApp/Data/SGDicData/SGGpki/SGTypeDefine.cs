using System;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore;

namespace OpenNetLinkApp.Data.SGDicData.SGGpki
{
    [StructLayout(LayoutKind.Sequential,CharSet = CharSet.Ansi)]
    public struct BINSTR
    {
        public byte[] pData;                   // 데이터의 포인터
        public int nLen;                       // 데이터의 길이

        /*
        public BINSTR()
        {
            pData = new byte[256];
            nLen = 0;
        }
        public BINSTR(int len)
        {
            pData = new byte[len];
            nLen = len;
        }
        ~BINSTR()
        {
                
        }
        */
    };
    public class PKCS11_TOKEN_INFO
    {
        Byte[] ucLabel = null;
        Byte[] ucModel = null;
        UInt64 ulTotalPublicMemory;
        UInt64 ulFreePublicMemory;
        UInt64 ulTotalPrivateMemory;
        UInt64 ulFreePrivateMemory;
        public PKCS11_TOKEN_INFO()
        {
            ucLabel = new Byte[33];
            ucModel = new Byte[17];
        }
        ~PKCS11_TOKEN_INFO()
        {

        }

    }

    public enum eGpkiAPIOPTType
    {
        API_OPT_RSA_ENC_V15 = 0x0001,	                    /* RSA V1.5의 암.복호화 수행 */
        API_OPT_RSA_ENC_V20 = 0x0002,	                    /* RSA V2.0의 암.복호화 수행 (기본값) */
        API_OPT_PADDING_TYPE_NONE = 0x0010,	                /* 대칭키 암.복호화 수행시 패딩하지 않음 */
        API_OPT_PADDING_TYPE_PKCS5 = 0x0020,	            /* 대칭키 암.복호화 수행시 PKCS5 패딩 수행 (기본값) */
        API_OPT_CMS_NO_CONTENT_INFO = 0x0100,	            /* CMS 모듈에서 암호 메시지 생성 시, ContentInfo 포함시키지 않음 (기본값) */
        API_OPT_CMS_CONTENT_INFO = 0x0200,	                /* CMS 모듈에서 암호 메시지 생성 시, ContentInfo 포함 시킴*/
        API_OPT_CMS_SET_CAPUBS = 0x0400                     /* 서명 메시지 생성 시, 서명자의 상위 인증서 목록(CaPubs)을 포함시킴 */
    };
    public enum eGpkiKeyType
    {
        KEY_TYPE_PRIVATE = 0x01,                            /* 개인키 */
        KEY_TYPE_PUBLIC = 0x02	                            /* 공개키 */
    };
    public enum eGpkiSymAlgType
    {
        SYM_ALG_DES_CBC = 0x10,                             /* DES CBC */
        SYM_ALG_3DES_CBC = 0x20,                            /* 3DES CBC */
        SYM_ALG_SEED_CBC = 0x30,                            /* SEED CBC */
        SYM_ALG_NEAT_CBC = 0x40,                            /* NEAT CBC */
        SYM_ALG_ARIA_CBC = 0x50,                            /* ARIA CBC */
        SYM_ALG_NES_CBC = 0x60	                            /* NES CBC */
    };
    public enum eGpkiHashAlgType
    {
        HASH_ALG_SHA1 = 0x01,                               /* SAH1 */
        HASH_ALG_MD5 = 0x02,                                /* MD5 */
        HASH_ALG_HAS160 = 0x03,                             /* HAS160 */
        HASH_ALG_SHA256 = 0x04	                            /* SHA256 */
    };
    public enum eGpkiMacAlgType
    {
        MAC_ALG_SHA1_HMAC = 0x01,                           /* SAH1 HMAC */
        MAC_ALG_MD5_HMAC = 0x02,                            /* MD5 HMAC */
        MAC_ALG_SHA256_HMAC = 0x03	                        /* SHA256 HMAC */
    };
    public enum eGpkiLdapDataType
    {
        LDAP_DATA_CA_CERT = 0x01,                           /* CA 인증서 */
        LDAP_DATA_SIGN_CERT = 0x02,                         /* 사용자 서명용 인증서 */
        LDAP_DATA_KM_CERT = 0x03,                           /* 사용자 암호화용 인증서 */
        LDAP_DATA_ARL = 0x05,                               /* CA 인증서 폐지 목록 */
        LDAP_DATA_CRL = 0x06,                               /* 사용자 인증서 폐지 목록 */
        LDAP_DATA_DELTA_CRL = 0x07,                         /* Delta 인증서 폐지 목록 */
        LDAP_DATA_CTL = 0x08,                               /* 인증서 신뢰 목록 */
        LDAP_DATA_GPKI_WCERT = 0x09	                        /* 행정 무선 인증체계의 인증서 */
    };
    public enum eGpkiMediaType
    {
        MEDIA_TYPE_FILE_PATH = 0x01,                        /* HardDisk, FloppyDisk, USBDriver, CD-KEY */
        MEDIA_TYPE_DYNAMIC = 0x02	                        /* 동적 저장매체(스마트카드 등) */
    };
    public enum eGpkiDataType
    {
        DATA_TYPE_OTHER = 0x00,                             /* 임의 위치의 데이터 */
        DATA_TYPE_GPKI_SIGN = 0x01,                         /* 행정 서명용 인증서.개인키 */
        DATA_TYPE_GPKI_KM = 0x02,                           /* 행정 암호화용 인증서.개인키 */
        DATA_TYPE_NPKI_SIGN = 0x10,                         /* 공인 서명용 인증서.개인키 */
        DATA_TYPE_NPKI_KM = 0x20                            /* 공인 암호화용 인증서.개인키 */
    };
    public enum eGpkiCertType
    {
        CERT_TYPE_SIGN = 0x01,                              /* 서명용 */
        CERT_TYPE_KM = 0x02,                                /* 암호화용 */
        CERT_TYPE_OCSP = 0x03,                              /* OCSP 서버용 */
        CERT_TYPE_TSA = 0x04                                /* TSA 서버용 */
    };
    public enum eGpkiCertVerify
    {
        CERT_VERIFY_FULL_PATH = 0x01,                       /* 전체 경로 검증 */
        CERT_VERIFY_CA_CERT = 0x02,                         /* CA 인증서만 검증 */
        CERT_VERIFY_USER_CERT = 0x04,                       /* 사용자 인증서만 검증 */
        CERT_VERIFY_STRICTLY = 0x08	                        /* 인증서 규격에 맞게 세밀하게 확인할 경우 지정함 */
    };
    public enum eGpkiCertRev
    {
        CERT_REV_CHECK_ALL = 0x01,                          /* ARL, CRL, OCSP 모두 확인 */
        CERT_REV_CHECK_ARL = 0x02,                          /* ARL 확인 */
        CERT_REV_CHECK_CRL = 0x04,                          /* CRL 확익 */
        CERT_REV_CHECK_OCSP = 0x08,                         /* OCSP 확인 */
        CERT_REV_CHECK_NONE = 0x10                          /* 폐지여부 확인하지 않음 */
    };
    public enum eGpkiCertStat
    {
        CERT_STAT_GOOD = 0x00,                              /* 인증서의 상태가 유효함 */
        CERT_STAT_REVOKED = 0x01,                           /* 인증서의 상태가 폐지됨 */
        CERT_STAT_UNKNOWN = 0x02                            /* 알 수 없는 인증서 */
    };
    public enum eGpkiRevReason
    {
        CERT_REV_REASON_UNSPECIFIED = 0x00,                 /* 특별한 사유 없음	*/
        CERT_REV_REASON_KEY_COMPROMISE = 0x01,              /* 키 파손 */
        CERT_REV_REASON_CA_COMPROMISE = 0x02,               /* 인증기관 키 파손 */
        CERT_REV_REASON_AFFILIATION_CHANGED = 0x03,         /* 소속 변경 */
        CERT_REV_REASON_SUPERSEDED = 0x04,                  /* 인증서 대체 */
        CERT_REV_REASON_CESSATION_OF_OPERATION = 0x05,      /* 권한 중지 */
        CERT_REV_REASON_CERTIFICATE_HOLD = 0x06,            /* 효력 정지 */
        CERT_REV_REASON_REMOVE_FROM_CRL = 0x08,             /* 폐지 목록에서 제외됨 */
        CERT_REV_REASON_PRIVILEGE_WITHDRAWN = 0x09,         /* 특권 철회 */
        CERT_REV_REASON_AA_COMPROMISE = 0x0A                /* 속성 인증서 인증기관 개인키 파손 */
    };
    public enum eGpkiError
	{
        GPKI_OK = 0,
        ERR_ALREADY_INITIALIZED = 1000,
        ERR_API_NOT_INITIALIZED	= 1001,
        ERR_SET_WORK_DIR = 1002,
        ERR_MEM_ALLOC = 1003,
        ERR_INSUFFICIENT_ALLOC_SIZE = 1004,
        ERR_NO_ERR_MSG = 1005,
        ERR_INVALID_INPUT = 1006,
        ERR_NOT_SUPPORTED_FUNCTION = 1007,
        ERR_TIME_OUT = 1008,
        ERR_LOAD_LICENSE = 1100,
        ERR_WRONG_LICENSE = 1101,
        ERR_LICENSE_AUTHORITY = 1102,
        ERR_LICENSE_EXPIRED	= 1103,
        ERR_SAVE_LICENSE_CACHE = 1104,
        ERR_LICENSE_CHECK_FAILED = 1105,
        ERR_LICENSE_MODIFIED = 1106,
        ERR_LICENSE_NOT_FOR_GPKIAPI	= 1107,
        ERR_GEN_LICENSE_CACHE = 1108,
        ERR_GET_SYSTEM_IP = 1109,
        ERR_GET_MAC_ADDRRESS = 1110,
        ERR_NOT_LICENSED = 1111,
        ERR_WRONG_CERT = 1200,
        ERR_LOAD_CERT = 1201,
        ERR_NOT_EXIST_FIELD	= 1202,
        ERR_EXPIRED_CERT = 1203,
        ERR_WRONG_CERTS	= 1204,
        ERR_EXCEED_INDEX = 1205,
        ERR_CERTS_INCLUDE_WRONG_CERT = 1206,
        ERR_NOT_EXIST_CRL_DP = 1207,
        ERR_WRONG_CRLDP	= 1208,
        ERR_NOT_CRL	= 1209,
        ERR_NOT_EXIST_CTL_SIGNER_CERT = 1210,
        ERR_WRONG_CRL = 1211,
        ERR_EXPIRED_CRL	= 1212,
        ERR_WRONG_CRL_VALIDITY = 1213,
        ERR_HOLDED_CERT = 1214,
        ERR_REVOKED_CERT = 1215,
        ERR_CONNECT_OCSP = 1216,
        ERR_OCSP_REQ_SEND = 1217,
        ERR_OCSP_MSG_REC = 1218,
        ERR_COMPOSE_OCSP_REQ_MSG = 1219,
        ERR_WRONG_OCSP_RES_MSG = 1220,
        ERR_OCSP_REQ_NOT_GRANTED = 1221,
        ERR_UNKNOWN_CERT = 1222,
        ERR_SAVE_CERT_PATH = 1223,
        ERR_FAIL_CONSTRUCT_PATH = 1224,
        ERR_SET_TRUST_ROOT_CERT = 1225,
        ERR_FAIL_READ_CONF_FILE = 1226,
        ERR_FAIL_READ_CTL_URL_FROM_CONF_FILE = 1227,
        ERR_FAIL_GET_CTL_FROM_LDAP = 1228,
        ERR_SAVE_CTL = 1229,
        ERR_WRONG_CTL = 1230,
        ERR_NOT_TRUST_CTL_ISSUER = 1231,
        ERR_NOT_TRUST_ROOT_CERT = 1232,
        ERR_PATH_VALIDATION = 1233,
        ERR_PATH_VALIDATION_VALIDITY = 1234,
        ERR_PATH_VALIDATION_KEY_USAGE = 1235,
        ERR_PATH_VALIDATION_BASIC_CONSTS = 1236,
        ERR_PATH_VALIDATION_NAME_CONSTS = 1237,
        ERR_PATH_VALIDATION_CERT_POLICIES = 1238,
        ERR_NEED_OCSP_INFO = 1239,
        ERR_SAVE_CRL = 1240,
        ERR_NOT_EXIST_OCSP_CERT = 1241,
        ERR_WRONG_TIME = 1242,
        ERR_FAIL_OPTAIN_CERT_PATH = 1243,
        ERR_FAIL_OPTAIN_CTL = 1244,
        ERR_NOT_CERT = 1245,
        ERR_NOT_CTL = 1246,
        ERR_NOT_CERTS = 1247,
        ERR_UPDATE_CERT = 1248,
        ERR_EXPIRED_CTL = 1249,
        ERR_WRONG_CTL_VALIDITY = 1250,
        ERR_FAIL_OPTAIN_CRL = 1251,
        ERR_WRONG_OCSP_REQ_MSG = 1252,
        ERR_NOT_EXIST_CERT_STAT_INFO = 1253,
        ERR_WRONG_CERT_STAT_INFO = 1254,
                                            
        ERR_WRONG_PRIKEY = 1300,
        ERR_WRONG_PASSWORD = 1301,
        ERR_ENCRYPT_PRIKEY = 1302,
        ERR_NOT_MATCHED_KEY_PAIR = 1303,
        ERR_WRONG_ENC_PRIKEY = 1304,
        ERR_NOT_PRIKEY = 1305,
        ERR_NOT_ENC_PRIKEY = 1306,

        ERR_READ_CERT = 1400,
        ERR_READ_PRIKEY = 1401,
        ERR_READ_FILE = 1402,
        ERR_SAVE_FILE = 1403,
        ERR_EMPTY_FILE = 1404,
        ERR_INVALID_STORAGE = 1405,
        ERR_LOAD_LIBRARY = 1406,
        ERR_ALREADY_LOADED = 1407,
        ERR_FAIL_LOAD_LIBRARY = 1408,
        ERR_FAIL_FREE_LIBRARY = 1409,
        ERR_FAIL_LOAD_FUNCTION = 1410,
        ERR_DELETE_FILE = 1411,
        ERR_SAVE_CERT = 1412,
        ERR_SAVE_PRIKEY = 1413,
        ERR_CHECK_CONNECTION = 1414,
        ERR_WRONG_PIN = 1415,
        ERR_FILE_SEEK_END = 1416,
        ERR_FILE_SEEK_BEGIN = 1417,
        ERR_FILE_SIZE = 1418,
        ERR_FILE_CREATE = 1419,
        ERR_FILE_SIZE_TOO_BIG = 1420,
        ERR_SAVE_PUBKEY = 1421,
        ERR_SAVE_RANDOM_FOR_VID = 1422,

        ERR_WRONG_SIGNED_DATA = 1500,
        ERR_COMPOSE_SIGNED_DATA = 1501,
        ERR_WRONG_ENVELOPED_DATA = 1502,
        ERR_COMPOSE_ENVELOPED_DATA = 1503,
        ERR_WRONG_SIGNED_AND_ENVELOPED_DATA = 1504,
        ERR_COMPOSE_SIGNED_AND_ENVELOPED_DATA = 1505,
        ERR_NOT_EXIST_SIGNER_CERT = 1506,
        ERR_CANNOT_DECRYPT_DATA = 1507,
        ERR_WRONG_RECIPIENT_CERT = 1508,
        ERR_WRONG_SIGNER_CERT = 1509,
        ERR_NOT_SIGN_CERT = 1510,
        ERR_NOT_KM_CERT = 1511,
        ERR_NOT_SIGNED_DATA = 1512,
        ERR_NOT_ENVELOPED_DATA = 1513,
        ERR_NOT_SIGNED_AND_ENVELOPED_DATA = 1514,
        ERR_NOT_ENCRYPTED_DATA = 1515,
        ERR_WRONG_ENCRYPTED_DATA = 1516,
        ERR_COMPOSE_ENCRYPTED_DATA = 1517,
        ERR_WRONG_SIGNER_CERTS = 1518,
        ERR_WRONG_SIGNED_CONTENET = 1519,
        ERR_COMPOSE_SIGNED_CONTENET = 1520,
        ERR_WRONG_WAP_ENV_DATA = 1521,
        ERR_COMPOSE_WAP_ENV_DATA = 1522,
        ERR_NOT_SUPPORTED_SIGNER_INFO = 1523,
        ERR_MAKE_TBSDATA = 1524,
        ERR_NOT_EXIST_TIME_STAMP_TOKEN = 1525,
        ERR_NOT_EXIST_SIGNER_CAPUBS = 1526,

        ERR_COMPOSE_TSP_REQ_MSG = 1600,
        ERR_CONNECT_TSA = 1601,
        ERR_TSA_REQ_SEND = 1602,
        ERR_TSA_MSG_REC = 1603,
        ERR_NOT_TSP_RES_MSG = 1604,
        ERR_WRONG_TSP_RES_MSG = 1605,
        ERR_WRONG_TOKEN = 1606,
        ERR_TSP_REQ_NOT_GRANTED = 1607,
        ERR_RES_NOT_SIGN_TOKEN = 1608,
        ERR_NOT_TIME_STAMP_TOKEN = 1609,
        ERR_REQ_INFO_NOT_EXIST = 1610,
        ERR_DIFFERENT_MESSAGE_IMPRINT = 1611,
        ERR_DIFFERENT_NONCE = 1612,
        ERR_NOT_PERMITTED_POLICY = 1613,
        ERR_NOT_TOKEN_FOR_DOCUMENT = 1614,

        ERR_INVALID_VID = 1700,
        ERR_NOT_EXIST_VID_IN_CERT = 1701,
        ERR_NOT_EXIST_RANDOM_IN_PRIKEY = 1702,
                                            
        ERR_INVALID_SYM_ALG = 1800,
        ERR_INVALID_KEY_LEN = 1801,
        ERR_INVALID_IV_LEN = 1802,
        ERR_INVALID_HASH_ALG = 1803,
        ERR_INVALID_MAC_ALG = 1804,
        ERR_NOT_SET_KEY_IV = 1805,
        ERR_ENCRYPT_DATA = 1806,
        ERR_DECRYPT_DATA = 1807,
        ERR_KCDSA_SIGN_NEED_CERT = 1808,
        ERR_SIGN = 1809,
        ERR_VERIFY_SIGNATURE = 1810,
        ERR_DIGEST_DATA = 1811,
        ERR_VERIFY_MAC = 1812,
        ERR_GEN_MAC = 1813,
        ERR_CHECK_KEY_PAIR = 1814,
        ERR_GEN_RANDOM = 1815,
        ERR_NOT_SUPPORTED_ALG = 1816,
        ERR_INVALID_KEY_TYPE = 1817,
        ERR_WRONG_PUBKEY = 1818,
        ERR_GENERATE_KEY_PAIR = 1819,
        ERR_KEY_GENERATE = 1820,
        ERR_INVALID_ASYM_ALG = 1821,
        ERR_MAKE_PUBKEY_INFO = 1822,

        ERR_BASE64_ENCODE = 1900,
        ERR_BASE64_DECODE = 1901,

        ERR_WRONG_URL = 2000,
        ERR_INVALID_PROTOCOL = 2001,
        ERR_LDAP_NO_DATA = 2002,
        ERR_INVALID_DATA_TYPE = 2003,
        ERR_LDAP_INIT = 2004,
        ERR_LDAP_SIMPLE_BIND_S = 2005,
        ERR_LDAP_SET_OPTION = 2006,
        ERR_LDAP_SEARCH_S = 2007,
        ERR_LDAP_FIRST_ENTRY = 2008,
        ERR_LDAP_GET_VALUES_LEN = 2009,
        ERR_GET_CRL_FROM_CERT = 2010,
        ERR_NOT_EXIST_LDAP_INFO = 2011,
        ERR_NOT_EXIST_ISSUER_CERT = 2012,
        ERR_READ_ENTRY = 2013,
        ERR_LDAP_GET_DN = 2014,
        ERR_MORE_THAN_ONE_DN = 2015,

        ERR_CONNECT_IVS = 2100,
        ERR_IVS_REQ_SEND = 2101,
        ERR_IVS_MSG_REC = 2102,
        ERR_WRONG_IVS_RES_MSG = 2103,
        ERR_NOT_SERVICE_CERT = 2104,
        ERR_SYSTEM_INTERNAL_ERROR = 2105,
        ERR_REQUESTER_CERT_REVOKED = 2106,
        ERR_REQUESTER_CERT_INVALID = 2107,
        ERR_REQUESTER_CERT_UNAUTHORIZED = 2109,
        ERR_REQ_MSG_FORMAT = 2110,
        ERR_UNKNOWN_IVS_CODE = 2111,
        ERR_FAIL_READ_IVS_IP_FROM_CONF_FILE = 2112,
        ERR_FAIL_READ_IVS_PORT_FROM_CONF_FILE = 2113,
        ERR_FAIL_READ_IVS_CERT_FROM_CONF_FILE = 2114,
        ERR_FAIL_GET_IVS_KM_CERT_FROM_LDAP = 2115,
        ERR_FAIL_GET_IVS_SIGN_CERT_FROM_LDAP = 2116,
        ERR_NOT_TRUST_IVS_CERT = 2117,

        ERR_EXPORT_PFX = 2200,
        ERR_IMPORT_PFX = 2201,

        ERR_TOKEN_INIT = 8000,
        ERR_GET_SLOT_LIST = 8001,
        ERR_CRYPTOKI_LIB_INIT = 8002,
        ERR_SESSION_OPEN = 8003,
        ERR_SESSION_CLOSE = 8004,
        ERR_SO_LOGIN = 8005,
        ERR_USER_LOGIN = 8006,
        ERR_INIT_PIN = 8007,
        ERR_LOG_OUT = 8008,
        ERR_FINISH = 8009,
        ERR_CHANGE_USER_PIN = 8010,
        ERR_FIND_OBJECTS_INIT = 8011,
        ERR_FIND_OBJECTS = 8012,
        ERR_GET_ATTR_VALUE = 8013,
        ERR_SET_ATTR_VALUE = 8014,
        ERR_GENERATE_CKA_ID = 8015,
        ERR_DESTROY_OBJECT = 8016,
        ERR_UNWRAP_KEY = 8017,
        ERR_CREATE_OBJECT = 8018,
        ERR_NOT_EXIST_OBJECT = 8019,
        ERR_EXIST_KEY_ID = 8020,
        ERR_SESSION_NOT_OPEN = 8021,
        ERR_WRONG_SIGNATURE_VALUE = 8022,
        ERR_LIB_MODIFIED = 8023,
        ERR_USER_NOT_LOGGED_IN = 8024,
        ERR_PIN_INCORRECT = 8025,
        ERR_DRIVER_INFO_NOT_EXIST = 8026,
        ERR_SLOT_NOT_LISTED = 8027,
        ERR_GET_TOKEN_INFO = 8028,
        ERR_DRIVER_PATH_COUNT = 8029,
        ERR_NOT_APPROPRIATE_KEY_USAGE = 8030,
                                            
        ERR_WRONG_CHALLENGE = 4000,
        ERR_COMPOSE_RESPONSE = 4001,
        ERR_WRONG_RESPONSE = 4002
    }
}

#ifndef NATIVE_LOG_H
#define NATIVE_LOG_H

#if 0
extern "C" void _NTLog_(const void *Self, int nLevel, const char *pszFuncName, const char *pszFileName, const int nLineNo, const char *pszFormat, ...)
{
#define MAX_LOG_LENGTH 4096
	int nLoc = 0;
    va_list valist;
	char szNativeLog[MAX_LOG_LENGTH];

    va_start(valist, pszFormat);
    nLoc += snprintf(szNativeLog+nLoc, MAX_LOG_LENGTH-1, "[NATIVE] ");
    nLoc += vsnprintf(szNativeLog+nLoc, MAX_LOG_LENGTH-1, pszFormat, valist);
    nLoc += snprintf(szNativeLog+nLoc, MAX_LOG_LENGTH-1, " in method %s at %s:%d", pszFuncName, pszFileName, nLineNo);
    va_end(valist);

   ((WebWindow *)Self)->NTLog(nLevel, (AutoString)szNativeLog);
}	
#else
extern "C" void _NTLog_(const void *Self, int nLevel, const char *pszFuncName, const char *pszFileName, const int nLineNo, const char *pszFormat, ...);
#endif

#define NTLog(_SELF_,_LEVEL_,_FMT_, ...) _NTLog_(_SELF_,_LEVEL_,__func__,__FILE__,__LINE__,_FMT_, ##__VA_ARGS__)

#endif /* TRAY_H */
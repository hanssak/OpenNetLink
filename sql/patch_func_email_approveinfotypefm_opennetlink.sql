-- FUNCTION: public.func_email_approveinfotypefm_open(character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying)

-- DROP FUNCTION IF EXISTS public.func_email_approveinfotypefm_open(character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying);

CREATE OR REPLACE FUNCTION public.func_email_approveinfotypefm_open(
	userid character varying,
	fromdate character varying,
	todate character varying,
	apprkind character varying,
	transkind character varying,
	approvestatus character varying,
	transstatus character varying,
	dlp character varying,
	sender character varying,
	reciever character varying,
	title character varying,
	network character varying,
	pagelistcount character varying,
	viewpageno character varying)
    RETURNS TABLE(email_seq bigint, approve_seq bigint, approvekind character varying, transkind2 character varying, dlpstatus character varying, addfile character varying, transtatus character varying, apprstatus character varying, mailsender character varying, recvuser character varying, recvcount bigint, title_text character varying, transdate character varying, approvedate character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
DECLARE
	sql varchar;	-- 쿼리
	recvuser varchar; -- 수신자 검색조검
	whand varchar;	-- 검색조건이 있을때의 연결(Where or and)
	nowdate varchar;
begin
/* -- 함수 파라미터 
userid : 사용자 아이디
fromdate : 조회기간(시작날짜)
todate : 조회기간(종료날짜)
apprkind : 결재구분(사전, 사후)
transkind : 발송구분(반출, 반입)
approvestatus : 결재상태
transstatus : 전송상태
dlp : 개인정보포함여부(미검사, 미포함, 포함, 검출불가)
sender : 발송자
reciever : 수신자
title : 제목
network : 망구분 
*/
  nowdate:=To_CHAR(now(), 'YYYYMMDD');
  
	
			
  sql:='WITH TBL_EMAIL_TRANSFER AS 
	(  
		SELECT T.* 
		FROM TBL_EMAIL_TRANSFER_HIS T 
		WHERE T.EMAIL_SEQ BETWEEN ''##FROMDATE##000000000'' AND ''##TODATE##9999999999'' 
			AND SUBSTRING(T.SYSTEM_ID,  2, 1)=''##CONNECTNETWORK##''  

		UNION ALL 

		SELECT T.* 
		FROM TBL_EMAIL_TRANSFER_INFO T 
		WHERE T.EMAIL_SEQ BETWEEN ''##FROMDATE##000000000'' AND ''##TODATE##9999999999'' 
			AND SUBSTRING(T.SYSTEM_ID,  2, 1)=''##CONNECTNETWORK##''  
	) 
	, TBL_APPROVE_USER AS  
	(  
		SELECT USER_SEQ, USER_ID, USER_NAME 
		FROM   TBL_USER_INFO  
		WHERE USER_ID=''##USERID##''
		
		UNION  ALL
		
		SELECT B.USER_SEQ, C.USER_ID, C.USER_NAME
		FROM TBL_USER_INFO A
		, TBL_USER_SFM B
		, TBL_USER_INFO C
		WHERE A.USER_SEQ = B.SFM_USER_SEQ 
		AND A.USER_ID=''##USERID##''
		AND ''##NOWDATE##'' BETWEEN B.FROMDATE AND B.TODATE
		AND B.USER_SEQ=C.USER_SEQ
        	
	)  
	, TBL_EMAIL_APPROVE AS  
	(  
		SELECT ''0'' AS APPROVEKIND, ''H'' AS POS, A.* 
		FROM TBL_EMAIL_APPROVE_HIS A 
		WHERE A.APPR_REQ_TIME BETWEEN ''##FROMDATE##000000'' AND ''##TODATE##235959'' 

		UNION ALL  

		SELECT ''0'' AS APPROVEKIND, ''C'' AS POS, A.* 
		FROM TBL_EMAIL_APPROVE_INFO A 
		WHERE A.APPR_REQ_TIME BETWEEN ''##FROMDATE##000000'' AND ''##TODATE##235959'' 
	)  
	, TBL_EMAIL_USER AS 
	( 
		SELECT R.EMAIL_SEQ, EMAIL_NO, R.RECV_TYPE, (CASE WHEN U.USER_ID IS NULL THEN R.ADDR ELSE U.USER_NAME END) ADDR  
		FROM TBL_EMAIL_RECEIVER R 
		LEFT OUTER JOIN  TBL_USER_INFO U ON R.ADDR=U.USER_ID 
		WHERE R.EMAIL_SEQ BETWEEN ''##FROMDATE##000000000'' AND ''##TODATE##9999999999'' 
		AND R.RECV_TYPE=''0'' 
	) 
	, TBL_USERINFO AS 
	( 
		SELECT ''H'' UPOS, USER_SEQ, USER_ID, USER_NAME, DEPT_SEQ 
		FROM TBL_USER_INFO_HIS 
		UNION ALL  
		SELECT ''C'' UPOS, USER_SEQ, USER_ID, USER_NAME, DEPT_SEQ 
		FROM TBL_USER_INFO 
	) 
	SELECT EMAIL_SEQ, APPROVE_SEQ, APPROVEKIND, TRANSKIND, DLP, ADDFILE, TRANSTATUS, APPROVESTATUS, MAILSENDER, RECVUSER, RECVCOUNT, TITLE, TRANSDATE, APPROVEDATE
	FROM ( 
		SELECT T.EMAIL_SEQ 
			, A.REQ_SEQ AS APPROVE_SEQ 
			, CAST (A.APPROVEKIND AS VARCHAR ) AS APPROVEKIND
			, CAST ((CASE WHEN SUBSTR(T.SYSTEM_ID, 1, 1) = ''I'' THEN  ''1'' ELSE ''2'' END ) AS VARCHAR) AS TRANSKIND 
			, CAST (T.DLP_FLAG AS VARCHAR) AS DLP 
			, CAST ((CASE WHEN ( SELECT COUNT(*) FROM TBL_EMAIL_ADD_FILE 
					WHERE EMAIL_SEQ=A.EMAIL_SEQ 
						AND UPPER(FILE_NAME) NOT LIKE ''%.EML'' 
						AND ADD_TYPE=''0''
				 )=0 THEN ''N'' ELSE ''Y''END )
			 AS VARCHAR) AS ADDFILE 
			, CAST ((CASE WHEN T.TRANS_FLAG = ''1''  THEN ''W'' 
				WHEN T.TRANS_FLAG = ''2''  THEN ''W'' 
				WHEN T.TRANS_FLAG = ''3''  THEN ''S'' 
				WHEN T.TRANS_FLAG = ''4''  THEN ''F'' 
				WHEN T.TRANS_FLAG = ''5''  THEN ''C'' 
				WHEN T.TRANS_FLAG = ''6''  THEN ''V''
				WHEN T.TRANS_FLAG = ''7''  THEN ''S'' 
				WHEN T.TRANS_FLAG = ''8''  THEN ''F'' 
				WHEN T.TRANS_FLAG = ''9''  THEN ''W'' 
				ELSE ''N'' END)
			AS VARCHAR ) AS TRANSTATUS 
			, CAST (A.APPROVE_FLAG AS VARCHAR ) AS APPROVESTATUS 
			, (SELECT CAST (USER_NAME || ''/'' || UPOS  AS VARCHAR) FROM TBL_USERINFO WHERE USER_SEQ=T.USER_SEQ) AS MAILSENDER 
			, R.ADDR AS RECVUSER 
			, (SELECT COUNT(*) FROM TBL_EMAIL_USER WHERE EMAIL_SEQ=T.EMAIL_SEQ ) AS RECVCOUNT  
			, T.TITLE 
			, CAST(T.REQUEST_TIME AS VARCHAR) AS TRANSDATE 
			, CAST((CASE WHEN A.APPROVE_FLAG = ''1'' THEN ''-'' ELSE A.APPR_RES_TIME END) AS VARCHAR) AS APPROVEDATE 
		FROM TBL_EMAIL_APPROVE A 
			, TBL_EMAIL_TRANSFER T 
			, TBL_APPROVE_USER U 
			, TBL_EMAIL_USER R 
			, (SELECT EMAIL_SEQ, MIN(EMAIL_NO) EMAIL_NO FROM TBL_EMAIL_USER  ##MAIL## GROUP BY EMAIL_SEQ ) AS R1
	WHERE A.EMAIL_SEQ=T.EMAIL_SEQ 
		AND A.APPROVE_USER_SEQ =U.USER_SEQ 
		AND R.EMAIL_SEQ=A.EMAIL_SEQ 
		AND R.EMAIL_SEQ=R1.EMAIL_SEQ 
		AND R.EMAIL_NO=R1.EMAIL_NO 
		AND T.USER_SEQ<> A.APPROVE_USER_SEQ 
	) A 
	';

	sql:=Replace(sql, '##USERID##', userid);
	sql:=Replace(sql, '##FROMDATE##', fromdate);
	sql:=Replace(sql, '##TODATE##', todate);
	sql:=Replace(sql, '##NOWDATE##', nowdate);

	IF network IS NULL AND network='' THEN 
		network:='0';
	END IF;
	sql:=Replace(sql, '##CONNECTNETWORK##', network);

	-- 수신자 조건에 따라서 달라짐
	IF reciever IS NULL OR reciever =''  THEN
		sql:=Replace(sql, '##MAIL##', '');
	ELSE 
		
		recvuser:='WHERE ADDR LIKE ''%'||reciever||'%''';	
		sql:=Replace(sql, '##MAIL##', recvuser);
		
	END IF;

	whand:='WHERE ';
	-- 결재구분
	IF apprkind IS NOT NULL AND apprkind!='' THEN
		sql:=sql||whand||'A.APPROVEKIND='''||apprkind||''''||chr(13);
		whand:=' AND ';
	END IF;

	
	-- 전송구분
	IF transkind IS NOT NULL AND transkind!='' THEN
		sql:=sql||whand||'A.TRANSKIND='''||transkind||''''||chr(13);
		whand:=' AND ';
	END IF;

	
	-- 결재상태
	IF approvestatus IS NOT NULL AND approvestatus!='' THEN
		sql:=sql||whand||'A.APPROVESTATUS='''||approvestatus||''''||chr(13);
		whand:=' AND ';
	END IF;

	-- 전송상태
	IF transstatus IS NOT NULL AND transstatus!='' THEN
		sql:=sql||whand||'A.TRANSTATUS='''||transstatus||''''||chr(13);
		whand:=' AND ';
	END IF;

	
	-- 개인정보상태
	IF dlp IS NOT NULL AND dlp!='' THEN
		sql:=sql||whand||'A.DLP='''||dlp||''''||chr(13);
		whand:=' AND ';
	END IF;

	-- 발신자
	IF sender IS NOT NULL AND sender!='' THEN
		sql:=sql||whand||'A.MAILSENDER LIKE ''%'||sender||'%'''||chr(13);
		whand:=' AND ';
	END IF;

	-- 제목
	IF title IS NOT NULL  AND title!='' THEN
		sql:=sql||whand||'A.TITLE LIKE ''%'||title||'%'''||chr(13);
		whand:=' AND ';
	END IF;

	IF PageListCount IS NOT NULL AND PageListCount != '' AND ViewPageNo IS NOT NULL AND ViewPageNo != '' THEN
		sql:=sql||'ORDER BY A.TRANSDATE DESC LIMIT '|| PageListCount || ' OFFSET (' || ViewPageNo || '-1) *' || PageListCount;
	END IF;

--	RAISE NOTICE 'Quantity here is %', sql;  -- Prints 50

--	RAISE NOTICE 'Quantity date %', nowdate;  -- Prints 50

	
RETURN QUERY EXECUTE
	sql;

end;
$BODY$;

ALTER FUNCTION public.func_email_approveinfotypefm_open(character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying)
    OWNER TO hsck;

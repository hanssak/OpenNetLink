-- FUNCTION: public.func_email_transferinfo_open(character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying)

-- DROP FUNCTION IF EXISTS public.func_email_transferinfo_open(character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying);

CREATE OR REPLACE FUNCTION public.func_email_transferinfo_open(
	userid character varying,
	fromdate character varying,
	todate character varying,
	apprkind character varying,
	transkind character varying,
	approvestatus character varying,
	transstatus character varying,
	dlp character varying,
	reciever character varying,
	title character varying,
	network character varying,
	pagelistcount character varying,
	viewpageno character varying)
    RETURNS TABLE(email_seq bigint, approvekind character varying, trans_kind character varying, dlptype character varying, addfile character varying, trans_status character varying, apprstatus character varying, recvuser character varying, recvcount bigint, title2 character varying, transdate character varying, approveuser character varying, approvedate character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
DECLARE
	sql varchar;	-- 쿼리
	recvuser varchar; -- 수신자 검색조검
	whand varchar;	-- 검색조건이 있을때의 연결(Where or and)
begin

-- userid : 사용자 아이디
-- fromdate : 조회기간(시작날짜)
-- todate : 조회기간(종료날짜)
-- apprkind : 결재구분(사전, 사후)
-- transkind : 발송구분(반출, 반입)
-- approvestatus : 결재상태
-- transstatus : 전송상태
-- dlp : 개인정보포함여부(미검사, 미포함, 포함, 검출불가)
-- reciever : 수신자
-- title : 제목
-- network : 망구분

				
	sql:='
	WITH TBL_USER AS
	(
		SELECT * FROM TBL_USER_INFO WHERE USER_ID=''##USERID##''
	)
	,TBL_EMAIL_TRANSFER AS
	(
		SELECT T.*
		FROM TBL_EMAIL_TRANSFER_HIS T
			, TBL_USER U
		WHERE T.USER_SEQ=U.USER_SEQ
			AND T.EMAIL_SEQ BETWEEN ##FROMDATE##0000000000 AND ##TODATE##99999999999
			AND T.REQUEST_TIME BETWEEN ''##FROMDATE##000000'' AND ''##TODATE##235959''
			AND SUBSTRING(T.SYSTEM_ID,  2, 1)=''##CONNECTNETWORK##''

		UNION ALL

		SELECT T.*
		FROM TBL_EMAIL_TRANSFER_INFO T
			, TBL_USER U
		WHERE T.USER_SEQ=U.USER_SEQ
			AND T.EMAIL_SEQ BETWEEN ##FROMDATE##0000000000 AND ##TODATE##99999999999
			AND T.REQUEST_TIME BETWEEN ''##FROMDATE##000000'' AND ''##TODATE##235959''
			AND SUBSTRING(T.SYSTEM_ID,  2, 1)=''##CONNECTNETWORK##''
	)
	, TBL_EMAIL_APPROVE AS
	(
		SELECT APPROVEKIND, EMAIL_SEQ
		FROM (
			SELECT A.APPROVE_KIND AS APPROVEKIND, ''H'' AS POS, A.EMAIL_SEQ
			FROM TBL_EMAIL_APPROVE_HIS A
			, TBL_EMAIL_TRANSFER T
			WHERE A.EMAIL_SEQ=T.EMAIL_SEQ

			UNION ALL

			SELECT ''0'' AS APPROVEKIND, ''C'' AS POS, A.EMAIL_SEQ
			FROM TBL_EMAIL_APPROVE_INFO A
			, TBL_EMAIL_TRANSFER T
			WHERE A.EMAIL_SEQ=T.EMAIL_SEQ
		) A
	  GROUP BY APPROVEKIND, EMAIL_SEQ
	)
	, TBL_EMAIL_RECVUSER AS
	(
		SELECT R.EMAIL_SEQ, R.EMAIL_NO
			, (CASE WHEN (SELECT USER_NAME FROM TBL_USER_INFO WHERE USER_ID=R.ADDR) IS NULL THEN R.ADDR
			ELSE (SELECT USER_NAME FROM TBL_USER_INFO WHERE USER_ID=R.ADDR) END) AS ADDR
		FROM TBL_EMAIL_RECEIVER R
			, TBL_EMAIL_TRANSFER T
		WHERE R.EMAIL_SEQ=T.EMAIL_SEQ
			AND RECV_TYPE=''0''
	)
	SELECT EMAIL_SEQ, APPROVEKIND, TRANSKIND, DLP, ADDFILE, TRANSSTATUS, APPROVESTATUS, RECVUSER, RECVCOUNT, TITLE, TRANSDATE, APPROVEUSER, APPROVEDATE
	FROM (
		SELECT E.EMAIL_SEQ
			, CAST (A.APPROVEKIND AS VARCHAR) AS APPROVEKIND
			, CAST ((CASE WHEN SUBSTRING(E.SYSTEM_ID, 1, 1) = ''I'' THEN  ''1'' ELSE ''2'' END ) AS VARCHAR) AS TRANSKIND
			, CAST (E.DLP_FLAG AS VARCHAR) AS DLP
			, CAST ((CASE WHEN ( SELECT COUNT(*) FROM TBL_EMAIL_ADD_FILE
						WHERE EMAIL_SEQ=E.EMAIL_SEQ
							AND UPPER(FILE_NAME) NOT LIKE ''%.EML''
							AND ADD_TYPE=''0''
						)=0 THEN ''N'' 
				ELSE ''Y'' END ) 
			AS VARCHAR)AS ADDFILE
			, CAST (( CASE WHEN E.TRANS_FLAG = ''1'' THEN ''W''
				  WHEN E.TRANS_FLAG = ''2''  THEN ''W''
				  WHEN E.TRANS_FLAG = ''3''  THEN ''S''
				  WHEN E.TRANS_FLAG = ''4''  THEN ''F''
				  WHEN E.TRANS_FLAG = ''5''  THEN ''C''
				  WHEN E.TRANS_FLAG = ''6''  THEN ''V''
				  	WHEN E.TRANS_FLAG = ''7''  THEN ''S'' 
				WHEN E.TRANS_FLAG = ''8''  THEN ''F'' 
				WHEN E.TRANS_FLAG = ''9''  THEN ''W'' 
				  ELSE ''N'' END) 
			AS VARCHAR ) AS TRANSSTATUS
			, CAST (E.APPROVE_FLAG AS VARCHAR) AS APPROVESTATUS
			, CAST ((CASE WHEN R.ADDR IS NULL THEN ''''
					WHEN R.ADDR ='''' THEN ''''
					ELSE R.ADDR END) AS VARCHAR ) AS RECVUSER
			, (SELECT COUNT(*) FROM TBL_EMAIL_RECVUSER WHERE EMAIL_SEQ=E.EMAIL_SEQ) AS RECVCOUNT
			, E.TITLE
			, CAST (E.REQUEST_TIME AS VARCHAR ) AS TRANSDATE
			, CAST ('''' AS VARCHAR) AS APPROVEUSER
			, CAST ('''' AS VARCHAR) AS APPROVEDATE
		FROM TBL_EMAIL_TRANSFER E
			, TBL_EMAIL_APPROVE A
			, TBL_EMAIL_RECVUSER R
			, (SELECT EMAIL_SEQ, MIN(EMAIL_NO) AS EMAIL_NO FROM TBL_EMAIL_RECVUSER ##MAIL## GROUP BY EMAIL_SEQ) R1
		WHERE A.EMAIL_SEQ=E.EMAIL_SEQ
			AND R.EMAIL_SEQ=E.EMAIL_SEQ
			AND R.EMAIL_SEQ=R1.EMAIL_SEQ
			AND R.EMAIL_NO=R1.EMAIL_NO
	) A
	';

	sql:=Replace(sql, '##USERID##', userid);
	sql:=Replace(sql, '##FROMDATE##', fromdate);
	sql:=Replace(sql, '##TODATE##', todate);
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
		sql:=sql||whand||'A.TRANSSTATUS='''||transstatus||''''||chr(13);
		whand:=' AND ';
	END IF;

	
	-- 개인정보상태
	IF dlp IS NOT NULL AND dlp!='' THEN
		sql:=sql||whand||'A.DLP='''||dlp||''''||chr(13);
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


RETURN QUERY EXECUTE
	sql;
end;
$BODY$;

ALTER FUNCTION public.func_email_transferinfo_open(character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying)
    OWNER TO hsck;

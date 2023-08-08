-- FUNCTION: public.func_email_approveinfo_open(haracter varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, BOOLEAN, BOOLEAN, character varying, character varying)

-- DROP FUNCTION IF EXISTS public.func_email_approveinfo_open_test(character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, BOOLEAN, BOOLEAN, character varying, character varying);

CREATE OR REPLACE FUNCTION public.func_email_approveinfo_open_test(
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
	securityis BOOLEAN,
	sfm2is BOOLEAN,
	pagelistcount character varying,
	viewpageno character varying)
    RETURNS TABLE(email_seq bigint, approve_seq bigint, approvekind character varying, transkind2 character varying, dlpstatus character varying, addfile character varying, transtatus character varying, apprstatus character varying, mailsender character varying, recvuser character varying, recvcount bigint, title_text character varying, transdate character varying, approvedate character varying, approver character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    -- VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
DECLARE
	sql varchar;	-- 쿼리
	recvuser varchar; -- 수신자 검색조검
	whand varchar;	-- 검색조건이 있을때의 연결(Where or and)
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
securityis : 보안결재구분 
sfm2is : sfm2 대결재 조회 인지 유무
*/

	-- TBL_APPROVE_USER : 결재관리 요청받는 사람들 목록 (##SFM## : 나를 대결재자로 지정한 사람의 table 정보)
	-- TBL_EMAIL_APPROVE : 결재요청 List
	-- TBL_EMAIL_REAL_APPROVER_TMP : 결재관리상에 나타나는 실제 결재자 목록
	
  sql:='WITH TBL_APPROVE_USER AS  
	(  
		SELECT USER_SEQ, USER_ID, USER_NAME 
		FROM   TBL_USER_INFO  
		WHERE USER_ID=''##USERID##''##SFM##
	)
	, TBL_EMAIL_APPROVE AS  
	(  
		SELECT A.APPROVE_KIND AS APPROVEKIND, ''H'' AS POS, A.* 
		FROM TBL_EMAIL_APPROVE_HIS A, TBL_APPROVE_USER U
		WHERE ((A.APPR_REQ_TIME BETWEEN ''##FROMDATE##000000'' AND ''##TODATE##235959'' ) AND (A.APPROVE_USER_SEQ IN (U.USER_SEQ) AND A.APPROVE_USER_SEQ <> A.USER_SEQ) AND (##ISSECURITY##))

		UNION ALL 

		SELECT ''0''  AS APPROVEKIND, ''C'' AS POS, A.* 
		FROM TBL_EMAIL_APPROVE_INFO A, TBL_APPROVE_USER U
		WHERE ((A.APPR_REQ_TIME BETWEEN ''##FROMDATE##000000'' AND ''##TODATE##235959'') AND (A.APPROVE_USER_SEQ IN (U.USER_SEQ) AND A.APPROVE_USER_SEQ <> A.USER_SEQ) AND (##ISSECURITY##)) 
	)  	
	, TBL_EMAIL_TRANSFER AS 
	(  
		SELECT T.* 
		FROM TBL_EMAIL_TRANSFER_HIS T, TBL_EMAIL_APPROVE A  
		WHERE T.EMAIL_SEQ BETWEEN ''##FROMDATE##000000000'' AND ''##TODATE##9999999999'' 
			AND SUBSTRING(T.SYSTEM_ID,  2, 1)=''##CONNECTNETWORK##'' AND  A.EMAIL_SEQ=T.EMAIL_SEQ 

		UNION ALL 

		SELECT T.* 
		FROM TBL_EMAIL_TRANSFER_INFO T, TBL_EMAIL_APPROVE A  
		WHERE T.EMAIL_SEQ BETWEEN ''##FROMDATE##000000000'' AND ''##TODATE##9999999999'' 
			AND SUBSTRING(T.SYSTEM_ID,  2, 1)=''##CONNECTNETWORK##''  AND  A.EMAIL_SEQ=T.EMAIL_SEQ 
	) ##SFM_TBL_REAL_APPROVER##
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
	SELECT EMAIL_SEQ, APPROVE_SEQ, APPROVEKIND, TRANSKIND, DLP, ADDFILE, TRANSTATUS, APPROVESTATUS, MAILSENDER, RECVUSER, RECVCOUNT, TITLE, TRANSDATE, APPROVEDATE, REAL_APPROVER
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
			, CAST ( ##SFM_REAL_APPROVER## AS VARCHAR) AS REAL_APPROVER
		FROM TBL_EMAIL_APPROVE A 
			, TBL_EMAIL_TRANSFER T 
			, TBL_APPROVE_USER U 
			, TBL_EMAIL_USER R 
			, (SELECT EMAIL_SEQ, MIN(EMAIL_NO) EMAIL_NO FROM TBL_EMAIL_USER  ##MAIL## GROUP BY EMAIL_SEQ ) AS R1
			##SFM_TBL_REAL_APPROVER_DATA##
	WHERE A.EMAIL_SEQ=T.EMAIL_SEQ 
		AND A.APPROVE_USER_SEQ IN (U.USER_SEQ) 
		AND R.EMAIL_SEQ=A.EMAIL_SEQ 
		AND R.EMAIL_SEQ=R1.EMAIL_SEQ 
		AND R.EMAIL_NO=R1.EMAIL_NO 
		AND T.USER_SEQ<> A.APPROVE_USER_SEQ 
	) A ';

	-- 보안결재구분
	-- IF securityis IS NOT NULL AND securityis ='1' THEN
	IF securityis IS TRUE THEN
		sql:=Replace(sql, '##ISSECURITY##', 'A.APPROVE_ORDER < 101 OR A.APPROVE_ORDER > 199');
		sfm2is := FALSE;	-- 보안결재 : 대결재 사용못함
	ELSE
		sql:=Replace(sql, '##ISSECURITY##', 'A.APPROVE_ORDER > 100 AND A.APPROVE_ORDER < 200');
	END IF;


	-- 대결재구분
	IF sfm2is IS TRUE THEN
		sql:=Replace(sql, '##SFM##', ' UNION ALL 
SELECT A.USER_SEQ, A.USER_ID, A.USER_NAME 
FROM tbl_user_info A 
where A.USER_SEQ IN (
	select B.USER_SEQ 
	from TBL_USER_SFM B 
	where B.sfm_user_seq=(
		select M.USER_SEQ 
		from tbl_user_info M 
		where M.user_id=''##USERID##'') AND (TO_CHAR(NOW(), ''YYYYMMDD'') BETWEEN B.FROMDATE AND B.TODATE) )');
		sql:=Replace(sql, '##SFM_TBL_REAL_APPROVER##', ', TBL_EMAIL_REAL_APPROVER_TMP AS 
	(  
		SELECT EMAIL_SEQ, (APPROVE_REAL_NAME || '' '' || APPROVE_REAL_RANK ) AS R_APPROVER
		FROM TBL_EMAIL_APPROVE_REAL_HIS T, TBL_APPROVE_USER A 
		WHERE (T.EMAIL_SEQ BETWEEN ''##FROMDATE##000000000'' AND ''##TODATE##9999999999'') AND (T.APPROVE_USER_SEQ=A.USER_SEQ)
	)'); 
	-- 나 혹은 나를 결재자로 지정한 사람들에게 실재 결재를 받은 모든 Row Data
		sql:=Replace(sql, '##SFM_TBL_REAL_APPROVER_DATA##', ', TBL_EMAIL_REAL_APPROVER_TMP D');
		sql:=Replace(sql, '##SFM_REAL_APPROVER##', '(SELECT D.R_APPROVER FROM TBL_EMAIL_REAL_APPROVER_TMP WHERE D.EMAIL_SEQ=T.EMAIL_SEQ)'); -- TBL_EMAIL_REAL_APPROVER_TMP 를 구한후 변경
	ELSE
		sql:=Replace(sql, '##SFM##', '');
		sql:=Replace(sql, '##SFM_TBL_REAL_APPROVER##', '');		
		sql:=Replace(sql, '##SFM_TBL_REAL_APPROVER_DATA##', '');
		sql:=Replace(sql, '##SFM_REAL_APPROVER##', '''-''');		
	END IF;

	sql:=Replace(sql, '##USERID##', userid);
	sql:=Replace(sql, '##FROMDATE##', fromdate);
	sql:=Replace(sql, '##TODATE##', todate);

	-- RAISE NOTICE 'Email - Quantity here is %', sql;

	IF network IS NULL OR network='' THEN 
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
	
	-- 전송구분
	IF transkind IS NOT NULL AND transkind!='' THEN
		sql:=sql||whand||'A.TRANSKIND='''||transkind||''''||chr(13);
		whand:=' AND ';
	END IF;
	
	
	-- 승인대기(전체조회)
	IF (approvestatus='1' AND transstatus = 'W') THEN
	
		-- 결재구분		
		IF apprkind = '0' THEN	-- 사전			
			sql:=sql||whand||'A.APPROVEKIND='''||apprkind||''''||chr(13);
			whand:=' AND ';			
			sql:=sql||whand||'A.TRANSTATUS='''||transstatus||''''||chr(13);
			sql:=sql||whand||'A.APPROVESTATUS='''||approvestatus||''''||chr(13);
		ELSIF apprkind = '1' THEN	-- 사후
			transstatus := 'S';		
			sql:=sql||whand||'A.APPROVEKIND='''||apprkind||''''||chr(13);
			whand:=' AND ';			
			sql:=sql||whand||'A.TRANSTATUS='''||transstatus||''''||chr(13);
			sql:=sql||whand||'A.APPROVESTATUS='''||approvestatus||''''||chr(13);
		ELSE	-- 전체		
			sql:=sql||whand||'((A.APPROVEKIND=''0'' AND A.TRANSTATUS=''W'' AND A.APPROVESTATUS=''1'') OR ';

			--transstatus := 'S';
			
			sql:=sql||'(A.APPROVEKIND=''1'' AND A.TRANSTATUS=''S'' AND A.APPROVESTATUS=''1''))';
			whand:=' AND ';
		END IF;

	ELSE
	
	    -- 사용자가 설정한 혹은 전체 조회
	
		-- 결재구분
		IF apprkind IS NOT NULL AND apprkind!='' THEN
			sql:=sql||whand||'A.APPROVEKIND='''||apprkind||''''||chr(13);
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
--	RAISE NOTICE 'Quantity Title %', title;  -- Prints 50
	
RETURN QUERY EXECUTE
	sql;

end;
$BODY$;

ALTER FUNCTION public.func_email_approveinfo_open_test(character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, BOOLEAN, BOOLEAN,character varying, character varying)
    OWNER TO postgres;

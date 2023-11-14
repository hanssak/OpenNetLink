-- FUNCTION: public.func_nl_getafterapprovewaitusercount(IN userseq character varying)

-- DROP FUNCTION IF EXISTS public.func_nl_getafterapprovewaitusercount(IN userseq character varying);

-- DROP FUNCTION IF EXISTS public.func_nl_getafterapprovewaitusercount(IN userseq character varying, _DATA_TYPE_ integer);

CREATE OR REPLACE FUNCTION public.func_nl_getafterapprovewaitusercount(_userseq_ character varying, _DATA_TYPE_ integer DEFAULT 0)
    RETURNS TABLE(approve_user_seq bigint, approve_user_id character varying, approve_user_name character varying, approve_user_rank character varying, after_approve_total_count character varying, after_approve_wait_over_day_count bigint, after_approve_wait_warring_day_count bigint, set_after_approve_wait_over_day character varying, set_after_approve_wait_warring_day character varying)
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
DECLARE
	V_SET VARCHAR;
	V_WARRING_DAY VARCHAR;				
	V_OVER_DAY VARCHAR;
	V_POS integer;
	V_CONDITION VARCHAR;
	V_SQL VARCHAR;	-- Query
begin


	V_SET:=(SELECT TAG_VALUE FROM TBL_SYSTEM_ENV
			WHERE TAG='AFTERAPPROVE_NOTI_DAY'
			AND SYSTEM_ID='I001');

	V_POS:=position('/' in V_SET);
	IF V_POS >0 THEN
		V_WARRING_DAY := SPLIT_PART(V_SET, '/', 1);
		V_OVER_DAY := SPLIT_PART(V_SET, '/', 2);
		IF V_WARRING_DAY IS NULL OR V_WARRING_DAY='' THEN RETURN; END IF;
		IF V_OVER_DAY IS NULL OR V_OVER_DAY='' THEN RETURN; END IF;
	ELSE
		RETURN;
	END IF;

	
	V_SQL:='
	SELECT A.APPROVE_USER_SEQ, U.USER_ID, U.USER_NAME, U.USER_RANK,
		CAST (COUNT(*) AS VARCHAR) AFTER_APPROVE_WAIT_COUNT
		, CASE WHEN '''||V_OVER_DAY||''' = ''0'' THEN ''0''
		ELSE SUM(CASE WHEN A.appr_req_time < TO_CHAR((NOW() - INTERVAL '''||V_OVER_DAY||' DAY''), ''YYYYMMDDHH24MISS'') THEN 1 ELSE 0 END)
		END AS OVER_DAY_COUNT
		, CASE WHEN '''||V_WARRING_DAY||''' = ''0'' THEN ''0''
		ELSE SUM(CASE WHEN A.appr_req_time < TO_CHAR((NOW() - INTERVAL '''||V_WARRING_DAY||' DAY''), ''YYYYMMDDHH24MISS'') THEN 1 ELSE 0 END)
		END AS WARRING_DAY_COUNT				
		, CAST ('||V_OVER_DAY||' AS VARCHAR) SET_OVER_DAY
		, CAST ('||V_WARRING_DAY||' AS VARCHAR) SET_WARRING_DAY
				FROM TBL_APPROVE_AFTER A
					, TBL_USER_INFO U
					, view_transfer_all K
				WHERE A.APPROVE_USER_SEQ=U.USER_SEQ	
					AND COALESCE(U.ACCOUNT_EXPIRES, ''99991231'') > TO_CHAR(NOW(), ''YYYYMMDD'')
					AND U.USE_STATUS = ''1''
					AND U.USER_SEQ IN('||_userseq_||')
					##DATATYPE##
					AND K.trans_seq = A.trans_seq
				group by APPROVE_USER_SEQ, USER_ID, USER_NAME, USER_RANK;					
	';

	
	IF _DATA_TYPE_ < 0 THEN	-- ALL 검색
		V_SQL:=Replace(V_SQL, '##DATATYPE##', '');
	ELSEIF _DATA_TYPE_ = 3 THEN -- ClipBoard 검색때 사용
	    V_CONDITION:='AND (K.data_type=''1'' OR K.data_type=''2'')';
		V_SQL:=Replace(V_SQL, '##DATATYPE##', V_CONDITION);
	ELSE	-- 파일 / 기타 나머지 항목 1개식 검색때 사용
	    V_CONDITION:='AND K.data_type='''||_DATA_TYPE_||'''';
		V_SQL:=Replace(V_SQL, '##DATATYPE##', V_CONDITION);
	END IF;
	
	
	RAISE NOTICE 'BY SQL IS %', V_SQL;  -- PRINTS 50

	
RETURN QUERY EXECUTE
	V_SQL;

end;
$BODY$;

ALTER FUNCTION public.func_nl_getafterapprovewaitusercount(_userseq_ character varying, _DATA_TYPE_ integer)
    OWNER TO hsck;

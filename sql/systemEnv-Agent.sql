-------------------------------------------------------------------------------
-- tbl_system_env : PreviewUtil 사용할 확장자 List
-------------------------------------------------------------------------------

DELETE FROM tbl_system_env WHERE tag='CLIENT_PREVIEW_VIEWER_EXT';

INSERT INTO tbl_system_env (system_id, tag, tag_value, tag_desc, writer) VALUES ('I001', 'CLIENT_PREVIEW_VIEWER_EXT', 'pdf;docx;doc;pptx;ppt;xlsx;xls;xlw;xlsb;xlsm;csv;dbf;dif;slk;sylk;prn;ods;fods;gif;jpg;jpeg;bmp;tiff;tif;png;svg;txt;hwp;log', 'PreView Util로 띄울 확장자들', '201004010800000001');

INSERT INTO tbl_system_env (system_id, tag, tag_value, tag_desc, writer) VALUES ('E001', 'CLIENT_PREVIEW_VIEWER_EXT', 'pdf;docx;doc;pptx;ppt;xlsx;xls;xlw;xlsb;xlsm;csv;dbf;dif;slk;sylk;prn;ods;fods;gif;jpg;jpeg;bmp;tiff;tif;png;svg;txt;hwp;log', 'PreView Util로 띄울 확장자들', '201004010800000001');

select * from tbl_system_env where tag='CLIENT_PREVIEW_VIEWER_EXT';

-------------------------------------------------------------------------------
-- tbl_system_env : OLE 검사할 문서파일 설정,확장자 설정들
-------------------------------------------------------------------------------

DELETE FROM tbl_system_env WHERE tag='CLIENT_OLE_EXTRACT';
DELETE FROM tbl_system_env WHERE tag='CLIENT_OLE_EXTRACT_EXT';

INSERT INTO tbl_system_env (system_id, tag, tag_value, tag_desc, writer) VALUES ('I001', 'CLIENT_OLE_EXTRACT', '3', '0:문서 검사 않함, 1:모듈검사 & OLE객체 마임리스트검사, 2:모듈검사 & 위변조 체크, 3:모듈검사 & OLE마임리스트검사 & 위변조체크', '201004010800000001');

INSERT INTO tbl_system_env (system_id, tag, tag_value, tag_desc, writer) VALUES ('E001', 'CLIENT_OLE_EXTRACT', '2', '0:문서 검사 않함, 1:모듈검사 & OLE객체 마임리스트검사, 2:모듈검사 & 위변조 체크, 3:모듈검사 & OLE마임리스트검사 & 위변조체크', '201004010800000001');

INSERT INTO tbl_system_env (system_id, tag, tag_value, tag_desc, writer) VALUES ('I001', 'CLIENT_OLE_EXTRACT_EXT', 'ODT;ODS;ODP;DOC;DOCM;DOCX;DOT;DOTM;DOTX;RTF;XLS;XLSB;XLSM;XLSX;XLT;XLTM;XLTX;XLW;POT;PPT;POTM;POTX;PPS;PPSM;PPSX;PPTM;PPTX;HML;HWP;HWPX', 'OLE검사 대상 문서파일 확장자들', '201004010800000001');

INSERT INTO tbl_system_env (system_id, tag, tag_value, tag_desc, writer) VALUES ('E001', 'CLIENT_OLE_EXTRACT_EXT', 'ODT;ODS;ODP;DOC;DOCM;DOCX;DOT;DOTM;DOTX;RTF;XLS;XLSB;XLSM;XLSX;XLT;XLTM;XLTX;XLW;POT;PPT;POTM;POTX;PPS;PPSM;PPSX;PPTM;PPTX;HML;HWP;HWPX', 'OLE검사 대상 문서파일 확장자들', '201004010800000001');


-------------------------------------------------------------------------------
-- tbl_system_env : 제목 / 설명 최소길이
-------------------------------------------------------------------------------

DELETE FROM tbl_system_env WHERE tag='CLIENT_TITLE_MIN_LENGTH';
DELETE FROM tbl_system_env WHERE tag='CLIENT_DESC_MIN_LENGTH';

INSERT INTO tbl_system_env (system_id, tag, tag_value, tag_desc, writer) VALUES ('I001', 'CLIENT_TITLE_MIN_LENGTH', '6', '제목 최소길이', '201004010800000001');
INSERT INTO tbl_system_env (system_id, tag, tag_value, tag_desc, writer) VALUES ('E001', 'CLIENT_TITLE_MIN_LENGTH', '6', '제목 최소길이', '201004010800000001');

INSERT INTO tbl_system_env (system_id, tag, tag_value, tag_desc, writer) VALUES ('I001', 'CLIENT_DESC_MIN_LENGTH', '6', '설명 최소길이', '201004010800000001');
INSERT INTO tbl_system_env (system_id, tag, tag_value, tag_desc, writer) VALUES ('E001', 'CLIENT_DESC_MIN_LENGTH', '6', '설명 최소길이', '201004010800000001');

-------------------------------------------------------------------------------
-- tbl_system_env : URLRedirection에 나오는 문구 정보 변경
-------------------------------------------------------------------------------

delete from tbl_system_env where tag='CLIENT_URLREDIRECTION_F_INFO';

INSERT INTO tbl_system_env (system_id, tag, tag_value, tag_desc, writer) VALUES ('I001', 'CLIENT_URLREDIRECTION_F_INFO', 'IT 보안팀(☎8708)', '포워딩된 Page에 표시할 부서이름 / 전화번호', '201004010800000001');
INSERT INTO tbl_system_env (system_id, tag, tag_value, tag_desc, writer) VALUES ('E001', 'CLIENT_URLREDIRECTION_F_INFO', 'IT 보안팀(☎8708)', '포워딩된 Page에 표시할 부서이름 / 전화번호', '201004010800000001');


DROP TABLE tbl_file_ole_mimetype;

CREATE TABLE IF NOT EXISTS tbl_file_ole_mimetype
(
  mimetype character varying(128) NOT NULL,
  type character varying(1) NOT NULL,
  upddate character(14) NOT NULL,
  writer bigint DEFAULT 1,
  CONSTRAINT pk_tbl_file_ole_mimetype PRIMARY KEY (mimetype)
);

insert into tbl_file_ole_mimetype values('application/msword', 'B', to_char(now(),'YYYYMMDDHH24MISS'));
insert into tbl_file_ole_mimetype values('application/vnd.ms-excel', 'B', to_char(now(),'YYYYMMDDHH24MISS'));
insert into tbl_file_ole_mimetype values('application/vnd.ms-powerpoint', 'B', to_char(now(),'YYYYMMDDHH24MISS'));
insert into tbl_file_ole_mimetype values('application/x-7z-compressed', 'B', to_char(now(),'YYYYMMDDHH24MISS'));
insert into tbl_file_ole_mimetype values('application/x-bzip2', 'B', to_char(now(),'YYYYMMDDHH24MISS'));
insert into tbl_file_ole_mimetype values('application/x-compress', 'B', to_char(now(),'YYYYMMDDHH24MISS'));
insert into tbl_file_ole_mimetype values('application/x-dosexec', 'B', to_char(now(),'YYYYMMDDHH24MISS'));
insert into tbl_file_ole_mimetype values('application/x-epoc-sheet', 'B', to_char(now(),'YYYYMMDDHH24MISS'));
insert into tbl_file_ole_mimetype values('application/x-epoc-word', 'B', to_char(now(),'YYYYMMDDHH24MISS'));
insert into tbl_file_ole_mimetype values('application/x-executable', 'B', to_char(now(),'YYYYMMDDHH24MISS'));
insert into tbl_file_ole_mimetype values('application/x-gzip', 'B', to_char(now(),'YYYYMMDDHH24MISS'));
insert into tbl_file_ole_mimetype values('application/x-hwp', 'B', to_char(now(),'YYYYMMDDHH24MISS'));
insert into tbl_file_ole_mimetype values('application/x-iso9660-image', 'B', to_char(now(),'YYYYMMDDHH24MISS'));
insert into tbl_file_ole_mimetype values('application/x-lha', 'B', to_char(now(),'YYYYMMDDHH24MISS'));
insert into tbl_file_ole_mimetype values('application/x-lrzip', 'B', to_char(now(),'YYYYMMDDHH24MISS'));
insert into tbl_file_ole_mimetype values('application/x-lzip', 'B', to_char(now(),'YYYYMMDDHH24MISS'));
insert into tbl_file_ole_mimetype values('application/x-lzma', 'B', to_char(now(),'YYYYMMDDHH24MISS'));
insert into tbl_file_ole_mimetype values('application/x-object', 'B', to_char(now(),'YYYYMMDDHH24MISS'));
insert into tbl_file_ole_mimetype values('application/x-rar', 'B', to_char(now(),'YYYYMMDDHH24MISS'));
insert into tbl_file_ole_mimetype values('application/x-rpm', 'B', to_char(now(),'YYYYMMDDHH24MISS'));
insert into tbl_file_ole_mimetype values('application/x-svr4-package', 'B', to_char(now(),'YYYYMMDDHH24MISS'));
insert into tbl_file_ole_mimetype values('application/x-tar', 'B', to_char(now(),'YYYYMMDDHH24MISS'));
insert into tbl_file_ole_mimetype values('application/zip', 'B', to_char(now(),'YYYYMMDDHH24MISS'));
insert into tbl_file_ole_mimetype values('text/x-msdos-batch', 'B', to_char(now(),'YYYYMMDDHH24MISS'));
insert into tbl_file_ole_mimetype values('application/vnd.ms-office', 'B', to_char(now(),'YYYYMMDDHH24MISS'));
insert into tbl_file_ole_mimetype values('application/vnd.ms-msi', 'B', to_char(now(),'YYYYMMDDHH24MISS'));
insert into tbl_file_ole_mimetype values('application/x-msi', 'B', to_char(now(),'YYYYMMDDHH24MISS'));		
insert into tbl_file_ole_mimetype values('application/CDFV2-corrupt', 'B', to_char(now(),'YYYYMMDDHH24MISS'));
insert into tbl_file_ole_mimetype values('application/CDFV2', 'B', to_char(now(),'YYYYMMDDHH24MISS'));
insert into tbl_file_ole_mimetype values('application/gzip', 'B', to_char(now(),'YYYYMMDDHH24MISS'));
insert into tbl_file_ole_mimetype values('application/zlib', 'B', to_char(now(),'YYYYMMDDHH24MISS'));
insert into tbl_file_ole_mimetype values('application/vnd.openxmlformats-officedocument.presentationml.presentation', 'B', to_char(now(),'YYYYMMDDHH24MISS'));
insert into tbl_file_ole_mimetype values('application/vnd.openxmlformats-officedocument.wordprocessingml.document', 'B', to_char(now(),'YYYYMMDDHH24MISS'));
insert into tbl_file_ole_mimetype values('application/vnd.openxmlformats-officedocument.spreadsheetml.sheet', 'B', to_char(now(),'YYYYMMDDHH24MISS'));
insert into tbl_file_ole_mimetype values('application/vnd.ms-outlook', 'B', to_char(now(),'YYYYMMDDHH24MISS'));
insert into tbl_file_ole_mimetype values('application/encrypted', 'B', to_char(now(),'YYYYMMDDHH24MISS'));
insert into tbl_file_ole_mimetype values('message/rfc822', 'B', to_char(now(),'YYYYMMDDHH24MISS'));

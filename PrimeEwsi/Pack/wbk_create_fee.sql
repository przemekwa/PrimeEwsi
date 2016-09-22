DROP FUNCTION wbk_create_fee(integer,integer,char,decimal,char,varchar,varchar,char,smallint,varchar);

CREATE DBA PROCEDURE "cap".wbk_create_fee (fSerno Integer,fBatchSerno Integer default Null,fTable Char(1),fTrxnAmount decimal(16,3),fTrxnCurr Char(3) default Null, fTrxnType VarChar(4) default Null, fTextData  VarChar(254, 0) default null,fReasonCode Char(4) Default Null,fPostRealTime SmallInt Default 0, fi002_number varchar(19) default null)

Returning Integer;

-- $Id$ ;

Define fTrxnProcCode Char(6);
Define fTrxnSerno integer;
--Define fi002_number char(25);
Define f_orig_msg_type char(4);
Define f_trxnProfSerno int;
Define f_FeeReasonCode varchar(4);


-- Trace for Procedure 
Define Sql_e Smallint;
Define Isam_e smallint;
Define Txt_e Char(80);
-- In case of error catch it here and log it.
    on exception                                 
        Set Sql_e, Isam_e, Txt_e

        begin
            define Sql_trace  smallint;
            define Isam_trace smallint;
            define Txt_trace  char(80);

            -- In case of an error during trace
            on exception 
                Set  Sql_trace, Isam_trace, Txt_trace
                trace  '    ' || sql_trace || '    ' || isam_trace || '    ' || txt_trace;
            end exception with resume;

            -- set debug file to cap_getparam('center', 'error log file',  '\temp\ws_cis_err.log') With Append;
            set debug file to '/tmp/caperr.log' With Append;

            trace  Current || '    ' || user || '    ' || sql_e || '    ' || isam_e || '    ' || txt_e;

            trace ' Procedure name: wbk_create_fee';
	    trace 'fSerno: ' || nvl(fSerno,'<null>');
	    trace 'fBatchSerno: '  || nvl(fBatchSerno,'<null>');	
	    trace 'fTable: ' || nvl(fTable,'<null>');
	    trace 'fTrxnAmount: ' || nvl(fTrxnAmount,'<null>');
	    trace 'fTrxnCurr: ' || nvl(fTrxnCurr,'<null>');
	    trace 'fTrxnType: ' || nvl(fTrxnType,'<null>');
	    trace 'fTextData: ' || nvl(fTextData,'<null>');
	    trace 'fReasonCode: ' || nvl(fReasonCode,'<null>');
	    trace 'fPostRealTime: ' || nvl(fPostRealTime,'<null>');
	    trace 'fi002_number: ' || nvl(fi002_number,'<null>');
	    trace ' END TRACE Procedure : wbk_create_fee';
        end
        raise exception Sql_e, Isam_e, Txt_e;
    end exception;
    -- end trace for Procedure


If fTable = 'C'  Then  	--fee on the card

	If  fserno is not  null  then
		Select	Number, trxnfeeprofserno
		Into	fi002_number, f_trxnProfSerno
		From	Cardx
		Where	Serno = fSerno;
	elif fserno is null then
		Select	Serno,trxnfeeprofserno
		Into	fserno,f_trxnProfSerno
		From	Cardx
		Where	number= fi002_number;
	end if;

	if NVL(fi002_number,"") = ""  or fserno is null then
		Raise Exception -746, 0, 'Card is not found';
	end if;
	
	If fTrxnType="DB"  Then
		Let fTrxnProcCode = '190000';		--  Debit Fee
		Let f_orig_msg_type='SFEE';
	Elif fTrxnType="CR"  Then
		Let fTrxnProcCode = '290000';		-- Credit Fee
		Let f_orig_msg_type='SFEE';

	Elif fTrxnType IN ("BALC", "PINC","MSTM")  Then
		
		Let fTrxnProcCode = '190000';		-- Credit Fee
		Let f_orig_msg_type='TFEE';
		let f_FeeReasonCode=null;
		let fTextData=null;
		let fTrxnAmount=null;
		select trxntype, description, flatamount 
		into f_FeeReasonCode,fTextData,fTrxnAmount
		from trxnfees
		where profileserno=f_trxnProfSerno 
		and trxntypeserno=cap_gettrxntype_f(null, null, null,null, null, null,null, fTrxnType, NULL, NULL, Null, null, null,null, null, null, null,null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,null, fReasonCode,null, null, null, null, null, null, null, null,null, null, null) ;
		let fReasonCode=f_FeeReasonCode;	
	     
		if fTrxnAmount is null or fTrxnAmount=0 then
			return 0;
		end if;



	else
		Raise Exception -746, 0, 'Icorrect transaction type';
	End If;	

	Let fTrxnSerno = txn_new(fcardserno=fSerno,fbatchserno = fBatchSerno, forig_msg_type=f_orig_msg_type, fi000_msg_type= '0220',
	 fi002_number=fi002_number, fi003_proc_code=fTrxnProcCode, fi004_amt_trxn=ABS(fTrxnAmount),fi044_reason_code = fReasonCode,
	 fi048_text_data = fTextData, fi049_cur_trxn=fTrxnCurr,fPostRealTime=fPostRealTime);	

elif fTable = 'A'  Then	--fee on account

If  fi002_number is null then
	Select	Number
	Into	fi002_number
	From	Caccounts
	Where	Serno = fSerno;
elif fserno is null then
	Select	serno
	Into	fserno
	From	Caccounts
	Where	Number = fi002_number;
end if;

	if NVL(fi002_number,"") = "" or fserno is null  then
		Raise Exception -746, 0, 'Account is not found';
	end if;
	
	If fTrxnType="DB"  Then
		Let fTrxnProcCode = '193000';		--  Debit Fee
	Elif fTrxnType="CR"  Then
		Let fTrxnProcCode = '293000';		-- Credit Fee
	else
		Raise Exception -746, 0, 'Icorrect transaction type';
	End If;	
	
	Let fTrxnSerno = txn_new(faccserno=fSerno,fbatchserno = fBatchSerno, forig_msg_type='SFEE', fi000_msg_type= '0220',
	 fi002_number=fi002_number, fi003_proc_code=fTrxnProcCode, fi004_amt_trxn=ABS(fTrxnAmount),fi044_reason_code = fReasonCode,
	 fi048_text_data = fTextData, fi049_cur_trxn=fTrxnCurr,fPostRealTime=fPostRealTime);
else
	Raise Exception -746, 0, 'Icorrect indicator';
end if;

Return fTrxnSerno;


End procedure
Document 'Specyfikacja projektu NP3206',
'Autor: Piotr Kowalski';



Sub Click(Source As Button)
	Dim ses As New NotesSession
	Dim db As NotesDatabase
	Dim coll As NotesDocumentCollection
	Dim view As NotesView
	Dim uiws As New NotesUIWorkspace
	Dim uidoc As NotesUIDocument
	Dim doc As NotesDocument	
	Dim docMstr As NotesDocument
	Dim docM As NotesDocument
	Dim docD As NotesDocument
	Dim nextDoc As NotesDocument
	Dim item As NotesItem
	Dim workspace As New NotesUIWorkspace
	Dim anyDetail As Boolean
	
	Set uidoc = uiws.CurrentDocument
	Set doc = uidoc.Document
	Set docMstr = uidoc.Document
	Set docM = uidoc.Document
	Set docD = uidoc.Document
	Set db = ses.currentdatabase	
	Set view = db.GetView("lkVMEPDetail")	
	
	'===============================
	
	Set docD = view.GetFirstDocument
	
	anyDetail = False
	While Not docD Is Nothing		
		Set nextDoc = view.GetNextDocument(docD)			
		
		If docD.GetItemValue("masterDocUniqueID")(0) = docM.GetItemValue("docUniqueID")(0) Then
			anyDetail = True
			Set docD = view.GetLastDocument
		End If
		
		Set docD = nextDoc
	Wend
	
	'===============================
	
	If anyDetail = True Then
		Messagebox "If you want to change the Contractor, please delete detail first!",16,"Warning"
		Goto Keluar
	Else
		Set collection = workspace.PickListCollection(PICKLIST_CUSTOM, False, "NSI-AD", "Application\NSI Applications\VMEP.nsf", "vContractor", "Select the Contractor", "Contractor Lists")
		
		If collection.Count = 0 Then
		'Messagebox "User canceled" ,,"Contractor Lists" 
		Else
			Set doc = collection.GetFirstDocument
			
			Set item = doc.GetFirstItem("ContractorName")		
			Call docMstr.ReplaceItemValue("vmepContractor", item.Text)	
			
			Set item = doc.GetFirstItem("ContractorRepresentative")
			Call docMstr.ReplaceItemValue("vmepContractorRepresentative", item.Text)	
			
			Call uidoc.Refresh
		End If
	End If
	
Keluar:
End Sub



Set collection = workspace.PickListCollection(PICKLIST_CUSTOM, False, thisDoc.MDServer(0), thisDoc.MDPath(0), "lkRefEmployeeDept2", "Select the Employees", "Employee List",alDepartment;)
	

@SetEnvironment("EditLock"; "Locked")

query modechange
Dim sess As New NotesSession
	If sess.GetEnvironmentString("EditLock") = "UnLocked" Then
		Continue = True
	Else
		Continue = False
	End If
	post mode change
	If Source.EditMode Then Call Source.Refresh
	
	
	
	Button SUbmit
	
	
	
	If thisDoc.alDate(0) = "" Then
		Messagebox "Please fill the Date of Annual Leave!",16,"Error"
		Goto MoveHere
	Else
		Call submit()
		Call thisDoc.ComputeWithForm(True,False)
		
		
		'If thisUIDoc.editmode Then
		'	Call thisUIDoc.Save		
		'Else
		'Call thisDoc.Save(True,False)
		'End If
		
		Call thisUIDoc.Save
		thisDoc.saveoptions = "0"	
		Call SendMailAL(thisDoc)		
		Call thisUIDoc.Refresh
		Call thisUIDoc.Close
	End If
MoveHere:

button Approve 
Call approve
	Call thisDoc.ComputeWithForm(True,False)
	
	'thisDoc.saveoptions = "0"	
	'Call thisDoc.Save(True,False)
	
	Call thisUIDoc.Save
	thisDoc.saveoptions = "0"
	Call SendMailAL(thisDoc)		
	Call thisUIDoc.Refresh
	Call thisUIDoc.Close
	
	button recorde
	Call recorded
	Call thisDoc.ComputeWithForm(True,False)
	
	'thisDoc.saveoptions = "0"	
	'Call thisDoc.Save(True,False)
	'thisDoc.saveoptions = "0"
	
	Call thisUIDoc.Save
	thisDoc.saveoptions = "0"
	Call SendMailAL(thisDoc)	
	Call thisUIDoc.Refresh
	Call thisUIDoc.Close
	
	
	@If(
	alDocStatus="-2";"Rejected";
	alDocStatus="-1";"Sent Back";
	alDocStatus=""|alDocStatus="0";"Draft";
	alDocStatus="1";"Waiting Approval by Supervisor";
	alDocStatus="2";"Waiting Approval by Dept Head";
	alDocStatus="3";"Waiting Recorded by HRPA";
    alDocStatus="4";"Completed";"")
	
	
	
	--------------------------------------------------------------------------------
	picklist = uiws.PickListStrings( _
	PICKLIST_CUSTOM, _
	True, _
	"NSI-AD/NSI", _
	"Application\Template NSI\CompanyVehicleUsage.ntf", _
	"lkEmployeeDetailForArrangement", _
	"Select the Employee", _
	"Employee Lists", _
	2, _
	sCAOutDate) 
	
	If ( Isempty( picklist ) ) Then 
		
	'Messagebox "Canceled" , , "Employees selected"
		
	Else
		
		Forall plist In picklist		
		'Messagebox( plist )
			Dim Ases As New NotesSession	
			Dim Adb As NotesDatabase		
			Dim Aview As NotesView	
			Dim Adc As NotesDocumentCollection	
			Dim Adoc1 As NotesDocument
			Dim Adoc2 As NotesDocument
			Dim nama As String
			
			Set Adb=Ases.CurrentDatabase
			Set Aview = Adb.GetView("lkFlagEmployeeDetail")	
			Set Adc = Aview.GetAllDocumentsByKey(doc.CAOutDate(0), False)
			Set Adoc1 = Adc.GetFirstDocument
			
			While Not Adoc1 Is Nothing
				Set Adoc2 = Adc.GetDocument(Adoc1)
				nama = Adoc2.GetItemValue("DEmpName")(0)+" - "+Adoc2.GetItemValue("DDestination")(0)
			'Msgbox(nama)
				If plist = nama Then
					Call Adoc2.ReplaceItemValue("masterArrangementID", doc.GetItemValue("docUniqueID")(0))
					Call Adoc2.ReplaceItemValue("FlagArrangement", "1")				
					Call Adoc2.Save(True,False)	
				End If	
				Set Adoc1 = Adc.GetNextDocument(Adoc1)
			Wend		
		End Forall
	End If 
	
	
	
	'------------------------------------------------------------
	
	Dim session As New NotesSession
	Dim db As NotesDatabase
	Dim view As NotesView
	Dim view1 As NotesView
	Dim coll As NotesDocumentCollection
	Dim doc As NotesDocument
	Dim docImport As NotesDocument
	Dim count As Integer
	Set db = session.CurrentDatabase
	Set view = db.GetView("lkRefEmployeeID")
	
	'Set coll = view.GetFirstDocument
	'Messagebox "View name: " & view.Name
	Set doc = view.GetFirstDocument
	
	count = 0
	Do Until doc Is Nothing
		count = count + 1
		Set db = session.CurrentDatabase
		Set view1 = db.GetView("View\Biodata\ImportBiodata")
		Set docImport =view1.GetFirstDocument
		Do Until docImport Is Nothing
			
			Print (doc.bioName(0) +"="+ docImport.IName(0))
			Set docImport = view1.GetNextDocument(docImport)	
			'Delete docImport
			
		Loop
		
		Set doc = view.GetNextDocument(doc)	
		'Delete doc
	Loop
	
	Print("Data has been trasnfered")

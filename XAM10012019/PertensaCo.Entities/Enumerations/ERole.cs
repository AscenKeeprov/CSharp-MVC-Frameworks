using System.ComponentModel;

namespace PertensaCo.Entities.Enumerations
{
	public enum ERole : ushort
	{
		[DisplayName("Chief Administrative Officer")]
		CAO = 200,
		[DisplayName("Chief Communications Officer")]
		CCO = 1000,
		[DisplayName("Chief Executive Officer")]
		CEO = 100,
		[DisplayName("Chief Financial Officer")]
		CFO = 300,
		[DisplayName("Chief Human Resources Officer")]
		CHRO = 400,
		[DisplayName("Chief Innovation Officer")]
		CINO = 500,
		[DisplayName("Chief Information Officer")]
		CIO = 900,
		[DisplayName("Chief Operating Officer")]
		COO = 700,
		[DisplayName("Chief Procurement Officer")]
		CPO = 600,
		[DisplayName("Chief Revenue Officer")]
		CRO = 800,
		[DisplayName("Client Support and Relationships Manager")]
		CSRManager = 1001,
		[DisplayName("Client Support and Relationships Worker")]
		CSRWorker = 1002,
		[DisplayName("Finance and Business Intelligence Manager")]
		FBIManager = 301,
		[DisplayName("Finance and Business Intelligence Worker")]
		FBIWorker = 302,
		[DisplayName("Human Resources Manager")]
		HRManager = 401,
		[DisplayName("Human Resources Worker")]
		HRWorker = 402,
		[DisplayName("Information Technologies Manager")]
		ITManager = 901,
		[DisplayName("Information Technologies Worker")]
		ITWorker = 902,
		[DisplayName("Logistics and Procurement Manager")]
		LnPManager = 601,
		[DisplayName("Logistics and Procurement Worker")]
		LnPWorker = 602,
		[DisplayName("Operations Manager")]
		OpsManager = 701,
		[DisplayName("Operations Worker")]
		OpsWorker = 702,
		[DisplayName("Administration Manager")]
		AdmManager = 201,
		[DisplayName("Administration Worker")]
		AdmWorker = 202,
		[DisplayName("Research and Development Manager")]
		RnDManager = 501,
		[DisplayName("Research and Development Worker")]
		RnDWorker = 502,
		[DisplayName("Sales and Marketing Manager")]
		SnMManager = 801,
		[DisplayName("Sales and Marketing Worker")]
		SnMWorker = 802,
		[DisplayName("Web Site User")]
		WebUser = 0,
	}
}

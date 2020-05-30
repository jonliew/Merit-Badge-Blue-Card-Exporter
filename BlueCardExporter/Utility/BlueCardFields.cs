namespace BlueCardExporter.Utility
{
    public class BlueCardFields
    {
        public const string BlueCardFormName = "BlueCardForm.pdf";

        // PDF form field names
        public const string StudentName = "Name1";
        public const string Address = "Address1";
        public const string CityStateZip = "CityStateZip1";
        public const string UnitTypeRB = "UnitTypeRB";
        public const string UnitType = "UnitType";
        public const string UnitNumber = "UnitNumber";
        public const string District = "District";
        public const string Council = "Council";

        // Append "1", "2", or "" to each for blue cards 1, 2, and 3 on the PDF
        // For requirements, append ".##" in addition to the above
        public const string MeritBadgeName = "MeritBadge";
        public const string DateApplied = "DateApplied";

        public const string Requirement = "Requirement";
        public const string RequirementDate = "ReqDate";
        public const string RequirementInitials = "ReqInitial";

        public const string Counselor = "Counselor";
        public const string CounselorAddress = "CounselorAddress";
        public const string CounselorCityStateZip = "CounselorCityStateZip";
        public const string CounselorPhone = "CounselorPhone";
        public const string DateCompleted = "DateCompleted";
        public const string Remarks = "Remarks";

        // Void fields
        public const string BlueCardVoid1 = "Void"; // without the 1 but applies to the second blue card
        public const string BlueCardVoid2 = "Void2";

    }
}

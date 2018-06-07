namespace programmersdigest.MT940Parser
{
    public class Information
    {
        public int TransactionCode { get; internal set; }
        public string PostingText { get; internal set; }
        public string JournalNumber { get; internal set; }
        public string OperationDescription { get; internal set; } = "";
        public System.DateTime UploadDate { get; internal set; }
        public string ContragentName { get; internal set; } = "";
        public string ContragentAddress { get; internal set; } = "";
        public string BankCodeOfPayer { get; internal set; }
        public string AccountIDOfPayer { get; internal set; }
        public string AccountNumberOfPayer { get; internal set; }
        public string NameOfPayer { get; internal set; }
        public string AddressOfPayer { get; internal set; }
        public int? TextKeyAddition { get; internal set; }
        public string EndToEndReference { get; internal set; }
        public string CustomerReference { get; internal set; }
        public string MandateReference { get; internal set; }
        public string CreditorReference { get; internal set; }
        public string OriginatorsIdentificationCode { get; internal set; }
        public string CompensationAmount { get; internal set; }
        public string OriginalAmount { get; internal set; }
        public string SepaRemittanceInformation { get; internal set; }
        public string PayersReferenceParty { get; internal set; }
        public string CreditorsReferenceParty { get; internal set; }
        public string UnstructuredRemittanceInformation { get; internal set; }
    }
}
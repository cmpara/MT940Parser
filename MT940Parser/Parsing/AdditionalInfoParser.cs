using System;
using System.IO;
using System.Text.RegularExpressions;

namespace programmersdigest.MT940Parser.Parsing
{
    internal class AdditionalInfoParser
    {
        private StringReader _reader;
        private char _separator;
        private string _lastRemittanceIdentifier;

        //erase all multi whitespace characters
        Regex regex = new Regex("[ ]{2,}", RegexOptions.None);

        public Information ParseInformation(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value), "Value may not be null");
            }

            _reader = new StringReader(value);
            _separator = default(char);
            _lastRemittanceIdentifier = null;

            var information = new Information();

            ReadStructuredData(ref information);

            return information;
        }



        private void ReadTransactionCode(ref Information information)
        {
            var value = _reader.ReadWhile(c => char.IsNumber(c), 4);
            if (value.Length > 0)
            {
                information.TransactionCode = int.Parse(value);
            }
        }

        private void ReadStructuredData(ref Information information)
        {
            //remove all new line symbols
            ReadTransactionCode(ref information);

            var value = _reader.Read();
            value = value.Replace("\r", "").Replace("\n", "").Replace("\t", "");
            _reader = new StringReader(value);

            DetectSeparator(ref information);
        }


        private void DetectSeparator(ref Information information)
        {
            var value = _reader.ReadWhile(c => char.IsPunctuation(c) || char.IsSymbol(c), 1);
            if (value.Length < 1)
            {
                _separator = '\n';
                information.OperationDescription += ReadValue();
                return;
            }

            _separator = value[0];      // can be any special character but not white space symbol, letter or digit.

            DetectFieldCode(ref information);
        }

        private void ReadSeparator(ref Information information)
        {
            var value = _reader.Read(1);
            if (value.Length < 1)
            {
                return;     // End of field contents.
            }

            if (value[0] != _separator)
            {
                throw new InvalidDataException($"Unexpected data \"{value}\". Expected separator \"{_separator}\"");
            }

            DetectFieldCode(ref information);
        }

        private void DetectFieldCode(ref Information information)
        {
            var value = _reader.Read(2);
            if (value.Length < 2)
            {
                throw new InvalidDataException("Unexpected end of statement. Expected \"Field Code\"");
            }
            if (!char.IsDigit(value[0]))
            {
                value += _reader.ReadWhile(c => c != ':').Trim();
                value += _reader.Read(1);
                value = value.Trim();
            }

            switch (value)
            {
                case "00":
                    information.PostingText = ReadValue();
                    break;
                case "10":
                    information.JournalNumber = ReadValue();
                    break;
                case "20":
                    information.OperationDescription = ReadValue();
                    break;
                case "21":
                case "22":
                case "23":
                case "24":
                case "25":
                case "TYT.:":
                    information.OperationDescription += ReadValue();
                    break;
                case "26":
                    information.UploadDate = DateParser.Parse(ReadValue());
                    break;
                case "27":
                case "28":
                    information.ContragentName += ReadValue();
                    break;
                case "29":
                case "60":
                    information.ContragentAddress += ReadValue();
                    break;

                case "30":
                    information.BankCodeOfPayer = ReadValue();
                    break;
                case "31":
                    information.AccountIDOfPayer = ReadValue();
                    break;
                case "32":
                case "DLA:":
                    ReadNameOfPayer(ref information);
                    break;
                case "33":
                    information.AddressOfPayer = ReadValue();
                    break;
                case "34":
                    ReadTextKeyAddition(ref information);
                    break;
                case "38":
                    information.AccountNumberOfPayer = ReadValue();
                    break;
                case "61":
                case "62":
                case "63":
                case "REF. KLIENTA:":
                    ReadRemittanceInformation(ref information);
                    break;
                default:
                    information.UnstructuredRemittanceInformation += ReadValue();
                    break;
            }

            ReadSeparator(ref information);
        }

        private void ReadNameOfPayer(ref Information information)
        {
            information.NameOfPayer += ReadValue();
        }

        private void ReadTextKeyAddition(ref Information information)
        {
            var value = ReadValue();
            information.TextKeyAddition = int.Parse(value);
        }

        private string ReadValue()
        {
            return regex.Replace(_reader.ReadWhile(c => c != _separator).Trim(), " ");
        }

        private void ReadRemittanceInformation(ref Information information)
        {
            var value = _reader.Read(5);
            if (!DetectRemittanceIdentifier(value, ref information))
            {
                _reader.Skip(-value.Length);   // Revert read

                // Could not detect identifier. Try to append to last one.
                if (_lastRemittanceIdentifier != null)
                {
                    DetectRemittanceIdentifier(_lastRemittanceIdentifier, ref information);
                }
                else
                {
                    information.UnstructuredRemittanceInformation += ReadValue();
                }
            }
        }

        private bool DetectRemittanceIdentifier(string identifier, ref Information information)
        {
            switch (identifier)
            {
                case "EREF+":
                    information.EndToEndReference += ReadValue();
                    break;
                case "KREF+":
                    information.CustomerReference += ReadValue();
                    break;
                case "MREF+":
                    information.MandateReference += ReadValue();
                    break;
                case "CRED+":
                    information.CreditorReference += ReadValue();
                    break;
                case "DEBT+":
                    information.OriginatorsIdentificationCode += ReadValue();
                    break;
                case "COAM+":
                    information.CompensationAmount += ReadValue();
                    break;
                case "OAMT+":
                    information.OriginalAmount += ReadValue();
                    break;
                case "SVWZ+":
                    information.SepaRemittanceInformation += ReadValue();
                    break;
                case "ABWA+":
                    information.PayersReferenceParty += ReadValue();
                    break;
                case "ABWE+":
                    information.CreditorsReferenceParty += ReadValue();
                    break;
                default:
                    return false;
            }

            _lastRemittanceIdentifier = identifier;
            return true;
        }
    }
}

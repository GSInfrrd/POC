using System.Collections.Generic;
using System.Text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

namespace CourtlandDox.Util
{
    public class Helper
    {
        public static string ExtractTextFromPdf(string path)
        {
            StringBuilder text = new StringBuilder();
            using (PdfReader reader = new PdfReader(path))
            {
                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    text.Append(PdfTextExtractor.GetTextFromPage(reader, i));
                }
            }
            return text.ToString();
        }


        public static Bank GetBank(string extractedText)
        {
            Bank resBank = Bank.None;
            var resText = extractedText.ToLower();
            var banklist = new List<string>()
            {
                "JPMorgan",
                "Wells Fargo",
                "Bank Of America",
                "Barclays",
                "Citi",
                "Deutsche",
                "Antares",
                "Goldman",
                "Macquarie",
                "UbsFortress",
                "@GE"
            };
            var foundBank = false;
            foreach (var bank in banklist)
            {
                if (resText.Contains(bank.ToLower()))
                {
                    resBank = GetBankEnum(bank);
                    if (resBank != Bank.None)
                        foundBank = true;
                }
                if (foundBank) break;
            }
            return resBank;
        }

        private static Bank GetBankEnum(string bank)
        {
            Bank resBank = Bank.None;
            switch (bank)
            {
                case "Wells Fargo":
                    resBank = Bank.WellsFargo;
                    break;
                case "Bank Of America":
                    resBank = Bank.BankOfAmerica;
                    break;
                case "Barclays":
                    resBank = Bank.Barclays;
                    break;
                case "Citi":
                    resBank = Bank.Citi;
                    break;
                case "Deutsche":
                    resBank = Bank.Deutsche;
                    break;
                case "Antares":
                    resBank = Bank.Antares;
                    break;
                case "@GE":
                    resBank = Bank.Antares;
                    break;
                case "Goldman":
                    resBank = Bank.Goldman;
                    break;
                case "JPMorgan":
                    resBank = Bank.JpMorgan;
                    break;
                case "Macquarie":
                    resBank = Bank.Macquarie;
                    break;
                case "UbsFortress":
                    resBank = Bank.UbsFortress;
                    break;
            }
            return resBank;
        }
    }
}
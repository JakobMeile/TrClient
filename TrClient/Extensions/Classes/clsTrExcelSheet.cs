using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows;
using Excel = Microsoft.Office.Interop.Excel;
using System.Reflection;
using TrClient;

namespace TrClient
{
    public class clsTrExcelSheet
    {
        private Excel.Application oXL;
        private Excel._Workbook oWB;
        private Excel._Worksheet oSheet;
        private Excel.Range oRng;

        private Dictionary<string, int> HeaderColumns = new Dictionary<string, int>();
        private int CurrentRow = 1;

        public clsTrExcelSheet()
        {
            try
            {
                //Start Excel and get Application object.
                oXL = new Excel.Application();
                oXL.Visible = true;

                //Get a new workbook.
                oWB = (Excel._Workbook)(oXL.Workbooks.Add(Missing.Value));
                oSheet = (Excel._Worksheet)oWB.ActiveSheet;

                ////Add table headers going cell by cell.
                //oSheet.Cells[1, 1] = "First Name";
                //oSheet.Cells[1, 2] = "Last Name";
                //oSheet.Cells[1, 3] = "Full Name";
                //oSheet.Cells[1, 4] = "Salary";

                ////Format A1:D1 as bold, vertical alignment = center.
                //oSheet.get_Range("A1", "D1").Font.Bold = true;
                //oSheet.get_Range("A1", "D1").VerticalAlignment =
                //Excel.XlVAlign.xlVAlignCenter;

                //// Create an array to multiple values at once.
                //string[,] saNames = new string[5, 2];

                //saNames[0, 0] = "John";
                //saNames[0, 1] = "Smith";
                //saNames[1, 0] = "Tom";
                //saNames[1, 1] = "Brown";
                //saNames[2, 0] = "Sue";
                //saNames[2, 1] = "Thomas";
                //saNames[3, 0] = "Jane";
                //saNames[3, 1] = "Jones";
                //saNames[4, 0] = "Adam";
                //saNames[4, 1] = "Johnson";

                ////Fill A2:B6 with an array of values (First and Last Names).
                //oSheet.get_Range("A2", "B6").Value2 = saNames;

                ////Fill C2:C6 with a relative formula (=A2 & " " & B2).
                //oRng = oSheet.get_Range("C2", "C6");
                //oRng.Formula = "=A2 & \" \" & B2";

                ////Fill D2:D6 with a formula(=RAND()*100000) and apply format.
                //oRng = oSheet.get_Range("D2", "D6");
                //oRng.Formula = "=RAND()*100000";
                //oRng.NumberFormat = "$0.00";

                ////AutoFit columns A:D.
                //oRng = oSheet.get_Range("A1", "D1");
                //oRng.EntireColumn.AutoFit();

                //Manipulate a variable number of columns for Quarterly Sales Data.
                // DisplayQuarterlySales(oSheet);

                //Make sure Excel is visible and give the user control
                //of Microsoft Excel's lifetime.
                oXL.Visible = true;
                oXL.UserControl = true;
            }
            catch (Exception e)
            {
                String errorMessage;
                errorMessage = "Error: ";
                errorMessage = String.Concat(errorMessage, e.Message);
                errorMessage = String.Concat(errorMessage, " Line: ");
                errorMessage = String.Concat(errorMessage, e.Source);

                MessageBox.Show(errorMessage, "Error");
            }
        }

        public void AddHeaders(List<string> Headers)
        {
            int c = 0;
            foreach(string H in Headers)
            {
                c++;
                HeaderColumns[H] = c;
                oSheet.Cells[CurrentRow, c] = H;
                oSheet.Cells[CurrentRow, c].Font.Bold = true;
            }
            CurrentRow++;
        }

        private int GetColumn(string Header)
        {
            int temp = 0;
            if (HeaderColumns.TryGetValue(Header, out temp))
            {
                return temp;
            }
            else
            {
                return -1;
            }
        }

        public void AddLemma(clsTrLemma trLemma)
        {
            oSheet.Cells[CurrentRow, 1] = (trLemma.Content).Trim();
            oSheet.Cells[CurrentRow, 2] = (trLemma.Stripped).Trim();
            oSheet.Cells[CurrentRow, 3] = (trLemma.OccurrenceCount);

            CurrentRow++;

        }

        public void AddRecord(clsTrParagraphs Paragraphs)
        {
            int Column;
            string Temp;

            foreach(clsTrParagraph P in Paragraphs)
            {
                Column = GetColumn(P.Name);
                if (Column > 0)
                {
                    Temp = Convert.ToString(oSheet.Cells[CurrentRow, Column].Value2);
                    oSheet.Cells[CurrentRow, Column] = (Temp + " " + P.Content).Trim();
                }
                    
            }
            CurrentRow++;
        }

    }
}

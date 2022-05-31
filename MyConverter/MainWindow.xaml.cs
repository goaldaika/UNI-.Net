using System.Windows;
using System.Windows.Input;
using System.Text.RegularExpressions;
using System.Data;
using System.Threading;
using System.Reflection;
using System;
using System.IO;
using System.Xml;
using System.Diagnostics;

namespace CurrencyConverter_Static
{
    public class MyListener : TraceListener
        {
            public override void Write(string message)
            {
                using (FileStream fs = new FileStream("mylog.txt", FileMode.OpenOrCreate, FileAccess.Write))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        sw.WriteLine(DateTime.Now.ToString());
                        sw.Write(message);
                    }
                }
            }

            public override void WriteLine(string message)
            {
                using (FileStream fs = new FileStream("mylog.txt", FileMode.OpenOrCreate, FileAccess.Write))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        sw.WriteLine(DateTime.Now.ToString());
                        sw.WriteLine(message);
                    }
                }
            }
        }
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //ClearControls method is used to clear all control values
            ClearControls();

            //BindCurrency is used to bind currency name with the value in the Combobox
            BindCurrency();
        }

        #region Bind Currency From and To Combobox
        private void BindCurrency()

        {
            //Create a Datatable Object
            DataTable dtCurrency = new DataTable();

            //Add the text column in the DataTable
            dtCurrency.Columns.Add("Text");

            //Add the value column in the DataTable
            dtCurrency.Columns.Add("Value");

            //Add rows in the Datatable with text and value
            dtCurrency.Rows.Add("--SELECT--", 0);
            dtCurrency.Rows.Add("INR", "INR");
            dtCurrency.Rows.Add("USD", "USD");
            dtCurrency.Rows.Add("EUR", "EUR");
            dtCurrency.Rows.Add("SAR", "SAR");
            dtCurrency.Rows.Add("POUND", "POUND");
            dtCurrency.Rows.Add("DEM", "DEM");
            dtCurrency.Rows.Add("CZK", "CZK");
            dtCurrency.Rows.Add("VND", "VND");

            //Datatable data assigned from the currency combobox
            cmbFromCurrency.ItemsSource = dtCurrency.DefaultView;

            //DisplayMemberPath property is used to display data in the combobox
            cmbFromCurrency.DisplayMemberPath = "Text";

            //SelectedValuePath property is used to set the value in the combobox
            cmbFromCurrency.SelectedValuePath = "Value";

            //SelectedIndex property is used to bind the combobox to its default selected item 
            cmbFromCurrency.SelectedIndex = 0;

            //All properties are set to To Currency combobox as it is in the From Currency combobox
            cmbToCurrency.ItemsSource = dtCurrency.DefaultView;
            cmbToCurrency.DisplayMemberPath = "Text";
            cmbToCurrency.SelectedValuePath = "Value";
            cmbToCurrency.SelectedIndex = 0;
        }
        #endregion

        #region Button Click Event

        //Convert the button click event
        private void Convert_Click(object sender, RoutedEventArgs e)
        {

            //Create the variable as ConvertedValue with double datatype to store currency converted value
            double ConvertedValue;

            //Check if the amount textbox is Null or Blank
            if (txtCurrency.Text == null || txtCurrency.Text.Trim() == "")
            {
                //If amount textbox is Null or Blank it will show this message box
                MessageBox.Show("Please Enter Currency", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                //After clicking on messagebox OK set focus on amount textbox
                txtCurrency.Focus();
                return;
            }
            //Else if currency From is not selected or select default text --SELECT--
            else if (cmbFromCurrency.SelectedValue == null || cmbFromCurrency.SelectedIndex == 0)
            {
                //Show the message
                MessageBox.Show("Please Select Currency From", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                //Set focus on the From Combobox
                cmbFromCurrency.Focus();
                return;
            }
            //Else if currency To is not selected or select default text --SELECT--
            else if (cmbToCurrency.SelectedValue == null || cmbToCurrency.SelectedIndex == 0)
            {
                //Show the message
                MessageBox.Show("Please Select Currency To", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                //Set focus on the To Combobox
                cmbToCurrency.Focus();
                return;
            }

            //Check if From and To Combobox selected values are same
            if (cmbFromCurrency.Text == cmbToCurrency.Text)
            {
                //Amount textbox value set in ConvertedValue.
                //double.parse is used for converting the datatype String To Double.
                //Textbox text have string and ConvertedValue is double Datatype
                ConvertedValue = double.Parse(txtCurrency.Text);

                //Show the label converted currency and converted currency name and ToString("N3") is used to place 000 after the dot(.)
                lblCurrency.Content = cmbToCurrency.Text + " " + ConvertedValue.ToString("N3");
            }
            else try
            {
                //Calculating the converted value

                ConvertedValue = ConvertMoney(cmbFromCurrency.SelectedValue.ToString(), double.Parse(txtCurrency.Text), cmbToCurrency.SelectedValue.ToString());

                //Show the label converted currency and converted currency name.
                lblCurrency.Content = cmbToCurrency.Text + " " + ConvertedValue.ToString("N3");
                storeData(cmbFromCurrency.SelectedValue.ToString(), txtCurrency.Text, cmbToCurrency.SelectedValue.ToString(), ConvertedValue.ToString("N3"));
                }
                catch
                {
                    Debug.Fail("Something happend!");
                }
        }

        //Clear Button click event
        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            //ClearControls method is used to clear all controls value
            ClearControls();
        }
        #endregion

        #region Extra Events

        //ClearControls method is used to clear all controls value
        private void ClearControls()
        {
            txtCurrency.Text = string.Empty;
            if (cmbFromCurrency.Items.Count > 0)
                cmbFromCurrency.SelectedIndex = 0;
            if (cmbToCurrency.Items.Count > 0)
                cmbToCurrency.SelectedIndex = 0;
            lblCurrency.Content = "";
            txtCurrency.Focus();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e) //Allow Only Integer in Text Box
        {
            //Regular Expression is used to add regex.
            // Add Library using System.Text.RegularExpressions;
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        #endregion
        //-----------------------------------------------------------------------------------------------------------------------------------
        #region get exchange ratio from external library
        static double ReturnRate(string Rate)
        {
            double moneyRate = 0;
            double moneyRate2 = 0;

            Thread t0 = new Thread(() => {
                Assembly myAssembly1 = Assembly.LoadFile(System.IO.Path.GetFullPath("../../../../../CurrencyConverter/RateLib/bin/Debug/net5.0/RateLib.dll"));
                //Assembly myAssembly = Assembly.LoadFile(System.IO.Path.GetFullPath("C:/Users/Goal/source/repos/MyCurrencyConverter/RateLib3/bin/Debug/net5.0/RateLib3.dll"));


                Type rateType = myAssembly1.GetType("RateLib.CurrencyRate");
                
                object myRateRef = Activator.CreateInstance(rateType);

                MethodInfo getRate = rateType.GetMethod("getRate");
         
                moneyRate = (double)getRate.Invoke(myRateRef, new object[] { Rate });
            });
            t0.Start();


            Thread t1 = new Thread(() => {
                Assembly myAssembly2 = Assembly.LoadFile(System.IO.Path.GetFullPath("../../../../../CurrencyConverter/RateLib2/bin/Debug/net5.0/RateLib2.dll"));

                Type rateType2 = myAssembly2.GetType("RateLib2.CurrencyRate");

                object myRateRef2 = Activator.CreateInstance(rateType2);

                MethodInfo getRate2 = rateType2.GetMethod("getRate");

                moneyRate2 = (double)getRate2.Invoke(myRateRef2, new object[] { Rate });
            });
            t1.Start();

            t0.Join();
            t1.Join();


            return (moneyRate + moneyRate2) / 2;
        }
        #endregion

        static double ConvertMoney(string rateIn, double money, string rateOut)
        {
            return money / ReturnRate(rateIn) * ReturnRate(rateOut);
        }

        #region generate xml doc
        static void storeData(string rateIn, string moneyIn, string rateOut, string moneyOut)
        {


            if (File.Exists("ConvertHistory.xml"))
            {
                XmlDocument processedData = new XmlDocument();
                processedData.Load("ConvertHistory.xml");

                XmlNode root = processedData.SelectSingleNode("/Root");

                XmlNode exchangeNode = processedData.CreateElement("Exchange");
                root.AppendChild(exchangeNode);

                XmlAttribute rateInAttr = processedData.CreateAttribute("RateIn");
                rateInAttr.Value = rateIn;
                exchangeNode.Attributes.Append(rateInAttr);

                XmlAttribute rateOutAttr = processedData.CreateAttribute("RateOut");
                rateOutAttr.Value = rateOut;
                exchangeNode.Attributes.Append(rateOutAttr);

                XmlNode myMoneyInNode = processedData.CreateElement("MoneyIn");
                myMoneyInNode.AppendChild(processedData.CreateTextNode(moneyIn));
                exchangeNode.AppendChild(myMoneyInNode);

                XmlNode myMoneyOutNode = processedData.CreateElement("MoneyOut");
                myMoneyOutNode.AppendChild(processedData.CreateTextNode(moneyOut));
                exchangeNode.AppendChild(myMoneyOutNode);


                processedData.Save("ConvertHistory.xml");
            }
            else
            {
                XmlDocument processedData = new XmlDocument();
                processedData.AppendChild(processedData.CreateXmlDeclaration("1.0", "UTF-8", "no"));

                XmlNode root = processedData.CreateElement("Root");
                processedData.AppendChild(root);

                XmlNode exchangeNode = processedData.CreateElement("Exchange");
                root.AppendChild(exchangeNode);

                XmlAttribute rateInAttr = processedData.CreateAttribute("RateIn");
                rateInAttr.Value = rateIn;
                exchangeNode.Attributes.Append(rateInAttr);

                XmlAttribute rateOutAttr = processedData.CreateAttribute("RateOut");
                rateOutAttr.Value = rateOut;
                exchangeNode.Attributes.Append(rateOutAttr);

                XmlNode myMoneyInNode = processedData.CreateElement("MoneyIn");
                myMoneyInNode.AppendChild(processedData.CreateTextNode(moneyIn));
                exchangeNode.AppendChild(myMoneyInNode);

                XmlNode myMoneyOutNode = processedData.CreateElement("MoneyOut");
                myMoneyOutNode.AppendChild(processedData.CreateTextNode(moneyOut));
                exchangeNode.AppendChild(myMoneyOutNode);

                processedData.Save("ConvertHistory.xml");


            }



        }
        #endregion

    }
}
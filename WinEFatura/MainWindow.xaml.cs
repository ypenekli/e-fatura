using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Xsl;
using System.IO;
using com.yp.efatura.forms;
using System.Globalization;
using System.Threading;
using System.Windows.Markup;
using Microsoft.Win32;

//üstte yeni satır eklendi.
//açıklama notu eklendi.
//ikinci açıklama eklendi.
//üçüncü açıklama eklendi.
//future 1 dördüncü açıklama eklendi.
//dördüncü açıklama eklendi.
namespace com.yp.efatura {
    public partial class MainWindow : Window {

        private data.vkFatura datasource;
        private data.vkFatura.faturaiciDataTable faturaicisource;
        private data.vkFatura.faturalarDataTable faturasource;
        private String faturaDosyaAdi;

        public MainWindow() {
            //do this if set locale only at this form
            // this.Language = XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag);
            InitializeComponent();

            datasource = new data.vkFatura();
            faturasource = datasource.faturalar;
            faturaicisource = datasource.faturaici;
            tblFaturaici.ItemsSource = faturaicisource;
        }

        private void btnFill_Click(object sender, RoutedEventArgs e) {
            faturaicisource.BeginLoadData();
            faturaicisource.Clear();
            faturaicisource.AddfaturaiciRow(1, "Stok-0001", 12, "C62", 55, (decimal)106.92, (decimal)0.18, 66, (decimal)0.1, 660, 594, (decimal)700.92, "Mouse", "Adet");
            faturaicisource.AddfaturaiciRow(2, "Stok-0002", 7, "C62", 110, (decimal)131.67, (decimal)0.18, (decimal)38.5, (decimal)0.05, 770, (decimal)731.5, (decimal)863.17, "Klavye", "Adet");
            faturaicisource.AddfaturaiciRow(3, "Stok-0003", 9, "C62", 1250, (decimal)1964.25, (decimal)0.18, (decimal)337.5, (decimal)0.03, 11250, (decimal)10912.50, (decimal)12876.75, "Monitör", "Adet");
            faturaicisource.AcceptChanges();
            faturaicisource.EndLoadData();

            faturasource.BeginLoadData();
            faturasource.Clear();
            data.vkFatura.faturalarRow row1 = faturasource.AddfaturalarRow(1, DateTime.Now, 1, 2, "EAR2019000000001", "TRY", 0, 0, 0, 0, 0, "120 10 01 001", "Abc Yazılım A.Ş.", "120 10 01 002", "Hasan Yılmaz");
            row1.tutar = faturaicisource.Sum(x => x.tutar);
            row1.iskonto = faturaicisource.Sum(x => x.iskonto);
            row1.nettutar = faturaicisource.Sum(x => x.nettutar);
            row1.kdv = faturaicisource.Sum(x => x.kdv);
            row1.toplamtutar = faturaicisource.Sum(x => x.toplamtutar);
            faturasource.AcceptChanges();
            faturasource.EndLoadData();
            this.DataContext = faturasource.First();

            btnCreate.IsEnabled = faturaicisource.Count > 0;
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e) {
            var fatura = faturasource.First();
            
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "XML dosyaları (*.xml)|*.xml";
            saveFileDialog.InitialDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
            saveFileDialog.FileName = $@"{ fatura.faturanu }.xml";            
            if (saveFileDialog.ShowDialog() == true) {
                var urlfile = saveFileDialog.FileName;

                var converter = new model.InvoiceConverter();
                var invoice = converter.ToInvoice(datasource, Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "general.xslt"));
                var settings = new XmlWriterSettings { OmitXmlDeclaration = true, Indent = true };
                var ms = new MemoryStream();
                var writer = XmlWriter.Create(ms, settings);
                var srl = new XmlSerializer(invoice.GetType());
                srl.Serialize(writer, invoice, XmlNameSpace());
                ms.Flush();
                ms.Seek(0, SeekOrigin.Begin);
                var srRead = new StreamReader(ms);
                var readXml = srRead.ReadToEnd();
                using (var sWriter = new StreamWriter(urlfile, false, Encoding.UTF8)) {
                    sWriter.Write(readXml);
                    sWriter.Close();
                }
                var urldir = Path.GetDirectoryName(urlfile);
                var message = $@"Fatura başarılı bir şekilde {urldir} konumunda oluşturuldu";
                MessageBox.Show(message);                
            }

            XmlSerializerNamespaces XmlNameSpace() {
                var ns = new XmlSerializerNamespaces();
                ns.Add("cac", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2");
                ns.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
                ns.Add("xades", "http://uri.etsi.org/01903/v1.3.2#");
                ns.Add("udt", "urn:un:unece:uncefact:data:specification:UnqualifiedDataTypesSchemaModule:2");
                ns.Add("ubltr", "urn:oasis:names:specification:ubl:schema:xsd:TurkishCustomizationExtensionComponents");
                ns.Add("qdt", "urn:oasis:names:specification:ubl:schema:xsd:QualifiedDatatypes-2");
                ns.Add("ext", "urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2");
                ns.Add("cbc", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2");
                ns.Add("ccts", "urn:un:unece:uncefact:documentation:2");
                ns.Add("ds", "http://www.w3.org/2000/09/xmldsig#");
                return ns;
            }
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e) {
            showInvoice(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "EArsivFaturalar", faturaDosyaAdi));
        }

        private void showInvoice(String urlXml) {
            String encodedText = model.Transformer.findXsltString(urlXml);
            var frm = new FrmPrint(model.Transformer.XmlToHtml(encodedText, urlXml));
            frm.ShowDialog();
        }

        private void btnXmlRead_Click(object sender, RoutedEventArgs e) {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            //set filter
            // openFileDialog.Filter = "XML dosyaları (*.xml)|*.txt|All files (*.*)|*.*";
            openFileDialog.Filter = "XML dosyaları (*.xml)|*.xml";

            //set initial directory
            //  openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            openFileDialog.InitialDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
            if (openFileDialog.ShowDialog() == true) {
                showInvoice(openFileDialog.FileName);
            }
        }
    }
}

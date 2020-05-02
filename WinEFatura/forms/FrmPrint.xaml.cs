using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace com.yp.efatura.forms {
    /// <summary>
    /// Yazdirma islemleri
    /// </summary>
    public partial class FrmPrint : Window {
        public FrmPrint(String invoiceText) {
            InitializeComponent();
            webbrowser.NavigateToString(invoiceText);
        }
        //test değişiklik
        private void btnPrint1_Click(object sender, RoutedEventArgs e) {
            webbrowser.InvokeScript("execScript", new object[] { "window.print();", "JavaScript" });
        }

        private void btnPrint2_Click(object sender, RoutedEventArgs e) {
            webbrowser.InvokeScript("execScript", new object[] { "window.print();", "JavaScript" });
        }
    }
}

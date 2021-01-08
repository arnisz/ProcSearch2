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
using System.Windows.Shapes;

namespace ProcSearch2
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            lblStatus.Text = "Es ist noch kein Datenbestand geladen...";
            ProcDB.RRChanged += Update;
        }

        private async void Menu_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                switch ((string)(e.Source as MenuItem).Uid)
                {
                    case "oeffnen":
                        await MenuLogic.FileOpen();
                        break;
                    case "beenden":
                        Application.Current.Shutdown();
                        break;

                }
                lblStatus.Text = ProcDB.GetInstace.Count.ToString();
                //   MessageBox.Show((e.Source as MenuItem).Name);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }


        }

        private void Update(object sender, EventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                DirectoryChangedEventArgs m = (DirectoryChangedEventArgs)e;
                lblStatus.Text = m.Counter.ToString() + " Ordner und Unterordner werden durchsucht...";
            });

        }

        private void BtnSuchen_Click(object sender, RoutedEventArgs e)
        {
            TextBox b = txtbxSuchbegriff;
            int cnt = 0;
            lstvErgebnisse.Items.Clear();
            try
            {

                //var pr = from p in ProcDB.GetInstace.ProcedureList where p.Name ==  b.Text select p ;
                // var result = ProcDB.GetInstace.ProcedureList.Where(o => o.Description.Contains(b.Text.ToUpper()));

                var procedures = ProcDB.GetInstace.ProcedureList;
                char[] separators = new char[] { ',', ';' };
                string[] bb = b.Text.ToUpper().Split(separators);
                var result = from p in procedures where bb.All(s => p.Description.Contains(s)) select p;

                foreach (var proc in result)
                {
                    try
                    {
                        if ((bool)CheckBox_IncludeSub.IsChecked)
                        {
                            if ((bool)RadioButton_Approved.IsChecked)
                            {
                                //Nur freigegebene Anzeigen
                                if (!proc.IsDraft && !proc.Path.Contains("00") && !proc.Path.Contains("02") && !proc.Path.Contains("pielwies"))
                                {
                                    lstvErgebnisse.Items.Add(proc);
                                    cnt++;
                                }

                            }
                            else if ((bool)RadioButton_Draft.IsChecked && !proc.Path.Contains("pielwies"))
                            {
                                //Entwurf und Freigegebene Anzeigen
                                if (!proc.Path.Contains("00"))
                                {
                                    lstvErgebnisse.Items.Add(proc);
                                    cnt++;
                                }
                            }
                            else if ((bool)RadioButton_Experimetal.IsChecked)
                            {
                                lstvErgebnisse.Items.Add(proc);
                                cnt++;
                            }
                        }
                        else
                        {
                            if ((bool)RadioButton_Approved.IsChecked)
                            {
                                //Nur freigegebene Anzeigen
                                if (!proc.IsDraft && !proc.Path.Contains("00") && !proc.Path.Contains("02") && !proc.IsSub && !proc.Path.Contains("pielwies"))
                                {
                                    lstvErgebnisse.Items.Add(proc);
                                    cnt++;
                                }

                            }
                            else if ((bool)RadioButton_Draft.IsChecked && !proc.IsSub && !proc.Path.Contains("pielwies"))
                            {
                                //Entwurf und Freigegebene Anzeigen
                                if (!proc.Path.Contains("00"))
                                {
                                    lstvErgebnisse.Items.Add(proc);
                                    cnt++;
                                }
                            }
                            else if ((bool)RadioButton_Experimetal.IsChecked && !proc.IsSub)
                            {
                                lstvErgebnisse.Items.Add(proc);
                                cnt++;
                            }
                        }
                    }
                    catch (InvalidOperationException ex)
                    {
                        MessageBox.Show(ex.StackTrace.ToString());
                    }
                }
                if (cnt == 0)
                {
                    lstvErgebnisse.Items.Add("--keine Treffer--  " + DateTime.Now.ToString());
                    Label_ProceduresInList.Content = cnt.ToString() + " Prozeduren gefunden.";
                }
                else
                {
                    Label_ProceduresInList.Content = cnt.ToString() + " Prozeduren gefunden.";
                }


            }
            catch (System.ArgumentException ex)
            {
                MessageBox.Show(ex.ToString());
            }


        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(" Eine Prozedursuche von Arnold Szathmary. \n wird 'WIE SIE IST' \n OHNE JEGLICHE GARANTIE, WEDER AUSDRÜCKLICH NOCH STILLSCHWEIGEND, zur Verfügung gestellt,  ");
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void LstvErgebnisse_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {

                System.Windows.Controls.ListView l = (System.Windows.Controls.ListView)sender;
                if (l.SelectedItem != null && l.SelectedItem.GetType() == typeof(Proc))
                {
                    string textData = ((Proc)l.SelectedItem).Path;
                    Clipboard.SetData(DataFormats.Text, (Object)textData);
                }

            }
            catch (NullReferenceException ex)
            {
                MessageBox.Show(ex.ToString());

            }

        }
    }
}

using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Threading.Tasks;

namespace ProcSearch2
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            lblStatus.Text = "Es ist noch kein Datenbestand geladen...";
            ProcDb.RrChanged += Update;
            Alias a = Alias.GetInstance;
            a.Load(Properties.Resources.ALIAS_FILE_NAME);
        }

        private async void Menu_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                switch ((e.Source as MenuItem)?.Uid)
                {
                    case "oeffnen":
                        await MenuLogic.FileOpen();
                        break;
                    case "beenden":
                        Application.Current.Shutdown();
                        break;

                }
                lblStatus.Text = ProcDb.GetInstace.Count.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }

        private void Update(object sender, EventArgs e)
        {
            if (Dispatcher.CheckAccess())
            {
                DirectoryChangedEventArgs m = (DirectoryChangedEventArgs)e;
                lblStatus.Text = m.Counter + " Prozeduren wurden geladen...";
            }
            else
            {
                Dispatcher.Invoke(() =>
                {
                    DirectoryChangedEventArgs m = (DirectoryChangedEventArgs)e;
                    lblStatus.Text = m.Counter + " Prozeduren wurden geladen...";
                });
            }


        }

        private void BtnSuchen_Click(object sender, RoutedEventArgs e)
        {
            TextBox b = txtbxSuchbegriff;
            int cnt = 0;
            lstvErgebnisse.Items.Clear();
            try
            {
                var procedures = ProcDb.GetInstace.ProcedureList;
                char[] separators = { ',', ';' };
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
                                if (!proc.IsDraft && !proc.Path.Contains(Properties.Resources.PathContains_Experimental_Indicator) && !proc.Path.Contains(Properties.Resources.PathContains_Draft_Indicator) && !proc.Path.Contains(Properties.Resources.PathContains_Experimental_Indicator_non_Germany))
                                {
                                    lstvErgebnisse.Items.Add(proc);
                                    cnt++;
                                }

                            }
                            else if ((bool)RadioButton_Draft.IsChecked && !proc.Path.Contains(Properties.Resources.PathContains_Experimental_Indicator_non_Germany))
                            {
                                //Entwurf und Freigegebene Anzeigen
                                if (!proc.Path.Contains(Properties.Resources.PathContains_Experimental_Indicator))
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
                                if (!proc.IsDraft && !proc.Path.Contains(Properties.Resources.PathContains_Experimental_Indicator) && !proc.Path.Contains(Properties.Resources.PathContains_Draft_Indicator) && !proc.IsSub && !proc.Path.Contains(Properties.Resources.PathContains_Experimental_Indicator_non_Germany))
                                {
                                    lstvErgebnisse.Items.Add(proc);
                                    cnt++;
                                }

                            }
                            else if ((bool)RadioButton_Draft.IsChecked && !proc.IsSub && !proc.Path.Contains(Properties.Resources.PathContains_Experimental_Indicator_non_Germany))
                            {
                                //Entwurf und Freigegebene Anzeigen
                                if (!proc.Path.Contains(Properties.Resources.PathContains_Experimental_Indicator))
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
                        MessageBox.Show(ex.StackTrace);
                    }
                }
                if (cnt == 0)
                {
                    lstvErgebnisse.Items.Add("--keine Treffer--  " + DateTime.Now);
                    Label_ProceduresInList.Content = cnt + " Prozeduren gefunden.";
                }
                else
                {
                    Label_ProceduresInList.Content = cnt + " Prozeduren gefunden.";
                }


            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.ToString());
            }


        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(Properties.Resources.Disclaimer);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void LstvErgebnisse_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {

                ListView l = (ListView)sender;
                if (l.SelectedItem != null && l.SelectedItem.GetType() == typeof(Proc))
                {
                    string textData = ((Proc)l.SelectedItem).Path;
                    Clipboard.SetData(DataFormats.Text, textData);
                }

            }
            catch (NullReferenceException ex)
            {
                MessageBox.Show(ex.ToString());

            }

        }

        private void Help_OnClick(object sender, RoutedEventArgs e)
        {
            Process.Start("anleitung.htm");
        }

        private async void Speichern_OnClick(object sender, RoutedEventArgs e)
        {
          await Task.Run(()=> ProcDb.GetInstace.SaveToFile("data.psdb"));
        }

        private async void FileOpen_OnClick(object sender, RoutedEventArgs e)
        {
          await Task.Run(()=> ProcDb.GetInstace.ReadFromFile("data.psdb"));
        }
    }
}

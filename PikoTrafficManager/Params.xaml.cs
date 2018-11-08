using PikoTrafficManager.Control;
using PikoTrafficManager.DataPikoClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
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
using System.Windows.Shapes;

namespace PikoTrafficManager
{
    /// <summary>
    /// Logique d'interaction pour Params.xaml
    /// </summary>
    public partial class Params : Window
    {

        public ObservableCollection<Category> Categories = new ObservableCollection<Category>();
        public ObservableCollection<Category> categoriesToDelete = new ObservableCollection<Category>();

        private bool bCreateCategory = false;
        public Params()
        {
            InitializeComponent();
        }

        private void btSaveParams_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Write in config file ...
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                if (rbConnectivityLocal.IsChecked.HasValue && rbConnectivityLocal.IsChecked.Value)
                {
                    config.AppSettings.Settings["Connectivity"].Value = "Local";
                    config.AppSettings.Settings["LocalDataHost"].Value = tbLocalURL.Text;
                }
                else
                {
                    config.AppSettings.Settings["Connectivity"].Value = "Remote";
                    config.AppSettings.Settings["RemoteDataHost"].Value = tbLocalURL.Text;
                }

                if (cbEnablePreview.IsChecked.HasValue && cbEnablePreview.IsChecked.Value)
                    config.AppSettings.Settings["EnablePreview"].Value = "True";
                else
                    config.AppSettings.Settings["EnablePreview"].Value = "False";

                if (cbAutoPlayMedia.IsChecked.HasValue && cbAutoPlayMedia.IsChecked.Value)
                    config.AppSettings.Settings["AutoPlayMedia"].Value = "True";
                else
                    config.AppSettings.Settings["AutoPlayMedia"].Value = "False";

                config.Save();
                ConfigurationManager.RefreshSection("appSettings");
            }
            catch(Exception ex)
            {

            }
            //Save new TemplateField & Categories
            this.DialogResult = true;
            this.Close();
        }

        private void btCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void btAddFieldName_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(tbNewCustomFieldName.Text) && cbFieldType.SelectedIndex > -1)
            {
                DataPikoClient.TemplateFieldData newTemplateField = new DataPikoClient.TemplateFieldData();
                newTemplateField.FieldName = tbNewCustomFieldName.Text;
                newTemplateField.FieldType = (DataPikoClient.TemplateFieldType)cbFieldType.SelectedIndex;

                int idxCategory = lbCategories.SelectedIndex;
                if (idxCategory > -1)
                {
                    List<DataPikoClient.TemplateFieldData> currentCategoriesAF = new List<DataPikoClient.TemplateFieldData>(Categories[idxCategory].TemplateFields);
                    currentCategoriesAF.Add(newTemplateField);
                    Categories[idxCategory].TemplateFields = currentCategoriesAF.ToArray();
                }
                AdditionalField newAdditionalField = new AdditionalField();
                newAdditionalField.Name = "C" + Categories[idxCategory].Id + newTemplateField.FieldType.ToString() + newTemplateField.FieldName.Replace(' ', '_');
                newAdditionalField.lblFieldName.Content = newTemplateField.FieldName;

                string fieldTypeString = "";
                switch (newTemplateField.FieldType)
                {
                    case DataPikoClient.TemplateFieldType.Picture:
                        fieldTypeString = "Picture";
                        break;
                    case DataPikoClient.TemplateFieldType.Video:
                        fieldTypeString = "Video";
                        break;
                    case DataPikoClient.TemplateFieldType.Text:
                        fieldTypeString = "Text";
                        break;
                }

                newAdditionalField.lblFieldType.Content = fieldTypeString;
                newAdditionalField.tbValue.IsEnabled = false;
                newAdditionalField.tbValue.Visibility = Visibility.Hidden;
                spAdditionalCategoryField.Children.Add(newAdditionalField);
            }
        }

        private void lbCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            spAdditionalCategoryField.Children.Clear();
            if (lbCategories.SelectedIndex > -1)
            {
                Category category = this.Categories[lbCategories.SelectedIndex];
                foreach (TemplateFieldData tf in category.TemplateFields)
                {
                    AdditionalField newAdditionalField = new AdditionalField();
                    newAdditionalField.Name = "C" + tf.IdFieldCategory + "_" + tf.Id + "_" + tf.FieldType.ToString() + tf.FieldName.Replace(' ', '_');
                    newAdditionalField.lblFieldName.Content = tf.FieldName;

                    string fieldTypeString = "";
                    switch (tf.FieldType)
                    {
                        case DataPikoClient.TemplateFieldType.Picture:
                            fieldTypeString = "Picture";
                            break;
                        case DataPikoClient.TemplateFieldType.Video:
                            fieldTypeString = "Video";
                            break;
                        case DataPikoClient.TemplateFieldType.Text:
                            fieldTypeString = "Text";
                            break;
                    }

                    newAdditionalField.lblFieldType.Content = fieldTypeString;
                    newAdditionalField.tbValue.IsEnabled = false;
                    newAdditionalField.tbValue.Visibility = Visibility.Hidden;
                    newAdditionalField.btDeleteField.Visibility = Visibility.Visible;
                    newAdditionalField.btDeleteField.Click += btDeleteField_Click;

                    spAdditionalCategoryField.Children.Add(newAdditionalField);
                }
            }
        }

        private void btDeleteField_Click(object sender, RoutedEventArgs e)
        {            
            this.spAdditionalCategoryField.Children.Remove((UIElement)sender);
            this.spAdditionalCategoryField.UpdateLayout();
        }

        private void btAddCategory_Click(object sender, RoutedEventArgs e)
        {
            this.spAdditionalCategoryField.Children.Clear();
            this.Categories.Add(new Category() { Id = -1, Name = "New category", TemplateFields = new TemplateFieldData[0] });
            this.lbCategories.UpdateLayout();
            this.lbCategories.SelectedIndex = this.lbCategories.Items.Count - 1;
            this.lbCategories_SelectionChanged(this.lbCategories, null);
        }

        private void btDeleteCategory_Click(object sender, RoutedEventArgs e)
        {
            if(lbCategories.SelectedIndex > 0)
            {
                //Flag categories to delete ... if no support use it ...
                this.categoriesToDelete.Add(this.Categories[lbCategories.SelectedIndex]);
                this.Categories.RemoveAt(lbCategories.SelectedIndex);
                this.lbCategories.UpdateLayout();
            }
        }
    }
}

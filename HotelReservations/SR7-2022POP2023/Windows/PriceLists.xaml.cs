using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using HotelReservations.Model;
using HotelReservations.Service;

namespace HotelReservations.Windows
{
    /// <summary>
    /// Interaction logic for PriceLists.xaml
    /// </summary>
    public partial class PriceLists : Window
    {
        private ICollectionView view;
        public PriceLists()
        {
            InitializeComponent();
            FillData();
        }

        private void FillData()
        {
            var pls = new PriceListService();
            var prices = pls.GetAllPrices();
            view = CollectionViewSource.GetDefaultView(prices);

            PriceListDG.ItemsSource = null;
            PriceListDG.ItemsSource = view;
            PriceListDG.IsSynchronizedWithCurrentItem = true;

            PriceListDG.SelectedIndex = -1;
            view.Refresh();
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            var addPriceWindow = new AddEditPriceList();

            Hide();
            if (addPriceWindow.ShowDialog() == true)
            {
                FillData();
            }
            Show();
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedPrice = (Price)view.CurrentItem;

            if (selectedPrice != null)
            {
                var editPLWindow = new AddEditPriceList(selectedPrice);

                Hide();

                if (editPLWindow.ShowDialog() == true)
                {
                    FillData();
                }

                Show();
            }
            else
            {
                MessageBox.Show("You didn't pick a guest.");
            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            var priceToDelete = (Price)PriceListDG.SelectedItem;
            if (priceToDelete != null)
            {
                var decision = MessageBox.Show($"Do you want to delete the price {priceToDelete.ToString()}", "Deleting a price", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (decision == MessageBoxResult.Yes)
                {
                    priceToDelete.IsActive = false;
                    FillData();
                }

            }
            else
            {
                MessageBox.Show("You didn't pick a guest.");
            }
        }

        private void PriceListDG_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName.ToLower() == "IsActive".ToLower())
            {
                e.Column.Visibility = Visibility.Collapsed;
            }
        }
    }
}

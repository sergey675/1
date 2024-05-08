using System;
using System.Data.SQLite;
using System.Windows;

namespace MetalFurnitureFactory
{
    public partial class MainWindow : Window
    {
        private readonly string connectionString = "Data Source=Warehouse.db;Version=3;";
        private SQLiteConnection connection;

        public MainWindow()
        {
            InitializeComponent();
            connection = new SQLiteConnection(connectionString);
            connection.Open();
            LoadWarehouseData();
        }

        private void LoadWarehouseData()
        {
            SQLiteCommand command = new SQLiteCommand("SELECT * FROM Warehouse", connection);
            SQLiteDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                txtScrews.Text = reader["Screws"].ToString();
                txtBigDoors.Text = reader["BigDoors"].ToString();
                txtCabinetDoors.Text = reader["CabinetDoors"].ToString();
                txtShelves.Text = reader["Shelves"].ToString();
            }
            reader.Close();
        }

        private void UpdateInventory_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SQLiteCommand command = new SQLiteCommand("UPDATE Warehouse SET Screws = @Screws, BigDoors = @BigDoors, CabinetDoors = @CabinetDoors, Shelves = @Shelves", connection);
                command.Parameters.AddWithValue("@Screws", int.Parse(txtScrews.Text));
                command.Parameters.AddWithValue("@BigDoors", int.Parse(txtBigDoors.Text));
                command.Parameters.AddWithValue("@CabinetDoors", int.Parse(txtCabinetDoors.Text));
                command.Parameters.AddWithValue("@Shelves", int.Parse(txtShelves.Text));
                command.ExecuteNonQuery();
                MessageBox.Show("Информация о складе обновлена.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при обновлении информации о складе: " + ex.Message);
            }
        }

        private void CheckProduction_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int screws = int.Parse(txtScrews.Text);
                int bigDoors = int.Parse(txtBigDoors.Text);
                int cabinetDoors = int.Parse(txtCabinetDoors.Text);
                int shelves = int.Parse(txtShelves.Text);

                int cabinetsPossible = Math.Min(screws / 20, Math.Min(bigDoors / 2, shelves / 5));
                int workbenchesPossible = Math.Min(screws / 10, Math.Min(cabinetDoors / 2, shelves / 2));

                string productionResult = $"Из материалов на складе можно изготовить {cabinetsPossible} архивных шкафов и {workbenchesPossible} верстаков.";

                if (screws < 10)
                {
                    productionResult += $" Для изготовления верстака не хватает: {10 - screws} саморезов\n";
                }
                if (shelves < 2)
                {
                    productionResult += $" Для изготовления верстака не хватает: {2 - shelves} полок";
                }
                if (screws < 20 && bigDoors < 2 && shelves < 5)
                {
                    productionResult += $" Для изготовления архивного шкафа не хватает:\n ";
                    if (screws < 20) productionResult += $" {20 - screws} саморезов";
                    if (bigDoors < 2) productionResult += $" {2 - bigDoors} больших дверей";
                    if (shelves < 5) productionResult += $" {5 - shelves} полок";
                }

                txtProductionResult.Text = productionResult;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при проверке возможности производства: " + ex.Message);
            }
        }

        private void UseUnlimitedResources_Click(object sender, RoutedEventArgs e)
        {
            txtScrews.Text = int.MaxValue.ToString();
            txtBigDoors.Text = int.MaxValue.ToString();
            txtCabinetDoors.Text = int.MaxValue.ToString();
            txtShelves.Text = int.MaxValue.ToString();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            connection.Close();
        }
    }
}
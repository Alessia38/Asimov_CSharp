using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Asimov
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {
            string connectionString = "server=localhost;user=root;database=asimov;port=3306;password=root;";

            // Charger les noms et prénoms des utilisateurs avec l'ID de rôle égal à 4 dans la combobox
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT id, CONCAT(nom, ' ', prenom) AS NomComplet FROM utilisateurs WHERE idRole = 4";

                using (MySqlDataAdapter adapter = new MySqlDataAdapter(query, connection))
                {
                    DataTable table = new DataTable();
                    adapter.Fill(table);

                    // Afficher les noms et prénoms des utilisateurs dans la combobox
                    comboBoxUtilisateurs.DataSource = table;
                    comboBoxUtilisateurs.DisplayMember = "NomComplet";
                    comboBoxUtilisateurs.ValueMember = "id";

                    // Empêcher l'ajout de nouvelles lignes dans la combobox
                    comboBoxUtilisateurs.DropDownStyle = ComboBoxStyle.DropDownList;

                    // Créer une colonne de boutons pour le DataGridView
                    DataGridViewButtonColumn deleteButtonColumn = new DataGridViewButtonColumn();
                    deleteButtonColumn.Name = "DeleteButtonColumn";
                    deleteButtonColumn.HeaderText = "Supprimer";
                    deleteButtonColumn.Text = "Supprimer";
                    deleteButtonColumn.UseColumnTextForButtonValue = true;
                    dataGridView1.Columns.Add(deleteButtonColumn);
                }
            }

        }

        private void comboBoxUtilisateurs_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            // Récupérer l'ID de l'utilisateur sélectionné dans la combobox
            int userId = Convert.ToInt32(((DataRowView)comboBoxUtilisateurs.SelectedItem)["id"]);

            // Filtrer les données de la table "moyenne" pour n'afficher que les moyennes de cet utilisateur
            string connectionString = "server=localhost;user=root;database=asimov;port=3306;password=root;";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM moyenne WHERE idUtilisateur = @userId";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@userId", userId);

                using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                {
                    DataTable table = new DataTable();
                    adapter.Fill(table);

                    // Afficher les données filtrées dans le contrôle DataGridView
                    dataGridView1.DataSource = table;
                }
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Vérifier si l'utilisateur a cliqué sur le bouton de suppression
            if (e.ColumnIndex == dataGridView1.Columns["DeleteButtonColumn"].Index && e.RowIndex >= 0)
            {
                // Récupérer l'ID de la ligne sélectionnée
                int idToDelete = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["ID"].Value);

                // Définir la connexion à la base de données
                string connectionString = "server=localhost;user=root;database=asimov;port=3306;password=root;";

                // Connexion à la base de données et exécution de la requête de suppression
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    // Définir la requête SQL de suppression
                    string query = "DELETE FROM moyenne WHERE id = @id";

                    // Créer une commande MySQL avec la requête SQL et les paramètres
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        // Ajouter le paramètre ID à la commande
                        command.Parameters.AddWithValue("@id", idToDelete);

                        // Exécuter la commande
                        int rowsAffected = command.ExecuteNonQuery();

                        // Vérifier si la suppression a réussi
                        if (rowsAffected > 0)
                        {
                            // Si la suppression réussit, supprimer la ligne du DataGridView
                            dataGridView1.Rows.RemoveAt(e.RowIndex);
                        }
                        else
                        {
                            // Si la suppression échoue, afficher un message d'erreur ou effectuer d'autres actions nécessaires
                            MessageBox.Show("La suppression a échoué.");
                        }
                    }
                }

                // Supprimer la ligne du DataGridView
                dataGridView1.Rows.RemoveAt(e.RowIndex);
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace ProImport.Classes
{
    class ExportData
    {
        public static void exportCSV(DataGridView DGV)
        {

            try
            {
                string arquivo = "";
                DateTime data = DateTime.Today;
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "CSV (*.csv)|*.csv";
                sfd.FileName = "proImport_" + data.ToString().Substring(0, 10).Replace('/', '-') + "_.csv";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    if (File.Exists(arquivo))
                    {
                        try
                        {
                            File.Delete(arquivo);
                        }
                        catch (IOException ex)
                        {
                            MessageBox.Show("Não foi possível exportar. Erro:" + ex.Message);
                        }
                    }
                    int columnCount = DGV.ColumnCount;
                    string columnNames = "";
                    string[] output = new string[DGV.RowCount + 1];
                    for (int i = 1; i < columnCount; i++)
                    {
                        columnNames += DGV.Columns[i].Name.ToString() + ";";
                    }
                    output[0] += columnNames;
                    for (int i = 1; (i - 1) < DGV.RowCount; i++)
                    {
                        for (int j = 1; j < columnCount; j++)
                        {
                            output[i] += DGV.Rows[i - 1].Cells[j].Value.ToString() + ";";
                        }
                    }
                    System.IO.File.WriteAllLines(sfd.FileName, output, System.Text.Encoding.UTF8);
                    MessageBox.Show("Seu arquivo foi gerado com sucesso.");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Erro, verifique se o arquivo está aberto! Erro " + e.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }
    }
}

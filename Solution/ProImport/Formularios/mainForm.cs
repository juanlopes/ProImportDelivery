using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using ProImport.Classes;
using ProImport.Formularios;
using Npgsql;

namespace ProImport
{
    public partial class mainForm : Form
    {
        private static NpgsqlConnection conecta = SQLconn.connection();
        public mainForm()
        {
            SQLconn.retriveDBX();
            InitializeComponent();

            //Define o formado dos DatePickers
            dtInicial.Format = DateTimePickerFormat.Custom;
            dtInicial.CustomFormat = "dd-MM-yyyy";
            dtFinal.Format = DateTimePickerFormat.Custom;
            dtFinal.CustomFormat = "dd-MM-yyyy";
            //Adiciona um dia no DatePicker dtFinal
            dtFinal.Value = System.DateTime.Now.AddDays(3);

            //Insere Opções na ComboBox
            cbTipSec.Items.Insert(0, "Categoria");
            cbTipSec.Items.Insert(1, "Departamento");

            //Verifica qual botão deixar ativo se "Carregar Novo" ou "Carregar Salvo"
            if (Data.contaProdutosPro03() == 0)
            {
                button3.Visible = false;
                btnNovo.Visible = true;
                cbTipSec.Enabled = true;
                cbTipSec.SelectedIndex = Data.getSecao();
            }
            else if(Data.contaProdutosPro03() > 0)
            {
                button3.Visible = true;
                btnNovo.Visible = false;
                cbTipSec.Enabled = false;
                cbTipSec.SelectedIndex = Data.getSecao();
            }

            cbEmpresa.DropDownStyle = ComboBoxStyle.DropDownList;
            cbEmpresa.DataSource = Data.GetEmp();
            cbEmpresa.ValueMember = "codigo";
            cbEmpresa.DisplayMember = "empresa";
            cbEmpresa.Update();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        public void LoadDataGrid()
        {
            //Preenche DataGridView
            dataGridView1.DataSource = Data.dataRecovery(dtInicial.Value.ToString(), dtFinal.Value.ToString(), cbTipSec.SelectedIndex, Convert.ToInt32(cbEmpresa.SelectedValue));
            dataGridView1.AutoResizeColumns();
            //cleanGrid();
            //Conta os Produtos dentro da DataGridView
            contaProduto.Text = dataGridView1.Rows.Count.ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //Minimiza o programa
            this.WindowState = FormWindowState.Minimized;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if(txtCodPro.Text != "" && txtDscPro.Text != "" && txtEstPro.Text != "" && cbTipSec.SelectedIndex == 0 || cbTipSec.SelectedIndex == 1)
            {
                //Salva o código do produto e converte para Long
                long codPro = Convert.ToInt64(txtCodPro.Text);
                //Salva o o produto na PROIMPORT03
                Data.manipulaDadosSalvos(dtInicial.Value, dtFinal.Value, cbTipSec.SelectedIndex, codPro, Convert.ToInt32(txtEstPro.Text), Convert.ToInt32(cbEmpresa.SelectedValue));
                //Recarrega PROIMPORT03
                reloadSalvo();
            }
            else
            {
                MessageBox.Show("Preencha todos os campos antes de prosseguir!", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            txtCodPro.Focus();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count > 0) { 
            //Remove linhas do DataGrid
            dataGridView1.Rows.RemoveAt(dataGridView1.CurrentRow.Index);
            //Atualiza o contador de produtos da tabela
            contaProduto.Text = dataGridView1.Rows.Count.ToString();
            }
            else
            {
                MessageBox.Show("Não há dados para remover!", "Opa!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void cleanGrid()
        {
            //Remove dados duplicados do grid
            //Data.limpaGrid(this.dataGridView1);
        }

        private void onEnterPressedCod(object sender, KeyEventArgs e)
        {
            //Verifica se Enter foi pressionado 
            if (e.KeyCode == Keys.Enter)
            {
                if (txtCodPro.Text.Length <= 6)
                {
                    //Verifica se foi dado entrada no campo Código
                    if (txtCodPro.Text != "")
                    {
                        //Pega o código do produto e seleciona uma busca
                        int codPro = Convert.ToInt32(txtCodPro.Text);
                        txtDscPro.Text = SQLconn.retornaProduto(codPro, 1, Convert.ToInt32(cbEmpresa.SelectedValue));
                    }
                    else
                    {
                        //define o campo descrição vazio caso o código esteja vazio também 
                        txtDscPro.Text = "";
                    }
                }
                else if (txtCodPro.Text.Length > 6 && txtCodPro.Text.Length <= 14)
                {
                    //Verifica se foi dado entrada no campo Código
                    if (txtCodPro.Text != "")
                    {
                        //Pega o código do produto e seleciona uma busca
                        long codPro = Convert.ToInt64(txtCodPro.Text);
                        txtDscPro.Text = SQLconn.retornaProduto(codPro, 2, Convert.ToInt32(cbEmpresa.SelectedValue));
                    }
                    else
                    {
                        //define o campo descrição vazio caso o código esteja vazio também 
                        txtDscPro.Text = "";
                    }
                }
                else
                {
                    MessageBox.Show("Insira um código válido!", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                btnAdicionar.Focus();
            }
        }


        private void btnSalvar_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            //Salva dados na Tabela PROIMPORT03 para a exportação em CSV
            Data.SalvarDados(dataGridView1);
            reloadSalvo();
            if (Convert.ToInt32(contaProduto.Text) > 0)
            {
                //desabilita o botão carregar novo e habilita o carregar salvo
                btnNovo.Visible = false;
                button3.Visible = true;
                cbTipSec.Enabled = false;
                cbTipSec.SelectedIndex = Data.getSecao(); 
            }else if (Convert.ToInt32(contaProduto.Text) == 0)
            {
                //desabilita o botão carregar novo e habilita o carregar salvo
                btnNovo.Visible = true;
                button3.Visible = false;
                cbTipSec.Enabled = true;
            }
            Cursor.Current = Cursors.Default;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            reloadSalvo();
        }

        private void reloadSalvo()
        {
            //Atualiza o grid com os dados da Proimport03
            dataGridView1.DataSource = Data.carregaDadoscarregaSalvoDoGrid(dtInicial.Value, dtFinal.Value, Convert.ToInt32(cbEmpresa.SelectedValue));
            dataGridView1.AutoResizeColumns();
            //Atualiza o contador de produtos
            contaProduto.Text = dataGridView1.Rows.Count.ToString();
            
        }

        private void recarregaSalvoCombo(object sender, EventArgs e)
        {
            //Atualiza o grid com os dados da Proimport03
            dataGridView1.DataSource = Data.carregaDadoscarregaSalvoDoGrid(dtInicial.Value, dtFinal.Value, Convert.ToInt32(cbEmpresa.SelectedValue));
            dataGridView1.AutoResizeColumns();
            //Atualiza o contador de produtos
            contaProduto.Text = dataGridView1.Rows.Count.ToString();

        }

        private void btnNovo_Click(object sender, EventArgs e)
        {
            if(cbTipSec.SelectedIndex == -1)
            {
                MessageBox.Show("Selecione uma Seção Por Favor", "Opa!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cbTipSec.Focus();
            }
            else
            {
                //Carrega dados da Proimport02
                LoadDataGrid();
            }            
        }

        private void cbTipSec_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void dtInicial_ValueChanged(object sender, EventArgs e)
        {
            //Atualiza a data dos produtos da planilha ao alterar datepicker Inicial
            if (Data.contaProdutosPro03() == 0)
            {
                LoadDataGrid();
                dataGridView1.AutoResizeColumns();
            }
            else if (Data.contaProdutosPro03() > 0)
            {
                reloadSalvo();
                dataGridView1.AutoResizeColumns();
            }
        }

        private void dtFinal_ValueChanged(object sender, EventArgs e)
        {
            //Atualiza a data dos produtos da planilha ao alterar datepicker Final
            if (Data.contaProdutosPro03() == 0)
            {
                LoadDataGrid();
                dataGridView1.AutoResizeColumns();
            }
            else if (Data.contaProdutosPro03() > 0)
            {
                reloadSalvo();
                dataGridView1.AutoResizeColumns();
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            //Exporta dados para CSV
            ExportData.exportCSV(dataGridView1);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Ooops! Sentimos muito, essa funcionalidade ainda está para ser implementada", "Ooops!", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void mainForm_KeyDown(object sender, KeyEventArgs e)
        {

        }
    }
}

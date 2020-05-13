using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProImport.Classes;
using System.Windows.Forms;
using System.Data;
using Npgsql;

namespace ProImport.Classes
{
    class Data
    {
        //Define contador de erros
        private static int contaErros = 0;
        private static int opcaoSec;
        //Importa a conexão da classe SQLconn
        private static NpgsqlConnection conecta = SQLconn.connection();
        public static DataTable dataRecovery(string dataInicio, string dataFim, int optCbBox, int empresa)
        {
            //Cria a variável para armazenar a query de preenchimento do Data Table
            string stringDataTable;
            opcaoSec = optCbBox;

            if(opcaoSec == 1)
            {
                //Query de Inicialização do DataTable
                stringDataTable =  "SELECT DISTINCT p2.cod as \"Cod Interno\", LPAD(COALESCE(cast(b1.bar as numeric (14,0)), p2.cod), 13, '0') AS \"Codigo de Barras\", ";
                stringDataTable += "upper(LEFT(lower(P2.DSC), 1))|| substr(lower(p2.dsc), 2, length(p2.dsc)) AS \"Nome\", ";
                stringDataTable += "P4.PV2 AS \"Preco Original\", ";
                stringDataTable += "0 AS \"Preco Promocional\", ";
                stringDataTable += "COALESCE(upper(LEFT(lower(D1.DSC), 1))|| substr(lower(D1.dsc), 2, length(D1.dsc)), 'Diversos') AS \"Categoria\", ";
                stringDataTable += "pi2.EST AS \"Estoque\", ";
                stringDataTable += "0 AS \"Limite por Pedido\", '";
                stringDataTable += dataInicio.Substring(0, 10) + "'|| ' 00:01' AS \"Data do Inicio\", '";
                stringDataTable += dataFim.Substring(0, 10) + "'|| ' 23:59' AS \"Data do Fim\" ";
                stringDataTable += "FROM proimport02 PI2 ";
                stringDataTable += "LEFT JOIN GCEPRO02 P2 ON PI2.COD = P2.COD ";
                stringDataTable += "LEFT JOIN GCEBAR01 B1 ON B1.PRO = P2.COD AND B1.EPR = 'S' ";
                stringDataTable += "LEFT JOIN GCEPRO04 P4 ON P2.COD = P4.COD ";
                stringDataTable += "LEFT JOIN GCEDEP01 D1 ON P2.DEP = D1.COD ";
                stringDataTable += "WHERE P4.EMP = " + empresa + ";";

                //Define o comando com a Query 
                NpgsqlCommand buscaPro = new NpgsqlCommand(stringDataTable, conecta);

                //Realiza a pesquisa
                conecta.Open();
                NpgsqlDataAdapter dataAdp = new NpgsqlDataAdapter(buscaPro);
                //Cria o DataTable
                DataTable dtProdutos = new DataTable();
                //Preenche o DataTable
                dataAdp.Fill(dtProdutos);


                conecta.Close();
                //retorna o DataTable
                return dtProdutos;
                

            }
            else if(opcaoSec == 0)
            {
                //Query de Inicialização do DataTable
                stringDataTable = "SELECT DISTINCT p2.cod as \"Cod Interno\", LPAD(COALESCE(cast(b1.bar as numeric (14,0)), p2.cod), 13, '0') AS \"Codigo de Barras\", ";
                stringDataTable += "replace(upper(LEFT(lower(P2.DSC), 1))|| substr(lower(p2.dsc), 2, length(p2.dsc)), '#', '') AS \"Nome\", ";
                stringDataTable += "P4.PV2 AS \"Preco Original\", ";
                stringDataTable += "0 AS \"Preco Promocional\", ";
                stringDataTable += "COALESCE(upper(LEFT(lower(S1.DSC), 1))|| substr(lower(S1.dsc), 2, length(S1.dsc)), 'Diversos') AS \"Categoria\", ";
                stringDataTable += "pi2.EST AS \"Estoque\", ";
                stringDataTable += "0 AS \"Limite por Pedido\", '";
                stringDataTable += dataInicio.Substring(0, 10) + "'|| ' 00:01' AS \"Data do Inicio\", '";
                stringDataTable += dataFim.Substring(0, 10) + "'|| ' 23:59' AS \"Data do Fim\" ";
                stringDataTable += "FROM proimport02 PI2 ";
                stringDataTable += "LEFT JOIN GCEPRO02 P2 ON PI2.COD = P2.COD ";
                stringDataTable += "LEFT JOIN GCEBAR01 B1 ON B1.PRO = P2.COD AND B1.EPR = 'S' ";
                stringDataTable += "LEFT JOIN GCEPRO04 P4 ON P2.COD = P4.COD ";
                stringDataTable += "LEFT JOIN GCESEC01 S1 ON P2.SEC = S1.COD AND S1.DEP = P2.DEP ";
                stringDataTable += "WHERE P4.EMP = " + empresa + ";";

                //Define o comando com a Query 
                NpgsqlCommand buscaPro = new NpgsqlCommand(stringDataTable, conecta);

                //Realiza a pesquisa
                conecta.Open();
                NpgsqlDataAdapter dataAdp = new NpgsqlDataAdapter(buscaPro);
                //Cria o DataTable
                DataTable dtProdutos = new DataTable();
                //Preenche o DataTable
                dataAdp.Fill(dtProdutos);

                conecta.Close();
                //retorna o DataTable
                return dtProdutos;
                
            }
            else
            {
                //Cria o DataTable
                DataTable dtProdutos = new DataTable();
                return dtProdutos;
            }
        }

        public static int getSecao()
        {
            int Departamento = 0;
            int Categoria = 0;

            NpgsqlCommand compara_PRO3DEP = new NpgsqlCommand("SELECT COUNT(DISTINCT D1.DSC) as TOTAL FROM PROIMPORT03 P3 LEFT JOIN GCEDEP01 D1 ON UPPER(D1.DSC) = UPPER(P3.SEC)", conecta);
            NpgsqlCommand compara_PRO3SEC = new NpgsqlCommand("SELECT COUNT(DISTINCT D1.DSC) as TOTAL FROM PROIMPORT03 P3 LEFT JOIN GCESEC01 D1 ON UPPER(D1.DSC) = UPPER(P3.SEC)", conecta);
            conecta.Open();
            NpgsqlDataReader leDep = compara_PRO3DEP.ExecuteReader();
            leDep.Read();
            Departamento = Convert.ToInt32(leDep["TOTAL"].ToString());
            conecta.Close();
            conecta.Open();
            NpgsqlDataReader leCat = compara_PRO3SEC.ExecuteReader();
            leCat.Read();
            Categoria = Convert.ToInt32(leCat["TOTAL"].ToString());
            conecta.Close();

            if (Departamento < Categoria && Departamento >=0 && Categoria >=0)
            {
                return 0;
            }
            else if (Departamento > Categoria && Departamento >= 0 && Categoria >= 0)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }

        //DataTableComboBox
        public static DataTable GetEmp()
        {

            DataTable dataEmp = new DataTable("EMP");
            try
            {
                NpgsqlCommand Command = new NpgsqlCommand("select sps.cod as codigo, sps.cod||' - '||trs.fan as empresa, trs.nom as razaoSocial, trs.cgc as cnpj from spsemp00 sps join trstra01 trs on sps.cod = trs.cod", conecta);
                conecta.Open();
                dataEmp.Load(Command.ExecuteReader());
                conecta.Close();
                return dataEmp;
            }
            catch (Exception)
            {
                conecta.Close();
            }
            return dataEmp;
        }

        public static int contaProdutosPro02()
        {
            string stringBusca = "select count(*) from proimport02";

            NpgsqlCommand buscaPro = new NpgsqlCommand(stringBusca, conecta);
            conecta.Open();
            NpgsqlDataReader dr = buscaPro.ExecuteReader();
            dr.Read();

            int produtos = Convert.ToInt32(dr["count"].ToString());

            conecta.Close();
            return produtos;
        }

        public static int contaProdutosPro03()
        {
            string stringBusca = "select count(*) from proimport03";

            NpgsqlCommand buscaPro = new NpgsqlCommand(stringBusca, conecta);
            conecta.Open();
            NpgsqlDataReader dr = buscaPro.ExecuteReader();
            dr.Read();

            int produtos = Convert.ToInt32(dr["count"].ToString());
            conecta.Close();

            return produtos;
            
        }

        public static DataTable carregaDadoscarregaSalvoDoGrid(DateTime dtInicial, DateTime dtFinal, int empresa)
        {
            string stringDataTable = "SELECT p2cod as \"Codigo Interno\", LPAD(cast(p3.cod as numeric(13,0)), 13, '0') as \"Codigo de Barras\", replace(upper(LEFT(lower(DSC), 1))|| substr(lower(dsc), 2, length(dsc)), '#', '') as \"Nome\", p4.pv2 as \"Preço Original\", p3.pmc as \"Preço Promocional\", sec as \"Categoria\", est as \"Estoque\", lim as \"Limite por Pedido\", '" + dtInicial.ToString().Substring(0, 10) + "' || ' 00:01' as \"Data do Início\", '" + dtFinal.ToString().Substring(0, 10) + "' || ' 23:59' as \"Data do Fim\" FROM PROIMPORT03 p3 left join gcepro04 p4 on p4.cod = p3.p2cod where p4.emp = " + empresa;

            //Define o comando com a Query 
            NpgsqlCommand buscaPro = new NpgsqlCommand(stringDataTable, conecta);

            //Realiza a pesquisa
            conecta.Open();
            NpgsqlDataAdapter dataAdp = new NpgsqlDataAdapter(buscaPro);
            //Cria o DataTable
            DataTable dtProdutos = new DataTable();
            //Preenche o DataTable
            dataAdp.Fill(dtProdutos);
            conecta.Close();

            //retorna o DataTable
            return dtProdutos;
        }

        private static void gravaDadosPro3(DateTime dtInicial, DateTime dtFinal, string cmdSql)
        {
            try
            {
                NpgsqlCommand retrievePro = new NpgsqlCommand(cmdSql, conecta);
                conecta.Open();
                NpgsqlDataReader dr = retrievePro.ExecuteReader();
                dr.Read();

                int codInt = Convert.ToInt32(dr["cod"].ToString());
                string codigo = dr["codigo"].ToString();
                string descricao = dr["desc"].ToString();
                decimal promo = Convert.ToDecimal(dr["pmc"].ToString());
                string secao = dr["sec"].ToString();
                int estoque = Convert.ToInt32(dr["est"].ToString());
                int limite = Convert.ToInt32(dr["lim"].ToString());
                DateTime dataInicial = dtInicial;
                DateTime dataFinal = dtFinal;

                dr.Close();

                //dataGridView1.Rows.Add(txtCodPro.Text, txtDscPro.Text, preco, promo, secao, estoque, limite, DTI, DTF) ;
                //cod, dsc, pv2, pmc, sec, est, lim, dti, dtf
                string insert = "INSERT INTO PROIMPORT03 (p2cod, cod, dsc, pmc, sec, est, lim, dti, dtf) VALUES (@codInt, @Codigo, @Descricao, @Promo, @Secao, @Estoque, @Limite, @DTI, @DTF)";
                NpgsqlCommand cmd = new NpgsqlCommand(insert, conecta);
                cmd.Parameters.AddWithValue("@codInt", codInt);
                cmd.Parameters.AddWithValue("@Codigo", codigo);
                cmd.Parameters.AddWithValue("@Descricao", descricao);
                cmd.Parameters.AddWithValue("@Promo", promo);
                cmd.Parameters.AddWithValue("@Secao", secao);
                cmd.Parameters.AddWithValue("@Estoque", estoque);
                cmd.Parameters.AddWithValue("@Limite", limite);
                cmd.Parameters.AddWithValue("@DTI", dataInicial);
                cmd.Parameters.AddWithValue("@DTF", dataFinal);
                try
                {
                    cmd.ExecuteNonQuery();
                    conecta.Close();
                }
                catch (Exception e)
                {
                    MessageBox.Show("Não foi possível adicionar. Erro " + e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    conecta.Close();
                }
            }catch (Exception e)
            {
                MessageBox.Show("Não foi possível adicionar o item! Erro: " + e.Message, "Ooops!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                conecta.Close();
            }
            
        }

        public static void manipulaDadosSalvos(DateTime dtInicial, DateTime dtFinal, int cbDepIndex, long codPro, int estoque, int empresa)
        {
            opcaoSec = cbDepIndex;
            if (opcaoSec == 1)
            {
                if (codPro.ToString().Length <= 6)
                {
                    string cmdSql = "SELECT p2.cod as cod, LPAD(coalesce(cast(b1.bar as numeric(13,0)), p2.cod), 13, '0') as codigo, replace(p2.dsc, '#', '') as desc,  p4.pv2 as preco, '0,00' as pmc, initcap(d1.dsc) as sec, " + estoque +" as est, 0 as lim FROM GCEPRO02 P2 left join gcebar01 b1 on p2.cod = b1.pro LEFT JOIN GCEDEP01 D1 ON P2.DEP = D1.COD  left join gcepro04 p4 on p2.cod = p4.cod WHERE p4.emp = " + empresa + " and P2.COD = " + codPro;
                    gravaDadosPro3(dtInicial, dtFinal, cmdSql);
                }
                else
                {
                    string cmdSql = "SELECT p2.cod as cod, LPAD(coalesce(cast(b1.bar as numeric(13,0)), p2.cod), 13, '0') as codigo, replace(p2.dsc, '#', '') as desc,  p4.pv2 as preco, '0,00' as pmc, initcap(d1.dsc) as sec, " + estoque + " as est, 0 as lim FROM GCEPRO02 P2 left join gcebar01 b1 on p2.cod = b1.pro LEFT JOIN GCEDEP01 D1 ON P2.DEP = D1.COD  left join gcepro04 p4 on p2.cod = p4.cod WHERE p4.emp = " + empresa + " and b1.bar = '" + codPro + "'";
                    gravaDadosPro3(dtInicial, dtFinal, cmdSql);
                }
            }
            else if (opcaoSec == 0)
            {
 
                if (codPro.ToString().Length <= 6)
                {
                    string cmdSql = "SELECT p2.cod as cod, LPAD(coalesce(cast(b1.bar as numeric(13,0)), p2.cod), 13, '0') as codigo, replace(p2.dsc, '#', '') as desc,  p4.pv2 as preco, '0,00' as pmc, initcap(s1.dsc) as sec, " + estoque + " as est, 0 as lim FROM GCEPRO02 P2 left join gcebar01 b1 on p2.cod = b1.pro LEFT JOIN GCESEC01 S1 ON P2.SEC = S1.COD AND S1.DEP = P2.DEP  left join gcepro04 p4 on p2.cod = p4.cod WHERE p4.emp = " + empresa + " and  P2.COD = " + codPro;
                    gravaDadosPro3(dtInicial, dtFinal, cmdSql);
                }
                else
                {
                    string cmdSql = "SELECT p2.cod as cod, LPAD(coalesce(cast(b1.bar as numeric(13,0)), p2.cod), 13, '0') as codigo, replace(p2.dsc, '#', '') as desc,  p4.pv2 as preco, '0,00' as pmc, initcap(s1.dsc) as sec, " + estoque + " as est, 0 as lim FROM GCEPRO02 P2 left join gcebar01 b1 on p2.cod = b1.pro LEFT JOIN GCESEC01 S1 ON P2.SEC = S1.COD AND S1.DEP = P2.DEP left join gcepro04 p4 on p2.cod = p4.cod WHERE p4.emp = " + empresa + " and  b1.bar = '" + codPro +"'";
                    gravaDadosPro3(dtInicial, dtFinal, cmdSql);
                }
            }
        }

        public static void SalvarDados(DataGridView dgvItens)
        {
            try
            {
                conecta.Open();
                string remove = "DELETE FROM PROIMPORT03";
                NpgsqlCommand rmv = new NpgsqlCommand(remove, conecta);
                rmv.ExecuteNonQuery();
                conecta.Close();
                conecta.Open();
                if (dgvItens.Rows.Count > 1)
                {
                    for (int i = 0; i <= dgvItens.Rows.Count - 1; i++)
                    {
                        long col0 = Convert.ToInt64(dgvItens.Rows[i].Cells[0].Value); //codInt
                        string col1 = dgvItens.Rows[i].Cells[1].Value.ToString(); //cod
                        string col2 = dgvItens.Rows[i].Cells[2].Value.ToString(); //Descricao
                        decimal col3 = Convert.ToDecimal(dgvItens.Rows[i].Cells[3].Value); //preço venda
                        decimal col4 = Convert.ToDecimal(dgvItens.Rows[i].Cells[4].Value); //Promocional
                        string col5 = Convert.ToString(dgvItens.Rows[i].Cells[5].Value); //Seção
                        int col6 = Convert.ToInt32(dgvItens.Rows[i].Cells[6].Value); //Estoque
                        int col7 = Convert.ToInt32(dgvItens.Rows[i].Cells[7].Value); //Limite por pedido
                        DateTime col8 = Convert.ToDateTime(dgvItens.Rows[i].Cells[8].Value); //Data Inicio
                        DateTime col9 = Convert.ToDateTime(dgvItens.Rows[i].Cells[9].Value); //Data Final
                        try
                        {
                            string insert = "INSERT INTO PROIMPORT03 (p2cod, cod, dsc, pmc, sec, est, lim, dti, dtf) VALUES (@codInt, @Codigo, @Descricao, @Promo, @Secao, @Estoque, @Limite, @DTI, @DTF)";
                            NpgsqlCommand cmd = new NpgsqlCommand(insert, conecta);
                            cmd.Parameters.AddWithValue("@codInt", col0);
                            cmd.Parameters.AddWithValue("@Codigo", col1);
                            cmd.Parameters.AddWithValue("@Descricao", col2);
                            cmd.Parameters.AddWithValue("@Promo", col4);
                            cmd.Parameters.AddWithValue("@Secao", col5);
                            cmd.Parameters.AddWithValue("@Estoque", col6);
                            cmd.Parameters.AddWithValue("@Limite", col7);
                            cmd.Parameters.AddWithValue("@DTI", col8);
                            cmd.Parameters.AddWithValue("@DTF", col9);
                            cmd.ExecuteNonQuery();
                        }
                        catch (Exception e)
                        {
                            contaErros += 1;
                            Console.WriteLine(contaErros.ToString() + " erros! Erro: " + e);
                        }
                    }
                }
                conecta.Close();

                if (contaErros == 0)
                {
                    MessageBox.Show("Dados incluídos com sucesso !!", "Inclusão", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Dados incluídos com " + contaErros + " erros de Código Interno!", "Inclusão", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    contaErros = 0;
                }

            }
            catch (Exception ex)
            {
                ex.Message.ToString();
                MessageBox.Show("Erro: " + ex);
                conecta.Close();
            }
        }

        public static void limpaGrid(DataGridView view)
        {
            //Contador de linhas para varrer o datagrid para comparar
            int cont = 0;

            foreach (DataGridViewRow row in view.Rows)
            {
                try
                {
                    //coloquei o contador para passar as linhas na grid
                    if (row.Cells[0].Value.Equals(view.Rows[cont].Cells[0].Value))
                    {
                        view.Rows.Remove(view.Rows[cont]);
                    }
                    cont++;
                }
                catch (Exception)
                {
                    
                }
                
            }
        }

    }
}

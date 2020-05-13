using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using System.IO;
using System.Windows.Forms;

namespace ProImport.Classes
{
    class SQLconn
    {
        //Define as variáveis de conexão para a criação da String
        private static string user = "";
        private static string pass = "";
        private static string host = "";
        private static string db = "";

        public static NpgsqlConnection connection()
        {
            //Busca a String de Conexão
            string crud = connectionString();
            try
            {
                //Cria a conexão e retorna para o solicitante
                NpgsqlConnection conn = new NpgsqlConnection(crud);
                return conn;
            }
            catch (Exception e)
            {
                MessageBox.Show("Não foi possível se conectar ao banco de dados, por favor, execute em uma máquina onde possua o Sysmo S1 instalado. \nCaso persista, favor entrar em contato com a Factorin." + e.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }

        public static string connectionString() {
            //Cria a string de conexão
            return "Server=" + host + ";Port=5432;UserID=" + user + ";password=" + pass + ";Database=" + db + ";";
        }

        public static void retriveDBX(){
            try
            {
                StreamReader config = new StreamReader(@"C:\SysmoVs\dbxconnections.ini");
                string valor = "";
                string linha = config.ReadLine();

                while (linha != null) // enquanto linha não nula
                {
                    if (linha.StartsWith("Database"))
                    {
                        valor = linha.Replace("Database=", "");
                        valor = valor.Replace(":5432/dados", "");
                    }

                    linha = config.ReadLine();
                }
                host = valor;
            }
            catch (Exception e)
            {
                MessageBox.Show("Não foi possível encontrar o arquivo: " + e.Message + ". Por favor, verifique e tente novamente. Erro: " + e.InnerException, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
            
        }

        public static string retornaProduto(long cod, int tipo, int empresa)
        {
            string produto;
            //Conecta banco
            NpgsqlConnection conecta = connection();
            //Abre a conexão com o banco
            conecta.Open();

            if(tipo == 1)
            {
                //Pesquisa o produto
                NpgsqlCommand pesquisaProduto = new NpgsqlCommand("select p2.cod, p2.dsc from gcepro02 p2 left join gcepro04 p4 on p4.cod = p2.cod where p2.cod = " + cod + " and p4.emp = " + empresa + ";", conecta);
                NpgsqlDataReader leProduto = pesquisaProduto.ExecuteReader();
                leProduto.Read();

                try
                {
                    //Se for possível encontrar o produto, retorne a descrição
                    produto = leProduto["dsc"].ToString();

                    return produto;
                }
                catch (Exception)
                {
                    //Caso não seja possível, retorne que não foi encontrado
                    produto = "Produto não encontrado!";

                    return produto;
                }
            }else if(tipo == 2)
            {
                //Pesquisa o produto
                NpgsqlCommand pesquisaProduto = new NpgsqlCommand("select b1.bar, p2.dsc from gcepro02 p2 left join gcepro04 p4 on p2.cod = p4.cod left join gcebar01 b1 on p2.cod = b1.pro where b1.bar = " + cod + " and p4.emp = " + empresa + ";", conecta);
                NpgsqlDataReader leProduto = pesquisaProduto.ExecuteReader();
                leProduto.Read();

                try
                {
                    //Se for possível encontrar o produto, retorne a descrição
                    produto = leProduto["dsc"].ToString();

                    return produto;
                }
                catch (Exception)
                {
                    //Caso não seja possível, retorne que não foi encontrado
                    produto = "Produto não encontrado!";

                    return produto;
                }
            }
            else
            {
                return "";
            }
           
            
        }
    }
}

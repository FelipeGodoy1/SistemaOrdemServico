﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SistemaOrdemServico
{
    public partial class CadastroPecas : Form
    {
        string sql;
        public CadastroPecas()
        {
            InitializeComponent();

            cboxFornecedorEditarPeca.Enabled = false;
        }



        //Conexao com banco de dados
        public SqlConnection abreConexao()
        {
            string conexao = @"Server=DESKTOP-U3P4RMT\SQLEXPRESS;
                            Database=OSFujita;
                            User Id=sa;
                            Password=1234;";
            return new SqlConnection(conexao);
        }


 

        //Prencher Combobox Cadastro de peças
        private void preencherComboBoxCadastrar ()
        {
            SqlConnection conexao = abreConexao();

            try
            {
                conexao.Open();

                sql = "SELECT * FROM cadClientForn WHERE categoria = 'Fornecedor'";

                SqlCommand comando = new SqlCommand(sql, conexao);

                SqlDataReader dados = comando.ExecuteReader();

                DataTable dt = new DataTable();

                dt.Load(dados);
               

                //Combobox Cadastrar Peca
                cboxFornecedorCadastrarPeca.DisplayMember = "nomeRazSoc";

                cboxFornecedorCadastrarPeca.ValueMember = "codPeca";

                cboxFornecedorCadastrarPeca.DataSource = dt;

                cboxFornecedorCadastrarPeca.Text = "Selecionar";
               
            }
            catch (Exception)
            {
                MessageBox.Show("Erro");
            }
            finally
            {
                conexao.Close();
            }
            
        }



        //Preencher combobox de Editar  Peca
        private void preencherComboBoxEditar()
        {
            SqlConnection conexao = abreConexao();

            try
            {
                conexao.Open();

                sql = "SELECT * FROM cadClientForn WHERE categoria = 'Fornecedor'";

                SqlCommand comando = new SqlCommand(sql, conexao);

                SqlDataReader dados = comando.ExecuteReader();

                DataTable dt = new DataTable();

                dt.Load(dados);


                //Combobox Editar peca
                cboxFornecedorEditarPeca.DisplayMember = "nomeRazSoc";

                cboxFornecedorEditarPeca.ValueMember = "codPeca";

                cboxFornecedorEditarPeca.DataSource = dt;

                cboxFornecedorEditarPeca.Text = "Selecionar";

            }
            catch (Exception)
            {
                MessageBox.Show("Erro");
            }
            finally
            {
                conexao.Close();
            }

        }




        //Metodo para gravar no bando
        private void btnSalvarPeca_Click(object sender, EventArgs e)
        {
            //Declarações das variaveis do input text
            string cadastroNomePeca = txtNomeCadastrarPeca.Text;
            string cadastroFornecedorPeca = cboxFornecedorCadastrarPeca.Text;
            string cadastroFabricantePeca = txtFabricanteCadastrarPeca.Text;
            string cadastroValorCompra = txtCadastroValorCompraPeca.Text;
            string cadastroValorVenda = numericValorVendaCadastrarPeca.Text;


            //Validaçao do formulario 
            if( cadastroNomePeca == string.Empty || 
                cadastroFornecedorPeca == "Selecionar" || 
                cadastroFabricantePeca == string.Empty ||
                cadastroValorCompra == string.Empty ||
                cadastroValorVenda == string.Empty)
            {
                MessageBox.Show("Campo precisa ser preenchido");
            }
            else
            {

                SqlConnection conexao = abreConexao();
                sql = "SELECT idCad FROM cadClientForn where nomeRazSoc = '" + cboxFornecedorCadastrarPeca.Text + "'";

                SqlCommand cmd = new SqlCommand(sql, conexao);
                conexao.Open();
                int idFornecedorPeca = (int)cmd.ExecuteScalar();


                try
                {
                    conexao = abreConexao();

                    sql = "INSERT INTO cadPeca VALUES ( '" + cadastroNomePeca + "'," +
                        "'" + idFornecedorPeca + "'," +
                        "'" + cadastroFabricantePeca + "'," +
                        "" + Convert.ToDouble(cadastroValorCompra.Replace(",", ".")) + "," +
                        "" + Convert.ToDouble(cadastroValorVenda.Replace(",", ".")) + ")";

                    SqlCommand comandCadastro = new SqlCommand(sql, conexao);
                    conexao.Open();

                    comandCadastro.ExecuteNonQuery();

                    comandCadastro.Dispose();

                    conexao.Close();

                    MessageBox.Show("Salvo com sucesso");

                    txtNomeCadastrarPeca.Text = string.Empty;
                    txtCadastroValorCompraPeca.Text = string.Empty;
                    cboxFornecedorCadastrarPeca.Text = "Selecionar";
                    txtFabricanteCadastrarPeca.Text = string.Empty;
                    numericValorVendaCadastrarPeca.Text = string.Empty;
                }

                catch (Exception)
                {
                    MessageBox.Show("Erro ao salvar dados");
                }
            }


        }


        //Area de deletar a peça
        private void buttonDeletarPeca_Click(object sender, EventArgs e)
        {
            string idDelete = txtIdDeletarPeca.Text;

            if ( idDelete == string.Empty )
            {
                MessageBox.Show("Insira o Id da peça para deletar");
            }
            else
            {
                SqlConnection conexao = abreConexao();

                sql = "DELETE FROM cadPeca WHERE codPeca = " + Convert.ToInt32( idDelete) + "";

                SqlCommand comandoDelete = new SqlCommand(sql, conexao);

                conexao.Open();
                comandoDelete.ExecuteNonQuery();

                comandoDelete.Dispose();
                conexao.Close();

                MessageBox.Show("Deletado com exito");
            }
        }




        //Botao de consulta
        private void btnConsultar_Click(object sender, EventArgs e)
        {
            dgvPecas.Rows.Clear();
            listagem();
           
        }





        //Listagem para dar select
        private void listagem()
        {
            SqlConnection conexao = abreConexao();


            sql = "SELECT * FROM cadPeca";

            SqlCommand comandoConsulta = new SqlCommand(sql, conexao);

            conexao.Open();
            SqlDataReader dados = comandoConsulta.ExecuteReader();

            comandoConsulta.Dispose();

            while (dados.Read())
            {
                dgvPecas.Rows.Add(dados[0], dados[1], dados[2], dados[3], dados[4], dados[5]);
            }

            conexao.Close();

        }




        //Metodo que preenche o combobox ao iniciar o form
        private void CadastroPecas_Shown(object sender, EventArgs e)
        {
            preencherComboBoxCadastrar();
           
        }



        //Botao de pesquisa na area de editar peca
        private void btnConsultaEditaPeca_Click(object sender, EventArgs e)
        {
            string idConsutarPeca = txtIdEditarPeca.Text;

            txtNomeEditarPeca.Text = string.Empty;
            txtFabricanteEditarPeca.Text = string.Empty;
            numericValorCompraEditar.Text = string.Empty;
            txtValorVendaEditarPeca.Text = string.Empty;



            if (idConsutarPeca == string.Empty)
            {
                MessageBox.Show("Digite um id");
            }
            else
            {
                cboxFornecedorEditarPeca.Enabled = true;

                preencherComboBoxEditar();


                try
                {
                    SqlConnection conexao = abreConexao();


                    sql = "SELECT * FROM cadPeca WHERE codPeca = " + idConsutarPeca + " ";

                    SqlCommand comandoConsultaEdita = new SqlCommand(sql, conexao);

                    conexao.Open();
                    SqlDataReader dados = comandoConsultaEdita.ExecuteReader();

                    dados.Read();

                    double valorCompraEditarPeca = Convert.ToDouble(dados[4]);
                    double valorVendaEditarPeca = Convert.ToDouble(dados[5]);

                    txtNomeEditarPeca.Text = (string)dados[1];
                    txtFabricanteEditarPeca.Text = (string)dados[3];
                    numericValorCompraEditar.Text = valorCompraEditarPeca.ToString();
                    txtValorVendaEditarPeca.Text = valorVendaEditarPeca.ToString();


                    comandoConsultaEdita.Dispose();

                }
                catch (Exception)
                {
                    cboxFornecedorEditarPeca.Enabled = false;
                    MessageBox.Show("Id não encontrado");
                }

            }
         
        }





        //Area do update da peca
        private void buttonEditarPeca_Click(object sender, EventArgs e)
        {

            SqlConnection conexao = abreConexao();

            if ( txtIdEditarPeca.Text == string.Empty ||
                txtNomeEditarPeca.Text == string.Empty || 
                cboxFornecedorEditarPeca.Text == "Selecionar" ||
                txtFabricanteEditarPeca.Text == string.Empty ||
                numericValorCompraEditar.Text == string.Empty ||
                txtValorVendaEditarPeca.Text == string.Empty)
            {
                MessageBox.Show("Veja se todos os campos estão preenchidos corretamente");
            }
            else
            {

                conexao = abreConexao();
                sql = "SELECT idCad FROM cadClientForn where nomeRazSoc = '" + cboxFornecedorEditarPeca.Text + "'";

                SqlCommand cmd = new SqlCommand(sql, conexao);
                conexao.Open();
                int idFornecedorEditarPeca = (int)cmd.ExecuteScalar();

                //Variaveis
                string idEdiatarPeca = txtIdEditarPeca.Text;
                string nomeEditarPeca = txtNomeEditarPeca.Text;
                string fabricanteEditarPeca = txtFabricanteEditarPeca.Text;
                string valorCompraEditarPeca = numericValorCompraEditar.Text;
                string valorVendaEditarPeca = txtValorVendaEditarPeca.Text;

                try
                {
                    conexao = abreConexao();

                    sql = "UPDATE cadPeca SET nomePeca = '" + nomeEditarPeca + "', fkFornecedor = " + idFornecedorEditarPeca + ", fabricante = '" + fabricanteEditarPeca + "', vlCompra = " + Convert.ToDouble(valorCompraEditarPeca) + ", vlVenda = " + Convert.ToDouble(valorVendaEditarPeca) + " WHERE codPeca = " + Convert.ToInt32(idEdiatarPeca) + "";

                    SqlCommand comandoEditar = new SqlCommand(sql, conexao);
                    conexao.Open();
                    comandoEditar.ExecuteNonQuery();

                    comandoEditar.Dispose();
                    conexao.Close();

                    MessageBox.Show("Atualizado com sucesso");


                    txtIdEditarPeca.Text = string.Empty;
                    txtNomeEditarPeca.Text = string.Empty;                
                    txtFabricanteEditarPeca.Text = string.Empty;
                    numericValorCompraEditar.Text = string.Empty;
                    txtValorVendaEditarPeca.Text = string.Empty;
                    txtIdEditarPeca.Enabled = true;


                }

                catch(Exception){
                    MessageBox.Show("Erro ao atualizar");
                }
                finally
                {
                    conexao.Close();

                    cboxFornecedorEditarPeca.Text = "Selecionar";
                    cboxFornecedorEditarPeca.Enabled = false;
                }

            }

        }
    }
}

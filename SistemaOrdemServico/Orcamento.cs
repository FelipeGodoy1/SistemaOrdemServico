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
    public partial class Orcamento : Form
    {
        private static SqlConnection conexaoSql;
        private static Dictionary<string, Action> modos;
        Dictionary<string, string> camposDeEntrada;


        public Orcamento()
        {
            conexaoSql = new SqlConnection(Form1.GetStringConecao("GustavoDanielCasa"));
            //conexaoSql = new SqlConnection(Form1.GetStringConecao("GustavoDanielFaculdade"));
            modos = new Dictionary<string, Action>
            {
                { "Inserir", Inserir },
                { "Editar", Editar },
                { "Deletar", Deletar }
            };
            camposDeEntrada = new Dictionary<string, string>();

            InitializeComponent();
        }

        private void Inserir()
        {
            CarregarValorCampos();

            if (!Form1.TemCamposVazios(camposDeEntrada) && ValidaComboBoxes())
            {
                Dictionary<string, string> dadosAEnviar = new Dictionary<string, string>();

                camposDeEntrada.ToList().ForEach(campo => dadosAEnviar.Add(
                    "@" + campo.Key.Split(' ')[0].Replace(":", ""),
                    campo.Value)
                );

                SqlInsert("cadOrcamento", dadosAEnviar);
            }
        }

        private void Deletar()
        {
            throw new NotImplementedException();
        }

        private void Editar()
        {
            throw new NotImplementedException();
        }

        private void CarregarValorCampos()
        {
            this.camposDeEntrada = new Dictionary<string, string>{
                { lblClienteOrcamento.Text, cbClienteOrcamento.Text},
                { lblDataEntradaOrcamento.Text, dtpDataEntradaOrcamento.Value.Date.ToString("yyyy-MM-dd")},
                { lblDescricaoOrcamento.Text, txtDescricaoOrcamento.Text},
                { lblPecasOrcamento.Text, cbPecasOrcamento.Text},
                { lblValorOrcamento.Text, nudValorOrcamento.Text == "0,00" ? string.Empty : nudValorOrcamento.Text.Replace(",", ".")},
                { lblRecebidoOrcamento.Text, cbRecebidoOrcamento.Text}
            };
        }

        private void PopularComboBox(ComboBox comboBox, List<List<string>> itens)
        {
            var itensDicionario = itens.ToDictionary(
                keySelector: cliente => cliente[0],
                elementSelector: cliente => string.Join(" - ", cliente.Skip(1))
                );

            comboBox.DisplayMember = "Value";
            comboBox.ValueMember = "Key";
            comboBox.DataSource = new BindingSource(itensDicionario, null);
            comboBox.SelectedIndex = -1;
        }

        public List<List<string>> SqlSelect(string tabela, params string[] colunas)
        {
            List<List<string>> registros = new List<List<string>>();

            string comandoString = $"SELECT {string.Join(", ", colunas)} from {tabela}";
            SqlCommand comandoSql = new SqlCommand(comandoString, conexaoSql);

            try
            {
                comandoSql.Connection.Open();
                SqlDataReader leitor = comandoSql.ExecuteReader();

                while (leitor.Read())
                {
                    List<string> campos = new List<string>();

                    for (int i = 0; i < leitor.FieldCount; i++)
                    {
                        campos.Add(leitor[i].GetType() != typeof(DateTime) ? leitor[i].ToString() : ((DateTime)leitor[i]).ToShortDateString());
                    }

                    registros.Add(campos);
                }
            }
            catch (Exception ex)
            {
                MostrarMensagemErro(ex.Message);
            }
            finally
            {
                comandoSql.Connection.Close();
                comandoSql.Dispose();
            }

            return registros;
        }
        
        private void SqlInsert(string tabela, Dictionary<string, string> campos)
        {
            string comandoString = $"INSERT INTO cadOrcamento VALUES({ string.Join(", ", campos.Select(campo => campo.Key) )})";
            SqlCommand comandoSql = new SqlCommand(comandoString, conexaoSql);
            campos.ToList().ForEach(campo => comandoSql.Parameters.AddWithValue(campo.Key, campo.Value));

            try
            {
                comandoSql.Connection.Open();
                comandoSql.ExecuteNonQuery();

                MostrarMensagemSucesso("Enviado para o banco de dados com sucesso.");
            }
            catch (Exception ex)
            {
                MostrarMensagemErro(ex.Message);
            }
            finally
            {
                comandoSql.Connection.Close();
                comandoSql.Dispose();
            }
        }

        public void MostrarMensagemErro(string message)
        {
            MessageBox.Show(
                message,
                "Erro",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
                );
        }
        
        private void MostrarMensagemSucesso(string message)
        {
            MessageBox.Show(
                message,
                "Sucesso",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
                );
        }

        private bool ValidaComboBoxes()
        {
            var comboBoxes = new Dictionary<string, ComboBox>
            {
                { lblClienteOrcamento.Text, cbClienteOrcamento },
                { lblPecasOrcamento.Text, cbPecasOrcamento },
                { lblRecebidoOrcamento.Text, cbRecebidoOrcamento }
            };

            foreach (var comboBox in comboBoxes)
            {
                if (comboBox.Value.SelectedIndex != -1)
                {
                    var itemSelecionado = (KeyValuePair<string, string>)comboBox.Value.SelectedItem;

                    camposDeEntrada[comboBox.Key] = itemSelecionado.Key;
                }
                else
                {
                    MostrarMensagemErro($"Selecione um valor valido no campo \"{comboBox.Key.Replace(":", "")}\"");
                    return false;
                }
            }

            return true;
        }

        private void Orcamento_Load(object sender, EventArgs e)
        {
            dtpDataEntradaOrcamento.Value = DateTime.Today;

            PopularComboBox(cbPecasOrcamento, SqlSelect("cadPeca", "codPeca", "nomePeca", "fabricante"));
            PopularComboBox(cbClienteOrcamento, SqlSelect("cadClientForn", "idCad", "nomeRazSoc"));
            PopularComboBox(cbRecebidoOrcamento, SqlSelect("cadFunc", "idFunc", "nome"));
        }

        private void btnInserirOrcamento_Click(object sender, EventArgs e)
        {
            modos[btnEnviar.Text]();
        }

        private void MudarModo(object sender, EventArgs e)
        {
            Button btnClicado = (Button)sender;
            btnEnviar.Text = btnClicado.Text;
            string[] btnsChamarJanela = { "Editar", "Excluir" };

            if (btnsChamarJanela.Contains(btnClicado.Text))
            {
                SelecionarOrcamento selecionarOrcamento = new SelecionarOrcamento();
                selecionarOrcamento.Show();
            }
            //Melhorar visualização do modo ativado no design do formulário
        }
    }
}

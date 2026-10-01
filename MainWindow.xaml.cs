using System.Globalization;
using System.Windows;
using Checkpoint05Crud.Models;
using Checkpoint05Crud.Repositories;
using Microsoft.Extensions.Configuration;

namespace Checkpoint05Crud
{
    public partial class MainWindow : Window
    {
        private readonly ProdutoRepository _repository;

        public MainWindow()
        {
            InitializeComponent();

            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            string connectionString =
                configuration.GetConnectionString("DefaultConnection")
                ?? throw new Exception("Connection string não encontrada.");

            _repository = new ProdutoRepository(connectionString);
        }

        private void BtnInserir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ValidarCampos())
                    return;

                Produto produto = new Produto
                {
                    Nome = txtNome.Text.Trim(),
                    Preco = decimal.Parse(txtPreco.Text, CultureInfo.CurrentCulture),
                    Estoque = int.Parse(txtEstoque.Text),
                    Categoria = txtCategoria.Text.Trim()
                };

                _repository.Inserir(produto);

                MessageBox.Show(
                    "Produto cadastrado com sucesso!",
                    "Sucesso",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                LimparCampos();
                CarregarProdutos();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Erro ao cadastrar produto:\n" + ex.Message,
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void BtnListar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CarregarProdutos();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Erro ao listar produtos:\n" + ex.Message,
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!int.TryParse(txtId.Text, out int id))
                {
                    MessageBox.Show(
                        "Informe um ID válido.",
                        "Atenção",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return;
                }

                Produto? produto = _repository.BuscarPorId(id);

                if (produto == null)
                {
                    MessageBox.Show(
                        "Produto não encontrado.",
                        "Busca",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    return;
                }

                txtNome.Text = produto.Nome;
                txtPreco.Text = produto.Preco.ToString("F2");
                txtEstoque.Text = produto.Estoque.ToString();
                txtCategoria.Text = produto.Categoria;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Erro ao buscar produto:\n" + ex.Message,
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void BtnAtualizar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!int.TryParse(txtId.Text, out int id))
                {
                    MessageBox.Show(
                        "Informe o ID do produto que deseja atualizar.",
                        "Atenção",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return;
                }

                if (!ValidarCampos())
                    return;

                Produto? existente = _repository.BuscarPorId(id);

                if (existente == null)
                {
                    MessageBox.Show(
                        "Produto não encontrado.",
                        "Atenção",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return;
                }

                Produto produto = new Produto
                {
                    Id = id,
                    Nome = txtNome.Text.Trim(),
                    Preco = decimal.Parse(txtPreco.Text, CultureInfo.CurrentCulture),
                    Estoque = int.Parse(txtEstoque.Text),
                    Categoria = txtCategoria.Text.Trim()
                };

                _repository.Atualizar(produto);

                MessageBox.Show(
                    "Produto atualizado com sucesso!",
                    "Sucesso",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                LimparCampos();
                CarregarProdutos();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Erro ao atualizar produto:\n" + ex.Message,
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void BtnExcluir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!int.TryParse(txtId.Text, out int id))
                {
                    MessageBox.Show(
                        "Informe o ID do produto que deseja excluir.",
                        "Atenção",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return;
                }

                Produto? produto = _repository.BuscarPorId(id);

                if (produto == null)
                {
                    MessageBox.Show(
                        "Produto não encontrado.",
                        "Atenção",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return;
                }

                MessageBoxResult resposta = MessageBox.Show(
                    $"Deseja realmente excluir o produto \"{produto.Nome}\"?",
                    "Confirmar exclusão",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (resposta != MessageBoxResult.Yes)
                    return;

                _repository.Excluir(id);

                MessageBox.Show(
                    "Produto excluído com sucesso!",
                    "Sucesso",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                LimparCampos();
                CarregarProdutos();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Erro ao excluir produto:\n" + ex.Message,
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void BtnLimpar_Click(object sender, RoutedEventArgs e)
        {
            LimparCampos();
        }

        private void CarregarProdutos()
        {
            dgProdutos.ItemsSource = _repository.Listar();
        }

        private void LimparCampos()
        {
            txtId.Clear();
            txtNome.Clear();
            txtPreco.Clear();
            txtEstoque.Clear();
            txtCategoria.Clear();

            txtNome.Focus();
        }

        private bool ValidarCampos()
        {
            if (string.IsNullOrWhiteSpace(txtNome.Text))
            {
                MessageBox.Show("Informe o nome do produto.");
                return false;
            }

            if (!decimal.TryParse(
                    txtPreco.Text,
                    NumberStyles.Number,
                    CultureInfo.CurrentCulture,
                    out decimal preco) || preco < 0)
            {
                MessageBox.Show("Informe um preço válido.");
                return false;
            }

            if (!int.TryParse(txtEstoque.Text, out int estoque) || estoque < 0)
            {
                MessageBox.Show("Informe um estoque válido.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtCategoria.Text))
            {
                MessageBox.Show("Informe a categoria do produto.");
                return false;
            }

            return true;
        }
    }
}
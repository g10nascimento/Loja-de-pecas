using System;
using System.Collections.Generic;
using System.IO;
using System.Linq; // Necessário para usar o método .All()

namespace ConsoleApp1
{
    struct Peca
    {
        public int Codigo;
        public string Nome;
        public double Preco;
    }

    internal class Program
    {
        // Caminho do arquivo TXT onde os dados serão salvos
        static string caminho = "pecas.txt";

        static void Main(string[] args)
        {
            int opcao; // Armazena a escolha do usuário no menu
            do
            {
                // Exibe o menu principal
                Console.Clear();

                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("#----------------------------------------------------------------------------------#");
                Console.WriteLine("||                                   MENU                                         ||");
                Console.WriteLine("||--------------------------------------------------------------------------------||");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.DarkBlue;
                Console.WriteLine("||---------------------------- Escolha uma opção ---------------------------------||");
                Console.WriteLine("||--------------------------------------------------------------------------------||");
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("|| 1 || Cadastrar peças ----------------------------------------------------------||");
                Console.WriteLine("|| 2 || Consultar estoque --------------------------------------------------------||");
                Console.WriteLine("|| 3 || Editar peça --------------------------------------------------------------||");
                Console.WriteLine("|| 4 || Excluir peça -------------------------------------------------------------||");
                Console.WriteLine("|| 0 || Sair ---------------------------------------------------------------------||");
                Console.WriteLine("#----------------------------------------------------------------------------------#");

                // Lê a opção escolhida pelo usuário
                int.TryParse(Console.ReadLine(), out opcao);

                // Escolhe a ação de acordo com a opção digitada
                switch (opcao)
                {
                    case 1:
                        CadastrarPeca();
                        break;
                    case 2:
                        ConsultarEstoque();
                        break;
                    case 3:
                        EditarPeca();
                        break;
                    case 4:
                        ExcluirPeca();
                        break;
                    case 0:
                        Console.WriteLine("Finalizando o programa...");
                        break;
                    default:
                        Console.WriteLine("Opção não encontrada!");
                        Console.ReadLine();
                        break;
                }

            } while (opcao != 0);
        }

        // -------------------- CADASTRAR --------------------
        public static void CadastrarPeca()
        {
            Peca p;

            // Entrada de dados
            Console.Write("Digite o código da peça: ");
            p.Codigo = int.Parse(Console.ReadLine());

            Console.Write("Digite o nome da peça: ");
            p.Nome = Console.ReadLine();

            Console.Write("Digite o preço da peça: ");
            p.Preco = double.Parse(Console.ReadLine());

            // Abre o arquivo em modo append (adicionar)
            using (StreamWriter sw = new StreamWriter(caminho, true))
            {
                // Salva no formato: Codigo;Nome;Preco
                sw.WriteLine($"{p.Codigo};{p.Nome};{p.Preco}");
            }

            Console.WriteLine("Peça cadastrada com sucesso!");
            Console.WriteLine("Pressione ENTER para voltar ao menu...");
            Console.ReadLine();
        }

        // -------------------- CONSULTAR --------------------
        public static void ConsultarEstoque()
        {
            if (!ArquivoValido()) return;

            string[] linhas = File.ReadAllLines(caminho);

            Console.WriteLine("\n--- LISTA DAS PEÇAS CADASTRADAS ---");

            // Lista apenas linhas válidas
            foreach (string linha in linhas)
            {
                if (string.IsNullOrWhiteSpace(linha)) continue;

                string[] dados = linha.Split(';');
                if (dados.Length >= 3)
                    Console.WriteLine($"Código: {dados[0]} | Nome: {dados[1]} | Preço: R$ {dados[2]}");
            }

            Console.WriteLine("\nPressione ENTER para voltar ao menu...");
            Console.ReadLine();
        }

        // -------------------- EDITAR --------------------
        public static void EditarPeca()
        {
            if (!ArquivoValido()) return;

            Console.Write("Digite o código da peça que deseja editar: ");
            int codigo = int.Parse(Console.ReadLine());

            List<string> linhas = new List<string>(File.ReadAllLines(caminho));
            bool encontrado = false;

            for (int i = 0; i < linhas.Count; i++)
            {
                if (string.IsNullOrWhiteSpace(linhas[i])) continue;

                string[] dados = linhas[i].Split(';');
                if (int.Parse(dados[0]) == codigo)
                {
                    encontrado = true;

                    Console.Write("Novo nome: ");
                    string novoNome = Console.ReadLine();

                    Console.Write("Novo preço: ");
                    double novoPreco = double.Parse(Console.ReadLine());

                    // Substitui a linha pela nova versão
                    linhas[i] = $"{codigo};{novoNome};{novoPreco}";
                    File.WriteAllLines(caminho, linhas);
                    Console.WriteLine("✏️ Peça editada com sucesso!");
                    break;
                }
            }

            if (!encontrado)
                Console.WriteLine("Peça não encontrada.");

            Console.WriteLine("Pressione ENTER para voltar ao menu...");
            Console.ReadLine();
        }

        // -------------------- EXCLUIR --------------------
        public static void ExcluirPeca()
        {
            if (!ArquivoValido()) return;

            Console.Write("Digite o código da peça que deseja excluir: ");
            int codigo = int.Parse(Console.ReadLine());

            List<string> linhas = new List<string>(File.ReadAllLines(caminho));
            bool encontrado = false;

            for (int i = 0; i < linhas.Count; i++)
            {
                if (string.IsNullOrWhiteSpace(linhas[i])) continue;

                string[] dados = linhas[i].Split(';');
                if (int.Parse(dados[0]) == codigo)
                {
                    encontrado = true;

                    linhas.RemoveAt(i);
                    File.WriteAllLines(caminho, linhas);
                    Console.WriteLine("🗑️ Peça excluída com sucesso!");
                    break;
                }
            }

            if (!encontrado)
                Console.WriteLine("Peça não encontrada.");

            Console.WriteLine("Pressione ENTER para voltar ao menu...");
            Console.ReadLine();
        }

        // -------------------- VERIFICAÇÃO AUXILIAR --------------------
        private static bool ArquivoValido()
        {
            // Verifica se o arquivo existe
            if (!File.Exists(caminho))
            {
                Console.WriteLine("Nenhuma peça foi cadastrada ainda! (arquivo não existe)");
                Console.WriteLine("Pressione ENTER para voltar ao menu...");
                Console.ReadLine();
                return false;
            }

            // Verifica se está vazio
            if (new FileInfo(caminho).Length == 0)
            {
                Console.WriteLine("Nenhuma peça foi cadastrada ainda! (arquivo vazio)");
                Console.WriteLine("Pressione ENTER para voltar ao menu...");
                Console.ReadLine();
                return false;
            }

            // Verifica se só contém linhas em branco
            string[] linhas = File.ReadAllLines(caminho);
            if (linhas.All(string.IsNullOrWhiteSpace))
            {
                Console.WriteLine("Nenhuma peça foi cadastrada ainda! (somente linhas em branco)");
                Console.WriteLine("Pressione ENTER para voltar ao menu...");
                Console.ReadLine();
                return false;
            }

            return true;
        }
    }
}





using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ConsoleApp1
{
    // Estrutura que representa uma peça em estoque
    struct Peca
    {
        public int Codigo;      // Código identificador da peça
        public string Nome;     // Nome da peça
        public double Preco;    // Preço 
        public int Quantidade;  // Quantidade em estoque
    }

    // Estrutura que representa uma venda registrada
    struct Venda
    {
        public DateTime Data;   // Data e hora da venda
        public int Codigo;      // Código da peça vendida
        public string Nome;     // Nome da peça
        public double Preco;    // Preço 
        public int Quantidade;  // Quantidade vendida
        public double Total;    // Valor total da venda (Preço x Quantidade)
    }

    internal class Program
    {
        // Caminhos dos arquivos que armazenam dados
        static string caminho = "pecas.txt";       // Arquivo de peças
        static string caminhoVendas = "vendas.txt"; // Arquivo de vendas

        // Lista temporária de peças em memória (antes de salvar em arquivo) 
        static List<Peca> pecas = new List<Peca>();

        static void Main(string[] args)
        {
            int opcao;
            do
            {
                // Mostra o menu principal formatado
                Console.Clear();
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("#----------------------------------------------------------------------------------#");
                Console.ResetColor();
                Console.WriteLine("                                     MENU                                           ");
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("||--------------------------------------------------------------------------------||");
                Console.ResetColor();
                Console.WriteLine("                                Escolha uma opção                                   ");
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("||--------------------------------------------------------------------------------||");
                Console.WriteLine("|| 1 || Cadastrar peças ----------------------------------------------------------||");
                Console.WriteLine("|| 2 || Consultar estoque --------------------------------------------------------||");
                Console.WriteLine("|| 3 || Editar peça --------------------------------------------------------------||");
                Console.WriteLine("|| 4 || Excluir peça -------------------------------------------------------------||");
                Console.WriteLine("|| 5 || Registrar venda ----------------------------------------------------------||");
                Console.WriteLine("|| 6 || Relatório de vendas ------------------------------------------------------||");
                Console.WriteLine("|| 0 || Sair ---------------------------------------------------------------------||");
                Console.WriteLine("#----------------------------------------------------------------------------------#");
                Console.ResetColor();

                // Lê a opção escolhida
                int.TryParse(Console.ReadLine(), out opcao);

                // Chama a função correspondente
                switch (opcao)
                {
                    case 1: CadastrarPecas(); break;
                    case 2: ConsultarEstoque(); break;
                    case 3: EditarPeca(); break;
                    case 4: ExcluirPeca(); break;
                    case 5: RegistrarVenda(); break;
                    case 6: RelatorioVendas(); break;
                    case 0:
                        // Antes de sair, salva as peças em arquivo, se houver na memória
                        if (pecas.Count > 0)
                        {
                            SalvarNoArquivo();
                            Console.WriteLine(" Alterações salvas no arquivo!");
                        }
                        Console.WriteLine("Finalizando o programa...");
                        break;
                    default:
                        // Caso o usuário digite uma opção inválida
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine("Opção não encontrada!");
                        Console.ReadLine();
                        Console.ResetColor();
                        break;
                }

            } while (opcao != 0); // Repete até o usuário escolher sair
        }

        // CADASTRAR PEÇAS 
        public static void CadastrarPecas()
        {
            string continuar;
            do
            {
                Peca p;

                // Entrada e validação do código
                while (true)
                {
                    Console.Write("Digite o código da peça: ");
                    if (int.TryParse(Console.ReadLine(), out p.Codigo)) break;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(" Código inválido! Digite um número inteiro.");
                    Console.ResetColor();
                }

                // Entrada do nome da peça
                Console.Write("Digite o nome da peça: ");
                p.Nome = Console.ReadLine();

                // Entrada e validação do preço
                while (true)
                {
                    Console.Write("Digite o preço da peça: ");
                    if (double.TryParse(Console.ReadLine(), out p.Preco)) break;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(" Preço inválido! Digite um número (ex: 10,50).");
                    Console.ResetColor();
                }

                // Entrada e validação da quantidade
                while (true)
                {
                    Console.Write("Digite a quantidade em estoque: ");
                    if (int.TryParse(Console.ReadLine(), out p.Quantidade)) break;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(" Quantidade inválida! Digite um número inteiro.");
                    Console.ResetColor();
                }

                // Adiciona peça na lista em memória
                pecas.Add(p);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(" Peça cadastrada na memória!");
                Console.ResetColor();

                Console.Write("Deseja cadastrar outra peça? (s/n): ");
                continuar = Console.ReadLine().ToLower();

            } while (continuar == "s");

            // Salva todas as peças cadastradas em arquivo
            SalvarNoArquivo();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(" Todas as peças foram salvas no arquivo!");
            Console.ResetColor();
            Console.WriteLine("Pressione ENTER para voltar ao menu...");
            Console.ReadLine();
        }

        //  CONSULTAR ESTOQUE 
        public static void ConsultarEstoque()
        {
            if (!ArquivoValido()) return;

            string[] linhas = File.ReadAllLines(caminho);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n--- LISTA DAS PEÇAS CADASTRADAS ---");
            Console.WriteLine($"\n{"CÓDIGO",-10} {"NOME",-20} {"PREÇO",-10} {"QTD",-5}");
            Console.WriteLine(new string('-', 55));
            Console.ResetColor();

            // Mostra todas as peças formatadas em tabela
            foreach (string linha in linhas)
            {
                if (string.IsNullOrWhiteSpace(linha)) continue;
                string[] dados = linha.Split(';');

                if (dados.Length >= 4 &&
                    double.TryParse(dados[2], out double preco) &&
                    int.TryParse(dados[3], out int qtd))
                {
                    Console.WriteLine($"{dados[0],-10} {dados[1],-20} R$ {preco,-10:F2} {qtd,-5}");
                }
            }

            Console.WriteLine("\nPressione ENTER para voltar ao menu...");
            Console.ReadLine();
        }

        //  EDITAR PEÇA 
        public static void EditarPeca()
        {
            if (!ArquivoValido()) return;

            Console.Write("Digite o código da peça que deseja editar: ");
            if (!int.TryParse(Console.ReadLine(), out int codigo))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(" Código inválido!");
                Console.ResetColor();
                Console.ReadLine();
                return;
            }

            List<string> linhas = new List<string>(File.ReadAllLines(caminho));
            bool encontrado = false;

            // Procura a peça pelo código
            for (int i = 0; i < linhas.Count; i++)
            {
                if (string.IsNullOrWhiteSpace(linhas[i])) continue;
                string[] dados = linhas[i].Split(';');
                if (dados.Length < 4) continue;

                if (int.Parse(dados[0]) == codigo)
                {
                    encontrado = true;

                    // Entrada de novos valores
                    Console.Write("Novo nome: ");
                    string novoNome = Console.ReadLine();

                    Console.Write("Novo preço: ");
                    if (!double.TryParse(Console.ReadLine(), out double novoPreco))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(" Preço inválido!");
                        Console.ResetColor();
                        return;
                    }

                    Console.Write("Nova quantidade: ");
                    if (!int.TryParse(Console.ReadLine(), out int novaQtd))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(" Quantidade inválida!");
                        Console.ResetColor();
                        return;
                    }

                    // Substitui linha no arquivo
                    linhas[i] = $"{codigo};{novoNome};{novoPreco};{novaQtd}";
                    File.WriteAllLines(caminho, linhas);

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(" Peça editada com sucesso!");
                    Console.ResetColor();
                    break;
                }
            }

            if (!encontrado)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Peça não encontrada.");
                Console.ResetColor();
            }

            Console.WriteLine("Pressione ENTER para voltar ao menu...");
            Console.ReadLine();
        }

        //  EXCLUIR PEÇA 
        public static void ExcluirPeca()
        {
            if (!ArquivoValido()) return;

            Console.Write("Digite o código da peça que deseja excluir: ");
            if (!int.TryParse(Console.ReadLine(), out int codigo))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(" Código inválido!");
                Console.ResetColor();
                Console.ReadLine();
                return;
            }

            List<string> linhas = new List<string>(File.ReadAllLines(caminho));
            bool encontrado = false;

            // Procura e remove a peça do arquivo
            for (int i = 0; i < linhas.Count; i++)
            {
                if (string.IsNullOrWhiteSpace(linhas[i])) continue;
                string[] dados = linhas[i].Split(';');

                if (int.Parse(dados[0]) == codigo)
                {
                    encontrado = true;
                    linhas.RemoveAt(i);
                    File.WriteAllLines(caminho, linhas);

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Peça excluída com sucesso!");
                    Console.ResetColor();
                    break;
                }
            }

            if (!encontrado)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Peça não encontrada.");
                Console.ResetColor();
            }

            Console.WriteLine("Pressione ENTER para voltar ao menu...");
            Console.ReadLine();
        }

        //  REGISTRAR VENDA 
        public static void RegistrarVenda()
        {
            if (!ArquivoValido()) return;

            List<string> linhas = File.ReadAllLines(caminho).ToList();
            List<Venda> vendasTemp = new List<Venda>(); // Lista temporária de vendas

            string continuar;
            do
            {
                Console.Write("Digite o código da peça vendida: ");
                if (!int.TryParse(Console.ReadLine(), out int codigo))
                {
                    Console.WriteLine("Código inválido!");
                    return;
                }

                bool encontrado = false;

                // Procura peça pelo código
                for (int i = 0; i < linhas.Count; i++)
                {
                    if (string.IsNullOrWhiteSpace(linhas[i])) continue;
                    string[] dados = linhas[i].Split(';');
                    if (dados.Length < 4) continue;

                    if (int.TryParse(dados[0], out int codPeca) && codPeca == codigo)
                    {
                        encontrado = true;

                        string nome = dados[1];
                        if (!double.TryParse(dados[2], out double preco) ||
                            !int.TryParse(dados[3], out int qtdEstoque))
                        {
                            Console.WriteLine("Erro nos dados da peça!");
                            return;
                        }

                        Console.Write("Quantidade a vender: ");
                        if (!int.TryParse(Console.ReadLine(), out int qtdVenda) || qtdVenda <= 0)
                        {
                            Console.WriteLine("Quantidade inválida!");
                            return;
                        }

                        if (qtdVenda > qtdEstoque)
                        {
                            Console.WriteLine("Estoque insuficiente!");
                            return;
                        }

                        // Atualiza estoque
                        int novoEstoque = qtdEstoque - qtdVenda;
                        linhas[i] = $"{codigo};{nome};{preco};{novoEstoque}";

                        // Calcula valor da venda
                        double total = preco * qtdVenda;

                        // Adiciona venda temporária (será salva depois)
                        vendasTemp.Add(new Venda
                        {
                            Data = DateTime.Now,
                            Codigo = codigo,
                            Nome = nome,
                            Preco = preco,
                            Quantidade = qtdVenda,
                            Total = total
                        });

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Venda registrada na memória! Total: R$ {total:F2}");
                        Console.ResetColor();
                        break;
                    }
                }

                if (!encontrado)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Peça não encontrada!");
                    Console.ResetColor();
                }

                Console.Write("Deseja registrar outra venda? (s/n): ");
                continuar = Console.ReadLine().ToLower();

            } while (continuar == "s");

            // Atualiza estoque no arquivo
            File.WriteAllLines(caminho, linhas);

            // Salva vendas no arquivo de vendas
            using (StreamWriter sw = new StreamWriter(caminhoVendas, true))
            {
                foreach (var v in vendasTemp)
                {
                    sw.WriteLine($"{v.Data};{v.Codigo};{v.Nome};{v.Preco};{v.Quantidade};{v.Total}");
                }
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Todas as vendas foram salvas no arquivo!");
            Console.ResetColor();

            Console.WriteLine("Pressione ENTER...");
            Console.ReadLine();
        }

        //  RELATÓRIO DE VENDAS 
        public static void RelatorioVendas()
        {
            // Verifica se já existem vendas registradas
            if (!File.Exists(caminhoVendas))
            {
                Console.WriteLine("Nenhuma venda registrada ainda!");
                Console.ReadLine();
                return;
            }

            // Lê todas as vendas do arquivo
            string[] vendas = File.ReadAllLines(caminhoVendas);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n--- RELATÓRIO DE VENDAS ---");
            Console.WriteLine($"\n{"DATA",-20} {"CÓDIGO",-8} {"NOME",-20} {"PREÇO",-10} {"QTD",-5} {"TOTAL",-10}");
            Console.WriteLine(new string('-', 75));

            double soma = 0; // Total acumulado das vendas

            foreach (string v in vendas)
            {
                if (string.IsNullOrWhiteSpace(v)) continue;
                string[] dados = v.Split(';');

                if (dados.Length < 6) continue;

                // Converte dados da linha
                if (double.TryParse(dados[3], out double preco) &&
                    int.TryParse(dados[4], out int qtd) &&
                    double.TryParse(dados[5], out double total))
                {
                    // Mostra cada venda formatada em colunas
                    Console.WriteLine($"{dados[0],-20} {dados[1],-8} {dados[2],-20} R$ {preco,-10:F2} {qtd,-5} R$ {total,-10:F2}");
                    soma += total; // Acumula total geral
                }
            }

            Console.WriteLine(new string('-', 75));
            Console.WriteLine($"TOTAL GERAL DE VENDAS: R$ {soma:F2}");
            Console.ResetColor();

            Console.WriteLine("\nPressione ENTER...");
            Console.ReadLine();
        }

        //  SALVAR PEÇAS EM ARQUIVO 
        private static void SalvarNoArquivo()
        {
            using (StreamWriter sw = new StreamWriter(caminho, true))
            {
                foreach (var p in pecas)
                    sw.WriteLine($"{p.Codigo};{p.Nome};{p.Preco};{p.Quantidade}");
            }
            pecas.Clear(); // Limpa a lista em memória
        }

        //  VERIFICAÇÃO DO ARQUIVO 
        private static bool ArquivoValido()
        {
            // Verifica se o arquivo existe
            if (!File.Exists(caminho))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Nenhuma peça foi cadastrada ainda! (arquivo não existe)");
                Console.ResetColor();
                Console.ReadLine();
                return false;
            }

            // Verifica se o arquivo está vazio
            if (new FileInfo(caminho).Length == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Nenhuma peça foi cadastrada ainda! (arquivo vazio)");
                Console.ResetColor();
                Console.ReadLine();
                return false;
            }

            // Verifica se todas as linhas são em branco
            string[] linhas = File.ReadAllLines(caminho);
            if (linhas.All(string.IsNullOrWhiteSpace))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Nenhuma peça foi cadastrada ainda! (somente linhas em branco)");
                Console.ResetColor();
                Console.ReadLine();
                return false;
            }

            return true; // Arquivo válido
        }
    }
}


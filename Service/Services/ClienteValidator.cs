using Database.Models;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using Service.Interface.Cliente;
using System.Diagnostics.CodeAnalysis;

namespace Domain.Services;

[ExcludeFromCodeCoverage]
internal class ClienteValidator : IClienteValidator
{
    private readonly string[] _colunasEsperadas = ["nome", "cpf", "endereço", "cidade", "estado", "ddd", "telefone"];
    private readonly string[] _formatosSuportados = [".xls", ".xlsx"];

    public void ValidarArquivoExcel(IFormFile arquivoExcel)
    {
        if (arquivoExcel == null)
        {
            throw new FileNotFoundException("Nenhum arquivo foi recebido para importação.");
        }

        if (arquivoExcel.Length == 0)
        {
            throw new FileLoadException("O arquivo recebido para importação é inválido.");
        }

        if (!_formatosSuportados.Contains(Path.GetExtension(arquivoExcel.FileName)))
        {
            throw new FileLoadException($"O arquivo recebido para importação não está dentro do formato esperado ({string.Join(',', _formatosSuportados)}).");
        }
    }

    public void ValidarPlanilha(MemoryStream memoryStream)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        using (var package = new ExcelPackage(memoryStream))
        {
            var planilha = package.Workbook.Worksheets[0];

            //Verifica se há uma planilha dentro do arquivo Excel recebido
            if (planilha == null)
            {
                throw new FileLoadException("O arquivo recebido para importação não contém uma planilha válida.");
            }

            ValidarCabecalho(planilha);
        }
    }

    private void ValidarCabecalho(ExcelWorksheet planilha)
    {
        //Verifica se a planilha possui linhas preenchidas
        if (planilha.Dimension.Rows == 0)
        {
            throw new FileLoadException("O arquivo recebido para importação não contém linhas preenchidas.");
        }

        //Verifica se o número de colunas está condizente com o esperado
        if (planilha.Cells[1, 1, 1, planilha.Dimension.End.Column].Count() != _colunasEsperadas.Count())
        {
            throw new FileLoadException("O arquivo recebido para importação não está dentro do padrão esperado.");
        }

        //Este laço verifica se o cabeçalho está dentro do padrão que está sendo esperado
        for (int i = 1; i <= planilha.Dimension.Columns; i++)
        {
            var nomeColuna = planilha.Cells[1, i].Value?.ToString()?.ToLower() ?? "";

            if (nomeColuna != _colunasEsperadas[i - 1])
            {
                throw new FileLoadException($"O arquivo recebido para importação não está dentro do padrão esperado: \r\nNome da coluna na planilha: {nomeColuna}. \r\nNome esperado: {_colunasEsperadas[i - 1]}");
            }
        }
    }

    public void ValidarCliente(Cliente cliente)
    {
        if (string.IsNullOrWhiteSpace(cliente.Nome))
        {
            throw new InvalidDataException("O nome do cliente não estava preenchido.");
        }

        if (string.IsNullOrWhiteSpace(cliente.Cpf))
        {
            throw new InvalidDataException("O CPF do cliente não estava preenchido.");
        }
        else
        {
            //Remove pontos e traços do CPF
            cliente.Cpf = cliente.Cpf.Replace(".", "").Replace("-", "");

            if (!ValidarCPF(cliente.Cpf))
            {
                throw new InvalidDataException("O CPF do cliente é inválido.");
            }
        }

        if (string.IsNullOrWhiteSpace(cliente.Endereco))
        {
            throw new InvalidDataException("O endereço do cliente não estava preenchido.");
        }

        if (string.IsNullOrWhiteSpace(cliente.Cidade))
        {
            throw new InvalidDataException("A cidade do cliente não estava preenchida.");
        }

        if (string.IsNullOrWhiteSpace(cliente.Estado))
        {
            throw new InvalidDataException("O estado do cliente não estava preenchido.");
        }

        if (string.IsNullOrWhiteSpace(cliente.Ddd))
        {
            throw new InvalidDataException("O DDD do cliente não estava preenchido.");
        }

        if (string.IsNullOrWhiteSpace(cliente.Telefone))
        {
            throw new InvalidDataException("O telefone do cliente não estava preenchido.");
        }
    }

    private static bool ValidarCPF(string cpf)
    {
        cpf = new string(cpf.Where(char.IsDigit).ToArray());

        if (cpf.Length != 11)
            return false;

        int soma = 0;
        for (int i = 0; i < 9; i++)
            soma += int.Parse(cpf[i].ToString()) * (10 - i);
        int resto = soma % 11;
        int digitoVerificador1 = resto < 2 ? 0 : 11 - resto;

        if (int.Parse(cpf[9].ToString()) != digitoVerificador1)
            return false;

        soma = 0;
        for (int i = 0; i < 10; i++)
            soma += int.Parse(cpf[i].ToString()) * (11 - i);
        resto = soma % 11;
        int digitoVerificador2 = resto < 2 ? 0 : 11 - resto;

        if (int.Parse(cpf[10].ToString()) != digitoVerificador2)
            return false;

        return true;
    }

}

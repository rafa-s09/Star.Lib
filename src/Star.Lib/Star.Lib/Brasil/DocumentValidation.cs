namespace Star.Lib.Brasil;

/// <summary>
/// Contém funções de validação para documentos brasileiros. <br/>
/// <i>Observação: Não há verificação de elegibilidade do documento, apenas verifica se o número é um valor válido.</i>
/// </summary>
public static class DocumentValidation
{
    /// <summary>
    /// Valida o CPF pelo dígito verificador
    /// </summary>
    /// <param name="cpf">Número do CPF</param>
    /// <returns>True se for valido</returns>
    public static bool IsValidCPF(this string cpf)
    {
        int[] firstDigit = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] secondDigit = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        string temp, digit;
        int sum, rest;

        cpf = cpf.ClearSymbols();

        if (cpf.Length != 11)
            throw new ArgumentException("Tamanho do documento incorreto.");

        temp = cpf[..9];
        sum = 0;

        for (int i = 0; i < 9; i++)
            sum += int.Parse(temp[i].ToString()) * firstDigit[i];

        rest = sum % 11;
        if (rest < 2)
            rest = 0;
        else
            rest = 11 - rest;

        digit = rest.ToString();
        temp += digit;
        sum = 0;

        for (int i = 0; i < 10; i++)
            sum += int.Parse(temp[i].ToString()) * secondDigit[i];

        rest = sum % 11;
        if (rest < 2)
            rest = 0;
        else
            rest = 11 - rest;

        digit += rest.ToString();

        if (cpf.EndsWith(digit))
            return true;
        else
            return false;
    }

    /// <summary>
    /// Valida o CNPJ pelo dígito verificador
    /// </summary>
    /// <param name="cnpj">Número do CNPF</param>
    /// <returns>True se for valido</returns>
    public static bool IsValidCNPJ(this string cnpj)
    {
        int[] firstDigit = new int[12] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] secondDigit = new int[13] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        int sum, rest;
        string digit, temp;

        cnpj = cnpj.ClearSymbols();

        if (cnpj.Length != 14)
            throw new ArgumentException("Tamanho do documento incorreto.");

        temp = cnpj[..12];
        sum = 0;

        for (int i = 0; i < 12; i++)
            sum += int.Parse(temp[i].ToString()) * firstDigit[i];

        rest = (sum % 11);
        if (rest < 2)
            rest = 0;
        else
            rest = 11 - rest;

        digit = rest.ToString();
        temp += digit;
        sum = 0;

        for (int i = 0; i < 13; i++)
            sum += int.Parse(temp[i].ToString()) * secondDigit[i];

        rest = (sum % 11);
        if (rest < 2)
            rest = 0;
        else
            rest = 11 - rest;

        digit += rest.ToString();

        if (cnpj.EndsWith(digit))
            return true;
        else
            return false;
    }

    /// <summary>
    /// Valida o PIS pelo dígito verificador
    /// </summary>
    /// <param name="pis">Número do PIS</param>
    /// <returns>True se for valido</returns>
    public static bool IsValidPIS(this string pis)
    {
        int[] validDigit = new int[10] { 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        int sum, rest;

        pis = pis.ClearSymbols();
        if (pis.Length != 11)
            throw new ArgumentException("Tamanho do documento incorreto.");

        pis = pis.Trim().Replace("-", "").Replace(".", "").PadLeft(11, '0');
        sum = 0;

        for (int i = 0; i < 10; i++)
            sum += int.Parse(pis[i].ToString()) * validDigit[i];

        rest = sum % 11;
        if (rest < 2)
            rest = 0;
        else
            rest = 11 - rest;

        if (pis.EndsWith(rest.ToString()))
            return true;
        else
            return false;
    }
}


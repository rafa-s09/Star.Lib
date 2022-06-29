using System.Text;

namespace Star.Lib;

/// <summary>
/// Contém várias extensões úteis para acelerar o desenvolvimento
/// </summary>
public static class Extensions
{
    #region Text        
    /// <summary>
    /// Retorna o texto até o primeiro caractere definido ou returna uma string vazia
    /// </summary>
    /// <param name="text">Texto</param>
    /// <param name="stopAt">Parada</param>
    /// <returns>Texto alcançado ate o caractere ou string vazia</returns>
    public static string GetUntilOrEmpty(this string text, char stopAt)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                int charLocation = text.IndexOf(stopAt, StringComparison.Ordinal);
                if (charLocation > 0)
                    return text[..charLocation];
            }
            return string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// Retorna o texto até o primeiro caractere definido, caso não encontre, não fará nada.
    /// </summary>
    /// <param name="text">Texto</param>
    /// <param name="stopAt">Parada</param>
    /// <returns>Texto alcançado ate o caractere ou texto original</returns>
    public static string GetUntil(this string text, char stopAt)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                int charLocation = text.IndexOf(stopAt, StringComparison.Ordinal);
                if (charLocation > 0)
                    return text[..charLocation];
            }
            return text;
        }
        catch
        {
            return text;
        }
    }

    /// <summary>
    /// Retorna o texto a partir do primeiro caractere definido ou returna uma string vazia
    /// </summary>
    /// <param name="text">Input text</param>
    /// <param name="startAt">Char to Start</param>
    /// <returns>Texto alcançado após o caractere ou string vazia</returns>
    public static string GetAfterOrEmpty(this string text, char startAt)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                int charLocation = text.IndexOf(startAt, StringComparison.Ordinal);
                if (charLocation > 0)
                    return text[charLocation..];
            }
            return string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// Retorna o texto a partir do primeiro caractere definido, caso não encontre, não fará nada.
    /// </summary>
    /// <param name="text">Texto</param>
    /// <param name="startAt">Começo</param>
    /// <returns>Texto alcançado ate o caractere ou texto original</returns>
    public static string GetAfter(this string text, char startAt)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                int charLocation = text.IndexOf(startAt, StringComparison.Ordinal);
                if (charLocation > 0)
                    return text[charLocation..];
            }
            return text;
        }
        catch
        {
            return text;
        }
    }

    /// <summary>
    /// Troca caracteres acentuados por caracteres não acentuados
    /// </summary>
    /// <param name="value">Texto</param>
    /// <returns>Texto não acentuado</returns>
    public static string ClearAccentedCharacters(this string value)
    {
        string result = value;
        string[] accented = new string[] { "ç", "Ç", "á", "é", "í", "ó", "ú", "ý", "Á", "É", "Í", "Ó", "Ú", "Ý", "à", "è", "ì", "ò", "ù", "À", "È", "Ì", "Ò", "Ù", "ã", "õ", "ñ", "ä", "ë", "ï", "ö", "ü", "ÿ", "Ä", "Ë", "Ï", "Ö", "Ü", "Ã", "Õ", "Ñ", "â", "ê", "î", "ô", "û", "Â", "Ê", "Î", "Ô", "Û" };
        string[] nonAccented = new string[] { "c", "C", "a", "e", "i", "o", "u", "y", "A", "E", "I", "O", "U", "Y", "a", "e", "i", "o", "u", "A", "E", "I", "O", "U", "a", "o", "n", "a", "e", "i", "o", "u", "y", "A", "E", "I", "O", "U", "A", "O", "N", "a", "e", "i", "o", "u", "A", "E", "I", "O", "U" };

        for (int i = 0; i < accented.Length; i++)
            result = result.Replace(accented[i], nonAccented[i]);

        return result;
    }

    /// <summary>
    /// Remove símbolos e pontos
    /// </summary>
    /// <param name="value">Texto</param>
    /// <returns>Texto limpo</returns>
    public static string ClearSymbols(this string value)
    {
        string result = value.TrimStart().TrimEnd();
        string[] symbols = new string[] { "-", "_", ".", ",", "\\", "/", "|", "~", "#", "$", "%", "&", "@", "\"", "'", "*", "=", "+", "ª", "º", ">", "<", ":", ";", "?", "!" };

        for (int i = 0; i < symbols.Length; i++)
            result = result.Replace(symbols[i], "");

        return result;
    }

    /// <summary>
    /// Troca caracteres acentuados por caracteres não acentuados e remove símbolos e pontos
    /// </summary>
    /// <param name="value">Texto</param>
    /// <returns>Texto não acentuado e limpo</returns>
    public static string ClearSpecialCharacters(this string value)
    {
        string result = value.ClearAccentedCharacters();
        return result.ClearSymbols();
    }
    #endregion

    #region Conversions         
    /// <summary>
    /// Converte string para ByteArray
    /// </summary>
    /// <param name="value">String value</param>
    /// <param name="encode">Formato do Encode (Default: UTF8)</param>
    /// <returns>Byte Array</returns>
    public static byte[] ToByteArray(this string value, TextEncode encode = TextEncode.UTF8)
    {
        return encode switch
        {
            TextEncode.ASCII => Encoding.ASCII.GetBytes(value),
            TextEncode.UTF8 => Encoding.UTF8.GetBytes(value),
            TextEncode.UTF32 => Encoding.UTF32.GetBytes(value),
            TextEncode.Unicode => Encoding.Unicode.GetBytes(value),
            TextEncode.Latin1 => Encoding.Latin1.GetBytes(value),
            _ => Encoding.UTF8.GetBytes(value),
        };
    }

    /// <summary>
    /// Converter ByteArray para string
    /// </summary>
    /// <param name="value">Byte Array</param>
    /// <param name="encode">Formato do Encode (Default: UTF8)</param>
    /// <returns>String</returns>
    public static string ToString(this byte[] value, TextEncode encode = TextEncode.UTF8)
    {
        return encode switch
        {
            TextEncode.ASCII => Encoding.ASCII.GetString(value),
            TextEncode.UTF8 => Encoding.UTF8.GetString(value),
            TextEncode.UTF32 => Encoding.UTF32.GetString(value),
            TextEncode.Unicode => Encoding.Unicode.GetString(value),
            TextEncode.Latin1 => Encoding.Latin1.GetString(value),
            _ => Encoding.UTF8.GetString(value),
        };
    }
    #endregion
}

#region Enumerators
/// <summary>
/// Tipo de Encode de Texto
/// </summary>
public enum TextEncode
{
    /// <summary>
    /// Encode: ASCII
    /// </summary>
    ASCII,
    /// <summary>
    /// Encode: UTF8
    /// </summary>
    UTF8,
    /// <summary>
    /// Encode: UTF32
    /// </summary>
    UTF32,
    /// <summary>
    /// Encode: Unicode
    /// </summary>
    Unicode,
    /// <summary>
    /// Encode: Latin1
    /// </summary>
    Latin1
}
#endregion
using Microsoft.VisualBasic.FileIO;
using System.Globalization;
using System.Text;

namespace Crm.Infrastructure.Import;

public static class ShoppingTrendsRuParser
{
    public static IReadOnlyList<ShoppingTrendRuRow> Parse(string filePath)
    {
        var result = new List<ShoppingTrendRuRow>();

        using var reader = new StreamReader(filePath, Encoding.GetEncoding(1251));

        var headerLine = reader.ReadLine();
        if (string.IsNullOrWhiteSpace(headerLine))
            throw new InvalidOperationException("Файл пустой.");

        while (!reader.EndOfStream)
        {
            var rawLine = reader.ReadLine();
            if (string.IsNullOrWhiteSpace(rawLine))
                continue;

            var translatedLine = GetFirstPart(rawLine);

            var columns = SplitCsvLine(translatedLine);

            if (columns.Length < 19)
                throw new InvalidOperationException($"Ожидалось 19 колонок, получено {columns.Length}. Строка: {translatedLine}");

            var row = new ShoppingTrendRuRow
            {
                CustomerId = columns[0],
                Age = int.Parse(columns[1], CultureInfo.InvariantCulture),
                Gender = columns[2],
                ItemPurchased = columns[3],
                Category = columns[4],
                PurchaseAmountUsd = decimal.Parse(columns[5], CultureInfo.InvariantCulture),
                Location = columns[6],
                Size = columns[7],
                Color = columns[8],
                Season = columns[9],
                ReviewRating = decimal.Parse(columns[10], CultureInfo.InvariantCulture),
                SubscriptionStatus = columns[11],
                PaymentMethod = columns[12],
                ShippingType = columns[13],
                DiscountApplied = columns[14],
                PromoCodeUsed = columns[15],
                PreviousPurchases = int.Parse(columns[16], CultureInfo.InvariantCulture),
                PreferredPaymentMethod = columns[17],
                FrequencyOfPurchases = columns[18]
            };

            result.Add(row);
        }

        return result;
    }

    private static string GetFirstPart(string line)
    {
        var index = line.IndexOf(';');

        return index > 0
            ? line[..index]
            : line;
    }

    private static string[] SplitCsvLine(string line)
    {
        using var stringReader = new StringReader(line);
        using var parser = new TextFieldParser(stringReader);

        parser.TextFieldType = FieldType.Delimited;
        parser.SetDelimiters(",");
        parser.HasFieldsEnclosedInQuotes = true;

        return parser.ReadFields() ?? Array.Empty<string>();
    }
}

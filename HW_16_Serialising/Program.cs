using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.IO;

using System.Text.Json.Serialization;

namespace HW_16_Serialising
{
    public class PaymentInvoice
    {
        // Поля
        public decimal PaymentPerDay { get; set; }  // Оплата за день
        public int Days { get; set; }  // Количество дней
        public decimal PenaltyPerDay { get; set; }  // Штраф за один день задержки оплаты
        public int DelayDays { get; set; }  // Количество дней задержи оплаты

        // Вычисляемые свойства
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public decimal AmountWithoutPenalty => PaymentPerDay * Days;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public decimal Penalty => PenaltyPerDay * DelayDays;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public decimal TotalAmount => AmountWithoutPenalty + Penalty;

        // Статическое свойство для управления сериализацией
        public static bool SerializeComputedFields { get; set; } = true;

        public string ToJson()
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            if (!SerializeComputedFields)
            {
                options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
            }

            return JsonSerializer.Serialize(this, options);
        }

        public static PaymentInvoice FromJson(string json)
        {
            return JsonSerializer.Deserialize<PaymentInvoice>(json);
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            // Создание объекта
            var invoice = new PaymentInvoice
            {
                PaymentPerDay = 100,
                Days = 30,
                PenaltyPerDay = 5,
                DelayDays = 10
            };

            // Сериализация
            Console.WriteLine("Сериализация с вычисляемыми полями:");
            PaymentInvoice.SerializeComputedFields = true;
            string jsonWithComputed = invoice.ToJson();
            Console.WriteLine(jsonWithComputed);

            // Сохранение в файл
            File.WriteAllText("invoice_with_computed.json", jsonWithComputed);

            // Сериализация без вычисляемых полей
            Console.WriteLine("\nСериализация без вычисляемых полей:");
            PaymentInvoice.SerializeComputedFields = false;
            string jsonWithoutComputed = invoice.ToJson();
            Console.WriteLine(jsonWithoutComputed);

            // Сохранение в файл
            File.WriteAllText("invoice_without_computed.json", jsonWithoutComputed);

            // Чтение из файла
            Console.WriteLine("\nЧтение из файла:");
            string jsonFromFile = File.ReadAllText("invoice_with_computed.json");
            var deserializedInvoice = PaymentInvoice.FromJson(jsonFromFile);
            Console.WriteLine("Объект после десериализации:");
            Console.WriteLine($"Оплата за день: {deserializedInvoice.PaymentPerDay}");
            Console.WriteLine($"Количество дней: {deserializedInvoice.Days}");
            Console.WriteLine($"Штраф за день: {deserializedInvoice.PenaltyPerDay}");
            Console.WriteLine($"Количество дней задержки: {deserializedInvoice.DelayDays}");
            Console.WriteLine($"Сумма без штрафа: {deserializedInvoice.AmountWithoutPenalty}");
            Console.WriteLine($"Штраф: {deserializedInvoice.Penalty}");
            Console.WriteLine($"Общая сумма: {deserializedInvoice.TotalAmount}");
        }
    }
}

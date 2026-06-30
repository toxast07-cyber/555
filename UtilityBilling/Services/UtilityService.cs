using System;
using System.Collections.Generic;
using System.Linq;
using UtilityBilling.Models;

namespace UtilityBilling.Services
{
    /// <summary>
    /// Сервис для управления коммунальными услугами
    /// </summary>
    public class UtilityService
    {
        private List<Utility> utilities;
        private List<BillingRecord> billingRecords;

        public UtilityService()
        {
            utilities = new List<Utility>();
            billingRecords = new List<BillingRecord>();
            InitializeDefaultUtilities();
        }

        /// <summary>
        /// Инициализация услуг по умолчанию
        /// </summary>
        private void InitializeDefaultUtilities()
        {
            utilities.Add(new Utility
            {
                Id = 1,
                Name = "Электричество",
                Type = UtilityType.Electricity,
                Rate = 5.50m,
                Unit = "кВт"
            });

            utilities.Add(new Utility
            {
                Id = 2,
                Name = "Вода",
                Type = UtilityType.Water,
                Rate = 40.00m,
                Unit = "м³"
            });

            utilities.Add(new Utility
            {
                Id = 3,
                Name = "Газ",
                Type = UtilityType.Gas,
                Rate = 8.50m,
                Unit = "м³"
            });

            utilities.Add(new Utility
            {
                Id = 4,
                Name = "Интернет",
                Type = UtilityType.Internet,
                Rate = 500.00m,
                Unit = "месяц"
            });
        }

        /// <summary>
        /// Получить все услуги
        /// </summary>
        public List<Utility> GetAllUtilities()
        {
            return utilities;
        }

        /// <summary>
        /// Получить услугу по ID
        /// </summary>
        public Utility GetUtilityById(int id)
        {
            return utilities.FirstOrDefault(u => u.Id == id);
        }

        /// <summary>
        /// Обновить показание услуги
        /// </summary>
        public bool UpdateReading(int utilityId, decimal currentReading)
        {
            var utility = GetUtilityById(utilityId);
            if (utility != null)
            {
                utility.PreviousReading = utility.CurrentReading;
                utility.CurrentReading = currentReading;
                utility.LastReadingDate = DateTime.Now;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Создать новый счет
        /// </summary>
        public BillingRecord CreateBillingRecord(string apartmentNumber)
        {
            var record = new BillingRecord
            {
                ApartmentNumber = apartmentNumber,
                BillingDate = DateTime.Now
            };

            foreach (var utility in utilities)
            {
                record.Utilities.Add(utility.Id, new Utility
                {
                    Id = utility.Id,
                    Name = utility.Name,
                    Type = utility.Type,
                    CurrentReading = utility.CurrentReading,
                    PreviousReading = utility.PreviousReading,
                    Rate = utility.Rate,
                    Unit = utility.Unit
                });
            }

            record.CalculateTotal();
            billingRecords.Add(record);
            return record;
        }

        /// <summary>
        /// Получить все счета
        /// </summary>
        public List<BillingRecord> GetAllBillingRecords()
        {
            return billingRecords;
        }

        /// <summary>
        /// Получить счета за период
        /// </summary>
        public List<BillingRecord> GetBillingRecordsByDateRange(DateTime startDate, DateTime endDate)
        {
            return billingRecords
                .Where(r => r.BillingDate >= startDate && r.BillingDate <= endDate)
                .ToList();
        }

        /// <summary>
        /// Получить среднее потребление услуги
        /// </summary>
        public decimal GetAverageConsumption(int utilityId, int monthCount = 6)
        {
            var startDate = DateTime.Now.AddMonths(-monthCount);
            var records = billingRecords
                .Where(r => r.BillingDate >= startDate && r.Utilities.ContainsKey(utilityId))
                .ToList();

            if (records.Count == 0) return 0;

            return records.Sum(r => r.Utilities[utilityId].GetConsumption()) / records.Count;
        }
    }
}

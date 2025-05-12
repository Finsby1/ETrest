namespace ETlib
{
    public record EnergyPriceRecord(double? DKK_per_kWh, DateTime? time_start);

    public static class EnergyPriceRecordHelper
    {
        public static EnergyPrice ConvertEnergyPriceRecord(EnergyPriceRecord record)
        {
            if (record.DKK_per_kWh == null)
            {
                throw new ArgumentNullException(nameof(record.DKK_per_kWh), "DKK_per_kWh cannot be null");
            }

            if (record.DKK_per_kWh <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(record.DKK_per_kWh), "DKK_per_kWh must be greater than 0");
            }

            if (record.time_start == null)
            {
                throw new ArgumentNullException(nameof(record.time_start), "time_start cannot be null");
            }

            if (record.time_start <= new DateTime(2025, 1, 1) || record.time_start >= new DateTime(2125, 1, 1))
            {
                throw new ArgumentOutOfRangeException(nameof(record.time_start), "time_start must be between 2025 and 2125");
            }

            return new EnergyPrice
            {
                DKK_per_kWh = record.DKK_per_kWh.Value,
                time_start = record.time_start.Value,
                Category = "Standard" // Default value to satisfy validation
            };
        }
    }
}
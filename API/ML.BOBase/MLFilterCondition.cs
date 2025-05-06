namespace API.ML.BOBase
{
    public class MLFilterCondition
    {
        public string Name { get; set; } = string.Empty;

        public string Operator { get; set; } = string.Empty;

        public object? Value { get; set; } = null;

        public bool CompareProperties { get; set; }
    }
}

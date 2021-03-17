namespace Core.CrossCuttingConcerns.Logging.Serilog.ConfigurationModels
{
    public class ElasticsearchConfiguration
    {
        public string ConnectionString { get; set; }
        public string TemplateName { get; set; }
        public string IndexFormat { get; set; }
        public string SeqConnectionString { get; set; }
    }
}
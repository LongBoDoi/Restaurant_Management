namespace API.ML.BOBase
{
    public class MLPagingData<IMLEntity> where IMLEntity : MLEntity
    {
        public IEnumerable<IMLEntity>? Data { get; set; }

        public int TotalCount { get; set; }
    }
}

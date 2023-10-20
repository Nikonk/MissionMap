namespace MissionMap.Bootstrap
{
    public interface IDataService
    {
        public bool Save<T>(string relativePath, T data);
        public T Load<T>(string relativePath);
    }
}
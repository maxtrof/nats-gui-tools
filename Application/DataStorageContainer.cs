using Application.Exceptions;

namespace Application;

/// <summary>
/// Data container for <see cref="DataStorage"/>
/// </summary>
/// <typeparam name="T"></typeparam>
internal class DataStorageContainer<T> where T : notnull
{
    private T? _data;
    private int _dataVersion = 0;
    private int _dataSavedVersion = 0;

    /// <summary>
    /// If True - data has to be saved
    /// </summary>
    public bool NeedsToBeSaved => _dataVersion > _dataSavedVersion;
    
    /// <summary>
    /// Increment current data version
    /// </summary>
    public void IncrementVersion() => _dataVersion++;

    /// <summary>
    /// Should be called when data is saved
    /// </summary>
    public void OnDataSaved() => _dataSavedVersion = _dataVersion;

    public T Data
    {
        get
        {
            if (_data is null) throw new DataStorageIsNotInitializedException(typeof(T).FullName);
            return _data;
        }
        set => _data = value;
    }
}
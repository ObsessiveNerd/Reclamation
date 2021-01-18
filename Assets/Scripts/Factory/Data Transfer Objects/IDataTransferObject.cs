public interface IDataTransferComponent
{
    IComponent Component { get; set; }
    void CreateComponent(string data);
    string CreateSerializableData(IComponent component);
}

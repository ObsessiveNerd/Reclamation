public interface IDataTransferComponent
{
    IComponent Component { get; set; }
    void CreateComponent(string data);
}

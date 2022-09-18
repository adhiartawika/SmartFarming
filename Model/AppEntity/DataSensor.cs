public class DataMc{
    public int Id {get;set;}
    public List<DataSensor> DataSensors {get;set;}
}
public class DataSensor{
    public int IdSensor{get;set;}
    public float Value {get;set;}
}
public class McConnection{
    public int Id {get;set;}
}
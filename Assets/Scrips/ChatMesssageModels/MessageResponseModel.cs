using MessagePack;

[MessagePackObject]
public class MessageResponseModel : MessageBaseModel {
  [Key(10)] public string MessageData { get; set; }
}
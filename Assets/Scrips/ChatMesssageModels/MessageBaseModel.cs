using MessagePack;
using System;

[MessagePackObject]
public class MessageBaseModel {
  [Key(0)] public long CreatedAt { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
  [Key(1)] public MessageTypeEnum MessageType { get; set; } = MessageTypeEnum.Chat;
  [Key(2)] public Guid Id { get; set; } = Guid.NewGuid();
}
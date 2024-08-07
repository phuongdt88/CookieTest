using System;
using System.Collections.Generic;
using System.Linq;
using MessagePack;

[MessagePackObject]
public class ChatMessageModel {
  [Key(0)] public string UDID { get; set; }
  [Key(1)] public string UserName { get; set; }
  [Key(2)] public string Message { get; set; }
  [Key(3)] public string AllianceID { get; set; }
  [Key(4)] public string TargetAlliance { get; set; }
  [Key(5)] public string TeamID { get; set; }
  [Key(6)] public string Portrait { get; set; }
  [Key(7)] public bool IsRecruitmentMessage { get; set; }
  [Key(8)] public string AllianceLogo { get; set; }
  [Key(9)] public DateTime TimeOfSubmission { get; set; }
  [Key(10)] public long TimestampSubmissionMS { get; set; }
  [Key(11)] public string Type { get; set; }
  [Key(12)] public string Polarity { get; set; }

  [Key(13)] public Dictionary<string, object> CustomData;
  
  public override string ToString() {
    string customDataString = CustomData != null ? 
      string.Join(", ", CustomData.Select(kvp => $"{kvp.Key}: {kvp.Value}")) : "null";

    return $"ChatMessageModel {{ " +
           $"UDID: {UDID}, UserName: {UserName}, Message: {Message}, AllianceID: {AllianceID}, " +
           $"TargetAlliance: {TargetAlliance}, TeamID: {TeamID}, Portrait: {Portrait}, " +
           $"IsRecruitmentMessage: {IsRecruitmentMessage}, AllianceLogo: {AllianceLogo}, " +
           $"TimeOfSubmission: {TimeOfSubmission}, TimestampSubmissionMS: {TimestampSubmissionMS}, " +
           $"Type: {Type}, Polarity: {Polarity}, CustomData: {{ {customDataString} }} }}";
  }
}
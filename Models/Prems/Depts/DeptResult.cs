using System.Text.Json.Serialization;

namespace api.Models;

public class DeptResult(string error)
{
    /// <summary>
    /// 計數
    /// </summary>
    public int Count { get; set; } = 0;

    /// <summary>
    /// 資料
    /// </summary>
    public IEnumerable<DeptData> Data { get; set; } = [];

    /// <summary>
    /// 錯誤訊息
    /// </summary>
    public string Error { get; set; } = error;
}

public class DeptData
{
    /// <summary>
    /// 學院代碼
    /// </summary>
    [JsonPropertyName("dep_academyno")]
    public string DepAcademyno { get; set; }

    /// <summary>
    /// 校區: 1.台北,2.光復, 3.博愛, 4.台南, 5.竹北, 6.陽明
    /// </summary>
    [JsonPropertyName("dep_area")]
    public int? DepArea { get; set; }

    /// <summary>
    /// 系所英文名稱
    /// </summary>
    [JsonPropertyName("dep_depename")]
    public string DepDepename { get; set; }

    /// <summary>
    /// 系所英文簡稱
    /// </summary>
    [JsonPropertyName("dep_depesname")]
    public string DepDepesname { get; set; }

    /// <summary>
    /// 系所名稱
    /// </summary>
    [JsonPropertyName("dep_depname")]
    public string DepDepname { get; set; }

    /// <summary>
    /// 系所代碼
    /// </summary>
    [JsonPropertyName("dep_depno")]
    public string DepDepno { get; set; }

    /// <summary>
    /// 系所簡稱
    /// </summary>
    [JsonPropertyName("dep_depsname")]
    public string DepDepsname { get; set; }

    /// <summary>
    /// 適用學年度
    /// </summary>
    [JsonPropertyName("dep_year")]
    public int DepYear { get; set; }
}
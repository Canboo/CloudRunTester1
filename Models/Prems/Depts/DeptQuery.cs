using System.ComponentModel.DataAnnotations;

namespace api.Models;

/// <summary>
/// 檢索教學單位
/// </summary>
public class DeptQuery
{
    /// <summary>
    /// 適用學年度
    /// </summary>
    [Required(ErrorMessage = "適用學年度必須輸入")]
    public int DepYear { get; set; }
}
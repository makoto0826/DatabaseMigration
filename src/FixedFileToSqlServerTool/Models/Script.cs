using LiteDB;

namespace FixedFileToSqlServerTool.Models;

public class Script
{
    public ObjectId Id { get; set; }

    public string Name { get; set; }

    public string Code { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public static Script Create(string name) =>
        new Script
        {
            Id = ObjectId.NewObjectId(),
            Name = name,
            Code = @"using System;
// テストデータは、Value変数から取得します。Valueの型はString
//
// 取得結果はreturn句を使用

return Value;",
            UpdatedAt = DateTime.Now,
            CreatedAt = DateTime.Now
        };
}

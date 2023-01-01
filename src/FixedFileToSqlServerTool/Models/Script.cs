using LiteDB;

namespace FixedFileToSqlServerTool.Models;

public record Script
{
    private const string DefaultCode = @"using System;
// テストデータは、Value変数から取得します。Valueの型はString
//
// 取得結果はreturn句を使用

return Value;";

    public ObjectId Id { get; init; }

    public string Name { get; init; }

    public string Code { get; init; }

    public DateTime UpdatedAt { get; init; }

    public DateTime CreatedAt { get; init; }

    public static Script Create(string name) =>
        new Script
        {
            Id = ObjectId.NewObjectId(),
            Name = name,
            Code = DefaultCode,
            UpdatedAt = DateTime.Now,
            CreatedAt = DateTime.Now
        };
}
